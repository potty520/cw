using System;
using System.Windows;
using System.Windows.Controls;
using FinanceApp.Views.Pages;

namespace FinanceApp
{
    public partial class MainWindow : Window
    {
        private string _currentModule = "home";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser != null)
            {
                txtUserInfo.Text = $"{App.CurrentUser.UserName} ({App.CurrentUser.UserRole})";
            }
            ShowHomePage();
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string module)
            {
                _currentModule = module;
                UpdateButtonStates();
                UpdateContentTitle();

                switch (module)
                {
                    case "home":
                        ShowHomePage();
                        break;
                    case "voucher":
                        ShowVoucherPage();
                        break;
                    case "ledger":
                        ShowLedgerPage();
                        break;
                    case "report":
                        ShowReportPage();
                        break;
                    case "fixedasset":
                        ShowFixedAssetPage();
                        break;
                    case "salary":
                        ShowSalaryPage();
                        break;
                    default:
                        MainContent.Content = new TextBlock { Text = $"模块：{module}", Margin = new Thickness(10) };
                        break;
                }
            }
        }

        private void UpdateButtonStates()
        {
            btnHome.Background = _currentModule == "home" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
            btnVoucher.Background = _currentModule == "voucher" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
            btnLedger.Background = _currentModule == "ledger" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
            btnReport.Background = _currentModule == "report" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
            btnFixedAsset.Background = _currentModule == "fixedasset" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
            btnSalary.Background = _currentModule == "salary" ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235)) : System.Windows.Media.Brushes.Transparent;
        }

        private void UpdateContentTitle()
        {
            txtContentTitle.Text = _currentModule switch
            {
                "home" => "欢迎使用财智星财务软件",
                "voucher" => "凭证管理",
                "ledger" => "账簿查询",
                "report" => "报表中心",
                "fixedasset" => "固定资产管理",
                "salary" => "工资管理",
                _ => "财智星财务软件"
            };
        }

        private void ShowHomePage()
        {
            var homePage = new HomePage();
            MainContent.Content = homePage;
        }

        private void ShowVoucherPage()
        {
            var voucherPage = new VoucherPage();
            MainContent.Content = voucherPage;
        }

        private void ShowLedgerPage()
        {
            var ledgerPage = new LedgerPage();
            MainContent.Content = ledgerPage;
        }

        private void ShowReportPage()
        {
            var reportPage = new ReportPage();
            MainContent.Content = reportPage;
        }

        private void ShowFixedAssetPage()
        {
            var assetPage = new FixedAssetPage();
            MainContent.Content = assetPage;
        }

        private void ShowSalaryPage()
        {
            var salaryPage = new SalaryPage();
            MainContent.Content = salaryPage;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("确定要退出系统吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                App.CurrentUser = null;
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
