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

namespace FinalADO
{
    public partial class PortalWindow : Window
    {
        public PortalWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Кнопки для перехода в другире окна и закрытие
        {
            this.Hide();
            new MainWindow().ShowDialog();
            this.Show();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new CrytoBaseNetWindow().ShowDialog();
            this.Show();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new RegisterWindow().ShowDialog();
            this.Show();
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e) // Кнопка для закртия 
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Подтверждение закрытия", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
        #region soon
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // пространство для будущей курсовой
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // пространство для будущей курсовой
        }
        #endregion

        
    }
}
