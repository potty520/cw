using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string UserCode { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime? LastLoginTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public Department? Department { get; set; }
    }

    /// <summary>
    /// 操作日志
    /// </summary>
    public class OperationLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OperationType { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string OperationDesc { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public DateTime CreateTime { get; set; }

        public User? User { get; set; }
    }

    /// <summary>
    /// 账套
    /// </summary>
    public class AccountSet
    {
        public int Id { get; set; }
        public string SetCode { get; set; } = string.Empty;
        public string SetName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
        public string? LegalPerson { get; set; }
        public string? ChiefAccountant { get; set; }
        public string? Accountant { get; set; }
        public string? Cashier { get; set; }
        public DateTime StartDate { get; set; }
        public string CurrencyCode { get; set; } = "RMB";
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public enum UserRole
    {
        系统管理员,
        账套管理员,
        财务主管,
        会计,
        出纳,
        审计
    }
}
