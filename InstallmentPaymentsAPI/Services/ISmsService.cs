namespace InstallmentPaymentsAPI.Services
{
	public interface ISmsService
	{
		void SendSMS(string PhoneNumber, string Message);
	}
}