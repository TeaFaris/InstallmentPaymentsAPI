#nullable disable

using InstallmentPaymentsAPI.Models.NonDB;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InstallmentPaymentsAPI.Models
{
    public class Category
    {
        [Key]
        public uint ID { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
		[DataType(DataType.Text)]
		[DisplayName("Название")]
		public string Name { get; set; }
        [Required(ErrorMessage = "Installment percentage is required")]
        [DisplayName("Процент рассрочки")]
        public decimal InstallmentPercentage { get; set; }
		[Required(ErrorMessage = "Installment minimum range is required")]
		[DisplayName("Минимальный диапазон рассрочки в месяцах")]
		public InstallmentRange InstallmentRangeMin { get; set; }
		[Required(ErrorMessage = "Installment maximum range is required")]
		[DisplayName("Максимальный диапазон рассрочки в месяцах")]
		public InstallmentRange InstallmentRangeMax { get; set; }

		[JsonIgnore]
		public List<Product> Products { get; set; }
	}
}
