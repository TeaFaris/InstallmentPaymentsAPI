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
		public async Task<JsonResult> ChargePayment([FromBody] PaymentRequest Payment)
		{
			try
			{
				// Validation

				if (!ModelState.IsValid)
					return new JsonResult(ModelState);

				var Product = Database
					.Products
					.Include(x => x.Category)
					.ThenInclude(x => x.Products)
					.FirstOrDefault(x => x.ID == Payment.ProductID);

				if (Product is null)
					return new JsonResult(NotFound("Product is not found."));

				var Category = Product.Category;

				if (Payment.InstallmentDuration > (uint)Category.InstallmentRangeMax || Payment.InstallmentDuration < (uint)Category.InstallmentRangeMin)
					return new JsonResult(BadRequest($"Installment duration must be between {Category.InstallmentRangeMin} and {Category.InstallmentRangeMax} months"));

				var InstallmentRanges = Enum.GetValues<InstallmentRange>();

				int InstallmentDurationUnit = Array.FindIndex(InstallmentRanges, x => (uint)x == Payment.InstallmentDuration);

				if (InstallmentDurationUnit < 0)
					return new JsonResult(BadRequest($"Installment duration must be {string.Join(", ", InstallmentRanges.Cast<uint>())}"));

				// Act

				uint InstallmentDuration = (uint)InstallmentDurationUnit;
				decimal TotalInstallmentPercentage = Category.InstallmentPercentage * InstallmentDuration;
				decimal AmoutWithInstallment = Payment.Amount * ((100 + TotalInstallmentPercentage) / 100);

				var SendSMSTask = SmsService.SendSMS(Payment.PhoneNumber, $"""
													 Вы купили {Payment.Product.Name} на сумму {Payment.Product.Price} сомони в рассрочку на {Payment.InstallmentDuration}.
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

				return new JsonResult(base.Ok(NewPayment));
			}
			catch(Exception Ex)
			{
				return new JsonResult(BadRequest(Ex));
			}
		}
	}
}
