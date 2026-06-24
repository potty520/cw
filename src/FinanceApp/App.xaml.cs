using System;
using System.Windows;
using FinanceApp.Models;
using FinanceApp.ViewModels;
using FinanceApp.Views.Pages;

namespace FinanceApp
{
    public partial class App : Application
    {
        public static string ConnectionString { get; set; } = "Server=103.236.96.82;Port=13306;Database=FinanceDB;User=remote;Password=ToCAt69Aidc16I1KvkQo2YWMlUl01U0o;CharSet=utf8;Connection Timeout=30;Default Command Timeout=30;";
        public static User? CurrentUser { get; set; }
        public static MainViewModel MainViewModel { get; } = new();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                MessageBox.Show("应用程序错误：" + ex.ExceptionObject?.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                MessageBox.Show("UI错误：" + ex.Exception.Message + "\n\n" + ex.Exception.StackTrace, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };

            var loginWindow = new LoginWindow();

            loginWindow.LoginSucceeded += () =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                loginWindow.Close();
            };

            loginWindow.Show();
        }
    }
}
