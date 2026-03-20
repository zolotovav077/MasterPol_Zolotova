using LiveCharts;
using LiveCharts.Wpf;
using MasterPol.Data_Zolotova.Database;
using MasterPol.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MasterPol.WPF
{
    public partial class SalesHistoryWindow : Window
    {
        private readonly PartnerRepository _repository;
        private readonly BusinessPartner _partner;
        private List<SaleRecord> _salesHistory;

        public SalesHistoryWindow(PartnerRepository repository, BusinessPartner partner)
        {
            InitializeComponent();
            _repository = repository;
            _partner = partner;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPartnerName.Text = $"История продаж: {_partner.CompanyName}";
            LoadSalesHistory();
            LoadPieChart();
        }

        private void LoadSalesHistory()
        {
            try
            {
                _salesHistory = _repository.GetPartnerSalesHistory(_partner.Id);
                SalesGrid.ItemsSource = _salesHistory;

                decimal totalSales = 0;
                foreach (var sale in _salesHistory)
                {
                    totalSales += sale.TotalAmount;
                }
                TxtTotalSales.Text = $"💰 Общая сумма продаж: {totalSales:N2} ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPieChart()
        {
            try
            {
                if (_salesHistory == null || _salesHistory.Count == 0)
                {
                    // Если нет продаж, показываем пустую диаграмму
                    SalesPieChart.Series = new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = "Нет данных",
                            Values = new ChartValues<decimal> { 1 },
                            Fill = System.Windows.Media.Brushes.Gray
                        }
                    };
                    return;
                }

                // Группировка продаж по товарам
                var productSales = new Dictionary<string, decimal>();

                foreach (var sale in _salesHistory)
                {
                    string productName = sale.Product?.Name ?? "Неизвестный товар";
                    if (productSales.ContainsKey(productName))
                    {
                        productSales[productName] += sale.TotalAmount;
                    }
                    else
                    {
                        productSales[productName] = sale.TotalAmount;
                    }
                }

                // Создание серий для круговой диаграммы
                var series = new SeriesCollection();
                var colors = new[]
                {
                    System.Windows.Media.Color.FromRgb(76, 175, 80),   // Зеленый
                    System.Windows.Media.Color.FromRgb(33, 150, 243),  // Синий
                    System.Windows.Media.Color.FromRgb(255, 152, 0),   // Оранжевый
                    System.Windows.Media.Color.FromRgb(156, 39, 176),  // Фиолетовый
                    System.Windows.Media.Color.FromRgb(233, 30, 99),   // Розовый
                    System.Windows.Media.Color.FromRgb(0, 188, 212),   // Голубой
                };

                int colorIndex = 0;
                foreach (var item in productSales)
                {
                    var pieSeries = new PieSeries
                    {
                        Title = item.Key,
                        Values = new ChartValues<decimal> { item.Value },
                        DataLabels = true,
                        LabelPoint = point => $"{point.Y:N0} ₽",
                        Fill = new System.Windows.Media.SolidColorBrush(colors[colorIndex % colors.Length])
                    };
                    series.Add(pieSeries);
                    colorIndex++;
                }

                SalesPieChart.Series = series;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении диаграммы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAddSale_Click(object sender, RoutedEventArgs e)
        {
            var addSaleWindow = new AddSaleWindow(_repository, _partner);
            addSaleWindow.Owner = this;

            if (addSaleWindow.ShowDialog() == true)
            {
                // Обновляем данные
                LoadSalesHistory();
                LoadPieChart();
            }
        }
    }
}