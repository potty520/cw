using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Models;
using MySqlConnector;

namespace FinanceApp.Data
{
    public class FixedAssetRepository : RepositoryBase<FixedAsset>
    {
        public FixedAssetRepository(DbContext context) : base(context) { }

        protected override string TableName => "FixedAssets";

        protected override FixedAsset MapToEntity(MySqlDataReader reader)
        {
            return new FixedAsset
            {
                Id = reader.GetInt32("Id"),
                AssetCode = reader.GetString("AssetCode"),
                AssetName = reader.GetString("AssetName"),
                CategoryId = reader.GetInt32("CategoryId"),
                Specification = reader.IsDBNull(reader.GetOrdinal("Specification")) ? null : reader.GetString("Specification"),
                MeasurementUnit = reader.IsDBNull(reader.GetOrdinal("MeasurementUnit")) ? null : reader.GetString("MeasurementUnit"),
                Quantity = reader.GetInt32("Quantity"),
                OriginalValue = reader.GetDecimal("OriginalValue"),
                NetSalvageValue = reader.GetDecimal("NetSalvageValue"),
                UsefulLife = reader.GetInt32("UsefulLife"),
                DepreciationMethod = Enum.Parse<DepreciationMethod>(reader.GetString("DepreciationMethod")),
                PurchaseDate = reader.GetDateTime("PurchaseDate"),
                StartUseDate = reader.GetDateTime("StartUseDate"),
                AccumulatedDepreciation = reader.GetDecimal("AccumulatedDepreciation"),
                NetValue = reader.GetDecimal("NetValue"),
                DepreciationAccountId = reader.GetInt32("DepreciationAccountId"),
                FixedAssetAccountId = reader.GetInt32("FixedAssetAccountId"),
                AccumulatedDepreciationAccountId = reader.GetInt32("AccumulatedDepreciationAccountId"),
                DepreciationStatus = Enum.Parse<DepreciationStatus>(reader.GetString("DepreciationStatus")),
                Memo = reader.IsDBNull(reader.GetOrdinal("Memo")) ? null : reader.GetString("Memo"),
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

        public async Task<IEnumerable<FixedAsset>> GetByStatusAsync(DepreciationStatus status)
        {
            using var connection = Context.CreateConnection();
            var sql = "SELECT * FROM FixedAssets WHERE DepreciationStatus = @Status AND IsEnabled = 1";
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Status", status.ToString());
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<FixedAsset>();
            while (await reader.ReadAsync())
            {
                results.Add(MapToEntity(reader));
            }
            return results;
        }
    }

    public class FixedAssetCategoryRepository : RepositoryBase<FixedAssetCategory>
    {
        public FixedAssetCategoryRepository(DbContext context) : base(context) { }

        protected override string TableName => "FixedAssetCategories";

        protected override FixedAssetCategory MapToEntity(MySqlDataReader reader)
        {
            return new FixedAssetCategory
            {
                Id = reader.GetInt32("Id"),
                CategoryCode = reader.GetString("CategoryCode"),
                CategoryName = reader.GetString("CategoryName"),
                DepreciationMethod = Enum.Parse<DepreciationMethod>(reader.GetString("DepreciationMethod")),
                DefaultUsefulLife = reader.GetInt32("DefaultUsefulLife"),
                DefaultNetSalvageRate = reader.GetDecimal("DefaultNetSalvageRate"),
                DepreciationAccountId = reader.IsDBNull(reader.GetOrdinal("DepreciationAccountId")) ? null : reader.GetInt32("DepreciationAccountId"),
                AccumulatedDepreciationAccountId = reader.IsDBNull(reader.GetOrdinal("AccumulatedDepreciationAccountId")) ? null : reader.GetInt32("AccumulatedDepreciationAccountId"),
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

    public class AssetDepreciationRecordRepository : RepositoryBase<AssetDepreciationRecord>
    {
        public AssetDepreciationRecordRepository(DbContext context) : base(context) { }

        protected override string TableName => "AssetDepreciationRecords";

        protected override AssetDepreciationRecord MapToEntity(MySqlDataReader reader)
        {
            return new AssetDepreciationRecord
            {
                Id = reader.GetInt32("Id"),
                AssetId = reader.GetInt32("AssetId"),
                DepreciationDate = reader.GetDateTime("DepreciationDate"),
                DepreciationAmount = reader.GetDecimal("DepreciationAmount"),
                AccumulatedDepreciation = reader.GetDecimal("AccumulatedDepreciation"),
                NetValue = reader.GetDecimal("NetValue"),
                VoucherId = reader.IsDBNull(reader.GetOrdinal("VoucherId")) ? null : reader.GetInt32("VoucherId"),
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
