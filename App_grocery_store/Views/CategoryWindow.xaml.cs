using App_grocery_store.Models;
using App_grocery_store.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class CategoryWindow : Window
    {
        private readonly DbService _dbService;
        private readonly Category _category;

        public CategoryWindow(DbService dbService, Category category)
        {
            InitializeComponent();
            _dbService = dbService;
            _category = category;

            // Установим начальную подсказку
            NameTextBox.Text = NameTextBox.Tag.ToString();

            if (_category != null)
            {
                NameTextBox.Text = _category.name;
                NameTextBox.Foreground = System.Windows.Media.Brushes.Black;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, чтобы пользователь ввёл данные вместо подсказки
                var name = NameTextBox.Text == NameTextBox.Tag.ToString() ? string.Empty : NameTextBox.Text;

                var category = new Category
                {
                    id = _category?.id ?? 0,
                    name = name
                };

                if (_category == null)
                    _dbService.CreateCategory(category);
                else
                    _dbService.UpdateCategory(category);

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