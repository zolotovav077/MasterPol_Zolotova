using MasterPol.Data_Zolotova.Database;
using MasterPol.Lib;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MasterPol.WPF
{
    public partial class AddSaleWindow : Window
    {
        private readonly PartnerRepository _repository;
        private readonly BusinessPartner _partner;
        private MerchandiseItem _selectedProduct;

        public AddSaleWindow(PartnerRepository repository, BusinessPartner partner)
        {
            InitializeComponent();
            _repository = repository;
            _partner = partner;

            Loaded += AddSaleWindow_Loaded;
        }

        private void AddSaleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
            DateSale.SelectedDate = DateTime.Today;
            DateSale.DisplayDateEnd = DateTime.Today; // Запрещаем будущие даты
        }

        private void LoadProducts()
        {
            try
            {
                var products = _repository.GetAllProducts();
                CmbProduct.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbProduct_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedProduct = CmbProduct.SelectedItem as MerchandiseItem;
            if (_selectedProduct != null)
            {
                TxtArticle.Text = _selectedProduct.Article;
                TxtPrice.Text = $"{_selectedProduct.MinPartnerPrice:N2} ₽";
                CalculateTotal();
            }
        }

        private void TxtQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void TxtQuantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void CalculateTotal()
        {
            if (_selectedProduct != null && int.TryParse(TxtQuantity.Text, out int quantity) && quantity > 0)
            {
                decimal total = _selectedProduct.MinPartnerPrice * quantity;
                TxtTotalAmount.Text = $"{total:N2} ₽";
            }
            else
            {
                TxtTotalAmount.Text = "0 ₽";
            }
        }

        private bool ValidateInput()
        {
            if (CmbProduct.SelectedItem == null)
            {
                ShowError("Выберите товар");
                return false;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                ShowError("Введите положительное целое число");
                return false;
            }

            if (DateSale.SelectedDate == null)
            {
                ShowError("Выберите дату продажи");
                return false;
            }

            if (DateSale.SelectedDate > DateTime.Today)
            {
                ShowError("Дата продажи не может быть в будущем");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                var sale = new SaleRecord
                {
                    PartnerId = _partner.Id,
                    ProductId = ((MerchandiseItem)CmbProduct.SelectedItem).Id,
                    Quantity = int.Parse(TxtQuantity.Text),
                    SaleDate = DateSale.SelectedDate.Value
                };

                _repository.AddSaleRecord(sale);

                MessageBox.Show("Продажа успешно добавлена", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}