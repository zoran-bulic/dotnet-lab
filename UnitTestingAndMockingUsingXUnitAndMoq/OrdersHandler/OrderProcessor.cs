using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler
{
    public class OrderProcessor : IOrderProcessor
    {
        private IDbDataAccess _database;

        private enum DateComparisonResult
        {
            Earlier = -1,
            Later = 1,
            TheSame = 0
        };

        public OrderProcessor(IDbDataAccess database)
        {
            _database = database;
        }

        public async Task<IList<OrderModel>> GetAllOrders()
        {
            string sql = $"select * from Shipment";
            var orders = await _database.LoadDataAsync<OrderModel>(sql);
            return orders;
        }
        public async Task<OrderModel> GetOrder(int orderId)
        {
            string sql = $"select * from Shipment where Id={orderId}";
            var order = await _database.LoadDataAsync<OrderModel>(sql, orderId);
            return order;
        }
        public async Task<IList<OrderModel>> GetUndeliveredOrdersForUser(string user)
        {
            string sql = $"select * from Shipment where User='{user}' and State!='{OrderState.Delivered}'";
            var orders = await _database.LoadDataAsync<OrderModel>(sql);
            return orders;
        }

        public async Task<IList<OrderModel>> GetDeliveredOrdersForUser(string user)
        {
            string sql = $"select * from Shipment where User='{user}' and State='{OrderState.Delivered}'";
            var orders = await _database.LoadDataAsync<OrderModel>(sql);
            return orders;
        }

        public async Task<bool> IsOrderDelivered(int orderId)
        {
            bool orderIsDelivered = false;
            string sql = $"select * from Shipment where Id={orderId}";
            var order = await _database.LoadDataAsync<OrderModel>(sql, orderId);
            if(order.State == OrderState.Delivered)
            {
                orderIsDelivered = true;
            }            
            return orderIsDelivered;
        }

        public async Task DeliverOrder(int orderId, DateTime delivered)
        {
            string sql = $"select * from Shipment where Id={orderId}";
            var order = await _database.LoadDataAsync<OrderModel>(sql, orderId);
            order.DeliveryDate = delivered;

            DateComparisonResult result = (DateComparisonResult)delivered.CompareTo(order.CreationDate);

            if (result == DateComparisonResult.Earlier)
            {
                throw new ArgumentException("DeliveryDate earlier than CreationDate", "DeliveryDate");
            }

            string deliveryDateTimeFormated = order.DeliveryDate.ToString("yyyy-MM-dd HH:MM:ss");
            string sqlUpdate = $"UPDATE Shipment SET DeliveryDate='{deliveryDateTimeFormated}', State='{OrderState.Delivered}' WHERE Id='{orderId}'";
            await _database.UpdateDataAsync<OrderModel>(sqlUpdate, order);
        }

        public async Task UpdateAddressAndStateOfOrder(int orderId, string address, OrderState state)
        {           
            if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address is null or empty", "Address");
            }

            string sql = $"select * from Shipment where Id={orderId}";
            var order = await _database.LoadDataAsync<OrderModel>(sql, orderId);
            order.Address = address;
            order.State = state;

            sql = $"UPDATE Shipment SET Address='{address}', State='{state}' WHERE Id='{orderId}'";            
            await _database.UpdateDataAsync<OrderModel>(sql, order);            
        }

        public async Task<int> CreateNewOrder(string user, string address)
        {
            int createdOrderId;
            OrderModel order = new OrderModel
            {
                User = user,
                CreationDate = DateTime.Now,
                Address = address,
                State = OrderState.Created
            };
            string sql = $"INSERT INTO Shipment (User, CreationDate, Address, State) VALUES (@User, @CreationDate, @Address, @State); SELECT last_insert_rowid();";            
            createdOrderId = await _database.InsertDataAsync<OrderModel>(sql, order);
            return createdOrderId;
        }
    }
}
