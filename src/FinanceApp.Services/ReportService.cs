using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Data;
using FinanceApp.Models;

namespace FinanceApp.Services
{
    /// <summary>
    /// 报表服务
    /// </summary>
    public class ReportService
    {
        private readonly DbContext _context;

        public ReportService(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 生成资产负债表
        /// </summary>
        public async Task<BalanceSheet> GenerateBalanceSheetAsync(int periodYear, int periodMonth)
        {
            var balanceSheet = new BalanceSheet
            {
                ReportDate = new DateTime(periodYear, periodMonth, 1),
                PeriodName = $"{periodYear}年{periodMonth}月"
            };

            // 获取科目余额数据
            var sql = @"SELECT
                        a.SubjectCode,
                        a.SubjectName,
                        a.SubjectType,
                        a.BalanceDirection,
                        COALESCE(SUM(e.DebitAmount), 0) - COALESCE(SUM(e.CreditAmount), 0) AS Balance
                       FROM AccountSubjects a
                       LEFT JOIN VoucherEntries e ON a.Id = e.AccountSubjectId
                          AND YEAR(e.EntryDate) = @Year AND MONTH(e.EntryDate) = @Month
                       WHERE a.IsDetail = 1 AND a.IsEnabled = 1
                       GROUP BY a.Id, a.SubjectCode, a.SubjectName, a.SubjectType, a.BalanceDirection
                       ORDER BY a.SubjectCode";

            using var connection = _context.CreateConnection();
            using var command = new MySqlConnector.MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Year", periodYear);
            command.Parameters.AddWithValue("@Month", periodMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var subjectType = Enum.Parse<SubjectType>(reader.GetString("SubjectType"));
                var balanceDirection = Enum.Parse<BalanceDirection>(reader.GetString("BalanceDirection"));
                var balance = reader.GetDecimal("Balance");

                // 根据余额方向调整
                if (balanceDirection == BalanceDirection.贷)
                {
                    balance = -balance;
                }

                var item = new BalanceSheetItem
                {
                    SubjectCode = reader.GetString("SubjectCode"),
                    SubjectName = reader.GetString("SubjectName"),
                    Balance = balance
                };

                switch (subjectType)
                {
                    case SubjectType.资产:
                        balanceSheet.Assets.Add(item);
                        balanceSheet.TotalAssets += balance;
                        break;
                    case SubjectType.负债:
                        balanceSheet.Liabilities.Add(item);
                        balanceSheet.TotalLiabilities += balance;
                        break;
                    case SubjectType.权益:
                        balanceSheet.Equity.Add(item);
                        balanceSheet.TotalEquity += balance;
                        break;
                }
            }

            balanceSheet.TotalLiabilitiesAndEquity = balanceSheet.TotalLiabilities + balanceSheet.TotalEquity;

            return balanceSheet;
        }

        /// <summary>
        /// 生成利润表
        /// </summary>
        public async Task<IncomeStatement> GenerateIncomeStatementAsync(int periodYear, int periodMonth)
        {
            var incomeStatement = new IncomeStatement
            {
                ReportDate = new DateTime(periodYear, periodMonth, 1),
                PeriodName = $"{periodYear}年{periodMonth}月"
            };

            var sql = @"SELECT
                        a.SubjectCode,
                        a.SubjectName,
                        a.SubjectType,
                        COALESCE(SUM(e.DebitAmount), 0) AS DebitTotal,
                        COALESCE(SUM(e.CreditAmount), 0) AS CreditTotal
                       FROM AccountSubjects a
                       LEFT JOIN VoucherEntries e ON a.Id = e.AccountSubjectId
                          AND YEAR(e.EntryDate) = @Year AND MONTH(e.EntryDate) = @Month
                       WHERE a.IsDetail = 1 AND a.IsEnabled = 1
                       AND a.SubjectType IN ('损益', '成本')
                       GROUP BY a.Id, a.SubjectCode, a.SubjectName, a.SubjectType
                       ORDER BY a.SubjectCode";

            using var connection = _context.CreateConnection();
            using var command = new MySqlConnector.MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Year", periodYear);
            command.Parameters.AddWithValue("@Month", periodMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var subjectType = Enum.Parse<SubjectType>(reader.GetString("SubjectType"));
                var creditTotal = reader.GetDecimal("CreditTotal");
                var debitTotal = reader.GetDecimal("DebitTotal");

                var item = new IncomeStatementItem
                {
                    SubjectCode = reader.GetString("SubjectCode"),
                    SubjectName = reader.GetString("SubjectName"),
                    Amount = subjectType == SubjectType.损益 ? creditTotal - debitTotal : debitTotal - creditTotal
                };

                if (item.SubjectName.Contains("主营收入") || item.SubjectName.Contains("其他收入"))
                {
                    incomeStatement.Revenues.Add(item);
                    incomeStatement.TotalRevenue += item.Amount;
                }
                else if (item.SubjectName.Contains("主营成本") || item.SubjectName.Contains("费用"))
                {
                    incomeStatement.Expenses.Add(item);
                    incomeStatement.TotalExpenses += item.Amount;
                }
            }

            incomeStatement.NetProfit = incomeStatement.TotalRevenue - incomeStatement.TotalExpenses;

            return incomeStatement;
        }
    }

    public class BalanceSheet
    {
        public DateTime ReportDate { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public List<BalanceSheetItem> Assets { get; set; } = new();
        public List<BalanceSheetItem> Liabilities { get; set; } = new();
        public List<BalanceSheetItem> Equity { get; set; } = new();
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public decimal TotalLiabilitiesAndEquity { get; set; }
    }

    public class BalanceSheetItem
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    public class IncomeStatement
    {
        public DateTime ReportDate { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public List<IncomeStatementItem> Revenues { get; set; } = new();
        public List<IncomeStatementItem> Expenses { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
    }

    public class IncomeStatementItem
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
