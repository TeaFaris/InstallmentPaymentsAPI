#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InstallmentPaymentsAPI.Models
{
	public class Product
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public uint ID { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
		[DataType(DataType.Text)]
		[DisplayName("Название")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Amount is required")]
		[DataType(DataType.Currency)]
		[DisplayName("Цена")]
		[Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than zero")]
		public decimal Price { get; set; }
		[Required(ErrorMessage = "Category ID is required")]
		public uint CategoryID { get; set; }

		[JsonIgnore]
		[ForeignKey(nameof(CategoryID))]
		public Category Category { get; set; }
	}
}
