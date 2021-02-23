using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrdersHandler.Models;

namespace OrdersHandler.UI.Models
{
    public class OrdersModel
    {
        public List<OrdersHandler.Models.OrderModel> Orders { get; }
        public OrdersModel(List<OrdersHandler.Models.OrderModel> orders)
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
