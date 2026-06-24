using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 应收票据
    /// </summary>
    public class AccountsReceivableBill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public BillType BillType { get; set; }
        public string Drawer { get; set; } = string.Empty;
        public string Payee { get; set; } = string.Empty;
        public string Drawee { get; set; } = string.Empty;
        public decimal BillAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? AcceptanceBank { get; set; }
        public BillStatus BillStatus { get; set; } = BillStatus.应收;
        public int? VoucherId { get; set; }
        public string? Memo { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Customer? Customer { get; set; }
    }

    /// <summary>
    /// 应付票据
    /// </summary>
    public class AccountsPayableBill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public BillType BillType { get; set; }
        public string Drawer { get; set; } = string.Empty;
        public string Payee { get; set; } = string.Empty;
        public string Drawee { get; set; } = string.Empty;
        public decimal BillAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? AcceptanceBank { get; set; }
        public BillStatus BillStatus { get; set; } = BillStatus.应付;
        public int? VoucherId { get; set; }
        public string? Memo { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Supplier? Supplier { get; set; }
    }

    /// <summary>
    /// 发票
    /// </summary>
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public InvoiceType InvoiceType { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TaxRate { get; set; }
        public int? VoucherId { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.未核销;
        public string? Memo { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Customer? Customer { get; set; }
        public Supplier? Supplier { get; set; }
    }

    public enum BillType
    {
        银行承兑汇票,
        商业承兑汇票
    }

    public enum BillStatus
    {
        应收,
        已收,
        已背书,
        已贴现,
        已到期,
        已作废,
        应付,
        已付
    }

    public enum InvoiceType
    {
        增值税专用发票,
        增值税普通发票,
        收据,
        其他
    }

    public enum InvoiceStatus
    {
        未核销,
        部分核销,
        已核销,
        已作废
    }
}
