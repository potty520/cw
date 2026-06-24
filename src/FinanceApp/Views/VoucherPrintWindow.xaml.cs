using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Printing;
using FinanceApp.Models;
using FinanceApp.Services;

namespace FinanceApp.Views
{
    public partial class VoucherPrintWindow : Window
    {
        private readonly ReportPrintService _reportService = new();
        private PrintSettingsData _printSettings = new();
        private Voucher? _currentVoucher;

        public VoucherPrintWindow()
        {
            InitializeComponent();
        }

        public void LoadVoucher(Voucher voucher)
        {
            _currentVoucher = voucher;
            PrintPreview.LoadVoucherData(voucher);
        }

        public void SetPrintSettings(PrintSettingsData settings)
        {
            _printSettings = settings;
            _reportService.SetPrintSettings(settings);
        }

        private void PrintSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new PrintSettingsWindow();
            settingsWindow.LoadSettings(_printSettings);

            if (settingsWindow.ShowDialog() == true)
            {
                _printSettings = settingsWindow.Settings;
                _reportService.SetPrintSettings(_printSettings);
            }
        }

        private void PrintDirect_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucher != null)
            {
                var printDialog = new PrintDialog();

                if (!string.IsNullOrEmpty(_printSettings.SelectedPrinter))
                {
                    try
                    {
                        var printServer = new PrintServer();
                        var queue = printServer.GetPrintQueue(_printSettings.SelectedPrinter);
                        if (queue != null)
                        {
                            printDialog.PrintQueue = queue;
                        }
                    }
                    catch { }
                }

                if (printDialog.ShowDialog() == true)
                {
                    var visual = CreateVoucherVisual(_currentVoucher);
                    printDialog.PrintVisual(visual, "凭证打印 - " + _currentVoucher.VoucherNumber);
                    MessageBox.Show("打印完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("请先加载凭证数据", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVoucher != null)
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PNG图像|*.png",
                    FileName = $"凭证_{_currentVoucher.VoucherNumber}.png"
                };

                if (dialog.ShowDialog() == true)
                {
                    ExportToImage(_currentVoucher, dialog.FileName);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("请先加载凭证数据", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToImage(Voucher voucher, string filePath)
        {
            var visual = CreateVoucherVisual(voucher);
            var width = 800;
            var height = 1100;

            var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = File.Create(filePath))
            {
                encoder.Save(stream);
            }
        }

        private FrameworkElement CreateVoucherVisual(Voucher voucher)
        {
            var root = new Border
            {
                Background = Brushes.White,
                Padding = new Thickness(
                    _printSettings.MarginLeft,
                    _printSettings.MarginTop,
                    _printSettings.MarginRight,
                    _printSettings.MarginBottom)
            };

            if (!_printSettings.EnableTaoDa)
            {
                root.BorderBrush = Brushes.Black;
                root.BorderThickness = new Thickness(1);
            }

            var content = new StackPanel { Width = 650 };

            // 公司标题
            content.Children.Add(new TextBlock
            {
                Text = "财智星财务软件",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            });

            // 凭证标题
            content.Children.Add(new TextBlock
            {
                Text = "记 账 凭 证",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 15)
            });

            // 凭证信息行
            var infoPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            infoPanel.Children.Add(new TextBlock { Text = $"凭证字：{voucher.VoucherWord?.WordName ?? "记"}", Margin = new Thickness(0, 0, 30, 0) });
            infoPanel.Children.Add(new TextBlock { Text = $"凭证号：{voucher.VoucherNumber}", Margin = new Thickness(0, 0, 30, 0) });
            infoPanel.Children.Add(new TextBlock { Text = $"日期：{voucher.VoucherDate:yyyy-MM-dd}" });
            content.Children.Add(infoPanel);

            // 分录表格
            var entriesGrid = CreateEntriesGrid(voucher);
            content.Children.Add(entriesGrid);

            // 合计行
            var totalDebit = _reportService.ConvertVoucherToPrintData(voucher).TotalDebit;
            var totalCredit = _reportService.ConvertVoucherToPrintData(voucher).TotalCredit;

            var totalGrid = new Grid { Margin = new Thickness(0, 0, 0, 15) };
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(460) });
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });
            totalGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });

            var totalBg = new SolidColorBrush(Color.FromRgb(241, 245, 249));
            totalGrid.Children.Add(CreateCell("合  计", 460, true, totalBg, null));
            totalGrid.Children.Add(CreateCell(totalDebit, 95, true, totalBg, HorizontalAlignment.Right));
            totalGrid.Children.Add(CreateCell(totalCredit, 95, true, totalBg, HorizontalAlignment.Right));
            content.Children.Add(totalGrid);

            // 签名区
            var sigPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 20, 0, 10) };
            sigPanel.Children.Add(new TextBlock { Text = "主管：", Margin = new Thickness(0, 0, 60, 0) });
            sigPanel.Children.Add(new TextBlock { Text = "复核：", Margin = new Thickness(0, 0, 60, 0) });
            sigPanel.Children.Add(new TextBlock { Text = "记账：", Margin = new Thickness(0, 0, 60, 0) });
            sigPanel.Children.Add(new TextBlock { Text = $"制单：User{voucher.CreatorId}" });
            content.Children.Add(sigPanel);

            // 附件张数
            content.Children.Add(new TextBlock
            {
                Text = $"附件张数：{voucher.AttachmentCount}",
                Margin = new Thickness(0, 5, 0, 0)
            });

            root.Child = content;
            return root;
        }

        private Grid CreateEntriesGrid(Voucher voucher)
        {
            var grid = new Grid { Margin = new Thickness(0, 0, 0, 10) };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });

            var headerBg = new SolidColorBrush(Color.FromRgb(241, 245, 249));
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
            grid.Children.Add(CreateCell("摘  要", 180, true, headerBg, null));
            grid.Children.Add(CreateCell("科目代码", 80, true, headerBg, HorizontalAlignment.Center));
            grid.Children.Add(CreateCell("科目名称", 200, true, headerBg, null));
            grid.Children.Add(CreateCell("借方金额", 95, true, headerBg, HorizontalAlignment.Right));
            grid.Children.Add(CreateCell("贷方金额", 95, true, headerBg, HorizontalAlignment.Right));

            var printData = _reportService.ConvertVoucherToPrintData(voucher);
            int rowIndex = 1;
            foreach (var entry in printData.Entries)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                grid.Children.Add(CreateCell(entry.Summary, 180, false, null, null));
                grid.Children.Add(CreateCell(entry.AccountCode, 80, false, null, HorizontalAlignment.Center));
                grid.Children.Add(CreateCell(entry.AccountName, 200, false, null, null));
                grid.Children.Add(CreateCell(entry.DebitAmount, 95, false, null, HorizontalAlignment.Right));
                grid.Children.Add(CreateCell(entry.CreditAmount, 95, false, null, HorizontalAlignment.Right));
                rowIndex++;
            }

            return grid;
        }

        private Border CreateCell(string text, double width, bool isHeader, SolidColorBrush? bg, HorizontalAlignment? align)
        {
            var border = new Border
            {
                Width = width,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = bg ?? Brushes.Transparent
            };

            var textBlock = new TextBlock
            {
                Text = text,
                FontSize = isHeader ? 10 : 9,
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                Padding = new Thickness(2),
                HorizontalAlignment = align ?? HorizontalAlignment.Left
            };

            border.Child = textBlock;
            return border;
        }
    }
}
