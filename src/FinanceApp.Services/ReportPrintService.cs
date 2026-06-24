using System;
using System.Collections.Generic;
using FinanceApp.Models;

namespace FinanceApp.Services
{
    /// <summary>
    /// 打印设置数据
    /// </summary>
    public class PrintSettingsData
    {
        public double PaperWidth { get; set; } = 210;  // mm
        public double PaperHeight { get; set; } = 297; // mm
        public bool IsLandscape { get; set; } = false;
        public double MarginLeft { get; set; } = 20;    // mm
        public double MarginRight { get; set; } = 20;
        public double MarginTop { get; set; } = 20;
        public double MarginBottom { get; set; } = 20;
        public bool EnableTaoDa { get; set; } = false;  // 套打模式
        public double TaoDaOffsetX { get; set; } = 0;    // mm
        public double TaoDaOffsetY { get; set; } = 0;
        public string SelectedPrinter { get; set; } = "";
    }

    /// <summary>
    /// 凭证打印数据（用于传递给打印服务）
    /// </summary>
    public class VoucherPrintData
    {
        public string CompanyName { get; set; } = "财智星财务软件";
        public string VoucherWord { get; set; } = "记";
        public string VoucherNumber { get; set; } = "";
        public string VoucherDate { get; set; } = "";
        public string AttachmentCount { get; set; } = "0";
        public string TotalDebit { get; set; } = "0.00";
        public string TotalCredit { get; set; } = "0.00";
        public string Creator { get; set; } = "";
        public List<VoucherEntryPrintData> Entries { get; set; } = new();
    }

    public class VoucherEntryPrintData
    {
        public string Summary { get; set; } = "";
        public string AccountCode { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string DebitAmount { get; set; } = "";
        public string CreditAmount { get; set; } = "";
    }

    /// <summary>
    /// 凭证数据转换服务
    /// </summary>
    public class ReportPrintService
    {
        private PrintSettingsData _printSettings = new();

        public void SetPrintSettings(PrintSettingsData settings)
        {
            _printSettings = settings;
        }

        public PrintSettingsData GetPrintSettings()
        {
            return _printSettings;
        }

        /// <summary>
        /// 将Voucher模型转换为打印数据
        /// </summary>
        public VoucherPrintData ConvertVoucherToPrintData(Voucher voucher)
        {
            var data = new VoucherPrintData
            {
                CompanyName = "财智星财务软件",
                VoucherWord = voucher.VoucherWord?.WordName ?? "记",
                VoucherNumber = voucher.VoucherNumber,
                VoucherDate = voucher.VoucherDate.ToString("yyyy-MM-dd"),
                AttachmentCount = voucher.AttachmentCount.ToString(),
                TotalDebit = CalculateTotal(voucher.Entries, true).ToString("N2"),
                TotalCredit = CalculateTotal(voucher.Entries, false).ToString("N2"),
                Creator = $"User{voucher.CreatorId}"
            };

            foreach (var entry in voucher.Entries)
            {
                data.Entries.Add(new VoucherEntryPrintData
                {
                    Summary = entry.Summary ?? "",
                    AccountCode = entry.AccountSubject?.SubjectCode ?? "",
                    AccountName = entry.AccountSubject?.SubjectName ?? "",
                    DebitAmount = entry.DebitAmount > 0 ? entry.DebitAmount.ToString("N2") : "",
                    CreditAmount = entry.CreditAmount > 0 ? entry.CreditAmount.ToString("N2") : ""
                });
            }

            return data;
        }

        private decimal CalculateTotal(IEnumerable<VoucherEntry> entries, bool isDebit)
        {
            decimal total = 0;
            foreach (var entry in entries)
            {
                total += isDebit ? entry.DebitAmount : entry.CreditAmount;
            }
            return total;
        }
    }
}
