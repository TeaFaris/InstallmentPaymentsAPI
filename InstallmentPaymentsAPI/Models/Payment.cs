#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstallmentPaymentsAPI.Models
{
	public class Payment
	{
		[Key]
		public uint ID { get; set; }
		[Required]
		public uint ProductID { get; set; }
		[ForeignKey(nameof(ProductID))]
		public Product Product { get; set; }
		[Required]
		[DataType(DataType.Currency)]
		[DisplayName("Цена")]
		public decimal Amount { get; set; }
		[Required]
		[DataType(DataType.PhoneNumber)]
		[DisplayName("Номер телефона")]
		public string PhoneNumber { get; set; }
		[Required]
		[DisplayName("Продолжительность рассрочки в месяцах")]
		public uint InstallmentDuration { get; set; }
	}
}
