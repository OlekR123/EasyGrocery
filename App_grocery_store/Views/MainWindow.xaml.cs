using App_grocery_store.Models;
using App_grocery_store.Services;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class MainWindow : Window
    {
        private readonly DbService _dbService;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DbService();

            // Установим начальные подсказки
            EmailTextBox.Text = EmailTextBox.Tag.ToString();
            PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, чтобы пользователь ввёл данные вместо подсказок
                var email = EmailTextBox.Text == EmailTextBox.Tag.ToString() ? string.Empty : EmailTextBox.Text;
                var password = PasswordBox.Password;

                var user = _dbService.Authenticate(email, password);
                if (user == null)
                {
                    MessageBox.Show("Неверный email или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Добро пожаловать, {user.name} {user.surname}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                if (user.role_id == 1) // Администратор
                {
                    var adminWindow = new AdminWindow(user);
                    adminWindow.Show();
                    Close();
                }
                else // Клиент
                {
                    var clientWindow = new ClientWindow(user);
                    clientWindow.Show();
                    Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }
    }
}