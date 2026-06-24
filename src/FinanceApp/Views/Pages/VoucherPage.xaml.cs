using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using FinanceApp.Models;

namespace FinanceApp.Views.Pages
{
    public partial class VoucherPage : UserControl
    {
        private ObservableCollection<Voucher> _vouchers = new();
        private ObservableCollection<VoucherEntry> _entries = new();

        public VoucherPage()
        {
            InitializeComponent();
            Loaded += VoucherPage_Loaded;
            dgEntries.ItemsSource = _entries;
            lstVouchers.ItemsSource = _vouchers;
        }

        private void VoucherPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVouchers();
        }

        private void LoadVouchers()
        {
            _vouchers.Clear();
            // 模拟数据
            for (int i = 1; i <= 10; i++)
            {
                _vouchers.Add(new Voucher
                {
                    Id = i,
                    VoucherNumber = $"记-2026-06-{i:D4}",
                    VoucherDate = DateTime.Now.AddDays(-i),
                    VoucherStatus = i % 3 == 0 ? VoucherStatus.已审核 : (i % 2 == 0 ? VoucherStatus.已过账 : VoucherStatus.新建),
                    CreatorId = 1
                });
            }
        }

        private void NewVoucher_Click(object sender, RoutedEventArgs e)
        {
            _entries.Clear();
            for (int i = 0; i < 2; i++)
            {
                _entries.Add(new VoucherEntry
                {
                    Summary = "",
                    DebitAmount = 0,
                    CreditAmount = 0
                });
            }
            txtVoucherNumber.Text = "自动生成";
            dtpVoucherDate.SelectedDate = DateTime.Now;
        }

        private void SaveVoucher_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("凭证保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AuditVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (lstVouchers.SelectedItem is Voucher voucher)
            {
                voucher.VoucherStatus = VoucherStatus.已审核;
                lstVouchers.Items.Refresh();
                MessageBox.Show("凭证审核成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void PostVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (lstVouchers.SelectedItem is Voucher voucher)
            {
                if (voucher.VoucherStatus != VoucherStatus.已审核)
                {
                    MessageBox.Show("凭证必须先审核才能过账！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                voucher.VoucherStatus = VoucherStatus.已过账;
                lstVouchers.Items.Refresh();
                MessageBox.Show("凭证过账成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (lstVouchers.SelectedItem is Voucher voucher)
            {
                var result = MessageBox.Show("确定要删除该凭证吗？", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _vouchers.Remove(voucher);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadVouchers();
        }

        private void LstVouchers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstVouchers.SelectedItem is Voucher voucher)
            {
                txtVoucherNumber.Text = voucher.VoucherNumber;
                dtpVoucherDate.SelectedDate = voucher.VoucherDate;
            }
        }

        private void PrintVoucher_Click(object sender, RoutedEventArgs e)
        {
            if (lstVouchers.SelectedItem is Voucher voucher)
            {
                var printWindow = new Views.VoucherPrintWindow();
                printWindow.LoadVoucher(voucher);
                printWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("请先选择要打印的凭证！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
