using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FinanceApp.Models;

namespace FinanceApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private User? _currentUser;
        private string _currentModule = "首页";
        private string _statusMessage = "就绪";
        private int _currentPeriodYear;
        private int _currentPeriodMonth;

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public string CurrentModule
        {
            get => _currentModule;
            set => SetProperty(ref _currentModule, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int CurrentPeriodYear
        {
            get => _currentPeriodYear;
            set => SetProperty(ref _currentPeriodYear, value);
        }

        public int CurrentPeriodMonth
        {
            get => _currentPeriodMonth;
            set => SetProperty(ref _currentPeriodMonth, value);
        }

        public ObservableCollection<MenuItem> MenuItems { get; } = new();
        public ObservableCollection<MenuItem> QuickAccessItems { get; } = new();

        public ICommand NavigateCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LogoutCommand { get; }

        public event EventHandler<string>? ModuleChanged;
        public event EventHandler? LogoutRequested;

        public MainViewModel()
        {
            NavigateCommand = new Command<string>(Navigate);
            RefreshCommand = new Command(Refresh);
            LogoutCommand = new Command(Logout);

            _currentPeriodYear = DateTime.Now.Year;
            _currentPeriodMonth = DateTime.Now.Month;

            InitializeMenuItems();
        }

        private void InitializeMenuItems()
        {
            // 主菜单
            MenuItems.Add(new MenuItem { Code = "home", Name = "首页", Icon = "home" });
            MenuItems.Add(new MenuItem { Code = "voucher", Name = "凭证管理", Icon = "document" });
            MenuItems.Add(new MenuItem { Code = "ledger", Name = "账簿查询", Icon = "book" });
            MenuItems.Add(new MenuItem { Code = "report", Name = "报表中心", Icon = "chart" });
            MenuItems.Add(new MenuItem { Code = "fixedasset", Name = "固定资产", Icon = "building" });
            MenuItems.Add(new MenuItem { Code = "salary", Name = "工资管理", Icon = "people" });
            MenuItems.Add(new MenuItem { Code = "customer", Name = "客户管理", Icon = "user" });
            MenuItems.Add(new MenuItem { Code = "supplier", Name = "供应商管理", Icon = "usergroup" });
            MenuItems.Add(new MenuItem { Code = "invoice", Name = "发票管理", Icon = "invoice" });
            MenuItems.Add(new MenuItem { Code = "bill", Name = "票据管理", Icon = "bill" });
            MenuItems.Add(new MenuItem { Code = "system", Name = "系统管理", Icon = "setting" });

            // 快捷方式
            QuickAccessItems.Add(new MenuItem { Code = "voucher_new", Name = "新增凭证", Icon = "plus" });
            QuickAccessItems.Add(new MenuItem { Code = "voucher_audit", Name = "凭证审核", Icon = "check" });
            QuickAccessItems.Add(new MenuItem { Code = "report_balance", Name = "资产负债表", Icon = "chart" });
            QuickAccessItems.Add(new MenuItem { Code = "report_income", Name = "利润表", Icon = "chart" });
        }

        private void Navigate(string moduleCode)
        {
            CurrentModule = MenuItems.FirstOrDefault(m => m.Code == moduleCode)?.Name ?? moduleCode;
            ModuleChanged?.Invoke(this, moduleCode);
            StatusMessage = $"当前模块：{CurrentModule}";
        }

        private void Refresh()
        {
            StatusMessage = "数据已刷新";
        }

        private void Logout()
        {
            CurrentUser = null;
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MenuItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
