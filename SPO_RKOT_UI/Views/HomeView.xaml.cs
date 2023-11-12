﻿using ExcelLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SPO_RKOT_UI.ViewModels;
using SpoRkotLibrary.Data;
using SpoRkotLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SPO_RKOT_UI.Views
{
    /// <summary>
    /// Логика взаимодействия для HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        HomeViewModel homeViewModel = new HomeViewModel();
                 
        public HomeView()
        {
            InitializeComponent();

            DataContext = homeViewModel;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
                if (ExcelReader.ImportFromExcel(fileName))
                    MessageBox.Show("Отчет успешно добавлен.");
                else
                    MessageBox.Show("Отчет с такими данными уже есть");
            }
        }


        private void UserList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenTableView();
        }

        private void WatchButtonInRow_Click(object sender, RoutedEventArgs e)
        {
            ReportInfo report = (sender as System.Windows.Controls.Button)?.DataContext as ReportInfo;
            var dataBase = new DataBaseViewWindow(report);
            dataBase.ShowDialog();
        }

        private void OpenTableView()
        {
            if (reportsListView.SelectedItem == null) return;
            ReportInfo reportInfo = (ReportInfo)reportsListView.SelectedItem;
            var dataBase = new DataBaseViewWindow(reportInfo);
            dataBase.ShowDialog();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            homeViewModel.Update();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                UpdateButton_Click(sender, e);
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.N)
            {
                SelectFileButton_Click(sender, e);
            }
        }

        //filtration
        private void FindLocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {            
            reportsListView.Items.Filter = FiltersMethod; //поиск по listview
        }

        private bool FilterLocationMethod(object obj)
        {
            var report = (ReportInfo)obj;
            return report.Location.Contains(findLocationTextBox.Text, StringComparison.OrdinalIgnoreCase);
        }

        private void FindDistrictTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            reportsListView.Items.Filter = FiltersMethod; //поиск по listview
        }

        private bool FilterDistrictMethod(object obj)
        {
            var report = (ReportInfo)obj;
            return report.FederalDistrict.Contains(findDistrictTextBox.Text, StringComparison.OrdinalIgnoreCase);
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            reportsListView.Items.Filter = FiltersMethod; //поиск по listview
        }

        private bool FilterDateMethod(object obj)
        {
            var report = (ReportInfo)obj;
            return report.StartDate >= startDatePicker.DisplayDate && report.EndDate <= endDatePicker.DisplayDate;
        }

        private bool FiltersMethod(object obj)
        {
            var report = (ReportInfo)obj;

            bool startDateFilter = !startDatePicker.SelectedDate.HasValue || report.StartDate >= startDatePicker.SelectedDate.Value;
            bool endDateFilter = !endDatePicker.SelectedDate.HasValue || report.EndDate <= endDatePicker.SelectedDate.Value;
            bool districtFilter = string.IsNullOrEmpty(findDistrictTextBox.Text) || report.FederalDistrict.Contains(findDistrictTextBox.Text, StringComparison.OrdinalIgnoreCase);
            bool locationFilter = string.IsNullOrEmpty(findLocationTextBox.Text) || report.Location.Contains(findLocationTextBox.Text, StringComparison.OrdinalIgnoreCase);

            return startDateFilter && endDateFilter && districtFilter && locationFilter;
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            findLocationTextBox.Text = string.Empty;
            findDistrictTextBox.Text = string.Empty;
            endDatePicker.SelectedDate = null;
            startDatePicker.SelectedDate = null;
            reportsListView.Items.Filter = FiltersMethod;
        }
    }
}
