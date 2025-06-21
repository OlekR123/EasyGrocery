using App_grocery_store.Models;
using App_grocery_store.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace App_grocery_store
{
    public partial class ClientWindow : Window
    {
        private readonly DbService _dbService;
        private readonly User _currentUser;
        private List<CartItem> _cart;

        public ClientWindow(User user)
        {
            InitializeComponent();
            _dbService = new DbService();
            _currentUser = user;
            _cart = new List<CartItem>();
            LoadCategories();
            LoadProducts();
            LoadOrders();
        }

        private void LoadCategories()
        {
            var categories = _dbService.GetCategories();
            categories.Insert(0, new Category { id = 0, name = "Все категории" });
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }

        private void LoadProducts(int? categoryId = null, string searchText = null)
        {
            ProductsDataGrid.ItemsSource = _dbService.GetProducts(categoryId == 0 ? null : categoryId, searchText);
        }

        private void LoadOrders()
        {
            OrdersDataGrid.ItemsSource = _dbService.GetOrders(_currentUser.id);
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = (Category)CategoryComboBox.SelectedItem;
            LoadProducts(selectedCategory?.id, SearchTextBox.Text);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedCategory = (Category)CategoryComboBox.SelectedItem;
            LoadProducts(selectedCategory?.id, SearchTextBox.Text);
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var product = (Product)button.Tag;

                var row = FindVisualParent<DataGridRow>(button);
                if (row == null)
                {
                    MessageBox.Show("Не удалось определить строку товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var quantityTextBox = FindVisualChild<TextBox>(row, "QuantityTextBox");
                if (quantityTextBox == null)
                {
                    MessageBox.Show("Не удалось найти поле количества", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(quantityTextBox.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var cartItem = _cart.FirstOrDefault(ci => ci.product.id == product.id);
                if (cartItem != null)
                    cartItem.quantity += quantity;
                else
                    _cart.Add(new CartItem { product = product, quantity = quantity });

                CartDataGrid.ItemsSource = null;
                CartDataGrid.ItemsSource = _cart;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var cartItem = (CartItem)button.Tag;
            _cart.Remove(cartItem);
            CartDataGrid.ItemsSource = null;
            CartDataGrid.ItemsSource = _cart;
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_cart.Any())
                {
                    MessageBox.Show("Корзина пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _dbService.CreateOrder(_currentUser.id, _cart);
                _cart.Clear();
                CartDataGrid.ItemsSource = null;
                LoadOrders();
                MessageBox.Show("Заказ оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selectedOrder)
            {
                OrderItemsDataGrid.ItemsSource = _dbService.GetOrderItems(selectedOrder.id);
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null) return null;

            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild && typedChild.Name == name)
                {
                    return typedChild;
                }

                var result = FindVisualChild<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            Close();
        }

        private void ImageUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}