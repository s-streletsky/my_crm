﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CRM.Models;
using Microsoft.Toolkit.Mvvm.Input;

namespace CRM.ViewModels
{
    internal class OrderPaymentViewModel
    {
        private Client client;
        private Order order;

        public RelayCommand<ICloseable> CloseWindowCommand { get; private set; }

        public Client Client { get { return client; } }
        public Order Order { get { return order; } }
        public string Amount { get; set; }
        public string Notes { get; set; }

        public OrderPaymentViewModel() { }
        public OrderPaymentViewModel(Client c, Order o, string amount)
        {
            client = c;
            order = o;
            Amount = amount;

            CloseWindowCommand = new RelayCommand<ICloseable>(CloseWindow);
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
