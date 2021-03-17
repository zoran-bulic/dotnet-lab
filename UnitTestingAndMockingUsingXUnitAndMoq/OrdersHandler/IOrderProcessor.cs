using OrdersHandler.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersHandler
{
    public interface IOrderProcessor
    {
        Task<IList<OrderModel>> GetAllOrders();
        Task<OrderModel> GetOrder(int orderId);
        Task<IList<OrderModel>> GetUndeliveredOrdersForUser(string user);
        Task<IList<OrderModel>> GetDeliveredOrdersForUser(string user);
        Task<bool> IsOrderDelivered(int orderId);
        Task DeliverOrder(int orderId, DateTime delivered);
        Task UpdateAddressAndStateOfOrder(int orderId, string address, OrderState state);
        Task<int> CreateNewOrder(string user, string address);
    }
}
