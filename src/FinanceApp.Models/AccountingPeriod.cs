using System;

namespace FinanceApp.Models
{
    /// <summary>
    /// 会计期间
    /// </summary>
    public class AccountingPeriod
    {
        public int Id { get; set; }
        public int PeriodYear { get; set; }
        public int PeriodMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreateTime { get; set; }

        public string PeriodName => $"{PeriodYear}年第{PeriodMonth}期";
    }
}
