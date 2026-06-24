using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FinanceApp.Models;

namespace FinanceApp.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class LoginViewModel : ViewModelBase
    {
        private string _userCode = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoggingIn;

        public string UserCode
        {
            get => _userCode;
            set => SetProperty(ref _userCode, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoggingIn
        {
            get => _isLoggingIn;
            set => SetProperty(ref _isLoggingIn, value);
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public event EventHandler<User>? LoginSuccessful;
        public event EventHandler? ExitRequested;
        public event EventHandler<string>? ErrorOccurred;

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await ExecuteLoginAsync());
            ExitCommand = new Command(() => ExitRequested?.Invoke(this, EventArgs.Empty));
        }

        private async Task ExecuteLoginAsync()
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(UserCode) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "用户名和密码不能为空";
                    return;
                }

                // 这里应该调用服务进行登录验证
                await Task.Delay(500); // 模拟网络请求

                if (UserCode == "admin" && Password == "123456")
                {
                    var user = new User
                    {
                        Id = 1,
                        UserCode = UserCode,
                        UserName = "系统管理员",
                        UserRole = UserRole.系统管理员
                    };
                    LoginSuccessful?.Invoke(this, user);
                }
                else
                {
                    ErrorMessage = "用户名或密码错误";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "登录失败：" + ex.Message;
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }

    public class Command : ICommand
    {
        private readonly Action _execute;
        private readonly Func<Task>? _executeAsync;
        private readonly Func<bool>? _canExecuteFunc;
        private readonly bool _canExecute;
        private bool _isExecuting;

        public Command(Action execute, bool canExecute = true)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(Action execute, Func<bool> canExecuteFunc)
        {
            _execute = execute;
            _canExecuteFunc = canExecuteFunc;
            _canExecute = true;
        }

        public Command(Func<Task> executeAsync, bool canExecute = true)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public Command(Func<Task> executeAsync, Func<bool> canExecuteFunc)
        {
            _executeAsync = executeAsync;
            _canExecuteFunc = canExecuteFunc;
            _canExecute = true;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => (_canExecuteFunc?.Invoke() ?? _canExecute) && !_isExecuting;

        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                _isExecuting = true;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                try
                {
                    await _executeAsync();
                }
                finally
                {
                    _isExecuting = false;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                _execute();
            }
        }
    }

    public class Command<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecuteFunc;
        private bool _isExecuting;

        public Command(Action<T?> execute, Func<T?, bool>? canExecuteFunc = null)
        {
            _execute = execute;
            _canExecuteFunc = canExecuteFunc;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecuteFunc?.Invoke((T?)parameter) ?? true);

        public void Execute(object? parameter)
        {
            _isExecuting = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                _execute((T?)parameter);
            }
            finally
            {
                _isExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
