using System;
using System.Collections.Generic;

namespace FinanceApp.Models
{
    public class BalanceSheetItem
    {
        public string SubjectCode { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public decimal Balance { get; set; }
    }

    public class BalanceSheet
    {
        public DateTime ReportDate { get; set; }
        public string PeriodName { get; set; } = "";
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public decimal TotalLiabilitiesAndEquity { get; set; }
        public List<BalanceSheetItem> Assets { get; set; } = new();
        public List<BalanceSheetItem> Liabilities { get; set; } = new();
        public List<BalanceSheetItem> Equity { get; set; } = new();
    }

    public class IncomeStatementItem
    {
        public string SubjectCode { get; set; } = "";
        public string SubjectName { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class IncomeStatement
    {
        public DateTime ReportDate { get; set; }
        public string PeriodName { get; set; } = "";
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public List<IncomeStatementItem> Revenues { get; set; } = new();
        public List<IncomeStatementItem> Expenses { get; set; } = new();
    }
}
