using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 会计科目
    /// </summary>
    public class AccountSubject
    {
        public int Id { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public SubjectType SubjectType { get; set; }
        public int? ParentId { get; set; }
        public bool IsDetail { get; set; } = true;
        public BalanceDirection BalanceDirection { get; set; } = BalanceDirection.借;
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public AccountSubject? Parent { get; set; }
    }

    public enum SubjectType
    {
        资产,
        负债,
        权益,
        成本,
        损益
    }

    public enum BalanceDirection
    {
        借,
        贷
    }
}
