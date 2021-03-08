using OrdersHandler.Models;
using System;
using System.Collections.Generic;

namespace OrdersHandler
{
    public interface IOrderProcessor
    {
        List<OrderModel> GetAllOrders();
        OrderModel GetOrder(int orderId);
        List<OrderModel> GetUndeliveredOrdersForUser(string user);
        List<OrderModel> GetDeliveredOrdersForUser(string user);
        bool IsOrderDelivered(int orderId);
        void DeliverOrder(int orderId, DateTime delivered);
        void UpdateOrder(int orderId, string address, OrderState state);
        void CreateNewOrder(string user, string address);
    }
}
