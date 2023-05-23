using FinalADO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalADO.CRUD
{
    /// <summary>
    /// Interaction logic for CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {

        public Entities.Cryptocurrencies Cryptocurrency { get; set; } = null!;

        public CrudDepartmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Cryptocurrency is null)  // режим "C" - добавление криптовалюты
            {
                Cryptocurrency = new Cryptocurrencies()
                {
                    Id = Guid.NewGuid()
                };
                ButtonDelete.IsEnabled = false;
            }
            else  // Режимы "UD" - есть переданная криптовалюта для изменения/удаления
            {
                ButtonDelete.IsEnabled = true;
            }
            CryptoId.Text = Cryptocurrency.Id.ToString();
            CryptoName.Text = Cryptocurrency.Name;
            CruptoCount.Text = Cryptocurrency.Count.ToString();
            CruptoPrice.Text = Cryptocurrency.Price.ToString();
            CruptoDate.DataContext = Cryptocurrency.DateStart;
            CruptoPercent.Text = Cryptocurrency.PercentChange.ToString();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
             MessageBox.Show(
                 "Вы подтверждаете удаление криптовалюты из БД?",
                 "Удаление данных из БД",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Question))
            {
                Cryptocurrency = null!;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (Cryptocurrency is null) return;

            if (CryptoName.Text == String.Empty)
            {
                MessageBox.Show("Введите название криптовалюты!");
                CryptoName.Focus();
            }
            else if (!double.TryParse(CruptoCount.Text, out double count))
            {
                MessageBox.Show("Введите корректное значение количества криптовалюты!");
                CruptoCount.Focus();
            }
            else if (!double.TryParse(CruptoPrice.Text, out double price))
            {
                MessageBox.Show("Введите корректное значение цены криптовалюты!");
                CruptoPrice.Focus();
            }
            else if (CruptoDate.DataContext is null)
            {
                MessageBox.Show("Выберите дату начала использования криптовалюты!");
                CruptoDate.Focus();
            }
            else if (!double.TryParse(CruptoPercent.Text, out double percentChange))
            {
                MessageBox.Show("Введите корректное значение процентного изменения криптовалюты!");
                CruptoPercent.Focus();
            }
            else
            {
                Cryptocurrency.Name = CryptoName.Text;
                Cryptocurrency.Count = count;
                Cryptocurrency.Price = price;
                CruptoDate.DataContext = Cryptocurrency.DateStart;
                Cryptocurrency.PercentChange = percentChange;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
