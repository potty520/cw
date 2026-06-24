using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceApp.Data;
using FinanceApp.Models;

namespace FinanceApp.Services
{
    /// <summary>
    /// 固定资产服务
    /// </summary>
    public class FixedAssetService
    {
        private readonly FixedAssetRepository _assetRepository;
        private readonly FixedAssetCategoryRepository _categoryRepository;
        private readonly AssetDepreciationRecordRepository _depreciationRepository;

        public FixedAssetService(DbContext context)
        {
            _assetRepository = new FixedAssetRepository(context);
            _categoryRepository = new FixedAssetCategoryRepository(context);
            _depreciationRepository = new AssetDepreciationRecordRepository(context);
        }

        /// <summary>
        /// 计算月折旧额（平均年限法）
        /// </summary>
        public decimal CalculateMonthlyDepreciation(FixedAsset asset)
        {
            var depreciableAmount = asset.OriginalValue - asset.NetSalvageValue;
            return depreciableAmount / (asset.UsefulLife * 12);
        }

        /// <summary>
        /// 计提折旧
        /// </summary>
        public async Task<bool> ProcessMonthlyDepreciationAsync(DateTime depreciationDate)
        {
            var assets = await _assetRepository.GetByStatusAsync(DepreciationStatus.正常使用);
            foreach (var asset in assets)
            {
                var monthlyDepreciation = CalculateMonthlyDepreciation(asset);
                var newAccumulatedDepreciation = asset.AccumulatedDepreciation + monthlyDepreciation;
                var newNetValue = asset.OriginalValue - newAccumulatedDepreciation;

                // 更新资产折旧信息
                var updateSql = @"UPDATE FixedAssets
                                  SET AccumulatedDepreciation = @AccumulatedDepreciation,
                                      NetValue = @NetValue,
                                      DepreciationStatus = CASE
                                          WHEN @AccumulatedDepreciation >= (OriginalValue - NetSalvageValue)
                                          THEN '已提足折旧' ELSE DepreciationStatus END
                                  WHERE Id = @Id";

                using var connection = _assetRepository.Context.CreateConnection();
                using var command = new MySqlConnector.MySqlCommand(updateSql, connection);
                command.Parameters.AddWithValue("@AccumulatedDepreciation", newAccumulatedDepreciation);
                command.Parameters.AddWithValue("@NetValue", newNetValue);
                command.Parameters.AddWithValue("@Id", asset.Id);
                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                // 记录折旧
                var recordSql = @"INSERT INTO AssetDepreciationRecords
                                  (AssetId, DepreciationDate, DepreciationAmount, AccumulatedDepreciation, NetValue)
                                  VALUES (@AssetId, @DepreciationDate, @DepreciationAmount, @AccumulatedDepreciation, @NetValue)";

                using var recordCommand = new MySqlConnector.MySqlCommand(recordSql, connection);
                recordCommand.Parameters.AddWithValue("@AssetId", asset.Id);
                recordCommand.Parameters.AddWithValue("@DepreciationDate", depreciationDate);
                recordCommand.Parameters.AddWithValue("@DepreciationAmount", monthlyDepreciation);
                recordCommand.Parameters.AddWithValue("@AccumulatedDepreciation", newAccumulatedDepreciation);
                recordCommand.Parameters.AddWithValue("@NetValue", newNetValue);
                await recordCommand.ExecuteNonQueryAsync();
            }
            return true;
        }

        public async Task<IEnumerable<FixedAsset>> GetAllAssetsAsync()
        {
            return await _assetRepository.GetAllAsync();
        }

        public async Task<FixedAsset?> GetAssetByIdAsync(int id)
        {
            return await _assetRepository.GetByIdAsync(id);
        }
    }
}
