using System.Collections.Generic;
using OrdersHandler.Models;

namespace OrdersHandler.UI.Models
{
    public class OrdersModel
    {
        public IList<OrdersHandler.Models.OrderModel> Orders { get; }
        public OrdersModel(IList<OrdersHandler.Models.OrderModel> orders)
        {
            Orders = orders;
        }

        public OrdersModel(OrdersHandler.Models.OrderModel order)
        {
            if(order!=null)
            {
                Orders = new List<OrderModel>() { order };
            }            
        }
    }
}
