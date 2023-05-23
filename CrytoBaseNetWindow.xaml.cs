using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Web;
using System.Security.Cryptography;
using FinalADO.Data;
using System.Net.Mail;

namespace FinalADO
{
    public partial class CrytoBaseNetWindow : Window
    {
        private dynamic? emailConfig;

        private readonly HttpClient httpClient = new HttpClient();
        private HttpClient _httpClient = new();
        private readonly DataContext _dataContext;
        public ObservableCollection<Params> Params { get; set; }
        private int indexShown;
        public CrytoBaseNetWindow()
        {
            InitializeComponent();
            httpClient = new();
            Params = new();
            this.DataContext = this;
            _dataContext = App.DataContext;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssets();
           
        }
        private async void LoadAssets() // подгрузка информации из CoinCap
        {
            var assetsResponse = await _httpClient.GetFromJsonAsync<ParamsResponse>(
               "https://api.coincap.io/v2/assets");
            if (assetsResponse is null)
            {
                MessageBox.Show("JSON error");
                return;
            }
            indexShown = 4;
            ShowHistory(assetsResponse.data[indexShown]);

            Params.Clear();
            foreach (Params asset in assetsResponse.data)
            {
                Params.Add(asset);
            }
            ParamsListView.SelectedIndex = indexShown;
        }
        private async void ShowHistory(Params asset) // Отрисовка графиков с использованием истории на сайте CoinCap
        {
            String url = $"https://api.coincap.io/v2/assets/{asset.id}/history?interval=d1";
            var historyResponse = await _httpClient.GetFromJsonAsync<HistoryResponse>(url);
            if (historyResponse is null)
            {
                MessageBox.Show("JSON history error");
                return;
            }
            long minimalT, maximumT;

            minimalT = historyResponse.data.Min(r => r.time);
            maximumT = historyResponse.data.Max(r => r.time);

            double minimalR, maximumR;

            minimalR = historyResponse.data.Min(r => r.priceUsd);
            maximumR = historyResponse.data.Max(r => r.priceUsd);

            double graphHeight = GraphCanvas.ActualHeight;
            double graphWidth = GraphCanvas.ActualWidth;
            double x1, y1, x2, y2;
            double dx = graphWidth / (maximumT - minimalT);

            const double offset = 0.05;
            double h = graphHeight * (1 - offset);
            double dy = graphHeight / (maximumR - minimalR) * (1 - 2 * offset);

            x1 = (historyResponse.data[0].time - minimalT) * dx;
            y1 = h - (historyResponse.data[0].priceUsd - minimalR) * dy;

            foreach (Rate rate in historyResponse.data)
            {
                x2 = (rate.time - minimalT) * dx;
                y2 = h - (rate.priceUsd - minimalR) * dy;
                DrawLine(x1, y1, x2, y2);
                x1 = x2;
                y1 = y2;
            }
        }
        private void DrawLine(double fX, double fY, double tX, double tY) // Создание линии для отрисовки
        {
            Line line = new()
            {
                X1 = fX,
                Y1 = fY,
                X2 = tX,
                Y2 = tY,

                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };
            GraphCanvas.Children.Add(line);
        }
        private async void ParamsListView_SelectionChanged(object sender, SelectionChangedEventArgs e) // Выбор пункта из списка и его отрисовка 
        {
            if (indexShown == ParamsListView.SelectedIndex)
            {
                return;
            }
            GraphCanvas.Children.Clear();
            Params selectedAsset = Params[ParamsListView.SelectedIndex];
            NamesC.Content = "Rate history: " + selectedAsset.name;

            String url = $"https://api.coincap.io/v2/assets/{selectedAsset.id}/history?interval=d1";
            var historyResponse = await _httpClient.GetFromJsonAsync<HistoryResponse>(url);
            if (historyResponse != null && historyResponse.data != null && historyResponse.data.Any())
            {
                double minRate, maxRate;
                minRate = historyResponse.data.Min(r => r.priceUsd);
                maxRate = historyResponse.data.Max(r => r.priceUsd);

                Label MinC = new Label();
                MinC.Content = "Min: " + minRate;
                MinC.SetValue(Canvas.LeftProperty, GraphCanvas.ActualWidth * 0.7);
                MinC.SetValue(Canvas.TopProperty, GraphCanvas.ActualHeight * 0.1);
                MinC.VerticalAlignment = VerticalAlignment.Top;

                if (GraphCanvas.Children.Contains(MinC))
                {
                    GraphCanvas.Children.Remove(MinC);
                }

                GraphCanvas.Children.Add(MinC);

                Label MaxC = new Label();
                MaxC.Content = "Max: " + maxRate;
                MaxC.SetValue(Canvas.LeftProperty, GraphCanvas.ActualWidth * 0.7);
                MaxC.SetValue(Canvas.TopProperty, GraphCanvas.ActualHeight * 0.01);
                MaxC.VerticalAlignment = VerticalAlignment.Top;
                GraphCanvas.Children.Add(MaxC);
            }

            ShowHistory(selectedAsset);
            indexShown = ParamsListView.SelectedIndex;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) //Выход
        {
            this.Hide();
            new PortalWindow().ShowDialog();
            this.Show();
        }
    }
    }
    // ORM
public class Params // Класс для хранения параметров активов
{
        public string id { get; set; }
        public string name { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }
    }
    public class ParamsResponse // Класс для хранения ответа, содержащего список параметров активов 
{
        public List<Params> data { get; set; }
        public long timestamp { get; set; }
    }
    public class Rate
    {
        public double priceUsd { get; set; }
        public long time { get; set; }
        public DateTime date { get; set; }
}    // Класс для хранения информации о курсе актива
public class HistoryResponse // Класс для хранения ответа, содержащего данные истории цен активов
{
        public List<Rate> data { get; set; }
        public long timestamp { get; set; }
    }
        public class ParamsPriceResponse // Класс для хранения ответа, содержащего список параметров цен активов
{
            public List<ParamsPrice> data { get; set; }
        }
        public class ParamsPrice // Класс для хранения информации о параметрах цены актива
{
            public DateTimeOffset time { get; set; }
            public double priceUsd { get; set; }
        }


