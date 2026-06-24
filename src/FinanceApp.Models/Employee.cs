using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 部门
    /// </summary>
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int? ManagerId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }

        public Department? Parent { get; set; }
    }

    /// <summary>
    /// 员工
    /// </summary>
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public string? Position { get; set; }
        public string? IdCard { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Department? Department { get; set; }
    }

    /// <summary>
    /// 工资项目
    /// </summary>
    public class SalaryItem
    {
        public int Id { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public SalaryItemType ItemType { get; set; }
        public bool IsTaxable { get; set; } = true;
        public bool IsDefault { get; set; }
        public string? CalculationFormula { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 工资发放
    /// </summary>
    public class SalaryPayment
    {
        public int Id { get; set; }
        public string PaymentMonth { get; set; } = string.Empty;
        public int? DepartmentId { get; set; }
        public int EmployeeId { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public decimal TaxAmount { get; set; }
        public int? VoucherId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.未审核;
        public string? Memo { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Employee? Employee { get; set; }
    }

    /// <summary>
    /// 工资项目明细
    /// </summary>
    public class SalaryPaymentDetail
    {
        public int Id { get; set; }
        public int SalaryPaymentId { get; set; }
        public int SalaryItemId { get; set; }
        public decimal Amount { get; set; }

        public SalaryItem? SalaryItem { get; set; }
    }

    public enum SalaryItemType
    {
        应发,
        代扣,
        实发
    }

    public enum PaymentStatus
    {
        未审核,
        已审核,
        已发放
    }
}
