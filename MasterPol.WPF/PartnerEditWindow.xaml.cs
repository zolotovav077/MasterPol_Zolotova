using MasterPol.Data_Zolotova.Database;
using MasterPol.Lib;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MasterPol.WPF
{
    public partial class PartnerEditWindow : Window
    {
        private readonly PartnerRepository _repository;
        private readonly BusinessPartner _currentPartner;
        private readonly bool _isEditMode;

        public PartnerEditWindow(PartnerRepository repository, BusinessPartner partner = null)
        {
            InitializeComponent();
            _repository = repository;
            _currentPartner = partner;
            _isEditMode = partner != null;

            Loaded += PartnerEditWindow_Loaded;
        }

        private void PartnerEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPartnerTypes();

            if (_isEditMode)
            {
                TxtWindowTitle.Text = "✏️ Редактирование партнера";
                LoadPartnerData();
            }
        }

        private void LoadPartnerTypes()
        {
            try
            {
                var types = _repository.GetAllPartnerTypes();
                CmbPartnerType.ItemsSource = types;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке типов партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerData()
        {
            if (_currentPartner != null)
            {
                CmbPartnerType.SelectedValue = _currentPartner.PartnerTypeId;
                TxtCompanyName.Text = _currentPartner.CompanyName;
                TxtLegalAddress.Text = _currentPartner.LegalAddress;
                TxtInn.Text = _currentPartner.Inn;
                TxtDirectorName.Text = _currentPartner.DirectorName;
                TxtPhone.Text = _currentPartner.Phone;
                TxtEmail.Text = _currentPartner.Email;
                TxtRating.Text = _currentPartner.Rating.ToString();
            }
        }

        private void TxtRating_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private bool ValidateInput()
        {
            // Проверка обязательных полей
            if (CmbPartnerType.SelectedItem == null)
            {
                ShowError("Выберите тип партнера");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCompanyName.Text))
            {
                ShowError("Введите наименование компании");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtDirectorName.Text))
            {
                ShowError("Введите ФИО директора");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                ShowError("Введите телефон");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                ShowError("Введите email");
                return false;
            }

            // Проверка email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(TxtEmail.Text, emailPattern))
            {
                ShowError("Введите корректный email адрес");
                return false;
            }

            // Проверка рейтинга
            if (!int.TryParse(TxtRating.Text, out int rating) || rating < 0)
            {
                ShowError("Рейтинг должен быть целым неотрицательным числом");
                return false;
            }

            // Проверка ИНН (если введен)
            if (!string.IsNullOrWhiteSpace(TxtInn.Text) && TxtInn.Text.Length != 10 && TxtInn.Text.Length != 12)
            {
                ShowError("ИНН должен содержать 10 или 12 цифр");
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
                var partner = _isEditMode ? _currentPartner : new BusinessPartner();

                partner.PartnerTypeId = (int)CmbPartnerType.SelectedValue;
                partner.CompanyName = TxtCompanyName.Text.Trim();
                partner.LegalAddress = TxtLegalAddress.Text?.Trim();
                partner.Inn = TxtInn.Text?.Trim();
                partner.DirectorName = TxtDirectorName.Text.Trim();
                partner.Phone = TxtPhone.Text.Trim();
                partner.Email = TxtEmail.Text.Trim();
                partner.Rating = int.Parse(TxtRating.Text);

                if (_isEditMode)
                {
                    _repository.UpdatePartner(partner);
                    MessageBox.Show("Данные партнера успешно обновлены", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _repository.AddPartner(partner);
                    MessageBox.Show("Новый партнер успешно добавлен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}