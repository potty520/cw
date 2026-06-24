using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FinanceApp.Models;

namespace FinanceApp.ViewModels
{
    public class VoucherViewModel : ViewModelBase
    {
        private ObservableCollection<Voucher> _vouchers = new();
        private Voucher? _selectedVoucher;
        private VoucherEntry? _selectedEntry;
        private DateTime _voucherDate = DateTime.Now;
        private string _memo = string.Empty;
        private int _attachmentCount;
        private bool _isEditing;
        private string _errorMessage = string.Empty;

        public ObservableCollection<Voucher> Vouchers
        {
            get => _vouchers;
            set => SetProperty(ref _vouchers, value);
        }

        public Voucher? SelectedVoucher
        {
            get => _selectedVoucher;
            set
            {
                if (SetProperty(ref _selectedVoucher, value))
                {
                    LoadVoucherDetails();
                }
            }
        }

        public VoucherEntry? SelectedEntry
        {
            get => _selectedEntry;
            set => SetProperty(ref _selectedEntry, value);
        }

        public DateTime VoucherDate
        {
            get => _voucherDate;
            set => SetProperty(ref _voucherDate, value);
        }

        public string Memo
        {
            get => _memo;
            set => SetProperty(ref _memo, value);
        }

        public int AttachmentCount
        {
            get => _attachmentCount;
            set => SetProperty(ref _attachmentCount, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ObservableCollection<VoucherEntry> CurrentEntries { get; } = new();
        public ObservableCollection<VoucherWord> VoucherWords { get; } = new()
        {
            new VoucherWord { Id = 1, WordName = "记" },
            new VoucherWord { Id = 2, WordName = "收" },
            new VoucherWord { Id = 3, WordName = "付" },
            new VoucherWord { Id = 4, WordName = "转" }
        };
        public ObservableCollection<AccountSubject> AccountSubjects { get; } = new();

        public VoucherWord? SelectedVoucherWord { get; set; }

        public ICommand NewVoucherCommand { get; }
        public ICommand SaveVoucherCommand { get; }
        public ICommand AddEntryCommand { get; }
        public ICommand RemoveEntryCommand { get; }
        public ICommand AuditVoucherCommand { get; }
        public ICommand PostVoucherCommand { get; }
        public ICommand DeleteVoucherCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler? VoucherSaved;
        public event EventHandler<string>? ErrorOccurred;

        public VoucherViewModel()
        {
            NewVoucherCommand = new Command(NewVoucher);
            SaveVoucherCommand = new Command(async () => await SaveVoucherAsync(), CanSaveVoucher());
            AddEntryCommand = new Command(AddEntry);
            RemoveEntryCommand = new Command(RemoveEntry);
            AuditVoucherCommand = new Command(async () => await AuditVoucherAsync(), CanAuditVoucher());
            PostVoucherCommand = new Command(async () => await PostVoucherAsync(), CanPostVoucher());
            DeleteVoucherCommand = new Command(async () => await DeleteVoucherAsync(), CanDeleteVoucher());
            RefreshCommand = new Command(async () => await LoadVouchersAsync());
        }

        private void LoadVoucherDetails()
        {
            if (SelectedVoucher != null)
            {
                CurrentEntries.Clear();
                foreach (var entry in SelectedVoucher.Entries)
                {
                    CurrentEntries.Add(entry);
                }
            }
        }

        private void NewVoucher()
        {
            SelectedVoucher = new Voucher
            {
                VoucherDate = DateTime.Now,
                VoucherStatus = VoucherStatus.新建,
                CreatorId = 1
            };
            CurrentEntries.Clear();
            AddEntry();
            AddEntry();
            IsEditing = true;
            ErrorMessage = string.Empty;
        }

        private Func<bool> CanSaveVoucher() => () => IsEditing && CurrentEntries.Count >= 2;

        private async Task SaveVoucherAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                // 验证借贷平衡
                decimal totalDebit = 0;
                decimal totalCredit = 0;
                foreach (var entry in CurrentEntries)
                {
                    totalDebit += entry.DebitAmount;
                    totalCredit += entry.CreditAmount;
                }

                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    ErrorMessage = $"借贷不平衡！借方金额：{totalDebit:N2}，贷方金额：{totalCredit:N2}";
                    ErrorOccurred?.Invoke(this, ErrorMessage);
                    return;
                }

                // 这里调用服务保存凭证
                await Task.Delay(100);

                VoucherSaved?.Invoke(this, EventArgs.Empty);
                IsEditing = false;
                await LoadVouchersAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "保存失败：" + ex.Message;
                ErrorOccurred?.Invoke(this, ErrorMessage);
            }
        }

        private void AddEntry()
        {
            CurrentEntries.Add(new VoucherEntry
            {
                EntryDate = VoucherDate,
                Summary = string.Empty,
                DebitAmount = 0,
                CreditAmount = 0
            });
        }

        private void RemoveEntry()
        {
            if (SelectedEntry != null && CurrentEntries.Count > 2)
            {
                CurrentEntries.Remove(SelectedEntry);
                SelectedEntry = null;
            }
        }

        private Func<bool> CanAuditVoucher() => () => SelectedVoucher?.VoucherStatus == VoucherStatus.新建;

        private async Task AuditVoucherAsync()
        {
            if (SelectedVoucher == null) return;
            await Task.Delay(100);
            SelectedVoucher.VoucherStatus = VoucherStatus.已审核;
            await LoadVouchersAsync();
        }

        private Func<bool> CanPostVoucher() => () => SelectedVoucher?.VoucherStatus == VoucherStatus.已审核;

        private async Task PostVoucherAsync()
        {
            if (SelectedVoucher == null) return;
            await Task.Delay(100);
            SelectedVoucher.VoucherStatus = VoucherStatus.已过账;
            await LoadVouchersAsync();
        }

        private Func<bool> CanDeleteVoucher() => () => SelectedVoucher?.VoucherStatus == VoucherStatus.新建;

        private async Task DeleteVoucherAsync()
        {
            if (SelectedVoucher == null) return;
            await Task.Delay(100);
            Vouchers.Remove(SelectedVoucher);
            SelectedVoucher = null;
        }

        public async Task LoadVouchersAsync()
        {
            // 模拟加载凭证
            await Task.Delay(100);
        }
    }
}
