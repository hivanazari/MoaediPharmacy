using MoaediPharamcy.ViewModels;

namespace MoaediPharamcy.Services
{
    public interface IDailySalesService
    {
        /// <summary>
        /// گزارش فروش روزانه برای یک تاریخ خاص
        /// </summary>
        Task<DailySalesReportViewModel> GetDailySalesReportAsync(
            int year,
            int month,
            int day,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// گزارش فروش برای بازه زمانی
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
        /// گزارش فروش ماهانه
        /// </summary>
        Task<List<DailySalesReportViewModel>> GetMonthlySalesReportAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default);
    }
}