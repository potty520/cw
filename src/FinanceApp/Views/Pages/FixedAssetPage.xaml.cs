using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FinanceApp.Views.Pages
{
    public partial class FixedAssetPage : UserControl
    {
        private ObservableCollection<AssetItem> _assets = new();

        public FixedAssetPage()
        {
            InitializeComponent();
            Loaded += FixedAssetPage_Loaded;
            dgAssets.ItemsSource = _assets;
        }

        private void FixedAssetPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssets();
        }

        private void LoadAssets()
        {
            _assets.Clear();
            _assets.Add(new AssetItem { AssetCode = "FA001", AssetName = "办公楼", Category = "房屋建筑", OriginalValue = 1000000, AccumulatedDepreciation = 200000, NetValue = 800000, Status = "正常使用", PurchaseDate = new DateTime(2020, 1, 1) });
            _assets.Add(new AssetItem { AssetCode = "FA002", AssetName = "生产设备", Category = "机器设备", OriginalValue = 500000, AccumulatedDepreciation = 150000, NetValue = 350000, Status = "正常使用", PurchaseDate = new DateTime(2021, 6, 15) });
            _assets.Add(new AssetItem { AssetCode = "FA003", AssetName = "轿车", Category = "运输设备", OriginalValue = 300000, AccumulatedDepreciation = 100000, NetValue = 200000, Status = "正常使用", PurchaseDate = new DateTime(2022, 3, 10) });
            _assets.Add(new AssetItem { AssetCode = "FA004", AssetName = "电脑", Category = "电子设备", OriginalValue = 80000, AccumulatedDepreciation = 60000, NetValue = 20000, Status = "已提足折旧", PurchaseDate = new DateTime(2019, 5, 20) });
            _assets.Add(new AssetItem { AssetCode = "FA005", AssetName = "打印机", Category = "电子设备", OriginalValue = 20000, AccumulatedDepreciation = 15000, NetValue = 5000, Status = "正常使用", PurchaseDate = new DateTime(2020, 8, 1) });
        }
    }

    public class AssetItem
    {
        public string AssetCode { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal OriginalValue { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal NetValue { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
    }
}
