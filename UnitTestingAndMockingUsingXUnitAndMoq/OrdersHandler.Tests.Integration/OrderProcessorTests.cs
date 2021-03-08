using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using System;
using System.Configuration;
using Xunit;

namespace OrdersHandler.Tests.Integration
{
    public class OrderProcessorTests
    {
        private ConnectionStringSettings _connStringSettings;
        private IDbDataAccess _sqliteDataAccess;
        private OrderProcessor _orderProcessor;

        /// <summary>
        /// Integration tests are executed against the test database  <param name="_connStringSettings"></param> which by default contains few sample orders
        /// </summary>         
        public OrderProcessorTests()
        {
            _connStringSettings = new ConnectionStringSettings("SQLite", @"Data Source=.\OrdersHandlerTestDB.db;Version=3;", "OrdersHandler.Tests.Integration");
            _sqliteDataAccess = new SqliteDataAccess(_connStringSettings);
            _orderProcessor = new OrderProcessor(_sqliteDataAccess);
        }

        [Theory]
        [InlineData("TestUser1", "Wien")]
        [InlineData("TestUser2", "Graz")]
        [InlineData("TestUser2", "Leibnitz")]
        public void CreateNewOrder_ShouldCreateNewOrder(string user, string address)
        {
            int nbrOfOrdersAtStart = _orderProcessor.GetAllOrders().Count;            
            _orderProcessor.CreateNewOrder(user, address);            
            int nbrOfOrdersAtEnd = _orderProcessor.GetAllOrders().Count;
            Assert.True(nbrOfOrdersAtEnd > nbrOfOrdersAtStart);
        }

        [Fact]
        public void GetOrder_ShouldReturnOrder_ForValidId()
        {            
            var orders = _orderProcessor.GetAllOrders();            
            foreach (var item in orders)
            {
                Assert.NotNull(_orderProcessor.GetOrder(item.Id));
            }
        }

        /// <summary>
        /// Method to test the orders with invalid Id. 
        /// </summary>
        /// <remarks>Invalid Id are zero or negative integer number for <paramref name="id"/></remarks>
        /// <param name="id"></param>
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetOrder_ShouldReturnNull_ForInvalidId(int id)
        {
            Assert.Null(_orderProcessor.GetOrder(id));       
        }

        [Theory]
        [InlineData("Wien", OrderState.Sent)]
        public void UpdateOrder_ShouldUpdate_ForValidData(string address, OrderState state)
        {
            var orders = _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);                        
            _orderProcessor.UpdateOrder(ranIdx, address, state);            
            var updatedOrder = _orderProcessor.GetOrder(ranIdx);
            Assert.Equal(address, updatedOrder.Address);
            Assert.Equal(state, updatedOrder.State);
        }

        [Theory]
        [InlineData("", OrderState.Sent)]        
        public void UpdateOrder_ShouldFail_ForInvalidData(string address, OrderState state)
        {
            var orders = _orderProcessor.GetAllOrders();            
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);                        
            Assert.Throws<ArgumentException>("Address", () => _orderProcessor.UpdateOrder(ranIdx, address, state));  
        }


        [Fact]
        public void IsOrderDelivered_ShouldReturnTrueForDeliveredOrder()
        {            
            var orders = _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);                        
            _orderProcessor.UpdateOrder(ranIdx, "Wien", OrderState.Delivered);            
            var output = _orderProcessor.IsOrderDelivered(ranIdx);
            Assert.True(output);
        }

        [Fact]
        public void IsOrderDelivered_ShouldReturnFalseForNotDeliveredOrder()
        {            
            var orders = _orderProcessor.GetAllOrders();
            var notDeliveredOrder = orders.Find(x => x.State != OrderState.Delivered);
            Assert.NotNull(notDeliveredOrder);                        
            _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);            
            var output = _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);
            Assert.False(output);
        }
    }
}
