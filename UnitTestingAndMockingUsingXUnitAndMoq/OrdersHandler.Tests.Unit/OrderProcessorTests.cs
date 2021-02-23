using System;
using Xunit;
using OrdersHandler;
using Moq;
using OrdersHandler.Models;
using OrdersHandler.DataAccess;

namespace OrdersHandler.Tests.Unit
{
    public class OrderProcessorTests
    {
        private OrderProcessor _orderProcessor;
        Mock<IDbDataAccess> _dbDataAccess = new Mock<IDbDataAccess>();

        public OrderProcessorTests()
        {
            _orderProcessor = new OrderProcessor(_dbDataAccess.Object);
        }

        [Fact]
        public void GetAllOrders_ReturnsValidData()
        {            

        }
    }
}
