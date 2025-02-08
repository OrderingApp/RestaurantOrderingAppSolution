//using Application.Core.MappingProfiles;
//using Application.Dtos.Orders;
//using Application.Services;
//using AutoMapper;
//using Domain;
//using Infrastructure.Database;
//using Microsoft.EntityFrameworkCore;
//using RestaurantOrdering.Tests.TestHelpers;
//using System.Net;

//public class OrderServiceTests
//{
//    private readonly RestaurantOrderingContext _context;
//    private readonly OrderService _orderService;
//    private readonly IMapper _mapper;

//    public OrderServiceTests()
//    {
//        var mappingConfig = new MapperConfiguration(cfg =>
//        {
//            cfg.AddProfile<OrderMappingProfile>();
//        });
//        _mapper = mappingConfig.CreateMapper();

//        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
//            .UseInMemoryDatabase(databaseName: "TestDatabase")
//            .Options;

//        _context = new RestaurantOrderingContext(options);

//        _orderService = new OrderService(_context, _mapper);
//    }

//    [Fact]
//    public async Task SplitBill_ShouldCreateNewOrder_WithSelectedItems()
//    {
//        var orderId = Guid.NewGuid();
//        var orderItem1 = OrderItemTestingContext.OrderItem1;
//        var orderItem2 = new OrderItem
//        {
//            Id = Guid.NewGuid(),
//            MenuItemId = Guid.NewGuid(),
//            Price = 15,
//            Quantity = 1,
//            SpecialInstructions = "Test item 2",
//            OrderItemIngredients = new List<OrderItemIngredient>()
//        };

//        var originalOrder = new Order
//        {
//            Id = orderId,
//            TotalAmount = 35,
//            OrderType = OrderType.DineIn,
//            TableId = Guid.NewGuid(),
//            OrderItems = new List<OrderItem> { orderItem1, orderItem2 },
//            PaymentStatus = PaymentStatus.Pending
//        };

//        _context.Orders.Add(originalOrder);
//        await _context.SaveChangesAsync();
//        var orders = _context.Orders.ToList();

//        var splitBillDto = new SplitBillDto
//        {
//            OrderItemIds = new List<Guid> { orderItem1.Id }
//        };

//        var result = await _orderService.SplitBill(splitBillDto, orderId);

//        Assert.NotNull(result);
//        if (!result.IsSuccess)
//        {
//            Console.WriteLine($"Error: {result.ErrorMessage}");
//        }
//        //Assert.True(result.IsSuccess);
//        //Assert.Equal(HttpStatusCode.Created, result.HttpStatusCode);
//        //Assert.NotNull(result.Data);
//        //Assert.Single(result.Data.OrderItems);
//        //Assert.Equal(orderItem1.Id, result.Data.OrderItems.First().Id);

//        //Assert.Single(originalOrder.OrderItems);
//        //Assert.Equal(orderItem2.Id, originalOrder.OrderItems.First().Id);
//        //Assert.Equal(15, originalOrder.TotalAmount);
//    }
//}
