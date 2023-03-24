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

		/// <summary>
		/// This method charges a payment with an installment plan and returns the resulting payment record.
		/// The method expects an HTTP POST request with a <see cref="PaymentRequest"/> object in the request body.
		/// </summary>
		/// <param name="Payment">The <see cref="PaymentRequest"/> object containing the payment information.</param>
		/// <returns>The created <see cref="InstallmentPayment"/> object.</returns>
		/// <response code="200">The payment was successfully charged and a payment record was created.</response>
		/// <response code="400">The request payload was invalid or the installment duration was outside of the allowed range.</response>
		/// <response code="404">The specified ProductID was not found in the database.</response>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]

		[Route(nameof(ChargePayment))]
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
