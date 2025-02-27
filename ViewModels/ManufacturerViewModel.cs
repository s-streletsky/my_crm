﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Models;
using CRM.Views;
using CRM.WPF;
using Microsoft.Toolkit.Mvvm.Input;

namespace CRM.ViewModels
{
    internal class ManufacturerViewModel : ViewModelBase
    {
        private ObservableCollection<Manufacturer> dbManufacturers;
        private Manufacturer selectedManufacturer;
        
        private ManufacturerRepository manufacturerRepo;

        private bool isListBoxEnabled;
        private string isAddGridVisible;
        private bool isAddGridEnabled;
        private string isEditGridVisible;
        private bool isEditDeleteButtonsEnabled;
        private string name;

        public RelayCommand AddManufacturerCommand { get; }
        public RelayCommand EditManufacturerCommand { get; }
        public RelayCommand DeleteManufacturerCommand { get; }
        public RelayCommand AddOKCommand { get; }
        public RelayCommand AddCancelCommand { get; }
        public RelayCommand EditOKCommand { get; }
        public RelayCommand EditCancelCommand { get; }

        public ObservableCollection<Manufacturer> DbManufacturers { get { return dbManufacturers; } }
        public Manufacturer SelectedManufacturer { 
            get { return selectedManufacturer; } 
            set { selectedManufacturer = value;
                OnPropertyChanged(nameof(IsEditDeleteButtonsEnabled));
            } 
        }
        public bool IsListBoxEnabled { get { return isListBoxEnabled; } set { isListBoxEnabled = value; OnPropertyChanged(); } }
        public string IsAddGridVisible { get { return isAddGridVisible; } set { isAddGridVisible = value; OnPropertyChanged(); } }
        public bool IsAddGridEnabled { get { return isAddGridEnabled; } set { isAddGridEnabled = value; OnPropertyChanged(); } }
        public string IsEditGridVisible { get { return isEditGridVisible; } set { isEditGridVisible = value; OnPropertyChanged(); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(); } }
        public bool IsEditDeleteButtonsEnabled { 
            get { return SelectedManufacturer != null; } 
        }
        private List<string> Visibility { get; set; } = new List<string>() { "Hidden", "Visible" };

        public ManufacturerViewModel(ObservableCollection<Manufacturer> m, ManufacturerRepository mr)
        {
            dbManufacturers = m;
            manufacturerRepo = mr;

            IsListBoxEnabled = true;
            IsAddGridEnabled = false;
            IsAddGridVisible = Visibility[1];
            IsEditGridVisible = Visibility[0];

            AddManufacturerCommand = new RelayCommand(OnAddMenuButtonClick);
            EditManufacturerCommand = new RelayCommand(OnEditMenuButtonClick);
            DeleteManufacturerCommand = new RelayCommand(OnDeleteMenuButtonClick);
            AddOKCommand = new RelayCommand(OnAddOKButtonClick);
            AddCancelCommand = new RelayCommand(OnAddCancelButtonClick);
            EditOKCommand = new RelayCommand(OnEditOKButtonClick);
            EditCancelCommand = new RelayCommand(OnEditCancelButtonClick);           
        }

        // Кнопки "Добавить/Изменить/Удалить"
        public void OnAddMenuButtonClick()
        {
            DisableListBox();
            //Name = "Новый производитель";
        }

        public void OnEditMenuButtonClick()
        {
            if (SelectedManufacturer != null)
            {
                IsAddGridVisible = Visibility[0];
                IsEditGridVisible = Visibility[1];

                DisableListBox();
                Name = SelectedManufacturer.Name;
            }
            else
            {
                Name = "Выберите запись из списка!";
            }           
        }

        public void OnDeleteMenuButtonClick()
        {
            manufacturerRepo.Delete(SelectedManufacturer);
            dbManufacturers.Remove(SelectedManufacturer);
        }

        // Кнопки добавления новой записи
        public void OnAddOKButtonClick()
        {
            var newManufacturer = manufacturerRepo.Add(new Manufacturer(-1, Name));
            dbManufacturers.Add(newManufacturer);
            EnableListBox();
        }

        public void OnAddCancelButtonClick()
        {
            EnableListBox();
        }

        // Кнопки редактирования записи
        public void OnEditOKButtonClick()
        {
            SelectedManufacturer.Name = Name;
            manufacturerRepo.Update(SelectedManufacturer);

            HideEditStackPanel();
            EnableListBox();
        }

        public void OnEditCancelButtonClick()
        {
            HideEditStackPanel();
            EnableListBox();
        }

        // Вспомогательные методы
        private void EnableListBox()
        {
            IsListBoxEnabled = true;
            IsAddGridEnabled = false;
            Name = "";
        }

        private void DisableListBox()
        {
            IsListBoxEnabled = false;
            IsAddGridEnabled = true;            
        }

        private void HideEditStackPanel()
        {
            IsAddGridVisible = Visibility[1];
            IsEditGridVisible = Visibility[0];
        }
    }
}
