using App_grocery_store.Models;
using App_grocery_store.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class UserWindow : Window
    {
        private readonly DbService _dbService;
        private readonly User _user;

        public UserWindow(DbService dbService, User user)
        {
            InitializeComponent();
            _dbService = dbService;
            _user = user;

            RoleComboBox.ItemsSource = _dbService.GetUserRoles();

            // Установим начальные подсказки
            SurnameTextBox.Text = SurnameTextBox.Tag.ToString();
            NameTextBox.Text = NameTextBox.Tag.ToString();
            PatronymicTextBox.Text = PatronymicTextBox.Tag.ToString();
            EmailTextBox.Text = EmailTextBox.Tag.ToString();
            PhoneTextBox.Text = PhoneTextBox.Tag.ToString();
            PasswordPlaceholder.Visibility = Visibility.Visible;

            if (_user != null)
            {
                SurnameTextBox.Text = _user.surname;
                SurnameTextBox.Foreground = System.Windows.Media.Brushes.Black;
                NameTextBox.Text = _user.name;
                NameTextBox.Foreground = System.Windows.Media.Brushes.Black;
                PatronymicTextBox.Text = _user.patronymic;
                PatronymicTextBox.Foreground = System.Windows.Media.Brushes.Black;
                EmailTextBox.Text = _user.email;
                EmailTextBox.Foreground = System.Windows.Media.Brushes.Black;
                RoleComboBox.SelectedValue = _user.role_id;
                PhoneTextBox.Text = _user.phone;
                PhoneTextBox.Foreground = System.Windows.Media.Brushes.Black;
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                RoleComboBox.SelectedValue = 2; // Клиент по умолчанию
            }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, чтобы пользователь ввёл данные вместо подсказок
                var surname = SurnameTextBox.Text == SurnameTextBox.Tag.ToString() ? string.Empty : SurnameTextBox.Text;
                var name = NameTextBox.Text == NameTextBox.Tag.ToString() ? string.Empty : NameTextBox.Text;
                var patronymic = PatronymicTextBox.Text == PatronymicTextBox.Tag.ToString() ? string.Empty : PatronymicTextBox.Text;
                var email = EmailTextBox.Text == EmailTextBox.Tag.ToString() ? string.Empty : EmailTextBox.Text;
                var phone = PhoneTextBox.Text == PhoneTextBox.Tag.ToString() ? string.Empty : PhoneTextBox.Text;
                var password = PasswordBox.Password;

                var user = new User
                {
                    id = _user?.id ?? 0,
                    surname = surname,
                    name = name,
                    patronymic = patronymic,
                    email = email,
                    role_id = (int)RoleComboBox.SelectedValue,
                    phone = phone
                };

                if (_user == null)
                    _dbService.RegisterUser(user, password);
                else
                    _dbService.UpdateUser(user, password);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}