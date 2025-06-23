using App_grocery_store.Models;
using App_grocery_store.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace App_grocery_store
{
    public partial class SupplierWindow : Window
    {
        private readonly DbService _dbService;
        private readonly Supplier _supplier;

        public SupplierWindow(DbService dbService, Supplier supplier)
        {
            InitializeComponent();
            _dbService = dbService;
            _supplier = supplier;

            OrganizationNameTextBox.Text = OrganizationNameTextBox.Tag.ToString();
            AddressTextBox.Text = AddressTextBox.Tag.ToString();
            PhoneNumberTextBox.Text = PhoneNumberTextBox.Tag.ToString();
            OgrnTextBox.Text = OgrnTextBox.Tag.ToString();

            if (_supplier != null)
            {
                OrganizationNameTextBox.Text = _supplier.organization_name;
                OrganizationNameTextBox.Foreground = System.Windows.Media.Brushes.Black;
                AddressTextBox.Text = _supplier.address;
                AddressTextBox.Foreground = System.Windows.Media.Brushes.Black;
                PhoneNumberTextBox.Text = _supplier.phone_number;
                PhoneNumberTextBox.Foreground = System.Windows.Media.Brushes.Black;
                OgrnTextBox.Text = _supplier.ogrn;
                OgrnTextBox.Foreground = System.Windows.Media.Brushes.Black;
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
                var organizationName = OrganizationNameTextBox.Text == OrganizationNameTextBox.Tag.ToString() ? string.Empty : OrganizationNameTextBox.Text;
                var address = AddressTextBox.Text == AddressTextBox.Tag.ToString() ? string.Empty : AddressTextBox.Text;
                var phoneNumber = PhoneNumberTextBox.Text == PhoneNumberTextBox.Tag.ToString() ? string.Empty : PhoneNumberTextBox.Text;
                var ogrn = OgrnTextBox.Text == OgrnTextBox.Tag.ToString() ? string.Empty : OgrnTextBox.Text;

                var supplier = new Supplier
                {
                    id = _supplier?.id ?? 0,
                    organization_name = organizationName,
                    address = address,
                    phone_number = phoneNumber,
                    ogrn = ogrn
                };

                if (_supplier == null)
                    _dbService.CreateSupplier(supplier);
                else
                    _dbService.UpdateSupplier(supplier);

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