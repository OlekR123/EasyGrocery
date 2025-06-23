using App_grocery_store.Models;
using App_grocery_store.Services;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class RegisterWindow : Window
    {
        private readonly DbService _dbService;

        public RegisterWindow()
        {
            InitializeComponent();
            _dbService = new DbService();

            SurnameTextBox.Text = SurnameTextBox.Tag.ToString();
            NameTextBox.Text = NameTextBox.Tag.ToString();
            PatronymicTextBox.Text = PatronymicTextBox.Tag.ToString();
            EmailTextBox.Text = EmailTextBox.Tag.ToString();
            PhoneTextBox.Text = PhoneTextBox.Tag.ToString();
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var surname = SurnameTextBox.Text == SurnameTextBox.Tag.ToString() ? string.Empty : SurnameTextBox.Text;
                var name = NameTextBox.Text == NameTextBox.Tag.ToString() ? string.Empty : NameTextBox.Text;
                var patronymic = PatronymicTextBox.Text == PatronymicTextBox.Tag.ToString() ? string.Empty : PatronymicTextBox.Text;
                var email = EmailTextBox.Text == EmailTextBox.Tag.ToString() ? string.Empty : EmailTextBox.Text;
                var phone = PhoneTextBox.Text == PhoneTextBox.Tag.ToString() ? string.Empty : PhoneTextBox.Text;
                var password = PasswordBox.Password;

                var user = new User
                {
                    surname = surname,
                    name = name,
                    patronymic = patronymic,
                    email = email,
                    role_id = 2,
                    phone = phone
                };
                _dbService.RegisterUser(user, password);
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                var loginWindow = new MainWindow();
                loginWindow.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            Close();
        }
    }
}