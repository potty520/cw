using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Data;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Services
{
    /// <summary>
    /// 凭证服务
    /// </summary>
    public class VoucherService
    {
        private readonly VoucherRepository _voucherRepository;
        private readonly VoucherEntryRepository _entryRepository;
        private readonly AccountSubjectRepository _accountRepository;

        public VoucherService(DbContext context)
        {
            _voucherRepository = new VoucherRepository(context);
            _entryRepository = new VoucherEntryRepository(context);
            _accountRepository = new AccountSubjectRepository(context);
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher, List<VoucherEntry> entries)
        {
            // 验证借贷平衡
            decimal totalDebit = 0;
            decimal totalCredit = 0;
            foreach (var entry in entries)
            {
                totalDebit += entry.DebitAmount;
                totalCredit += entry.CreditAmount;
            }

            if (Math.Abs(totalDebit - totalCredit) > 0.01m)
            {
                throw new Exception("借贷不平衡，借方金额合计：" + totalDebit + "，贷方金额合计：" + totalCredit);
            }

            // 生成凭证号
            voucher.VoucherNumber = await _voucherRepository.GenerateVoucherNumberAsync(
                voucher.VoucherWordId, voucher.VoucherDate);

            // 保存凭证
            var sql = @"INSERT INTO Vouchers (VoucherNumber, VoucherWordId, VoucherDate, AccountingPeriod,
                        AttachmentCount, VoucherStatus, Memo, CreatorId)
                        VALUES (@VoucherNumber, @VoucherWordId, @VoucherDate, @AccountingPeriod,
                        @AttachmentCount, @VoucherStatus, @Memo, @CreatorId);
                        SELECT LAST_INSERT_ID();";

            using var connection = _voucherRepository.Context.CreateConnection();
            await connection.OpenAsync();
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@VoucherNumber", voucher.VoucherNumber);
            command.Parameters.AddWithValue("@VoucherWordId", voucher.VoucherWordId);
            command.Parameters.AddWithValue("@VoucherDate", voucher.VoucherDate);
            command.Parameters.AddWithValue("@AccountingPeriod", voucher.AccountingPeriod);
            command.Parameters.AddWithValue("@AttachmentCount", voucher.AttachmentCount);
            command.Parameters.AddWithValue("@VoucherStatus", voucher.VoucherStatus.ToString());
            command.Parameters.AddWithValue("@Memo", voucher.Memo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatorId", voucher.CreatorId);

            voucher.Id = Convert.ToInt32(await command.ExecuteScalarAsync());

            // 保存分录
            foreach (var entry in entries)
            {
                entry.VoucherId = voucher.Id;
                var entrySql = @"INSERT INTO VoucherEntries (VoucherId, AccountSubjectId, EntryDate,
                                 Summary, DebitAmount, CreditAmount, Quantity, UnitPrice,
                                 ForeignCurrencyAmount, CurrencyCode, SettlementDate, BillNumber)
                                 VALUES (@VoucherId, @AccountSubjectId, @EntryDate, @Summary,
                                 @DebitAmount, @CreditAmount, @Quantity, @UnitPrice,
                                 @ForeignCurrencyAmount, @CurrencyCode, @SettlementDate, @BillNumber)";

                using var entryCommand = new MySqlCommand(entrySql, connection);
                entryCommand.Parameters.AddWithValue("@VoucherId", entry.VoucherId);
                entryCommand.Parameters.AddWithValue("@AccountSubjectId", entry.AccountSubjectId);
                entryCommand.Parameters.AddWithValue("@EntryDate", entry.EntryDate);
                entryCommand.Parameters.AddWithValue("@Summary", entry.Summary ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@DebitAmount", entry.DebitAmount);
                entryCommand.Parameters.AddWithValue("@CreditAmount", entry.CreditAmount);
                entryCommand.Parameters.AddWithValue("@Quantity", entry.Quantity ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@UnitPrice", entry.UnitPrice ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@ForeignCurrencyAmount", entry.ForeignCurrencyAmount ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@CurrencyCode", entry.CurrencyCode ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@SettlementDate", entry.SettlementDate ?? (object)DBNull.Value);
                entryCommand.Parameters.AddWithValue("@BillNumber", entry.BillNumber ?? (object)DBNull.Value);

                await entryCommand.ExecuteNonQueryAsync();
            }

            voucher.Entries = entries;
            return voucher;
        }

        public async Task<Voucher> GetVoucherWithEntriesAsync(int voucherId)
        {
            var voucher = await _voucherRepository.GetByIdAsync(voucherId);
            if (voucher != null)
            {
                voucher.Entries = (List<VoucherEntry>)await _entryRepository.GetByVoucherIdAsync(voucherId);
            }
            return voucher!;
        }

        public async Task<IEnumerable<Voucher>> GetVouchersByPeriodAsync(int periodYear, int periodMonth)
        {
            return await _voucherRepository.GetByPeriodAsync(periodYear, periodMonth);
        }

        public async Task<bool> AuditVoucherAsync(int voucherId, int auditorId)
        {
            var sql = "UPDATE Vouchers SET VoucherStatus = '已审核', AuditorId = @AuditorId WHERE Id = @Id";
            using var connection = _voucherRepository.Context.CreateConnection();
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", voucherId);
            command.Parameters.AddWithValue("@AuditorId", auditorId);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> PostVoucherAsync(int voucherId, int posterId)
        {
            var sql = "UPDATE Vouchers SET VoucherStatus = '已过账', PosterId = @PosterId WHERE Id = @Id AND VoucherStatus = '已审核'";
            using var connection = _voucherRepository.Context.CreateConnection();
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", voucherId);
            command.Parameters.AddWithValue("@PosterId", posterId);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteVoucherAsync(int voucherId)
        {
            var sql = "DELETE FROM Vouchers WHERE Id = @Id AND VoucherStatus = '新建'";
            using var connection = _voucherRepository.Context.CreateConnection();
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", voucherId);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
