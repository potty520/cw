using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Data;
using FinanceApp.Models;

namespace FinanceApp.Services
{
    /// <summary>
    /// 账簿查询服务
    /// </summary>
    public class LedgerService
    {
        private readonly DbContext _context;

        public LedgerService(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取总账数据
        /// </summary>
        public async Task<IEnumerable<LedgerBalance>> GetGeneralLedgerAsync(int periodYear, int periodMonth)
        {
            var sql = @"SELECT
                        a.Id AS AccountSubjectId,
                        a.SubjectCode,
                        a.SubjectName,
                        a.SubjectType,
                        a.BalanceDirection,
                        COALESCE(b.InitialDebit, 0) AS InitialDebit,
                        COALESCE(b.InitialCredit, 0) AS InitialCredit,
                        COALESCE(c.CurrentPeriodDebit, 0) AS CurrentPeriodDebit,
                        COALESCE(c.CurrentPeriodCredit, 0) AS CurrentPeriodCredit,
                        COALESCE(b.InitialDebit, 0) + COALESCE(c.CurrentPeriodDebit, 0) AS YtdDebit,
                        COALESCE(b.InitialCredit, 0) + COALESCE(c.CurrentPeriodCredit, 0) AS YtdCredit
                       FROM AccountSubjects a
                       LEFT JOIN (
                           SELECT AccountSubjectId,
                                  SUM(DebitAmount) AS InitialDebit,
                                  SUM(CreditAmount) AS InitialCredit
                           FROM VoucherEntries
                           WHERE YEAR(EntryDate) < @Year OR (YEAR(EntryDate) = @Year AND MONTH(EntryDate) < @Month)
                           GROUP BY AccountSubjectId
                       ) b ON a.Id = b.AccountSubjectId
                       LEFT JOIN (
                           SELECT AccountSubjectId,
                                  SUM(DebitAmount) AS CurrentPeriodDebit,
                                  SUM(CreditAmount) AS CurrentPeriodCredit
                           FROM VoucherEntries
                           WHERE YEAR(EntryDate) = @Year AND MONTH(EntryDate) = @Month
                           GROUP BY AccountSubjectId
                       ) c ON a.Id = c.AccountSubjectId
                       WHERE a.IsDetail = 1 AND a.IsEnabled = 1
                       ORDER BY a.SubjectCode";

            var results = new List<LedgerBalance>();
            using var connection = _context.CreateConnection();
            using var command = new MySqlConnector.MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Year", periodYear);
            command.Parameters.AddWithValue("@Month", periodMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new LedgerBalance
                {
                    AccountSubjectId = reader.GetInt32("AccountSubjectId"),
                    SubjectCode = reader.GetString("SubjectCode"),
                    SubjectName = reader.GetString("SubjectName"),
                    SubjectType = Enum.Parse<SubjectType>(reader.GetString("SubjectType")),
                    BalanceDirection = Enum.Parse<BalanceDirection>(reader.GetString("BalanceDirection")),
                    InitialDebit = reader.GetDecimal("InitialDebit"),
                    InitialCredit = reader.GetDecimal("InitialCredit"),
                    CurrentPeriodDebit = reader.GetDecimal("CurrentPeriodDebit"),
                    CurrentPeriodCredit = reader.GetDecimal("CurrentPeriodCredit"),
                    YtdDebit = reader.GetDecimal("YtdDebit"),
                    YtdCredit = reader.GetDecimal("YtdCredit")
                });
            }
            return results;
        }

        /// <summary>
        /// 获取明细账
        /// </summary>
        public async Task<IEnumerable<VoucherEntry>> GetAccountDetailLedgerAsync(int accountSubjectId, int periodYear, int periodMonth)
        {
            var sql = @"SELECT * FROM VoucherEntries
                        WHERE AccountSubjectId = @AccountSubjectId
                        AND YEAR(EntryDate) = @Year AND MONTH(EntryDate) = @Month
                        ORDER BY EntryDate, Id";

            var results = new List<VoucherEntry>();
            using var connection = _context.CreateConnection();
            using var command = new MySqlConnector.MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@AccountSubjectId", accountSubjectId);
            command.Parameters.AddWithValue("@Year", periodYear);
            command.Parameters.AddWithValue("@Month", periodMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new VoucherEntry
                {
                    Id = reader.GetInt32("Id"),
                    VoucherId = reader.GetInt32("VoucherId"),
                    AccountSubjectId = reader.GetInt32("AccountSubjectId"),
                    EntryDate = reader.GetDateTime("EntryDate"),
                    Summary = reader.IsDBNull(reader.GetOrdinal("Summary")) ? null : reader.GetString("Summary"),
                    DebitAmount = reader.GetDecimal("DebitAmount"),
                    CreditAmount = reader.GetDecimal("CreditAmount"),
                    Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? null : reader.GetDecimal("Quantity"),
                    UnitPrice = reader.IsDBNull(reader.GetOrdinal("UnitPrice")) ? null : reader.GetDecimal("UnitPrice"),
                    CreateTime = reader.GetDateTime("CreateTime")
                });
            }
            return results;
        }
    }

    public class LedgerBalance
    {
        public int AccountSubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public SubjectType SubjectType { get; set; }
        public BalanceDirection BalanceDirection { get; set; }
        public decimal InitialDebit { get; set; }
        public decimal InitialCredit { get; set; }
        public decimal CurrentPeriodDebit { get; set; }
        public decimal CurrentPeriodCredit { get; set; }
        public decimal YtdDebit { get; set; }
        public decimal YtdCredit { get; set; }
        public decimal EndingBalance => BalanceDirection == BalanceDirection.借
            ? YtdDebit - YtdCredit
            : YtdCredit - YtdDebit;
    }
}
