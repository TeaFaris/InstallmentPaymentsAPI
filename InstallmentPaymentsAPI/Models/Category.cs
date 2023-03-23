#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InstallmentPaymentsAPI.Models
{
    public class Category
    {
        [Key]
        public uint ID { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Название не может быть пустым.")]
		[DataType(DataType.Text)]
		[DisplayName("Название")]
		public string Name { get; set; }
        public List<Product> Products { get; set; }
        [Required]
        [DisplayName("Процент рассрочки")]
        public decimal InstallmentPercentage { get; set; }
		[Required]
		[DisplayName("Минимальный диапазон рассрочки в месяцах")]
		public uint InstallmentRangeMin { get; set; }
		[Required]
		[DisplayName("Максимальный диапазон рассрочки в месяцах")]
		public uint InstallmentRangeMax { get; set; }
	}
}
