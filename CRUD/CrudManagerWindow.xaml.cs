using FinalADO.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CrudManagerWindow.xaml
    /// </summary>
    public partial class CrudManagerWindow : Window
    {
        public Entities.Investors Investor { get; set; } = null!;
        public CrudManagerWindow(Investors? investor = null)
        {
            InitializeComponent();

            if (investor == null)
            {
                Investor = new Investors { Id = Guid.NewGuid() };
            }
            else
            {
                Investor = investor;
                InvestorName.Text = Investor.Name;
            }
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DataContext = (Owner as DisconnectWindow);
        //}

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InvestorName.Text))
            {
                MessageBox.Show("Введите имя инвестора");
            }
            else
            {
                Investor.Name = InvestorName.Text;

                MessageBox.Show(
                    $"{Investor.ToShortString()} - сохранено"
                );
            }

        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes ==
                 MessageBox.Show(
                     "Вы подтверждаете удаление инвестора из БД?",
                     "Удаление данных из БД",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question))
            {
                Investor = null;
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
