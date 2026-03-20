using MasterPol.Data_Zolotova.Database;
using MasterPol.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MasterPol.WPF
{
    public partial class MainWindow : Window
    {
        private PartnerRepository _repository;
        private List<BusinessPartner> _partners;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                TxtStatus.Text = "Загрузка данных...";
            }));

            _repository = new PartnerRepository();

            // Проверка подключения к БД
            if (_repository.TestConnection())
            {
                TxtDbStatus.Text = "✅ Подключено к БД";
                LoadPartners();
            }
            else
            {
                TxtDbStatus.Text = "❌ Ошибка подключения к БД";
                MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartners()
        {
            try
            {
                _partners = _repository.GetAllPartners();
                PartnersList.ItemsSource = _partners;
                TxtPartnerCount.Text = $"Всего партнеров: {_partners.Count}";
                TxtStatus.Text = "Данные загружены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtStatus.Text = "Ошибка загрузки";
            }
        }

        private void PartnerCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                int partnerId = (int)border.Tag;
                OpenEditWindow(partnerId);
            }
        }

        private void BtnAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new PartnerEditWindow(_repository);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                LoadPartners(); // Обновляем список
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int partnerId = (int)button.Tag;
                OpenEditWindow(partnerId);
            }
        }

        private void OpenEditWindow(int partnerId)
        {
            var partner = _partners.FirstOrDefault(p => p.Id == partnerId);
            if (partner != null)
            {
                var editWindow = new PartnerEditWindow(_repository, partner);
                editWindow.Owner = this;

                if (editWindow.ShowDialog() == true)
                {
                    LoadPartners(); // Обновляем список
                }
            }
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int partnerId = (int)button.Tag;
                var partner = _partners.FirstOrDefault(p => p.Id == partnerId);

                if (partner != null)
                {
                    var historyWindow = new SalesHistoryWindow(_repository, partner);
                    historyWindow.Owner = this;
                    historyWindow.ShowDialog();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int partnerId = (int)button.Tag;
                var partner = _partners.FirstOrDefault(p => p.Id == partnerId);

                if (partner != null)
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить партнера '{partner.CompanyName}'?\n" +
                        "Это действие нельзя отменить.",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            _repository.DeletePartner(partnerId);
                            LoadPartners(); // Обновляем список
                            TxtStatus.Text = "Партнер удален";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
        }
    }
    public class DiscountToColorConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is BusinessPartner partner)
            {
                using (var repo = new PartnerRepository())
                {
                    decimal totalSales = repo.GetPartnerTotalSales(partner.Id);
                    int discount = repo.CalculateDiscount(totalSales);

                    string colorCode;
                    if (discount >= 15)
                        colorCode = "#4CAF50";
                    else if (discount >= 10)
                        colorCode = "#2196F3";
                    else if (discount >= 5)
                        colorCode = "#FF9800";
                    else
                        colorCode = "#9E9E9E";

                    return new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorCode));
                }
            }
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}