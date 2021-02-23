﻿using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersHandler
{
    public class OrderProcessor : IOrderProcessor
    {
        private IDbDataAccess _database;

        public OrderProcessor(IDbDataAccess database)
        {
            _database = database;
        }

        public List<OrderModel> GetAllOrders()
        {
            string sql = $"select * from Shipment";
            var orders = _database.LoadData<OrderModel>(sql);
            return orders;
        }
        public OrderModel GetOrder(int orderId)
        {
            string sql = $"select * from Shipment where Id={orderId}";
            var order = _database.LoadData<OrderModel>(sql, orderId);
            return order;
        }
        public List<OrderModel> GetUndeliveredOrdersForUser(string user)
        {
            string sql = $"select * from Shipment where User='{user}' and State!='{OrderState.Delivered}'";
            var orders = _database.LoadData<OrderModel>(sql);
            return orders;
        }

        public List<OrderModel> GetDeliveredOrdersForUser(string user)
        {
            string sql = $"select * from Shipment where User='{user}' and State='{OrderState.Delivered}'";
            var orders = _database.LoadData<OrderModel>(sql);
            return orders;
        }

        public bool IsOrderDelivered(int orderId)
        {
            bool orderIsDelivered = false;
            string sql = $"select * from Shipment where Id={orderId}";
            var order = _database.LoadData<OrderModel>(sql, orderId);
            if(order.State == OrderState.Delivered)
            {
                orderIsDelivered = true;
            }            
            return orderIsDelivered;
        }

        public void DeliverOrder(int orderId, DateTime delivered)
        {
            string sql = $"select * from Shipment where Id={orderId}";
            var order = _database.LoadData<OrderModel>(sql, orderId);
            order.DeliveryDate = delivered;

            string deliveryDateTimeFormated = order.DeliveryDate.ToString("yyyy-MM-dd HH:MM:ss");
            string sqlUpdate = $"UPDATE Shipment SET DeliveryDate='{deliveryDateTimeFormated}', State='{OrderState.Delivered}' WHERE Id='{orderId}'";
            _database.UpdateData<OrderModel>(sqlUpdate, order);
        }

        public void UpdateOrder(int orderId, string address, OrderState state)
        {
            string sql = $"select * from Shipment where Id={orderId}";
            var order = _database.LoadData<OrderModel>(sql, orderId);
            order.Address = address;
            order.State = state;

            sql = $"UPDATE Shipment SET Address='{address}', State='{state}' WHERE Id='{orderId}'";
            _database.UpdateData<OrderModel>(sql, order);
        }

        public void CreateNewOrder(string user, string address)
        {
            OrderModel order = new OrderModel
            {
                User = user,
                CreationDate = DateTime.Now,
                Address = address,
                State = OrderState.Created
            };
            string sql = $"INSERT INTO Shipment (User, CreationDate, Address, State) VALUES (@User, @CreationDate, @Address, @State);";
            _database.SaveData<OrderModel>(sql, order);
        }
    }
}