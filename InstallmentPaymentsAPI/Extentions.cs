using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace InstallmentPaymentsAPI
{
	public static class Extentions
	{
		/// <summary>
		///     Method for getting an enum <see cref="{TAttribute}"/>.
		/// </summary>
		public static TAttribute GetAttribute<TAttribute>(this Enum Enum)
				where TAttribute : Attribute
		{
			return Enum.GetType()
							.GetMember(Enum.ToString())
							.First()
							.GetCustomAttribute<TAttribute>()!;
		}

		/// <summary>
		///		Method to get human display <see cref="string"/>.
		///		Usually gets the display <see cref="string"/> via the <see cref="DisplayAttribute"/>.
		/// </summary>
		public static string GetDisplayName(this Enum Enum)
		{
			return Enum.GetAttribute<DisplayAttribute>()
							.Name!;
		}
	}
}
