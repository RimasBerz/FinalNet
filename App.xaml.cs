﻿using FinalADO.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace FinalADO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static String ConnectinString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\ADMIN\SOURCE\FinalADO\FinalADO\CRIPTODB.MDF;Integrated Security=True"; 
        public static readonly Random random = new();

        private static DataContext dataContext = null!;
        public static DataContext DataContext
        {
            get
            {
                if (dataContext == null)
                {
                    dataContext = new DataContext();
                }
                return dataContext;
            }
        }
        public static NpUser? AuthUser { get; set; }
    }
}
