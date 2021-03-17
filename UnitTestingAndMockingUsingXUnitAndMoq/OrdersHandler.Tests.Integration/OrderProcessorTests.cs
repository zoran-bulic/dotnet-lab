using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
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
        public async void CreateNewOrder_ShouldCreateNewOrder(string user, string address)
        {
            var ordersAtStart = await _orderProcessor.GetAllOrders();
            int nbrOfOrdersAtStart = ordersAtStart.Count;
            await _orderProcessor.CreateNewOrder(user, address);            
            var ordersAtEnd = await _orderProcessor.GetAllOrders();
            int nbrOfOrdersAtEnd = ordersAtEnd.Count;
            Assert.True(nbrOfOrdersAtEnd > nbrOfOrdersAtStart);
        }

        [Fact]
        public async void GetOrder_ShouldReturnValidOrder_ForValidId()
        {            
            var orders = await _orderProcessor.GetAllOrders();            
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
        public async void GetOrder_ShouldReturnNull_ForInvalidId(int id)
        {
            var order = await _orderProcessor.GetOrder(id);
            Assert.Null(order);       
        }

        [Theory]
        [InlineData("Wien", OrderState.Sent)]
        public async void UpdateOrder_ShouldUpdate_ForValidData(string address, OrderState state)
        {
            var orders = await _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);                        
            await _orderProcessor.UpdateAddressAndStateOfOrder(ranIdx, address, state);            
            var updatedOrder = await _orderProcessor.GetOrder(ranIdx);
            Assert.Equal(address, updatedOrder.Address);
            Assert.Equal(state, updatedOrder.State);
        }

        [Theory]
        [InlineData("", OrderState.Sent)]        
        public async void UpdateOrder_ShouldFail_ForInvalidData(string address, OrderState state)
        {
            var orders = await _orderProcessor.GetAllOrders();            
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);            
            await Assert.ThrowsAsync<ArgumentException>(() => _orderProcessor.UpdateAddressAndStateOfOrder(ranIdx, address, state));            
        }


        [Fact]
        public async void IsOrderDelivered_ShouldReturnTrueForDeliveredOrder()
        {            
            var orders = await _orderProcessor.GetAllOrders();
            Random r = new Random();
            int ranIdx = r.Next(0, orders.Count);                        
            await _orderProcessor.UpdateAddressAndStateOfOrder(ranIdx, "Wien", OrderState.Delivered);
            var output = _orderProcessor.IsOrderDelivered(ranIdx);
            Assert.True(output);
        }

        [Fact]
        public async void IsOrderDelivered_ShouldReturnFalseForNotDeliveredOrder()
        {            
            var orders = await _orderProcessor.GetAllOrders();
            
            List<OrderModel> ordersList = new List<OrderModel>(orders);
            var notDeliveredOrder = ordersList.Find(x => x.State != OrderState.Delivered);
            Assert.NotNull(notDeliveredOrder);                        
            
            _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);            
            var output = _orderProcessor.IsOrderDelivered(notDeliveredOrder.Id);
            Assert.False(output);
        }
    }
}
