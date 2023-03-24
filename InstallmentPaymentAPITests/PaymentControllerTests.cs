using InstallmentPaymentsAPI.Controllers;
using InstallmentPaymentsAPI.Data;
using InstallmentPaymentsAPI.Models;
using InstallmentPaymentsAPI.Models.NonDB;
using InstallmentPaymentsAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace InstallmentPaymentAPITests
{
	public class PaymentControllerTests : IDisposable
	{
		private readonly Mock<ISmsService> MockSmsService;
		private readonly APIContext MockDatabase;

		public PaymentControllerTests()
		{
			MockSmsService = new Mock<ISmsService>();

			var Options = new DbContextOptionsBuilder<APIContext>()
				.UseInMemoryDatabase("MockDatabase")
				.Options;

			MockDatabase = new APIContext(Options);
		}

		[Theory]

		[InlineData(1000, InstallmentRange.ThreeMonth, 1030)]
		[InlineData(1000, InstallmentRange.SixMonth, 1060)]
		[InlineData(1000, InstallmentRange.NineMonth, 1090)]
		[InlineData(1000, InstallmentRange.TwelveMonth, 1120)]
		[InlineData(1000, InstallmentRange.EighteenMonth, 1150)]
		[InlineData(1000, InstallmentRange.TwentyFourMonth, 1180)]

		[InlineData(1500, InstallmentRange.ThreeMonth, 1545)]
		[InlineData(1500, InstallmentRange.SixMonth, 1590)]
		[InlineData(1500, InstallmentRange.NineMonth, 1635)]
		[InlineData(1500, InstallmentRange.TwelveMonth, 1680)]
		[InlineData(1500, InstallmentRange.EighteenMonth, 1725)]
		[InlineData(1500, InstallmentRange.TwentyFourMonth, 1770)]
		public async Task ChargePayment_CalculatesExpectedAmount_WithInstallment(
			decimal Amount,
			InstallmentRange InstallmentRange,
			decimal ExpectedAmount)
		{
			// Arrage
			var Category = new Category
			{
				InstallmentPercentage = 3,
				Name = "TestCategory",
				InstallmentRangeMin = InstallmentRange.ThreeMonth,
				InstallmentRangeMax = InstallmentRange.TwentyFourMonth,
			};
			MockDatabase.Add(Category);
			var Product = new Product
			{
				CategoryID = Category.ID,
				Name = "TestProduct",
				Price = Amount
			};
			MockDatabase.Add(Product);
			var Payment = new PaymentRequest
			{
				ProductID = Product.ID,
				Amount = Product.Price,
				PhoneNumber = "+992123121231",
				InstallmentDuration = (uint)InstallmentRange
			};

			await MockDatabase.SaveChangesAsync();

			MockSmsService.Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			var Controller = new PaymentController(MockDatabase, MockSmsService.Object);

			// Act
			var Result = await Controller.ChargePayment(Payment);
			var StatusResult = (ObjectResult)Result;
			var ValueResult = (InstallmentPayment)StatusResult.Value!;

			// Assert
			Assert.Equal(ExpectedAmount, ValueResult.AmountWithInstallment);
		}

		[Theory]

		[InlineData(InstallmentRange.SixMonth, InstallmentRange.NineMonth, 3, 400)]
		[InlineData(InstallmentRange.SixMonth, InstallmentRange.NineMonth, 12, 400)]
		[InlineData(InstallmentRange.SixMonth, InstallmentRange.NineMonth, 7, 400)]
		[InlineData(InstallmentRange.SixMonth, InstallmentRange.NineMonth, 6, 200)]
		[InlineData(InstallmentRange.SixMonth, InstallmentRange.NineMonth, 9, 200)]

		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 1, 400)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 25, 400)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 4, 400)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 3, 200)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 6, 200)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 9, 200)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 18, 200)]
		[InlineData(InstallmentRange.ThreeMonth, InstallmentRange.TwentyFourMonth, 24, 200)]

		public async Task ChargePayment_ReturnsExpectedStatusCode_WithInstallmentRanges(
			InstallmentRange MinRange,
			InstallmentRange MaxRange,
			uint InstallmentDuration,
			int ExpectedStatusCode)
		{
			// Arrage
			var Category = new Category
			{
				InstallmentPercentage = 3,
				Name = "TestCategory",
				InstallmentRangeMin = MinRange,
				InstallmentRangeMax = MaxRange,
			};
			MockDatabase.Add(Category);
			var Product = new Product
			{
				CategoryID = Category.ID,
				Name = "TestProduct",
				Price = 1000
			};
			MockDatabase.Add(Product);
			var Payment = new PaymentRequest
			{
				ProductID = Product.ID,
				Amount = Product.Price,
				PhoneNumber = "+992123121231",
				InstallmentDuration = InstallmentDuration
			};

			await MockDatabase.SaveChangesAsync();

			MockSmsService.Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			var Controller = new PaymentController(MockDatabase, MockSmsService.Object);

			// Act
			var Result = await Controller.ChargePayment(Payment);
			var StatusResult = (ObjectResult)Result;

			// Assert
			Assert.Equal(ExpectedStatusCode, StatusResult.StatusCode);
		}

		[Fact]
		public async Task ChargePayment_ReturnsNotFound_WithInvalidProductID()
		{
			// Arrage
			var Payment = new PaymentRequest
			{
				ProductID = 0,
				Amount = 1000,
				PhoneNumber = "+992123121231",
				InstallmentDuration = 3
			};

			MockSmsService.Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(Task.CompletedTask);

			var Controller = new PaymentController(MockDatabase, MockSmsService.Object);

			// Act
			var Result = await Controller.ChargePayment(Payment);
			var StatusResult = (ObjectResult)Result;

			// Assert
			Assert.Equal(404, StatusResult.StatusCode);
		}

		public void Dispose()
		{
			MockDatabase.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}