using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Data
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(DbContext context) : base(context) { }

        protected override string TableName => "Users";

        protected override User MapToEntity(MySqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32("Id"),
                UserCode = reader.GetString("UserCode"),
                UserName = reader.GetString("UserName"),
                Password = reader.GetString("Password"),
                UserRole = Enum.Parse<UserRole>(reader.GetString("UserRole")),
                DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32("DepartmentId"),
                IsEnabled = reader.GetBoolean("IsEnabled"),
                LastLoginTime = reader.IsDBNull(reader.GetOrdinal("LastLoginTime")) ? null : reader.GetDateTime("LastLoginTime"),
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

        public async Task<User?> ValidateLoginAsync(string userCode, string password)
        {
            System.Diagnostics.Debug.WriteLine("ValidateLoginAsync 开始");
            using var connection = Context.CreateConnection();
            System.Diagnostics.Debug.WriteLine("创建连接完成");
            var sql = "SELECT * FROM Users WHERE UserCode = @UserCode AND Password = @Password";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserCode", userCode);
            command.Parameters.AddWithValue("@Password", password);
            System.Diagnostics.Debug.WriteLine("准备打开连接");
            await connection.OpenAsync();
            System.Diagnostics.Debug.WriteLine("连接已打开");
            using var reader = await command.ExecuteReaderAsync();
            System.Diagnostics.Debug.WriteLine("执行查询完成");
            if (await reader.ReadAsync())
            {
                System.Diagnostics.Debug.WriteLine("读取到数据");
                return MapToEntity(reader);
            }
            System.Diagnostics.Debug.WriteLine("没有读取到数据");
            return null;
        }
    }

    public class AccountSetRepository : RepositoryBase<AccountSet>
    {
        public AccountSetRepository(DbContext context) : base(context) { }

        protected override string TableName => "AccountSets";

        protected override AccountSet MapToEntity(MySqlDataReader reader)
        {
            return new AccountSet
            {
                Id = reader.GetInt32("Id"),
                SetCode = reader.GetString("SetCode"),
                SetName = reader.GetString("SetName"),
                CompanyName = reader.GetString("CompanyName"),
                TaxNumber = reader.IsDBNull(reader.GetOrdinal("TaxNumber")) ? null : reader.GetString("TaxNumber"),
                LegalPerson = reader.IsDBNull(reader.GetOrdinal("LegalPerson")) ? null : reader.GetString("LegalPerson"),
                ChiefAccountant = reader.IsDBNull(reader.GetOrdinal("ChiefAccountant")) ? null : reader.GetString("ChiefAccountant"),
                Accountant = reader.IsDBNull(reader.GetOrdinal("Accountant")) ? null : reader.GetString("Accountant"),
                Cashier = reader.IsDBNull(reader.GetOrdinal("Cashier")) ? null : reader.GetString("Cashier"),
                StartDate = reader.GetDateTime("StartDate"),
                CurrencyCode = reader.GetString("CurrencyCode"),
                IsEnabled = reader.GetBoolean("IsEnabled"),
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
    }

    public class AccountingPeriodRepository : RepositoryBase<AccountingPeriod>
    {
        public AccountingPeriodRepository(DbContext context) : base(context) { }

        protected override string TableName => "AccountingPeriods";

        protected override AccountingPeriod MapToEntity(MySqlDataReader reader)
        {
            return new AccountingPeriod
            {
                Id = reader.GetInt32("Id"),
                PeriodYear = reader.GetInt32("PeriodYear"),
                PeriodMonth = reader.GetInt32("PeriodMonth"),
                StartDate = reader.GetDateTime("StartDate"),
                EndDate = reader.GetDateTime("EndDate"),
                IsClosed = reader.GetBoolean("IsClosed"),
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

        public async Task<AccountingPeriod?> GetCurrentPeriodAsync()
        {
            using var connection = Context.CreateConnection();
            var sql = @"SELECT * FROM AccountingPeriods
                        WHERE PeriodYear = @Year AND PeriodMonth = @Month";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Year", DateTime.Now.Year);
            command.Parameters.AddWithValue("@Month", DateTime.Now.Month);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToEntity(reader);
            }
            return null;
        }
    }
}
