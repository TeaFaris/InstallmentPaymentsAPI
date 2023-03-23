#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstallmentPaymentsAPI.Models
{
	public class Product
	{
		[Key]
		public uint ID { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Название не может быть пустым.")]
		[DataType(DataType.Text)]
		[DisplayName("Название")]
		public string Name { get; set; }
		[Required]
		[DataType(DataType.Currency)]
		[DisplayName("Цена")]
		public decimal Price { get; set; }
		[Required]
		public uint CategoryID { get; set; }
		[ForeignKey(nameof(CategoryID))]
		public Category Category { get; set; }
	}
}
