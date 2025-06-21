using App_grocery_store.Models;
using App_grocery_store.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class ProductWindow : Window
    {
        private readonly DbService _dbService;
        private readonly Product _product;

        public ProductWindow(DbService dbService, Product product)
        {
            InitializeComponent();
            _dbService = dbService;
            _product = product;

            CategoryComboBox.ItemsSource = _dbService.GetCategories();
            SupplierComboBox.ItemsSource = _dbService.GetSuppliers();

            // Установим начальные подсказки
            ArticleTextBox.Text = ArticleTextBox.Tag.ToString();
            NameTextBox.Text = NameTextBox.Tag.ToString();
            PriceTextBox.Text = PriceTextBox.Tag.ToString();
            StockTextBox.Text = StockTextBox.Tag.ToString();
            ImageUrlTextBox.Text = ImageUrlTextBox.Tag.ToString();
            DescriptionTextBox.Text = DescriptionTextBox.Tag.ToString();

            if (_product != null)
            {
                ArticleTextBox.Text = _product.article;
                ArticleTextBox.Foreground = System.Windows.Media.Brushes.Black;
                NameTextBox.Text = _product.name;
                NameTextBox.Foreground = System.Windows.Media.Brushes.Black;
                CategoryComboBox.SelectedValue = _product.category_id;
                PriceTextBox.Text = _product.price.ToString();
                PriceTextBox.Foreground = System.Windows.Media.Brushes.Black;
                StockTextBox.Text = _product.stock.ToString();
                StockTextBox.Foreground = System.Windows.Media.Brushes.Black;
                SupplierComboBox.SelectedValue = _product.supplier_id;
                ImageUrlTextBox.Text = _product.image_url;
                ImageUrlTextBox.Foreground = System.Windows.Media.Brushes.Black;
                DescriptionTextBox.Text = _product.description;
                DescriptionTextBox.Foreground = System.Windows.Media.Brushes.Black;
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
                // Проверяем, чтобы пользователь ввёл данные вместо подсказок
                var article = ArticleTextBox.Text == ArticleTextBox.Tag.ToString() ? string.Empty : ArticleTextBox.Text;
                var name = NameTextBox.Text == NameTextBox.Tag.ToString() ? string.Empty : NameTextBox.Text;
                var priceText = PriceTextBox.Text == PriceTextBox.Tag.ToString() ? "0" : PriceTextBox.Text;
                var stockText = StockTextBox.Text == StockTextBox.Tag.ToString() ? "0" : StockTextBox.Text;
                var imageUrl = ImageUrlTextBox.Text == ImageUrlTextBox.Tag.ToString() ? string.Empty : ImageUrlTextBox.Text;
                var description = DescriptionTextBox.Text == DescriptionTextBox.Tag.ToString() ? string.Empty : DescriptionTextBox.Text;

                if (!decimal.TryParse(priceText, out decimal price) || price < 0)
                {
                    MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(stockText, out int stock) || stock < 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var product = new Product
                {
                    id = _product?.id ?? 0,
                    article = article,
                    name = name,
                    category_id = (int)CategoryComboBox.SelectedValue,
                    price = price,
                    stock = stock,
                    supplier_id = (int)SupplierComboBox.SelectedValue,
                    image_url = imageUrl,
                    description = description
                };

                if (_product == null)
                    _dbService.CreateProduct(product);
                else
                    _dbService.UpdateProduct(product);

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