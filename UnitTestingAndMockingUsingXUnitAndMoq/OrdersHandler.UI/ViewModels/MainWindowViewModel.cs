using System;
using System.Windows.Input;
using OrdersHandler.DataAccess;
using OrdersHandler.Models;
using OrdersHandler.UI.Commands;
using OrdersHandler.UI.Models;

namespace OrdersHandler.UI.ViewModels
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

        IOrderProcessor orderProcessor;

        #region Properties
        public int OrderId { get => orderId; set => SetProperty(ref orderId, value); }
        public string User { get => user; set => SetProperty(ref user, value); }
        public string Message { get => message; set => SetProperty(ref message, value); }
        public DateTime DeliveryDate { get => deliveryDate; set => SetProperty(ref deliveryDate, value); }
        public string Address { get => address; set => SetProperty(ref address, value); }
        public OrderState State { get => state; set => SetProperty(ref state, value); }
        public OrdersModel OrdersModel { get => ordersModel; set => SetProperty(ref ordersModel, value); }
        #endregion // Properties

        #region Commands
        public ICommand GetAllOrdersCommand 
        {
            get
            {
                if (getAllOrdersCommand == null)
                {
                    getAllOrdersCommand = new RelayCommand(p => GetAllOrders());
                }
                return getAllOrdersCommand;
            }            
        }
        public ICommand GetOrderCommand
        {
            get
            {
                if (getOrderCommand == null)
                {
                    getOrderCommand = new RelayCommand(p => GetOrder(OrderId));
                }
                return getOrderCommand;
            }            
        }

        public ICommand GetUndeliveredOrdersForUserCommand
        {
            get
            {
                if (getUndeliveredOrdersForUserCommand == null)
                {
                    getUndeliveredOrdersForUserCommand = new RelayCommand(p => GetUndeliveredOrdersForUser());
                }
                return getUndeliveredOrdersForUserCommand;
            }
        }

        public ICommand GetDeliveredOrdersForUserCommand
        {
            get
            {
                if (getDeliveredOrdersForUserCommand == null)
                {
                    getDeliveredOrdersForUserCommand = new RelayCommand(p => GetDeliveredOrdersForUser());
                }
                return getDeliveredOrdersForUserCommand;
            }            
        }

        public ICommand IsOrderDeliveredCommand
        {
            get
            {
                if (isOrderDeliveredCommand == null)
                {
                    isOrderDeliveredCommand = new RelayCommand(p => IsOrderDelivered());
                }
                return isOrderDeliveredCommand;
            }            
        }
        public ICommand DeliverOrderCommand
        {
            get
            {
                if (deliverOrderCommand == null)
                {
                    deliverOrderCommand = new RelayCommand(p => DeliverOrder());
                }
                return deliverOrderCommand;
            }            
        }
        public ICommand UpdateOrderCommand
        {
            get
            {
                if (updateOrderCommand == null)
                {
                    updateOrderCommand = new RelayCommand(p => UpdateOrder());
                }
                return updateOrderCommand;
            }            
        }
        public ICommand CreateNewOrderCommand
        {
            get
            {
                if (createNewOrderCommand == null)
                {
                    createNewOrderCommand = new RelayCommand(p => CreateNewOrder(User, Address));
                }
                return createNewOrderCommand;
            }
        }        
        #endregion // Commands

        public MainWindowViewModel() 
        {
            orderProcessor = new OrderProcessor(new SqliteDataAccess());            
        }

        public void GetAllOrders()
        {
            Message = $"GetAllOrders() called";
            var orders = orderProcessor.GetAllOrders();
            OrdersModel = new OrdersModel(orders);
        }

        public async void GetOrder(int orderId)
        {
            Message = $"GetOrder() called, OrderId: '{orderId}'";
            var order = await orderProcessor.GetOrder(orderId);
            OrdersModel = new OrdersModel(order);
            OrderId = order.Id;
            User = order.User;
            Address = order.Address;
            State = order.State;
            DeliveryDate = order.DeliveryDate;
        }

        public void GetUndeliveredOrdersForUser()
        {
            Message = "GetUndeliveredOrdersForUser() called";
            var orders = orderProcessor.GetUndeliveredOrdersForUser(User);
            OrdersModel = new OrdersModel(orders);
        }
        public void GetDeliveredOrdersForUser()
        {
            Message = "GetDeliveredOrdersForUser() called";
            var orders = orderProcessor.GetDeliveredOrdersForUser(User);
            OrdersModel = new OrdersModel(orders);
        }

        public void IsOrderDelivered()
        {
            Message = "IsOrderDelivered() called";
            bool isOrderDelivered = orderProcessor.IsOrderDelivered(OrderId);
            Message =  (isOrderDelivered)? $"Order Id:{OrderId} is delivered" :
                $"Order Id:{OrderId} is still NOT delivered";
        }

        public void DeliverOrder()
        {
            Message = "DeliverOrder() called";
            DateTime deliveryDate = DateTime.Now;
            orderProcessor.DeliverOrder(OrderId, deliveryDate);
            Message = $"Order Id:{OrderId} is delivered with date: '{deliveryDate}'";                
        }

        public void UpdateOrder()
        {
            Message = "UpdateOrder(Address, State) called";
            orderProcessor.UpdateAddressAndStateOfOrder(OrderId, Address, State);

        }

        public async void CreateNewOrder(string user, string address)
        {
            int createdOrderId;
            Message = "CreateNewOrder(User, Address) called";
            createdOrderId = await orderProcessor.CreateNewOrder(User, Address);
            OrderId = createdOrderId;
        }
    }
}