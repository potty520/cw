using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinanceApp.Views.Pages
{
    public partial class SalaryPage : UserControl
    {
        private ObservableCollection<SalaryItem> _salaryItems = new();

        public SalaryPage()
        {
            InitializeComponent();
            Loaded += SalaryPage_Loaded;
            dgSalary.ItemsSource = _salaryItems;
        }

        private void SalaryPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadSalaryData();
        }

        private void LoadSalaryData()
        {
            _salaryItems.Clear();
            _salaryItems.Add(new SalaryItem { EmployeeCode = "E001", EmployeeName = "张三", Department = "财务部", BaseSalary = 5000, PositionSalary = 2000, Bonus = 1000, SocialDeduction = 800, HousingDeduction = 500, Tax = 300, NetSalary = 6400, Status = "已发放" });
            _salaryItems.Add(new SalaryItem { EmployeeCode = "E002", EmployeeName = "李四", Department = "财务部", BaseSalary = 4500, PositionSalary = 1500, Bonus = 800, SocialDeduction = 700, HousingDeduction = 400, Tax = 200, NetSalary = 5500, Status = "已发放" });
            _salaryItems.Add(new SalaryItem { EmployeeCode = "E003", EmployeeName = "王五", Department = "销售部", BaseSalary = 4000, PositionSalary = 1500, Bonus = 2000, SocialDeduction = 600, HousingDeduction = 400, Tax = 400, NetSalary = 6100, Status = "已审核" });
            _salaryItems.Add(new SalaryItem { EmployeeCode = "E004", EmployeeName = "赵六", Department = "销售部", BaseSalary = 4000, PositionSalary = 1500, Bonus = 1500, SocialDeduction = 600, HousingDeduction = 400, Tax = 300, NetSalary = 5700, Status = "未审核" });
            _salaryItems.Add(new SalaryItem { EmployeeCode = "E005", EmployeeName = "钱七", Department = "生产部", BaseSalary = 3500, PositionSalary = 1000, Bonus = 500, SocialDeduction = 500, HousingDeduction = 300, Tax = 150, NetSalary = 4050, Status = "已发放" });
        }
    }

    public class SalaryItem
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public decimal PositionSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal SocialDeduction { get; set; }
        public decimal HousingDeduction { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
