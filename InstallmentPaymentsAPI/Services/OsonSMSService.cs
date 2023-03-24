using InstallmentPaymentsAPI.Configs;
using RestSharp;
using System.Text;

namespace InstallmentPaymentsAPI.Services
{
	public class OsonSmsService : ISmsService
	{
		private const string SendSMSRequestURL = "http://82.196.1.18/sendsms_v1.php";
		private const string TParameter = "23"; // Don't know why, i'm doing what documentation says.
		private const char Separator = ';';

		private readonly OsonSMSOptions Options;

		public OsonSmsService() { }
		public OsonSmsService(OsonSMSOptions Options)
		{
			this.Options = Options;
		}

		public async Task SendSMS(string PhoneNumber, string Message)
		{
			using var Client = new RestClient(SendSMSRequestURL);
			var Request = new RestRequest(SendSMSRequestURL, Method.Get);

			var TxnID = Random.Shared.Next(100000, 100000000).ToString();

			const string Sender = "Rassrochka";

			var StrHash = new StringBuilder()
				.Append(TxnID)
				.Append(Separator)
				.Append(Options.Username)
				.Append(Separator)
				.Append(Sender)
				.Append(Separator)
				.Append(PhoneNumber)
				.Append(Separator)
				.Append(Options.HashPassword)
				.ToString();

			Request.AddParameter("from", Sender)
				.AddParameter("login", Options.Username)
				.AddParameter("t", TParameter)
				.AddParameter("phone_number", PhoneNumber)
				.AddParameter("msg", Message)
				.AddParameter("str_hash", StrHash)
				.AddParameter("txn_id", TxnID);

			var Response = await Client.GetAsync(Request);
		}
	}
}
