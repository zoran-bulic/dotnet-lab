using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using System;
using System.Collections.Generic;
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
            _connStringSettings = new ConnectionStringSettings("SQLite", @"Data Source=.\OrdersHandlerDb.db;Version=3;", "OrdersHandler.Tests.Integration");
            _sqliteDataAccess = new SqliteDataAccess(_connStringSettings);
            _orderProcessor = new OrderProcessor(_sqliteDataAccess);
        }

        [Theory]
        [InlineData("TestUser1", "Wien")]
        [InlineData("TestUser2", "Graz")]
        [InlineData("TestUser2", "Leibnitz")]
        public void CreateNewOrder_ShouldCreateNewOrder(string user, string address)
        {
            // Arrange 
            int nbrOfOrdersAtStart = _orderProcessor.GetAllOrders().Count;

            // Act
            _orderProcessor.CreateNewOrder(user, address);

            // Assert
            int nbrOfOrdersAtEnd = _orderProcessor.GetAllOrders().Count;
            Assert.True(nbrOfOrdersAtEnd > nbrOfOrdersAtStart);
        }


        [Fact]
        public void GetOrder_ShouldReturnOrder_ForValidId()
        {
            // Arrange 
            var orders = _orderProcessor.GetAllOrders();               

            // Assert
            foreach (var item in orders)
            {
                Assert.NotNull(_orderProcessor.GetOrder(item.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetOrder_ShouldReturnNull_ForInvalidId(int id)
        {
            // Assert
            Assert.Null(_orderProcessor.GetOrder(id));       
        }

        [Theory]
        [InlineData("Wien", OrderState.Sent)]
        [InlineData("Graz", OrderState.Delivered)]
        public void UpdateOrder_ShouldUpdate_ForValidData(string address, OrderState state)
        {
            // Arrange 
            var orders = _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);

            // Act            
            _orderProcessor.UpdateOrder(ranIdx, address, state);

            // Assert
            var updatedOrder = _orderProcessor.GetOrder(ranIdx);
            Assert.Equal(address, updatedOrder.Address);
            Assert.Equal(state, updatedOrder.State);
        }

        [Theory]
        [InlineData("", OrderState.Sent)]        
        public void UpdateOrder_ShouldFail_ForInvalidData(string address, OrderState state)
        {
            // Arrange 
            var orders = _orderProcessor.GetAllOrders();            
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);

            // Act            
            Assert.Throws<ArgumentException>("Address", () => _orderProcessor.UpdateOrder(ranIdx, address, state));  
        }


        [Fact]
        public void IsOrderDelivered_ShouldReturnTrueForDeliveredOrder()
        {
            // Arrange 
            var orders = _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);

            // Act            
            _orderProcessor.UpdateOrder(ranIdx, "Wien", OrderState.Delivered);

            // Assert
            var output = _orderProcessor.IsOrderDelivered(ranIdx);
            Assert.True(output);
        }

        [Fact]
        public void IsOrderDelivered_ShouldReturnFalseForNotDeliveredOrder()
        {
            // Arrange 
            var orders = _orderProcessor.GetAllOrders();
            var notDeliveredOrder = orders.Find(x => x.State != OrderState.Delivered);
            Assert.NotNull(notDeliveredOrder);

            // Act            
            _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);

            // Assert
            var output = _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);
            Assert.False(output);
        }
    }
}
