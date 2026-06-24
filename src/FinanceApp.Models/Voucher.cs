using System;
using System.Collections.Generic;

namespace FinanceApp.Models
{
    /// <summary>
    /// 凭证字
    /// </summary>
    public class VoucherWord
    {
        public int Id { get; set; }
        public string WordName { get; set; } = string.Empty;
        public string? PrintTitle { get; set; }
        public bool IsDefault { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 凭证
    /// </summary>
    public class Voucher
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; } = string.Empty;
        public int VoucherWordId { get; set; }
        public DateTime VoucherDate { get; set; }
        public int AccountingPeriod { get; set; }
        public int AttachmentCount { get; set; }
        public int? AuditorId { get; set; }
        public int? PosterId { get; set; }
        public VoucherStatus VoucherStatus { get; set; } = VoucherStatus.新建;
        public string? Memo { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public VoucherWord? VoucherWord { get; set; }
        public List<VoucherEntry> Entries { get; set; } = new();
    }

    /// <summary>
    /// 凭证分录
    /// </summary>
    public class VoucherEntry
    {
        public int Id { get; set; }
        public int VoucherId { get; set; }
        public int AccountSubjectId { get; set; }
        public DateTime EntryDate { get; set; }
        public string? Summary { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? ForeignCurrencyAmount { get; set; }
        public string? CurrencyCode { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string? BillNumber { get; set; }
        public DateTime CreateTime { get; set; }

        public AccountSubject? AccountSubject { get; set; }
    }

    public enum VoucherStatus
    {
        新建,
        已审核,
        已过账,
        已结账
    }
}
