using InstallmentPaymentsAPI.Configs;
using InstallmentPaymentsAPI.Data;
using InstallmentPaymentsAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
			Builder.Services.AddSwaggerGen(Options =>
			{
				var XmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				Options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, XmlFilename));
			});

			var ConnectionString = Builder.Configuration.GetConnectionString("PostgreSQLConnection");
			Builder.Services.AddDbContext<APIContext>(Options => Options.UseNpgsql(ConnectionString));

			var OsonSMSConfig = Builder.Configuration.GetSection(nameof(OsonSMSOptions));
			Builder.Services.Configure<OsonSMSOptions>(OsonSMSConfig);

			Builder.Services.AddScoped<ISmsService, OsonSmsService>();

			var App = Builder.Build();

			App.UseSwagger();
			App.UseSwaggerUI();

			App.UseHttpsRedirection();

			App.UseAuthorization();

			App.MapControllers();

			App.Run();
		}
	}
}