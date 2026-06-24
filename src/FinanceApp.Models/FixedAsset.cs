using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 固定资产类别
    /// </summary>
    public class FixedAssetCategory
    {
        public int Id { get; set; }
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DepreciationMethod DepreciationMethod { get; set; } = DepreciationMethod.平均年限法;
        public int DefaultUsefulLife { get; set; } = 10;
        public decimal DefaultNetSalvageRate { get; set; } = 0.05m;
        public int? DepreciationAccountId { get; set; }
        public int? AccumulatedDepreciationAccountId { get; set; }
        public DateTime CreateTime { get; set; }

        public AccountSubject? DepreciationAccount { get; set; }
        public AccountSubject? AccumulatedDepreciationAccount { get; set; }
    }

    /// <summary>
    /// 固定资产卡片
    /// </summary>
    public class FixedAsset
    {
        public int Id { get; set; }
        public string AssetCode { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? Specification { get; set; }
        public string? MeasurementUnit { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal OriginalValue { get; set; }
        public decimal NetSalvageValue { get; set; }
        public int UsefulLife { get; set; }
        public DepreciationMethod DepreciationMethod { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime StartUseDate { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal NetValue { get; set; }
        public int DepreciationAccountId { get; set; }
        public int FixedAssetAccountId { get; set; }
        public int AccumulatedDepreciationAccountId { get; set; }
        public DepreciationStatus DepreciationStatus { get; set; } = DepreciationStatus.正常使用;
        public string? Memo { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public FixedAssetCategory? Category { get; set; }
    }

    /// <summary>
    /// 固定资产折旧记录
    /// </summary>
    public class AssetDepreciationRecord
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public DateTime DepreciationDate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal NetValue { get; set; }
        public int? VoucherId { get; set; }
        public DateTime CreateTime { get; set; }

        public FixedAsset? Asset { get; set; }
    }

    public enum DepreciationMethod
    {
        平均年限法,
        双倍余额递减法,
        年数总和法,
        工作量法
    }

    public enum DepreciationStatus
    {
        正常使用,
        已提足折旧,
        已处置
    }
}
