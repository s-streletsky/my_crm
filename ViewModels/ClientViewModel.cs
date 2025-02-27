﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Models;
using CRM.WPF;
using CRM.Views;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

namespace CRM.ViewModels
{
    class ClientViewModel : ViewModelBase
    {
        public RelayCommand AddOrderCommand { get; }
        public RelayCommand EditOrderCommand { get; }
        public RelayCommand DeleteOrderCommand { get; }
        public RelayCommand<ICloseable> CloseWindowCommand { get; }

        private int? id;
        private DateTime date;
        private string name;
        private string nickname;
        private string phone;
        private string email;
        private Country country;
        private string city;
        private string address;
        private ShippingMethod shippingMethod;
        private string postalCode;
        private string notes;
        private float balance;
        private string balanceBackground;

        private ObservableCollection<Client> dbClients;
        private ObservableCollection<Order> dbOrders;
        private ObservableCollection<OrderItem> dbOrdersItems;
        private ObservableCollection<StockItem> dbStockItems;
        private ObservableCollection<ExchangeRate> dbExchangeRates;
        private ObservableCollection<Payment> dbPayments;
        private ObservableCollection<Order> orders;

        private Client selectedClient;
        private Order selectedOrder;        
        private ClientRepository clientRepo;
        private OrderRepository orderRepo;
        private ExchangeRateRepository exchangeRateRepo;
        private OrderItemRepository orderItemRepo;
        private StockItemRepository stockItemRepo;
        private PaymentRepository paymentRepo;

        public int? Id { 
            get { return id; } 
            set { id = value; 
                OnPropertyChanged(); } 
        }
        public DateTime Date { 
            get { return date; } 
            set { date = value; 
                OnPropertyChanged(); } 
        }
        public string Name { 
            get { return name; } 
            set { name = value; 
                OnPropertyChanged(); } 
        }
        public string Nickname { 
            get { return nickname; } 
            set { nickname = value; 
                OnPropertyChanged(); } 
        }
        public string Phone { 
            get { return phone; } 
            set { phone = value; 
                OnPropertyChanged(); } 
        }
        public string Email { 
            get { return email; } 
            set { email = value; 
                OnPropertyChanged(); } 
        }        
        public Country Country { 
            get { return country; } 
            set { country = value; 
                OnPropertyChanged(); } 
        }
        public string City { 
            get { return city; } 
            set { city = value; 
                OnPropertyChanged(); } 
        }
        public string Address { 
            get { return address; } 
            set { address = value; 
                OnPropertyChanged(); } 
        }
        public ShippingMethod ShippingMethod { 
            get { return shippingMethod; } 
            set { shippingMethod = value; 
                OnPropertyChanged(); } 
        }
        public string PostalCode { 
            get { return postalCode; } 
            set { postalCode = value; 
                OnPropertyChanged(); } 
        }
        public string Notes { 
            get { return notes; } 
            set { notes = value; 
                OnPropertyChanged(); } 
        }      
        public float Balance { 
            get { return balance; } 
            set { balance = value; 
                OnPropertyChanged(); if (value < 0) BalanceBackground = "#ffb3ba"; else BalanceBackground = "#baffc9"; } 
        }
        public string BalanceBackground { 
            get { return balanceBackground; } 
            set { balanceBackground = value; 
                OnPropertyChanged(); } 
        }
        public Client SelectedClient { 
            get { return selectedClient; } 
            set { selectedClient = value; 
                OnPropertyChanged(); } 
        }
        //public Database Database {
        //    get { return database; }
        //    set { database = value;
        //        OnPropertyChanged(); }
        //}
        public ObservableCollection<Order> Orders {
            get { return orders; }
            set { orders = value; }
        }
        public Order SelectedOrder { 
            get { return selectedOrder; } 
            set { selectedOrder = value;
                OnPropertyChanged(nameof(IsOrderEditDeleteButtonsEnabled));
            } 
        }
        public bool IsOrderEditDeleteButtonsEnabled { 
            get { return SelectedOrder != null; } 
        }
        public bool IsDataGridEnabled { get; set; }
        public string WindowTitle { get; set; }

