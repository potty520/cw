using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace FinanceApp.Data
{
    /// <summary>
    /// 数据库连接上下文
    /// </summary>
    public class DbContext
    {
        private readonly string _connectionString;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<MySqlConnection> CreateConnectionAsync()
        {
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }

    /// <summary>
    /// 泛型仓储基类
    /// </summary>
    public abstract class RepositoryBase<T> where T : class
    {
        public readonly DbContext Context;
        protected abstract string TableName { get; }

        public RepositoryBase(DbContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = Context.CreateConnection();
            using var command = new MySqlCommand($"SELECT * FROM {TableName}", connection);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<T>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            using var connection = Context.CreateConnection();
            using var command = new MySqlCommand($"SELECT * FROM {TableName} WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToEntity(reader);
            }
            return null;
        }

        public async Task<int> InsertAsync(string sql, object parameters)
        {
            using var connection = Context.CreateConnection();
            using var command = new MySqlCommand(sql, connection);
            AddParameters(command, parameters);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateAsync(string sql, object parameters)
        {
            using var connection = Context.CreateConnection();
            using var command = new MySqlCommand(sql, connection);
            AddParameters(command, parameters);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            using var connection = Context.CreateConnection();
            using var command = new MySqlCommand($"DELETE FROM {TableName} WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        protected abstract T MapToEntity(MySqlDataReader reader);
        protected abstract void AddParameters(MySqlCommand command, object parameters);
    }
}
