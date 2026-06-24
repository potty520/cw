using System;
using System.Windows;
using System.Windows.Controls;
using FinanceApp.Models;
using FinanceApp.Services;
using FinanceApp.ViewModels;

namespace FinanceApp
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        private readonly AuthService _authService;

        public event Action? LoginSucceeded;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            _authService = new AuthService(new Data.DbContext(App.ConnectionString));

            _viewModel.LoginSuccessful += OnLoginSuccessful;
            _viewModel.ErrorOccurred += OnErrorOccurred;
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            txtError.Visibility = Visibility.Collapsed;
            txtError.Text = "正在连接服务器...";
            txtError.Visibility = Visibility.Visible;

            try
            {
                _viewModel.UserCode = txtUserCode.Text.Trim();
                _viewModel.Password = txtPassword.Password;

                if (string.IsNullOrEmpty(_viewModel.UserCode) || string.IsNullOrEmpty(_viewModel.Password))
                {
                    txtError.Text = "请输入用户名和密码";
                    txtError.Visibility = Visibility.Visible;
                    btnLogin.IsEnabled = true;
                    return;
                }

                txtError.Text = "正在验证用户...";
                MessageBox.Show("开始登录，请确认", "调试");

                // 直接使用同步方法测试
                var user = await Task.Run(() => _authService.LoginAsync(_viewModel.UserCode, _viewModel.Password).Result);

                MessageBox.Show("登录完成", "调试");

                if (user != null)
                {
                    App.CurrentUser = user;
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    txtError.Text = "用户名或密码错误";
                    txtError.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                var errorMsg = "登录失败：" + ex.Message;
                txtError.Text = errorMsg;
                txtError.Visibility = Visibility.Visible;
                System.Diagnostics.Debug.WriteLine("登录异常：" + ex.ToString());
                MessageBox.Show(errorMsg, "登录错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private void OnLoginSuccessful(object? sender, User user)
        {
            App.CurrentUser = user;
            LoginSucceeded?.Invoke();
        }

        private void OnErrorOccurred(object? sender, string error)
        {
            txtError.Text = error;
            txtError.Visibility = Visibility.Visible;
        }
    }
}
