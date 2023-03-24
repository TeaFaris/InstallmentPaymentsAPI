using InstallmentPaymentsAPI.Data;
using InstallmentPaymentsAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
	}
}
