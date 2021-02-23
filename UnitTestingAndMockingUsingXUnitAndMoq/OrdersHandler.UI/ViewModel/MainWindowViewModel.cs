using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using OrdersHandler.UI.Commands;
using OrdersHandler.UI.Models;

namespace OrdersHandler.UI.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private int orderId;
        private string user;
        private string message;
        private DateTime deliveryDate;
        private string address;
        private OrderState state;
        private OrdersModel ordersModel;        

        private ICommand getAllOrdersCommand;
        private ICommand getOrderCommand;
        private ICommand getUndeliveredOrdersForUserCommand;
        private ICommand getDeliveredOrdersForUserCommand;
        private ICommand isOrderDeliveredCommand;
        private ICommand deliverOrderCommand;
        private ICommand updateOrderCommand;
        private ICommand createNewOrderCommand;

        IOrderProcessor orderProcessor = new OrderProcessor(new SqliteDataAccess());

        #region Properties
        public int OrderId { get => orderId; set => SetProperty(ref orderId, value, "OrderId"); }
        public string User { get => user; set => SetProperty(ref user, value, "User"); }
        public string Message { get => message; set => SetProperty(ref message, value, "Message"); }
        public DateTime DeliveryDate { get => deliveryDate; set => SetProperty(ref deliveryDate, value, "DeliveryDate"); }
        public string Address { get => address; set => SetProperty(ref address, value, "Address"); }
        public OrderState State { get => state; set => SetProperty(ref state, value, "State"); }
        public OrdersModel OrdersModel { get => ordersModel; set => SetProperty(ref ordersModel, value, "OrdersModel"); }
        #endregion // Properties

        #region Commands
        public ICommand GetAllOrdersCommand
        {
            get { return getAllOrdersCommand; }
            private set { getAllOrdersCommand = value; }
        }
        public ICommand GetOrderCommand
        {
            get { return getOrderCommand; }
            private set { getOrderCommand = value; }
        }

        public ICommand GetUndeliveredOrdersForUserCommand
        {
            get { return getUndeliveredOrdersForUserCommand; }
            private set { getUndeliveredOrdersForUserCommand = value; }
        }

        public ICommand GetDeliveredOrdersForUserCommand
        {
            get { return getDeliveredOrdersForUserCommand; }
            private set { getDeliveredOrdersForUserCommand = value; }
        }

        public ICommand IsOrderDeliveredCommand
        {
            get { return isOrderDeliveredCommand; }
            private set { isOrderDeliveredCommand = value; }
        }
        public ICommand DeliverOrderCommand
        {
            get { return deliverOrderCommand; }
            private set { deliverOrderCommand = value; }
        }
        public ICommand UpdateOrderCommand
        {
            get { return updateOrderCommand; }
            private set { updateOrderCommand = value; }
        }
        public ICommand CreateNewOrderCommand
        {
            get { return createNewOrderCommand; }
            private set { createNewOrderCommand = value; }
        }        
        #endregion // Commands

        public MainWindowViewModel()
        {
            GetAllOrdersCommand = new RelayCommand(GetAllOrders);
            GetOrderCommand = new RelayCommand(GetOrder);
            GetUndeliveredOrdersForUserCommand = new RelayCommand(GetUndeliveredOrdersForUser);
            GetDeliveredOrdersForUserCommand = new RelayCommand(GetDeliveredOrdersForUser);
            IsOrderDeliveredCommand = new RelayCommand(IsOrderDelivered);
            DeliverOrderCommand = new RelayCommand(DeliverOrder);
            UpdateOrderCommand = new RelayCommand(UpdateOrder);
            CreateNewOrderCommand = new RelayCommand(CreateNewOrder);
    }

        public void GetAllOrders(object obj)
        {
            Message = $"GetAllOrders() called";
            var orders = orderProcessor.GetAllOrders();
            OrdersModel = new OrdersModel(orders);
        }

        public void GetOrder(object obj)
        {
            Message = $"GetOrder() called, OrderId: '{OrderId}'";
            var order = orderProcessor.GetOrder(OrderId);
            OrdersModel = new OrdersModel(order);            
        }

        public void GetUndeliveredOrdersForUser(object obj)
        {
            Message = "GetUndeliveredOrdersForUser() called";
            var orders = orderProcessor.GetUndeliveredOrdersForUser(User);
            OrdersModel = new OrdersModel(orders);
        }
        public void GetDeliveredOrdersForUser(object obj)
        {
            Message = "GetDeliveredOrdersForUser() called";
            var orders = orderProcessor.GetDeliveredOrdersForUser(User);
            OrdersModel = new OrdersModel(orders);
        }

        public void IsOrderDelivered(object obj)
        {
            Message = "IsOrderDelivered() called";
            bool isOrderDelivered = orderProcessor.IsOrderDelivered(OrderId);
            Message =  (isOrderDelivered)? $"Order Id:{OrderId} is delivered" :
                $"Order Id:{OrderId} is still NOT delivered";
        }

        public void DeliverOrder(object obj)
        {
            Message = "DeliverOrder() called";
            DateTime deliveryDate = DateTime.Now;
            orderProcessor.DeliverOrder(OrderId, deliveryDate);
            Message = $"Order Id:{OrderId} is delivered with date: '{deliveryDate}'";                
        }

        public void UpdateOrder(object obj)
        {
            Message = "UpdateOrder(Address, State) called";
            orderProcessor.UpdateOrder(OrderId, Address, State);

        }

        public void CreateNewOrder(object obj)
        {
            Message = "CreateNewOrder(User, Address) called";
            orderProcessor.CreateNewOrder(User, Address);
        }
    }
}