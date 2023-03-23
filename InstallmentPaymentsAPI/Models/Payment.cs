#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InstallmentPaymentsAPI.Models
{
	public class Payment
	{
		[Key]
		public uint ID { get; set; }

		[Required(ErrorMessage = "Product ID is required")]
		public Product Product { get; set; }

		[Required(ErrorMessage = "Amount is required")]
		[DataType(DataType.Currency)]
		[DisplayName("Цена")]
		[Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than zero")]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[DataType(DataType.PhoneNumber)]
		[DisplayName("Номер телефона")]
		[RegularExpression(@"/^\+992\d{9}$/", ErrorMessage = "Invalid phone number format")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Installment duration is required")]
		[DisplayName("Продолжительность рассрочки в месяцах")]
		[Range(3, 18, ErrorMessage = "Installment duration must be between 3 and 18 months")]
		public uint InstallmentDuration { get; set; }
	}
}
