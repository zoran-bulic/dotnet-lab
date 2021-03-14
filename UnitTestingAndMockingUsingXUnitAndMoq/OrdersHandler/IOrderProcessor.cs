using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler
{
    public interface IOrderProcessor
    {
        List<OrderModel> GetAllOrders();
        Task<OrderModel> GetOrder(int orderId);
        List<OrderModel> GetUndeliveredOrdersForUser(string user);
        List<OrderModel> GetDeliveredOrdersForUser(string user);
        bool IsOrderDelivered(int orderId);
        void DeliverOrder(int orderId, DateTime delivered);
        void UpdateAddressAndStateOfOrder(int orderId, string address, OrderState state);
        Task<int> CreateNewOrder(string user, string address);
    }
}
