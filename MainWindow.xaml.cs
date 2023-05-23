﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using FinalADO.CRUD;
using System.Collections.ObjectModel;

namespace FinalADO
{

    public partial class MainWindow : Window
    {
        private readonly SqlConnection _connection;  
        private List<Entities.Cryptocurrencies>? _cryptocurrencies;  
        private List<Entities.Investors>? _investors;
        //private List<Entities.Transactions>? _transactions;
        public ObservableCollection<Entities.Cryptocurrencies>? Crypto { get; set; }
        public ObservableCollection<Entities.Investors>? Investors { get; set; }
        public ObservableCollection<Entities.Transactions>? Transactions { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _connection = new(App.ConnectinString);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _connection.Open();  
                MonitorConnection.Content = "Установлено";
                MonitorConnection.Foreground = Brushes.Green;
            }
            catch(SqlException ex)
            {
                MonitorConnection.Content = "Закрыто";
                MonitorConnection.Foreground = Brushes.Red;
                MessageBox.Show(ex.Message);
                this.Close();
            }
            ShowDepartmentsCount();
            ShowManagersCount();
            ShowProductsCount();
            ShowSalesCount();
            ShowTransactions();
            //ShowDailyStatistics();

            ShowDepartments();
            ShowProductsOrm();
           
        }
        #region Show Monitor

        private void ShowDepartmentsCount()
        {
            String sql = "SELECT COUNT(*) FROM Cryptocurrencies";
            // SqlCommand объект для передачи команд (запросов) к БД.
            // Требует закрытия, поэтому using
            using var cmd = new SqlCommand(sql, _connection);
            // созание объекта не исполняет команду, для этого есть методы ExecuteXxxx
            MonitorCryptoCount.Content =
                Convert.ToString(
                    cmd.ExecuteScalar()
                );
        }

        private void ShowProductsCount()
        {
            String sql = "SELECT COUNT(*) FROM Investors";
            using var cmd = new SqlCommand(sql, _connection);
            MonitorInvestors.Content = Convert.ToString( cmd.ExecuteScalar() );
        }


        private void ShowManagersCount()
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM Transactions", _connection);
            MonitorTranzactionsCount.Content = Convert.ToString(cmd.ExecuteScalar());
        }


        private void ShowSalesCount()
        {
            using SqlCommand cmd = new("SELECT COUNT(C.Price) FROM Cryptocurrencies C", _connection);
            MonitorMoneyCount.Content = Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion
        //private void ShowDailyStatistics()
        //{
        //    SqlCommand cmd = new()
        //    {
        //        Connection = _connection
        //    };
        //    String date = $"2022-{DateTime.Now.Month}-{DateTime.Now.Day}";

        //    cmd.CommandText = $"SELECT COUNT(*) FROM Sales S WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalSales.Content = Convert.ToString(cmd.ExecuteScalar());

        //    cmd.CommandText = $"SELECT SUM(S.Cnt) FROM Sales S WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalProducts.Content = Convert.ToString(cmd.ExecuteScalar());

        //    cmd.CommandText = $"SELECT ROUND( SUM( S.Cnt * P.Price ), 2 ) FROM Sales S JOIN Products P ON S.Id_product = P.Id WHERE CAST( S.Moment AS DATE ) = '{date}'";
        //    StatTotalMoney.Content = Convert.ToString(cmd.ExecuteScalar());

        //    cmd.Dispose();
        //}
        private void ShowTransactions()
        {
            using SqlCommand cmd = new("SELECT TOP 1 Name, Count FROM Cryptocurrencies ORDER BY Count DESC;", _connection);
          //  CryptoName.Content = Convert.ToString(cmd.ExecuteScalar());
        }
        //private void ShowTransactionsExp()
        //{
        //    using SqlCommand cmd = new("SELECT * FROM Transactions", _connection);
        //    SqlDataReader reader = cmd.ExecuteReader();
        //    TransInfo.Text = "";                      
        //    while (reader.Read())                            
        //    {                                              
        //        Guid id = reader.GetGuid("id");           
        //        String name = (String)reader[1];

        //        TransInfo.Text += $"{id} {name} \n";
        //    }
           
        //    reader.Dispose();
        //}

        //private void ShowDepartmentsOrm()
        //{
        //    if (_cryptocurrencies is null)
        //    {
        //        using SqlCommand cmd =
        //            new("SELECT C.Id, C.Name,C.Count,C.Price,C.DateStart,C.PercentChange,C.InvestorId FROM Cryptocurrencies C", _connection);
        //        try
        //        {
        //            using SqlDataReader reader = cmd.ExecuteReader();
        //            _cryptocurrencies = new();
        //            while (reader.Read())
        //            {
        //                _cryptocurrencies.Add(new()
        //                {
        //                    Id = reader.GetGuid(0),
        //                    Name = reader.GetString(1),
        //                    Count = (double)reader.GetDecimal(2),
        //                    Price = (double)reader.GetDecimal(3),
        //                    DateStart = reader.GetDateTime(4),
        //                    PercentChange = (double)reader.GetDecimal(5),
        //                });
        //            }
        //        }
        //        catch (SqlException ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            return;
        //        }
        //    }
        //    CrytoInfo.Text = "";
        //    foreach (var department in _cryptocurrencies)
        //    {
        //        CrytoInfo.Text += department.ToShortString() + "\n";
        //    }
        //}
        private void ShowDepartments()
        {
            if (_cryptocurrencies is null)
            {
                using SqlCommand cmd =
                    new("SELECT C.Id, C.Name,C.Count,C.Price,C.DateStart,C.PercentChange,C.InvestorId FROM Cryptocurrencies C", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _cryptocurrencies = new();
                    while (reader.Read())
                    {
                        _cryptocurrencies.Add(new()
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1),
                            Count = (double)reader.GetDecimal(2),
                            Price = (double)reader.GetDecimal(3),
                            DateStart = reader.GetDateTime(4),
                            PercentChange = (double)reader.GetDecimal(5),
                        });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            CrytoInfo1.Text = "";
            CrytoInfo2.Text = "";
            CrytoInfo3.Text = "";
            CrytoInfo4.Text = "";
            foreach (var department in _cryptocurrencies)
            {
                CrytoInfo1.Text += department.Name + "\n";
                CrytoInfo2.Text += department.Count.ToString() + "\n";
                CrytoInfo3.Text += department.Price.ToString() + "\n";
                CrytoInfo4.Text += department.PercentChange.ToString() + "\n";
            }
        }
        private void ShowProductsOrm()
        {
            if (_investors is null)  
            {
                using SqlCommand cmd =
                    new("SELECT id,Name FROM Investors;", _connection);
                try
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    _investors = new();
                    while (reader.Read())
                    {
                       _investors.Add(new()
                         {
                           Id = reader.GetGuid(0),
                           Name = reader.GetString(1),
                         });
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            InvestInfo1.Text = "";
            InvestInfo2.Text = "------";
            InvestInfo3.Text = "------";
            foreach (var product in _investors)
            {
                InvestInfo1.Text += product.ToShortString() + "\n";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        private void AddCriptocurrencies_Click(object sender, RoutedEventArgs e)
        {
            var window = new CRUD.CrudManagerWindow();
            window.Show();
        }
        private void ListViewItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
           
        }

     
        private void AddAddInvestor_Click(object sender, RoutedEventArgs e)
        {
            var window = new CRUD.CrudDepartmentWindow();
            window.Show();
        }
    }
}
