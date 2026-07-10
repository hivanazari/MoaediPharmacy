namespace MoaediPharamcy.Repositories
{
    public interface IDailySalesRepository
    {
        /// <summary>
        /// گزارش فروش روزانه برای یک تاریخ خاص (سال، ماه، روز شمسی)
        /// </summary>
        Task<DailySalesReportViewModel> GetDailySalesReportAsync(
            int year, 
            int month, 
            int day,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// گزارش فروش برای بازه زمانی (شمسی)
        /// </summary>
        Task<List<DailySalesReportViewModel>> GetDailySalesReportRangeAsync(
            int fromYear,
            int fromMonth,
            int fromDay,
            int toYear,
            int toMonth,
            int toDay,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// گزارش فروش برای یک ماه (شمسی)
        /// </summary>
        Task<List<DailySalesReportViewModel>> GetMonthlySalesReportAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default);
    }
}
