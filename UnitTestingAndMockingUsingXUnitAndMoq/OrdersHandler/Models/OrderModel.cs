using System;

namespace OrdersHandler.Models
{
    public enum OrderState
    {
        Created,
        Sent,
        Delivered
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Address { get; set; }
        public OrderState State { get; set; }
    }
}
