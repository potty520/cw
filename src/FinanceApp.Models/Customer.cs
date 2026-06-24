using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 客户档案
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CreditLimit { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 供应商档案
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public decimal InitialAmount { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
