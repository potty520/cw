using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FinanceApp.Views.Pages
{
    public partial class LedgerPage : UserControl
    {
        private ObservableCollection<LedgerItem> _ledgerItems = new();

        public LedgerPage()
        {
            InitializeComponent();
            Loaded += LedgerPage_Loaded;
            dgLedger.ItemsSource = _ledgerItems;
        }

        private void LedgerPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLedgerData();
        }

        private void LoadLedgerData()
        {
            _ledgerItems.Clear();
            _ledgerItems.Add(new LedgerItem { SubjectCode = "1001", SubjectName = "库存现金", InitialBalance = 10000, DebitAmount = 5000, CreditAmount = 3000 });
            _ledgerItems.Add(new LedgerItem { SubjectCode = "1002", SubjectName = "银行存款", InitialBalance = 500000, DebitAmount = 200000, CreditAmount = 150000 });
            _ledgerItems.Add(new LedgerItem { SubjectCode = "1122", SubjectName = "应收账款", InitialBalance = 200000, DebitAmount = 50000, CreditAmount = 30000 });
            _ledgerItems.Add(new LedgerItem { SubjectCode = "1601", SubjectName = "固定资产", InitialBalance = 1000000, DebitAmount = 0, CreditAmount = 0 });
            _ledgerItems.Add(new LedgerItem { SubjectCode = "2202", SubjectName = "应付账款", InitialBalance = 150000, DebitAmount = 50000, CreditAmount = 80000 });

            UpdateTotals();
        }

        private void Query_Click(object sender, RoutedEventArgs e)
        {
            var year = int.Parse((cboYear.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "2026");
            var month = int.Parse((cboMonth.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "6");
            txtPeriod.Text = $"{year}年第{month}期";
            LoadLedgerData();
        }

        private void UpdateTotals()
        {
            decimal totalInitial = 0, totalDebit = 0, totalCredit = 0;
            foreach (var item in _ledgerItems)
            {
                totalInitial += item.InitialBalance;
                totalDebit += item.DebitAmount;
                totalCredit += item.CreditAmount;
            }
            txtTotalInitial.Text = totalInitial.ToString("N2");
            txtTotalDebit.Text = totalDebit.ToString("N2");
            txtTotalCredit.Text = totalCredit.ToString("N2");
        }
    }

    public class LedgerItem
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal EndingBalance => InitialBalance + DebitAmount - CreditAmount;
    }
}
