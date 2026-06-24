using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Data
{
    public class AccountSubjectRepository : RepositoryBase<AccountSubject>
    {
        public AccountSubjectRepository(DbContext context) : base(context) { }

        protected override string TableName => "AccountSubjects";

        protected override AccountSubject MapToEntity(MySqlDataReader reader)
        {
            return new AccountSubject
            {
                Id = reader.GetInt32("Id"),
                SubjectCode = reader.GetString("SubjectCode"),
                SubjectName = reader.GetString("SubjectName"),
                SubjectType = Enum.Parse<SubjectType>(reader.GetString("SubjectType")),
                ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? null : reader.GetInt32("ParentId"),
                IsDetail = reader.GetBoolean("IsDetail"),
                BalanceDirection = Enum.Parse<BalanceDirection>(reader.GetString("BalanceDirection")),
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

        public async Task<IEnumerable<AccountSubject>> GetByTypeAsync(SubjectType subjectType)
        {
            using var connection = Context.CreateConnection();
            var sql = "SELECT * FROM AccountSubjects WHERE SubjectType = @SubjectType AND IsEnabled = 1";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SubjectType", subjectType.ToString());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<AccountSubject>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }

        public async Task<IEnumerable<AccountSubject>> GetDetailSubjectsAsync()
        {
            using var connection = Context.CreateConnection();
            var sql = "SELECT * FROM AccountSubjects WHERE IsDetail = 1 AND IsEnabled = 1 ORDER BY SubjectCode";
            using var command = new MySqlCommand(sql, connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<AccountSubject>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }
    }

    public class CustomerRepository : RepositoryBase<Customer>
    {
        public CustomerRepository(DbContext context) : base(context) { }

        protected override string TableName => "Customers";

        protected override Customer MapToEntity(MySqlDataReader reader)
        {
            return new Customer
            {
                Id = reader.GetInt32("Id"),
                CustomerCode = reader.GetString("CustomerCode"),
                CustomerName = reader.GetString("CustomerName"),
                ShortName = reader.IsDBNull(reader.GetOrdinal("ShortName")) ? null : reader.GetString("ShortName"),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson")) ? null : reader.GetString("ContactPerson"),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                Mobile = reader.IsDBNull(reader.GetOrdinal("Mobile")) ? null : reader.GetString("Mobile"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                TaxNumber = reader.IsDBNull(reader.GetOrdinal("TaxNumber")) ? null : reader.GetString("TaxNumber"),
                BankName = reader.IsDBNull(reader.GetOrdinal("BankName")) ? null : reader.GetString("BankName"),
                BankAccount = reader.IsDBNull(reader.GetOrdinal("BankAccount")) ? null : reader.GetString("BankAccount"),
                InitialAmount = reader.GetDecimal("InitialAmount"),
                CreditLimit = reader.GetDecimal("CreditLimit"),
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

    public class SupplierRepository : RepositoryBase<Supplier>
    {
        public SupplierRepository(DbContext context) : base(context) { }

        protected override string TableName => "Suppliers";

        protected override Supplier MapToEntity(MySqlDataReader reader)
        {
            return new Supplier
            {
                Id = reader.GetInt32("Id"),
                SupplierCode = reader.GetString("SupplierCode"),
                SupplierName = reader.GetString("SupplierName"),
                ShortName = reader.IsDBNull(reader.GetOrdinal("ShortName")) ? null : reader.GetString("ShortName"),
                ContactPerson = reader.IsDBNull(reader.GetOrdinal("ContactPerson")) ? null : reader.GetString("ContactPerson"),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                Mobile = reader.IsDBNull(reader.GetOrdinal("Mobile")) ? null : reader.GetString("Mobile"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString("Address"),
                TaxNumber = reader.IsDBNull(reader.GetOrdinal("TaxNumber")) ? null : reader.GetString("TaxNumber"),
                BankName = reader.IsDBNull(reader.GetOrdinal("BankName")) ? null : reader.GetString("BankName"),
                BankAccount = reader.IsDBNull(reader.GetOrdinal("BankAccount")) ? null : reader.GetString("BankAccount"),
                InitialAmount = reader.GetDecimal("InitialAmount"),
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
}