        public ClientViewModel() { }
        //public ClientViewModel(Database db, ClientRepository cr)
        //{
        //    Database = db;
        //    clientRepo = cr;

        //    IsDataGridEnabled = false;
        //}
        public ClientViewModel(ObservableCollection<Client> clients, ObservableCollection<Order> orders, ObservableCollection<OrderItem> oi, ObservableCollection<StockItem> si, ObservableCollection<ExchangeRate> er, ObservableCollection<Payment> p, Client sc, ClientRepository cr, OrderRepository or, ExchangeRateRepository err, OrderItemRepository oir, StockItemRepository sir, PaymentRepository pr)
        {
            dbClients = clients;
            dbOrders = orders;
            dbOrdersItems = oi;
            dbStockItems = si;
            dbExchangeRates = er;
            dbPayments = p;
            SelectedClient = sc;
            clientRepo = cr;
            orderRepo = or;
            exchangeRateRepo = err;
            orderItemRepo = oir;
            stockItemRepo = sir;
            paymentRepo = pr;

            Orders = new ObservableCollection<Order>();

            IsDataGridEnabled = false;

            Country = Country.Ukraine;
            ShippingMethod = ShippingMethod.NovaPoshta;

            AddOrderCommand = new RelayCommand(OnAddOrder);
            EditOrderCommand = new RelayCommand(OnEditOrder);
            DeleteOrderCommand = new RelayCommand(OnDeleteOrder);
            CloseWindowCommand = new RelayCommand<ICloseable>(CloseWindow);
        }

        private void OnAddOrder()
        {
            var o = new Order(DateTime.Now, SelectedClient, OrderStatus.NEW);
            var order = orderRepo.Add(o);
            Orders.Insert(0, order);
            dbOrders.Insert(0, order);
        }
        private void OnEditOrder()
        {
            {
                var vm = new OrderViewModel(dbClients, dbOrders, dbOrdersItems, dbStockItems, dbExchangeRates, dbPayments, clientRepo, exchangeRateRepo, orderItemRepo, stockItemRepo, paymentRepo, SelectedOrder);
                OrderView orderView = new OrderView();
                orderView.DataContext = vm;
                orderView.Owner = App.Current.MainWindow;

                vm.Id = SelectedOrder.Id;
                vm.Client = SelectedOrder.Client;
                vm.Date = SelectedOrder.Date;
                vm.Status = SelectedOrder.Status;
                vm.Notes = SelectedOrder.Notes;
                vm.IsDataGridEnabled = true;
                vm.IsChooseClientButtonEnabled = false;
                vm.WindowTitle = "Изменить содержимое заказа";

                foreach (var item in dbOrdersItems)
                {
                    if (item.Order.Id == SelectedOrder.Id)
                    {
                        vm.OrderItems.Add(item);
                    }
                }

                vm.UpdateBillingDetails();
                orderView.ShowDialog();

                if (orderView.DialogResult == true)
                {
                    SelectedOrder.Status = vm.Status;
                    SelectedOrder.Notes = vm.Notes;

                    orderRepo.Update(SelectedOrder);
                }
            }
        }
        private void OnDeleteOrder()
        {
            var userChoice = MessageBox.Show("Заказ №" + $"{SelectedOrder.Id}\nУдалить?", "Удаление заказа", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            List<OrderItem> orderItems = new List<OrderItem>();

            if (userChoice == MessageBoxResult.Yes)
            {
                foreach (var item in dbOrdersItems)
                {
                    if (item.Order.Id == SelectedOrder.Id)
                    {
                        orderItemRepo.Delete(item);
                        orderItems.Add(item);
                    }
                }

                foreach (var item in orderItems)
                {
                    dbOrdersItems.Remove(item);
                }

                orderRepo.Delete(SelectedOrder);                
                dbOrders.Remove(SelectedOrder);
                Orders.Remove(SelectedOrder);
            }
        }

        private void CloseWindow(ICloseable window)
        {
            if (window != null)
            {
                window.DialogResult = true;
                window.Close();
            }
        }
    }
}
