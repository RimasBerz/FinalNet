using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Data.SqlClient;
namespace FinalADO
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private const string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin\source\FinalADO\FinalADO\PasswordCheck.mdf;Integrated Security=True";
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Focus();
        }

        private void LoginButton_Click_1(object sender, RoutedEventArgs e) // Проверка пользователя 
        {
            string login = LoginTextBox.Text;
            string password = PasswordTextBox.Password;

            // Создаем подключение к базе данных
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Создаем SQL-запрос для проверки наличия пользователя в базе данных
                string query = "SELECT COUNT(*) FROM NpUsers WHERE Login = @Login AND Password = @Password";

                // Создаем команду и передаем параметры
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    // Выполняем запрос и получаем результат
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        // Пользователь найден, переходим на главную страницу
                        this.Hide();
                        new PortalWindow().ShowDialog();
                        PasswordTextBox.Password = "";
                    }
                    else
                    {
                        // Пользователь не найден, выводим сообщение об ошибке
                        MessageBox.Show("Вход отклонен");
                    }
                }
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e) // Закрытие приложения
        {
            var result = MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Подтверждение закрытия", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    

        private void RegButton_Click_1(object sender, RoutedEventArgs e) // Переход на регистрацию
        {
            this.Hide();
            new RegisterWindow().ShowDialog();
            this.Show();
        }
    }
}
