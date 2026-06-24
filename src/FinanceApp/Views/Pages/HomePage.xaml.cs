using System.Windows;
using System.Windows.Controls;

namespace FinanceApp.Views.Pages
{
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void NewVoucher_Click(object sender, RoutedEventArgs e)
        {
            // 导航到凭证页面
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // 触发凭证页面的新增凭证操作
            }
        }
    }
}
