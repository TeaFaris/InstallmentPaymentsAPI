using Microsoft.EntityFrameworkCore;

namespace InstallmentPaymentsAPI.Data
{
	public class APIContext : DbContext
	{
		public APIContext(DbContextOptions<APIContext> Options)
			: base(Options)
		{
		}
	}
}
