using System.ComponentModel.DataAnnotations;

namespace InstallmentPaymentsAPI.Models.NonDB
{
	public enum InstallmentRange : uint
	{
		[Display(Name = "три месяца")]
		ThreeMonth = 3,
		[Display(Name = "шесть месяцов")]
		SixMonth = 6,
		[Display(Name = "девять месяцов")]
		NineMonth = 9,
		[Display(Name = "двенадацть месяцов")]
		TwelveMonth = 12,
		[Display(Name = "восемьнадцать месяцов")]
		EighteenMonth = 18,
		[Display(Name = "двадцать четыре месяца")]
		TwentyFourMonth = 24
	}
}
