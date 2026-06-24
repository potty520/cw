using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinanceApp.Models;

namespace FinanceApp.Views.Pages
{
    public partial class VoucherPrintPreview : UserControl
    {
        public ObservableCollection<VoucherEntryPrintItem> Entries { get; } = new();

        public VoucherPrintPreview()
        {
            InitializeComponent();
            EntriesControl.ItemsSource = Entries;
        }

        public void LoadVoucherData(Voucher voucher)
        {
            txtVoucherNumber.Text = voucher.VoucherNumber;
            txtVoucherDate.Text = voucher.VoucherDate.ToString("yyyy-MM-dd");
            txtVoucherWord.Text = voucher.VoucherWord?.WordName ?? "记";
            txtAttachmentCount.Text = voucher.AttachmentCount.ToString();
            txtCreator.Text = $"User{voucher.CreatorId}";

            Entries.Clear();
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (var entry in voucher.Entries)
            {
                Entries.Add(new VoucherEntryPrintItem
                {
                    Summary = entry.Summary ?? "",
                    AccountSubjectCode = entry.AccountSubject?.SubjectCode ?? "",
                    AccountSubjectName = entry.AccountSubject?.SubjectName ?? "",
                    DebitAmount = entry.DebitAmount,
                    CreditAmount = entry.CreditAmount
                });
                totalDebit += entry.DebitAmount;
                totalCredit += entry.CreditAmount;
            }

            txtTotalDebit.Text = totalDebit.ToString("N2");
            txtTotalCredit.Text = totalCredit.ToString("N2");
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new System.Windows.Controls.PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(VoucherPrintContent, "凭证打印");
                    txtStatus.Text = "打印完成";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打印失败：" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }
    }

    public class VoucherEntryPrintItem
    {
        public string Summary { get; set; } = string.Empty;
        public string AccountSubjectCode { get; set; } = string.Empty;
        public string AccountSubjectName { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
