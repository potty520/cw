using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FinanceApp.Models;

namespace FinanceApp.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private BalanceSheet? _balanceSheet;
        private IncomeStatement? _incomeStatement;
        private int _selectedYear;
        private int _selectedMonth;
        private string _selectedReportType = "资产负债表";
        private bool _isLoading;
        private DateTime _reportDate = DateTime.Now;

        public BalanceSheet? BalanceSheet
        {
            get => _balanceSheet;
            set => SetProperty(ref _balanceSheet, value);
        }

        public IncomeStatement? IncomeStatement
        {
            get => _incomeStatement;
            set => SetProperty(ref _incomeStatement, value);
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                if (SetProperty(ref _selectedYear, value))
                {
                    _ = LoadReportAsync();
                }
            }
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    _ = LoadReportAsync();
                }
            }
        }

        public string SelectedReportType
        {
            get => _selectedReportType;
            set
            {
                if (SetProperty(ref _selectedReportType, value))
                {
                    _ = LoadReportAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public DateTime ReportDate
        {
            get => _reportDate;
            set => SetProperty(ref _reportDate, value);
        }

        public ObservableCollection<int> AvailableYears { get; } = new();
        public ObservableCollection<int> AvailableMonths { get; } = new();
        public ObservableCollection<string> ReportTypes { get; } = new()
        {
            "资产负债表",
            "利润表"
        };

        public ICommand RefreshCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ExportCommand { get; }

        public ReportViewModel()
        {
            _selectedYear = DateTime.Now.Year;
            _selectedMonth = DateTime.Now.Month;

            // 初始化年份（近5年）
            for (int year = DateTime.Now.Year; year >= DateTime.Now.Year - 5; year--)
            {
                AvailableYears.Add(year);
            }

            // 初始化月份
            for (int month = 1; month <= 12; month++)
            {
                AvailableMonths.Add(month);
            }

            RefreshCommand = new Command(async () => await LoadReportAsync());
            PrintCommand = new Command(PrintReport);
            ExportCommand = new Command(ExportReport);
        }

        public async Task LoadReportAsync()
        {
            IsLoading = true;
            try
            {
                await Task.Delay(500); // 模拟加载

                if (SelectedReportType == "资产负债表")
                {
                    BalanceSheet = new BalanceSheet
                    {
                        ReportDate = new DateTime(SelectedYear, SelectedMonth, 1),
                        PeriodName = $"{SelectedYear}年{SelectedMonth}月",
                        TotalAssets = 1000000,
                        TotalLiabilities = 400000,
                        TotalEquity = 600000,
                        TotalLiabilitiesAndEquity = 1000000
                    };

                    // 示例数据
                    BalanceSheet.Assets.Add(new BalanceSheetItem { SubjectCode = "1001", SubjectName = "库存现金", Balance = 10000 });
                    BalanceSheet.Assets.Add(new BalanceSheetItem { SubjectCode = "1002", SubjectName = "银行存款", Balance = 500000 });
                    BalanceSheet.Assets.Add(new BalanceSheetItem { SubjectCode = "1122", SubjectName = "应收账款", Balance = 200000 });
                    BalanceSheet.Assets.Add(new BalanceSheetItem { SubjectCode = "1601", SubjectName = "固定资产", Balance = 290000 });

                    BalanceSheet.Liabilities.Add(new BalanceSheetItem { SubjectCode = "2202", SubjectName = "应付账款", Balance = 150000 });
                    BalanceSheet.Liabilities.Add(new BalanceSheetItem { SubjectCode = "2001", SubjectName = "短期借款", Balance = 250000 });

                    BalanceSheet.Equity.Add(new BalanceSheetItem { SubjectCode = "4001", SubjectName = "实收资本", Balance = 500000 });
                    BalanceSheet.Equity.Add(new BalanceSheetItem { SubjectCode = "4103", SubjectName = "本年利润", Balance = 100000 });
                }
                else
                {
                    IncomeStatement = new IncomeStatement
                    {
                        ReportDate = new DateTime(SelectedYear, SelectedMonth, 1),
                        PeriodName = $"{SelectedYear}年{SelectedMonth}月",
                        TotalRevenue = 500000,
                        TotalExpenses = 350000,
                        NetProfit = 150000
                    };

                    IncomeStatement.Revenues.Add(new IncomeStatementItem { SubjectCode = "6001", SubjectName = "主营业务收入", Amount = 450000 });
                    IncomeStatement.Revenues.Add(new IncomeStatementItem { SubjectCode = "6051", SubjectName = "其他业务收入", Amount = 50000 });

                    IncomeStatement.Expenses.Add(new IncomeStatementItem { SubjectCode = "6401", SubjectName = "主营业务成本", Amount = 250000 });
                    IncomeStatement.Expenses.Add(new IncomeStatementItem { SubjectCode = "6602", SubjectName = "销售费用", Amount = 30000 });
                    IncomeStatement.Expenses.Add(new IncomeStatementItem { SubjectCode = "6603", SubjectName = "管理费用", Amount = 70000 });
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void PrintReport()
        {
            // 打印报表逻辑
        }

        private void ExportReport()
        {
            // 导出报表逻辑
        }
    }
}
