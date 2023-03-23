using InstallmentPaymentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InstallmentPaymentsAPI.Data
{
	public class APIContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }

		public APIContext(DbContextOptions<APIContext> Options)
			: base(Options)
		{
		}
	}
}
