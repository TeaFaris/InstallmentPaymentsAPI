using InstallmentPaymentsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InstallmentPaymentsAPI.Data
{
	public class APIContext : DbContext
	{
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; } // Using a common database between the online store and the installment payment api service so that the products and categories are the same as in the online store
		public DbSet<InstallmentPayment> InstallmentPayments { get; set; }

		public APIContext(DbContextOptions<APIContext> Options)
			: base(Options)
		{
		}
	}
}
