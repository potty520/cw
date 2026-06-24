using System;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using FinanceApp.Services;

namespace FinanceApp.Views
{
    public partial class PrintSettingsWindow : Window
    {
        public PrintSettingsData Settings { get; private set; } = new();

        public PrintSettingsWindow()
        {
            InitializeComponent();
            Loaded += PrintSettingsWindow_Loaded;
        }

        private void PrintSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPrinters();
        }

        private void LoadPrinters()
        {
            cboPrinter.Items.Clear();
            var printServer = new PrintServer();
            var queues = printServer.GetPrintQueues();

            foreach (var queue in queues)
            {
                cboPrinter.Items.Add(queue.Name);
            }

            if (cboPrinter.Items.Count > 0)
            {
                cboPrinter.SelectedIndex = 0;
            }
        }

        private void ChkTaoDa_Changed(object sender, RoutedEventArgs e)
        {
            bool isEnabled = chkEnableTaoDa.IsChecked == true;
            txtTaoDaOffsetX.IsEnabled = isEnabled;
            txtTaoDaOffsetY.IsEnabled = isEnabled;
            btnCalibrate.IsEnabled = isEnabled;
        }

        private void BtnCalibrate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("将打印校准测试页，请根据打印结果调整偏移量。", "套打校准", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Settings = new PrintSettingsData
            {
                PaperWidth = double.Parse(txtPaperWidth.Text),
                PaperHeight = double.Parse(txtPaperHeight.Text),
                IsLandscape = rbLandscape.IsChecked == true,
                MarginLeft = double.Parse(txtMarginLeft.Text),
                MarginRight = double.Parse(txtMarginRight.Text),
                MarginTop = double.Parse(txtMarginTop.Text),
                MarginBottom = double.Parse(txtMarginBottom.Text),
                EnableTaoDa = chkEnableTaoDa.IsChecked == true,
                TaoDaOffsetX = double.Parse(txtTaoDaOffsetX.Text),
                TaoDaOffsetY = double.Parse(txtTaoDaOffsetY.Text),
                SelectedPrinter = cboPrinter.SelectedItem?.ToString() ?? ""
            };

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public void LoadSettings(PrintSettingsData data)
        {
            txtPaperWidth.Text = data.PaperWidth.ToString();
            txtPaperHeight.Text = data.PaperHeight.ToString();
            rbPortrait.IsChecked = !data.IsLandscape;
            rbLandscape.IsChecked = data.IsLandscape;
            txtMarginLeft.Text = data.MarginLeft.ToString();
            txtMarginRight.Text = data.MarginRight.ToString();
            txtMarginTop.Text = data.MarginTop.ToString();
            txtMarginBottom.Text = data.MarginBottom.ToString();
            chkEnableTaoDa.IsChecked = data.EnableTaoDa;
            txtTaoDaOffsetX.Text = data.TaoDaOffsetX.ToString();
            txtTaoDaOffsetY.Text = data.TaoDaOffsetY.ToString();

            if (!string.IsNullOrEmpty(data.SelectedPrinter))
            {
                for (int i = 0; i < cboPrinter.Items.Count; i++)
                {
                    if (cboPrinter.Items[i].ToString() == data.SelectedPrinter)
                    {
                        cboPrinter.SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
