namespace InstallmentPaymentsAPI.Services
{
	public interface ISmsService // We don't care about respose, this service only for sending
	{
		Task SendSMS(string PhoneNumber, string Message);
	}
}