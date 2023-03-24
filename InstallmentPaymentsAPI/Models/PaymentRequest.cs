#nullable disable

using InstallmentPaymentsAPI.Models.NonDB;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InstallmentPaymentsAPI.Models
{
	public class PaymentRequest
	{
		[Required(ErrorMessage = "Product ID is required")]
		public uint ProductID { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(ProductID))]
		public Product Product { get; set; }

		[Required(ErrorMessage = "Amount is required")]
		[DataType(DataType.Currency)]
		[DisplayName("Цена")]
		[Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than zero")]
		public decimal Amount { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[DataType(DataType.PhoneNumber)]
		[DisplayName("Номер телефона")]
		[Phone]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Installment duration is required")]
		[DisplayName("Продолжительность рассрочки в месяцах")]
		[Range((uint)InstallmentRange.ThreeMonth, (uint)InstallmentRange.TwentyFourMonth, ErrorMessage = "Installment duration must be between 3 and 18 months")]
		public uint InstallmentDuration { get; set; }
	}
}
