using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinanceApp.Views.Pages
{
    public partial class ReportPage : UserControl
    {
        public ReportPage()
        {
            InitializeComponent();
            Loaded += ReportPage_Loaded;
        }

        private void ReportPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void QueryReport_Click(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            var reportType = (cboReportType.SelectedItem as ComboBoxItem)?.Content.ToString();
            var year = (cboYear.SelectedItem as ComboBoxItem)?.Content.ToString();
            var month = (cboMonth.SelectedItem as ComboBoxItem)?.Content.ToString();

            txtReportTitle.Text = reportType ?? "资产负债表";
            txtReportPeriod.Text = $"{year}年{month}月";

            ReportRowsPanel.Children.Clear();

            // 添加报表数据行 - 左列标签 | 左借方 | 左贷方 | 右列标签 | 右借方 | 右贷方
            AddReportRow("流动资产：", "", "", "流动负债：", "", "", true, false);
            AddReportRow("  库存现金", "10,000.00", "12,000.00", "  应付账款", "150,000.00", "180,000.00", false, false);
            AddReportRow("  银行存款", "500,000.00", "550,000.00", "  短期借款", "200,000.00", "200,000.00", false, false);
            AddReportRow("流动资产合计", "710,000.00", "782,000.00", "流动负债合计", "350,000.00", "380,000.00", true, true);

            AddReportRow("非流动资产：", "", "", "非流动负债：", "", "", true, false);
            AddReportRow("  固定资产", "1,000,000.00", "1,000,000.00", "  长期借款", "150,000.00", "150,000.00", false, false);
            AddReportRow("  累计折旧", "-100,000.00", "-120,000.00", "", "", "", false, false);
            AddReportRow("非流动资产合计", "900,000.00", "880,000.00", "非流动负债合计", "150,000.00", "150,000.00", true, true);

            AddReportRow("资产总计", "1,610,000.00", "1,662,000.00", "负债和所有者权益总计", "500,000.00", "530,000.00", true, true);
        }

        private void AddReportRow(string leftLabel, string leftDebit, string leftCredit, string rightLabel, string rightDebit, string rightCredit, bool isBold, bool isTotal)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };

            row.Children.Add(new TextBlock
            {
                Text = leftLabel,
                Width = 290,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = isTotal ? new SolidColorBrush(Color.FromRgb(37, 99, 235)) : Brushes.Black
            });
            row.Children.Add(new TextBlock { Text = leftDebit, Width = 150, TextAlignment = TextAlignment.Right, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal });
            row.Children.Add(new TextBlock { Text = leftCredit, Width = 150, TextAlignment = TextAlignment.Right, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal });
            row.Children.Add(new TextBlock
            {
                Text = rightLabel,
                Width = 290,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = isTotal ? new SolidColorBrush(Color.FromRgb(37, 99, 235)) : Brushes.Black
            });
            row.Children.Add(new TextBlock { Text = rightDebit, Width = 150, TextAlignment = TextAlignment.Right, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal });
            row.Children.Add(new TextBlock { Text = rightCredit, Width = 150, TextAlignment = TextAlignment.Right, FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal });

            ReportRowsPanel.Children.Add(row);
        }
    }
}