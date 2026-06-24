using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Data
{
    public class SalaryPaymentRepository : RepositoryBase<SalaryPayment>
    {
        public SalaryPaymentRepository(DbContext context) : base(context) { }

        protected override string TableName => "SalaryPayments";

        protected override SalaryPayment MapToEntity(MySqlDataReader reader)
        {
            return new SalaryPayment
            {
                Id = reader.GetInt32("Id"),
                PaymentMonth = reader.GetString("PaymentMonth"),
                DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32("DepartmentId"),
                EmployeeId = reader.GetInt32("EmployeeId"),
                GrossSalary = reader.GetDecimal("GrossSalary"),
                TotalDeductions = reader.GetDecimal("TotalDeductions"),
                NetSalary = reader.GetDecimal("NetSalary"),
                TaxAmount = reader.GetDecimal("TaxAmount"),
                VoucherId = reader.IsDBNull(reader.GetOrdinal("VoucherId")) ? null : reader.GetInt32("VoucherId"),
                PaymentStatus = Enum.Parse<PaymentStatus>(reader.GetString("PaymentStatus")),
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

        public async Task<IEnumerable<SalaryPayment>> GetByMonthAsync(string paymentMonth)
        {
            using var connection = Context.CreateConnection();
            var sql = "SELECT * FROM SalaryPayments WHERE PaymentMonth = @PaymentMonth ORDER BY Id";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@PaymentMonth", paymentMonth);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<SalaryPayment>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }
    }

    public class EmployeeRepository : RepositoryBase<Employee>
    {
        public EmployeeRepository(DbContext context) : base(context) { }

        protected override string TableName => "Employees";

        protected override Employee MapToEntity(MySqlDataReader reader)
        {
            return new Employee
            {
                Id = reader.GetInt32("Id"),
                EmployeeCode = reader.GetString("EmployeeCode"),
                EmployeeName = reader.GetString("EmployeeName"),
                DepartmentId = reader.IsDBNull(reader.GetOrdinal("DepartmentId")) ? null : reader.GetInt32("DepartmentId"),
                Position = reader.IsDBNull(reader.GetOrdinal("Position")) ? null : reader.GetString("Position"),
                IdCard = reader.IsDBNull(reader.GetOrdinal("IdCard")) ? null : reader.GetString("IdCard"),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString("Phone"),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString("Email"),
                BankName = reader.IsDBNull(reader.GetOrdinal("BankName")) ? null : reader.GetString("BankName"),
                BankAccount = reader.IsDBNull(reader.GetOrdinal("BankAccount")) ? null : reader.GetString("BankAccount"),
                EntryDate = reader.IsDBNull(reader.GetOrdinal("EntryDate")) ? null : reader.GetDateTime("EntryDate"),
                LeaveDate = reader.IsDBNull(reader.GetOrdinal("LeaveDate")) ? null : reader.GetDateTime("LeaveDate"),
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

    public class DepartmentRepository : RepositoryBase<Department>
    {
        public DepartmentRepository(DbContext context) : base(context) { }

        protected override string TableName => "Departments";

        protected override Department MapToEntity(MySqlDataReader reader)
        {
            return new Department
            {
                Id = reader.GetInt32("Id"),
                DepartmentCode = reader.GetString("DepartmentCode"),
                DepartmentName = reader.GetString("DepartmentName"),
                ParentId = reader.IsDBNull(reader.GetOrdinal("ParentId")) ? null : reader.GetInt32("ParentId"),
                ManagerId = reader.IsDBNull(reader.GetOrdinal("ManagerId")) ? null : reader.GetInt32("ManagerId"),
                IsEnabled = reader.GetBoolean("IsEnabled"),
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
    }
}
