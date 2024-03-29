using System;
using Xunit;
using Moq;
using OrdersHandler.Models;
using OrdersHandler.DataAccess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler.Tests.Unit
{
    public class OrderProcessorTests
    {
        private OrderProcessor _orderProcessor;
        Mock<IDbDataAccess> _dbDataAccessMock = new Mock<IDbDataAccess>();

        public OrderProcessorTests()
        {
            _orderProcessor = new OrderProcessor(_dbDataAccessMock.Object);
        }

        [Fact]
        public async void GetAllOrders_ShouldReturnValidData()
        {            
            string sql = "select * from Shipment";
            List<OrderModel> expected = GetSampleOrders();
            _dbDataAccessMock.Setup(x => x.LoadDataAsync<OrderModel>(sql))
                .ReturnsAsync(expected);
            var actual = await _orderProcessor.GetAllOrders();            
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].User, actual[i].User);
            }            
        }

        [Fact]
        public async void GetOrder_ReturnsValidData_ForValidOrderId()
        {            
            OrderModel expected = GetSampleOrder();            
            string sql = $"select * from Shipment where Id={expected.Id}";                        
            _dbDataAccessMock.Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);
            var actual = await _orderProcessor.GetOrder(expected.Id);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void IsOrderDelivered_ShouldReturnsTrue_ForDeliveredOrders()
        {
            OrderModel expected = GetDeliveredSampleOrder();            
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);
            var actual = await _orderProcessor.IsOrderDelivered(expected.Id);            
            Assert.True(actual);
        }

        [Fact]
        public async void IsOrderDelivered_ShouldReturnsFalse_ForNotDeliveredOrders()
        {
            OrderModel expected = GetNotDeliveredSampleOrder();
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);            
            var actual = await _orderProcessor.IsOrderDelivered(expected.Id);            
            Assert.False(actual);
        }

        [Theory]
        [InlineData("Wien", OrderState.Sent)]
        public async void UpdateOrder_UpdateDataMethodShouldBeCalledOnlyOnce_ForValidData(string address, OrderState state)
        {            
            OrderModel expected = GetSampleOrder();
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);         
            var actual = await _orderProcessor.GetOrder(expected.Id);
            await _orderProcessor.UpdateAddressAndStateOfOrder(actual.Id, address, state);            
            sql = $"UPDATE Shipment SET Address='{actual.Address}', State='{actual.State}' WHERE Id='{actual.Id}'";
            _dbDataAccessMock.Verify(x => x.UpdateDataAsync<OrderModel>(sql, actual), Times.Once);
        }

        [Theory]
        [InlineData("", OrderState.Sent)]
        [InlineData(" ", OrderState.Delivered)]        
        public async void UpdateOrder_UpdateFails_ForInvalidAddress(string address, OrderState state)
        {           
            OrderModel expected = GetSampleOrder();
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);            
            var actual = await _orderProcessor.GetOrder(expected.Id);            
            await Assert.ThrowsAsync<ArgumentException>("Address", () => _orderProcessor.UpdateAddressAndStateOfOrder(actual.Id, address, state));
            sql = $"UPDATE Shipment SET Address='{actual.Address}', State='{actual.State}' WHERE Id='{actual.Id}'";
            _dbDataAccessMock.Verify(x => x.UpdateDataAsync<OrderModel>(sql, actual), Times.Never);
        }

        [Fact]
        public async void DeliverOrder_UpdateDataMethodShouldBeCalledOnlyOnce()
        {
            OrderModel expected = GetSampleOrder();
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);            
            var actual = await _orderProcessor.GetOrder(expected.Id);
            DateTime deliveryDate = DateTime.Now;
            await _orderProcessor.DeliverOrder(actual.Id, deliveryDate);
            string deliveryDateFormated = deliveryDate.ToString("yyyy-MM-dd HH:MM:ss");
            sql = $"UPDATE Shipment SET DeliveryDate='{deliveryDateFormated}', State='{OrderState.Delivered}' WHERE Id='{actual.Id}'";
            _dbDataAccessMock.Verify(x => x.UpdateDataAsync<OrderModel>(sql, actual), Times.Once);
        }

        [Fact]
        public async void DeliverOrder_ShouldThrowException_ForInvalidDeliveryDate()
        {            
            OrderModel expected = GetSampleOrder();
            string sql = $"select * from Shipment where Id={expected.Id}";
            _dbDataAccessMock
                .Setup(x => x.LoadDataAsync<OrderModel>(sql, expected.Id))
                .ReturnsAsync(expected);
            var actual = await _orderProcessor.GetOrder(expected.Id);
            DateTime deliveryDate = actual.CreationDate.AddDays(-1);
            await Assert.ThrowsAsync<ArgumentException>(() => _orderProcessor.DeliverOrder(actual.Id, deliveryDate));
            string deliveryDateFormated = deliveryDate.ToString("yyyy-MM-dd HH:MM:ss");
            sql = $"UPDATE Shipment SET DeliveryDate='{deliveryDateFormated}', State='{OrderState.Delivered}' WHERE Id='{actual.Id}'";
            _dbDataAccessMock.Verify(x => x.UpdateDataAsync<OrderModel>(sql, actual), Times.Never);
        }

        #region Helper Methods
        private List<OrderModel> GetSampleOrders()
        {
            List<OrderModel> output = new List<OrderModel>
            {
                new OrderModel
                {
                    Id = 1,
                    User = "User1",
                    CreationDate = new DateTime(2021,1,15,11,36,35),
                    Address = "Graz",
                    State = OrderState.Created
                },
                new OrderModel
                {
                    Id = 2,
                    User = "User1",
                    CreationDate = new DateTime(2021,2,5,10,36,20),
                    Address = "Graz-Mitte",
                    State = OrderState.Sent
                },
                new OrderModel
                {
                    Id = 3,
                    User = "User1",
                    CreationDate = new DateTime(2020,11,15,8,18,22),
                    Address = "Graz-Ost",
                    State = OrderState.Delivered
                },
                new OrderModel
                {
                    Id = 4,
                    User = "User2",
                    CreationDate = new DateTime(2020,12,24,20,20,20),
                    Address = "Wien",
                    State = OrderState.Created
                },
                new OrderModel
                {
                    Id = 5,
                    User = "User2",
                    CreationDate = new DateTime(2021,1,1,10,15,25),
                    Address = "Wiener Wald",
                    State = OrderState.Sent
                }
            };
            return output;
        }

        private OrderModel GetSampleOrder()
        {
            OrderModel output = new OrderModel
            {
                Id = 1,
                User = "User1",
                CreationDate = new DateTime(2021, 1, 15, 11, 36, 35),
                Address = "Graz",
                State = OrderState.Created
            };            
            return output;
        }

        private OrderModel GetDeliveredSampleOrder()
        {
            OrderModel output = new OrderModel
            {
                Id = 1,
                User = "User1",
                CreationDate = new DateTime(2021, 1, 15, 11, 36, 35),
                Address = "Graz",
                State = OrderState.Delivered
            };
            return output;
        }

        private OrderModel GetNotDeliveredSampleOrder()
        {
            OrderModel output = new OrderModel
            {
                Id = 1,
                User = "User2",
                CreationDate = new DateTime(2021, 1, 15, 11, 36, 35),
                Address = "Wien",
                State = OrderState.Sent
            };
            return output;
        }
        #endregion // Helper Methods
    }
}
