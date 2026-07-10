using MoaediPharamcy.Repositories;
using MoaediPharamcy.ViewModels;

namespace MoaediPharamcy.Services
{
    public class DailySalesService : IDailySalesService
    {
        private readonly IDailySalesRepository _repository;
        private readonly ILogger<DailySalesService> _logger;

        public DailySalesService(IDailySalesRepository repository, ILogger<DailySalesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// گزارش فروش روزانه برای یک تاریخ خاص
        /// </summary>
        public async Task<DailySalesReportViewModel> GetDailySalesReportAsync(
            int year,
            int month,
            int day,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"درخواست گزارش فروش روزانه - سال: {year}، ماه: {month}، روز: {day}");

                var report = await _repository.GetDailySalesReportAsync(year, month, day, cancellationToken);

                if (report == null)
                {
                    _logger.LogWarning($"هیچ داده‌ای برای تاریخ {year}/{month}/{day} یافت نشد");
                    return new DailySalesReportViewModel();
                }

                _logger.LogInformation($"گزارش فروش روزانه برای {year}/{month}/{day} دریافت شد - کل فروش: {report.TotalDailySales}");
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در دریافت گزارش فروش روزانه - سال: {year}، ماه: {month}، روز: {day}");
                throw;
            }
        }

        /// <summary>
        /// گزارش فروش برای بازه زمانی
        /// </summary>
        public async Task<List<DailySalesReportViewModel>> GetDailySalesReportRangeAsync(
            int fromYear,
            int fromMonth,
            int fromDay,
            int toYear,
            int toMonth,
            int toDay,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"درخواست گزارش فروش برای بازه زمانی - از {fromYear}/{fromMonth}/{fromDay} تا {toYear}/{toMonth}/{toDay}");

                var reports = await _repository.GetDailySalesReportRangeAsync(
                    fromYear, fromMonth, fromDay,
                    toYear, toMonth, toDay,
                    cancellationToken);

                if (reports == null || reports.Count == 0)
                {
                    _logger.LogWarning($"هیچ داده‌ای برای بازه زمانی {fromYear}/{fromMonth}/{fromDay} تا {toYear}/{toMonth}/{toDay} یافت نشد");
                    return new List<DailySalesReportViewModel>();
                }

                var totalSales = reports.Sum(r => r.TotalDailySales);
                _logger.LogInformation($"گزارش فروش برای بازه زمانی دریافت شد - تعداد روزها: {reports.Count}، کل فروش: {totalSales}");
                return reports;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در دریافت گزارش فروش برای بازه زمانی");
                throw;
            }
        }

        /// <summary>
        /// گزارش فروش ماهانه
        /// </summary>
        public async Task<List<DailySalesReportViewModel>> GetMonthlySalesReportAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"درخواست گزارش فروش ماهانه - سال: {year}، ماه: {month}");

                var reports = await _repository.GetMonthlySalesReportAsync(year, month, cancellationToken);

                if (reports == null || reports.Count == 0)
                {
                    _logger.LogWarning($"هیچ داده‌ای برای ماه {year}/{month} یافت نشد");
                    return new List<DailySalesReportViewModel>();
                }

                var totalSales = reports.Sum(r => r.TotalDailySales);
                _logger.LogInformation($"گزارش فروش ماهانه برای {year}/{month} دریافت شد - تعداد روزها: {reports.Count}، کل فروش: {totalSales}");
                return reports;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"خطا در دریافت گزارش فروش ماهانه - سال: {year}، ماه: {month}");
                throw;
            }
        }
    }
}