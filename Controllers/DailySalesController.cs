using Microsoft.AspNetCore.Mvc;
using MoaediPharamcy.Services;

namespace MoaediPharamcy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailySalesController : ControllerBase
    {
        private readonly IDailySalesService _salesService;
        private readonly ILogger<DailySalesController> _logger;

        public DailySalesController(IDailySalesService salesService, ILogger<DailySalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// دریافت گزارش فروش روزانه
        /// </summary>
        /// <param name="year">سال شمسی</param>
        /// <param name="month">ماه شمسی (1-12)</param>
        /// <param name="day">روز شمسی (1-31)</param>
        /// <returns>گزارش فروش روزانه</returns>
        [HttpGet("daily")]
        [ProduceResponseType(StatusCodes.Status200OK)]
        [ProduceResponseType(StatusCodes.Status400BadRequest)]
        [ProduceResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDailySalesReport(
            [FromQuery] int year,
            [FromQuery] int month,
            [FromQuery] int day,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (year < 1300 || year > 1500)
                    return BadRequest(new { message = "سال نامعتبر است (باید بین 1300 و 1500 باشد)" });

                if (month < 1 || month > 12)
                    return BadRequest(new { message = "ماه نامعتبر است (باید بین 1 و 12 باشد)" });

                if (day < 1 || day > 31)
                    return BadRequest(new { message = "روز نامعتبر است (باید بین 1 و 31 باشد)" });

                var report = await _salesService.GetDailySalesReportAsync(year, month, day, cancellationToken);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت گزارش فروش روزانه");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// دریافت گزارش فروش برای بازه زمانی
        /// </summary>
        /// <param name="fromYear">سال شمسی شروع</param>
        /// <param name="fromMonth">ماه شمسی شروع</param>
        /// <param name="fromDay">روز شمسی شروع</param>
        /// <param name="toYear">سال شمسی پایان</param>
        /// <param name="toMonth">ماه شمسی پایان</param>
        /// <param name="toDay">روز شمسی پایان</param>
        /// <returns>لیست گزارش‌های فروش</returns>
        [HttpGet("range")]
        [ProduceResponseType(StatusCodes.Status200OK)]
        [ProduceResponseType(StatusCodes.Status400BadRequest)]
        [ProduceResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDailySalesReportRange(
            [FromQuery] int fromYear,
            [FromQuery] int fromMonth,
            [FromQuery] int fromDay,
            [FromQuery] int toYear,
            [FromQuery] int toMonth,
            [FromQuery] int toDay,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (fromYear < 1300 || fromYear > 1500 || toYear < 1300 || toYear > 1500)
                    return BadRequest(new { message = "سال نامعتبر است" });

                if (fromMonth < 1 || fromMonth > 12 || toMonth < 1 || toMonth > 12)
                    return BadRequest(new { message = "ماه نامعتبر است" });

                if (fromDay < 1 || fromDay > 31 || toDay < 1 || toDay > 31)
                    return BadRequest(new { message = "روز نامعتبر است" });

                var reports = await _salesService.GetDailySalesReportRangeAsync(
                    fromYear, fromMonth, fromDay,
                    toYear, toMonth, toDay,
                    cancellationToken);

                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت گزارش فروش برای بازه زمانی");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// دریافت گزارش فروش ماهانه
        /// </summary>
        /// <param name="year">سال شمسی</param>
        /// <param name="month">ماه شمسی (1-12)</param>
        /// <returns>لیست گزارش‌های فروش روزانه برای یک ماه</returns>
        [HttpGet("monthly")]
        [ProduceResponseType(StatusCodes.Status200OK)]
        [ProduceResponseType(StatusCodes.Status400BadRequest)]
        [ProduceResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMonthlySalesReport(
            [FromQuery] int year,
            [FromQuery] int month,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (year < 1300 || year > 1500)
                    return BadRequest(new { message = "سال نامعتبر است" });

                if (month < 1 || month > 12)
                    return BadRequest(new { message = "ماه نامعتبر است" });

                var reports = await _salesService.GetMonthlySalesReportAsync(year, month, cancellationToken);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت گزارش فروش ماهانه");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// دریافت گزارش فروش امروز
        /// </summary>
        /// <returns>گزارش فروش روزانه امروز</returns>
        [HttpGet("today")]
        [ProduceResponseType(StatusCodes.Status200OK)]
        [ProduceResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodaysSalesReport(CancellationToken cancellationToken = default)
        {
            try
            {
                // تاریخ شمسی امروز - باید از کتابخانه‌ای مانند Persianidate استفاده کنید
                // این یک نمونه ساده است
                var today = ConvertToShamsi(DateTime.Now);

                var report = await _salesService.GetDailySalesReportAsync(
                    today.Year, today.Month, today.Day, cancellationToken);

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در دریافت گزارش فروش امروز");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "خطا در دریافت گزارش", error = ex.Message });
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به شمسی (نمونه ساده)
        /// </summary>
        private (int Year, int Month, int Day) ConvertToShamsi(DateTime date)
        {
            // این یک نمونه ساده است - برای استفاده واقعی از کتابخانه مخصوص استفاده کنید
            return (1405, 3, 27);
        }
    }
}