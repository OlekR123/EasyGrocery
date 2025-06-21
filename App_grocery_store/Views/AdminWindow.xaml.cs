using App_grocery_store.Models;
using App_grocery_store.Services;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class AdminWindow : Window
    {
        private readonly DbService _dbService;
        private readonly User _currentUser;
        private Product _selectedProduct;
        private Category _selectedCategory;
        private Supplier _selectedSupplier;
        private User _selectedUser;
        private Order _selectedOrder;

        public AdminWindow(User user)
        {
            InitializeComponent();
            _dbService = new DbService();
            _currentUser = user;
            LoadData();
        }

        private void LoadData()
        {
            ProductsDataGrid.ItemsSource = _dbService.GetProducts();
            CategoriesDataGrid.ItemsSource = _dbService.GetCategories();
            SuppliersDataGrid.ItemsSource = _dbService.GetSuppliers();
            UsersDataGrid.ItemsSource = _dbService.GetUsers();
            OrdersDataGrid.ItemsSource = _dbService.GetOrders();
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductsDataGrid.SelectedItem as Product;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productWindow = new ProductWindow(_dbService, null);
            if (productWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var productWindow = new ProductWindow(_dbService, _selectedProduct);
            if (productWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _dbService.DeleteProduct(_selectedProduct.id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateStockReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reports = _dbService.GetStockReport();
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Остатки товаров");
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Название";
                    worksheet.Cell(1, 3).Value = "Категория";
                    worksheet.Cell(1, 4).Value = "Цена";
                    worksheet.Cell(1, 5).Value = "Наличие";
                    worksheet.Cell(1, 6).Value = "Общая стоимость";

                    int row = 2;
                    foreach (var report in reports)
                    {
                        worksheet.Cell(row, 1).Value = report.id;
                        worksheet.Cell(row, 2).Value = report.name;
                        worksheet.Cell(row, 3).Value = report.category_name;
                        worksheet.Cell(row, 4).Value = report.price;
                        worksheet.Cell(row, 5).Value = report.stock;
                        worksheet.Cell(row, 6).Value = report.total_value;
                        row++;
                    }

                    worksheet.Cell(row, 4).Value = "Итого:";
                    worksheet.Cell(row, 5).Value = reports.Sum(r => r.stock);
                    worksheet.Cell(row, 6).Value = reports.Sum(r => r.total_value);

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs("StockReport.xlsx");
                }
                MessageBox.Show("Отчёт сохранён в StockReport.xlsx", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoriesDataGrid.SelectedItem as Category;
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryWindow = new CategoryWindow(_dbService, null);
            if (categoryWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var categoryWindow = new CategoryWindow(_dbService, _selectedCategory);
            if (categoryWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Выберите категорию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _dbService.DeleteCategory(_selectedCategory.id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuppliersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSupplier = SuppliersDataGrid.SelectedItem as Supplier;
        }

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            var supplierWindow = new SupplierWindow(_dbService, null);
            if (supplierWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier == null)
            {
                MessageBox.Show("Выберите поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var supplierWindow = new SupplierWindow(_dbService, _selectedSupplier);
            if (supplierWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSupplier == null)
            {
                MessageBox.Show("Выберите поставщика", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _dbService.DeleteSupplier(_selectedSupplier.id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = UsersDataGrid.SelectedItem as User;
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var userWindow = new UserWindow(_dbService, null);
            if (userWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var userWindow = new UserWindow(_dbService, _selectedUser);
            if (userWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _dbService.DeleteUser(_selectedUser.id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrder = OrdersDataGrid.SelectedItem as Order;
            if (_selectedOrder != null)
            {
                OrderItemsDataGrid.ItemsSource = _dbService.GetOrderItems(_selectedOrder.id);
                StatusComboBox.SelectedItem = StatusComboBox.Items.OfType<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == _selectedOrder.status);
            }
        }

        private void ChangeOrderStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var newStatus = (StatusComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                _dbService.UpdateOrderStatus(_selectedOrder.id, newStatus);
                LoadData();
                MessageBox.Show("Статус заказа обновлён", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _dbService.DeleteOrder(_selectedOrder.id);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            Close();
        }
    }
}