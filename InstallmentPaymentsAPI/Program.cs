using InstallmentPaymentsAPI.Configs;
using InstallmentPaymentsAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace InstallmentPaymentsAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var Builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			Builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			Builder.Services.AddEndpointsApiExplorer();
			Builder.Services.AddSwaggerGen();

			var ConnectionString = Builder.Configuration.GetConnectionString("PostgreSQLConnection");
			Builder.Services.AddDbContext<APIContext>(Options => Options.UseNpgsql(ConnectionString));

			var OsonSMSConfig = Builder.Configuration.GetSection(nameof(OsonSMSOptions));
			Builder.Services.Configure<OsonSMSOptions>(OsonSMSConfig);

			var App = Builder.Build();

			// Configure the HTTP request pipeline.
			if (App.Environment.IsDevelopment())
			{
				App.UseSwagger();
				App.UseSwaggerUI();
			}

			App.UseHttpsRedirection();

			App.UseAuthorization();

			App.MapControllers();

			App.Run();
		}
	}
}