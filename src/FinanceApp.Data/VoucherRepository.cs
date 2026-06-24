using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Data
{
    public class VoucherRepository : RepositoryBase<Voucher>
    {
        public VoucherRepository(DbContext context) : base(context) { }

        protected override string TableName => "Vouchers";

        protected override Voucher MapToEntity(MySqlDataReader reader)
        {
            return new Voucher
            {
                Id = reader.GetInt32("Id"),
                VoucherNumber = reader.GetString("VoucherNumber"),
                VoucherWordId = reader.GetInt32("VoucherWordId"),
                VoucherDate = reader.GetDateTime("VoucherDate"),
                AccountingPeriod = reader.GetInt32("AccountingPeriod"),
                AttachmentCount = reader.GetInt32("AttachmentCount"),
                AuditorId = reader.IsDBNull(reader.GetOrdinal("AuditorId")) ? null : reader.GetInt32("AuditorId"),
                PosterId = reader.IsDBNull(reader.GetOrdinal("PosterId")) ? null : reader.GetInt32("PosterId"),
                VoucherStatus = Enum.Parse<VoucherStatus>(reader.GetString("VoucherStatus")),
                Memo = reader.IsDBNull(reader.GetOrdinal("Memo")) ? null : reader.GetString("Memo"),
                CreatorId = reader.GetInt32("CreatorId"),
                CreateTime = reader.GetDateTime("CreateTime"),
                UpdateTime = reader.GetDateTime("UpdateTime")
            };
        }

        protected override void AddParameters(MySqlCommand command, object parameters)
        {
            var props = parameters.GetType().GetProperties();
            foreach (var prop in props)
            {
                command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
            }
        }

        public async Task<string> GenerateVoucherNumberAsync(int voucherWordId, DateTime voucherDate)
        {
            using var connection = Context.CreateConnection();
            var sql = @"SELECT COUNT(*) + 1 FROM Vouchers
                        WHERE VoucherWordId = @VoucherWordId
                        AND YEAR(VoucherDate) = @Year
                        AND MONTH(VoucherDate) = @Month";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VoucherWordId", voucherWordId);
            command.Parameters.AddWithValue("@Year", voucherDate.Year);
            command.Parameters.AddWithValue("@Month", voucherDate.Month);
            await connection.OpenAsync();
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return $"{DateTime.Now.Year}{DateTime.Now.Month:D2}{voucherWordId:D2}{count:D4}";
        }

        public async Task<IEnumerable<Voucher>> GetByPeriodAsync(int periodYear, int periodMonth)
        {
            using var connection = Context.CreateConnection();
            var sql = @"SELECT * FROM Vouchers
                        WHERE YEAR(VoucherDate) = @Year
                        AND MONTH(VoucherDate) = @Month
                        ORDER BY VoucherDate, Id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Year", periodYear);
            command.Parameters.AddWithValue("@Month", periodMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<Voucher>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }
    }

    public class VoucherEntryRepository : RepositoryBase<VoucherEntry>
    {
        public VoucherEntryRepository(DbContext context) : base(context) { }

        protected override string TableName => "VoucherEntries";

        protected override VoucherEntry MapToEntity(MySqlDataReader reader)
        {
            return new VoucherEntry
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
                ForeignCurrencyAmount = reader.IsDBNull(reader.GetOrdinal("ForeignCurrencyAmount")) ? null : reader.GetDecimal("ForeignCurrencyAmount"),
                CurrencyCode = reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString("CurrencyCode"),
                SettlementDate = reader.IsDBNull(reader.GetOrdinal("SettlementDate")) ? null : reader.GetDateTime("SettlementDate"),
                BillNumber = reader.IsDBNull(reader.GetOrdinal("BillNumber")) ? null : reader.GetString("BillNumber"),
                CreateTime = reader.GetDateTime("CreateTime")
            };
        }

        protected override void AddParameters(MySqlCommand command, object parameters)
        {
            var props = parameters.GetType().GetProperties();
            foreach (var prop in props)
            {
                command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters) ?? DBNull.Value);
            }
        }

        public async Task<IEnumerable<VoucherEntry>> GetByVoucherIdAsync(int voucherId)
        {
            using var connection = Context.CreateConnection();
            var sql = "SELECT * FROM VoucherEntries WHERE VoucherId = @VoucherId";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VoucherId", voucherId);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<VoucherEntry>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }
    }
}
