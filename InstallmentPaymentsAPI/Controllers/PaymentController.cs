using InstallmentPaymentsAPI.Data;
using InstallmentPaymentsAPI.Models.NonDB;
using InstallmentPaymentsAPI.Models;
using InstallmentPaymentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstallmentPaymentsAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly APIContext Database;
		private readonly ISmsService SmsService;

		public PaymentController(APIContext Database, ISmsService SmsService)
		{
			this.Database = Database;
			this.SmsService = SmsService;
		}

		[HttpPost]
		public async Task<IActionResult> ChargePayment([FromBody] PaymentRequest Payment)
		{
			try
			{
				// Validation

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var Product = Database
					.Products
					.Include(x => x.Category)
					.ThenInclude(x => x.Products)
					.FirstOrDefault(x => x.ID == Payment.ProductID);

				if (Product is null)
					return NotFound("Product is not found.");

				var Category = Product.Category;

				if (Payment.InstallmentDuration > (uint)Category.InstallmentRangeMax || Payment.InstallmentDuration < (uint)Category.InstallmentRangeMin)
					return BadRequest($"Installment duration must be between {Category.InstallmentRangeMin} and {Category.InstallmentRangeMax} months");

				var InstallmentRanges = Enum.GetValues<InstallmentRange>();

				int InstallmentDurationUnit = Array.FindIndex(InstallmentRanges, x => (uint)x == Payment.InstallmentDuration);

				if (InstallmentDurationUnit < 0)
					return BadRequest($"Installment duration must be {string.Join(", ", InstallmentRanges.Cast<uint>())}");

				// Act

				uint InstallmentDuration = (uint)InstallmentDurationUnit + 1;
				decimal TotalInstallmentPercentage = Category.InstallmentPercentage * InstallmentDuration;
				decimal AmoutWithInstallment = Payment.Amount * ((100 + TotalInstallmentPercentage) / 100);

				var SendSMSTask = SmsService.SendSMS(Payment.PhoneNumber, $"""
													 Вы купили {Product.Name} на сумму {Product.Price} сомони в рассрочку на {Payment.InstallmentDuration}.
													 Общая сумма платежа: {AmoutWithInstallment} сомони.
													 """);

				// Result

				InstallmentPayment NewPayment = new InstallmentPayment
				{
					Amount = Payment.Amount,
					InstallmentDuration = Payment.InstallmentDuration,
					PhoneNumber = Payment.PhoneNumber,
					ProductID = Payment.ProductID,
					AmountWithInstallment = AmoutWithInstallment
				};

				Database.InstallmentPayments.Add(NewPayment);

				await Database.SaveChangesAsync();

				await SendSMSTask;

				return Ok(NewPayment);
			}
			catch(Exception Ex)
			{
				return BadRequest(Ex);
			}
		}
	}
}
