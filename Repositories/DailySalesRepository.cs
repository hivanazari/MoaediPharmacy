using Dapper;
using MoaediPharamcy.ViewModels;
using System.Data;

namespace MoaediPharamcy.Repositories
{
    public class DailySalesRepository : IDailySalesRepository
    {
        private readonly IDbConnection _dbConnection;

        public DailySalesRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// گزارش فروش روزانه برای یک تاریخ خاص (سال، ماه، روز شمسی)
        /// </summary>
        public async Task<DailySalesReportViewModel> GetDailySalesReportAsync(
            int year,
            int month,
            int day,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
WITH InvoiceSummary AS (
    -- خلاصه فاکتور‌ها (بدون تکرار)
    SELECT 
        e.Exporttransid,
        CAST(e.Firstcreatetime AS DATE) AS ReportDate,
        e.PrefExchangePrice,
        e.Workmoney,
        e.Payableprice,
        e.Paidprice
    FROM [Exportdrugmastertrans] AS e
    WHERE e.Deletedatetime IS NULL
        AND e.Docyear = @Year
        AND e.Docmonth = @Month
        AND e.Docday = @Day
),
ItemSummary AS (
    -- خلاصه اقلام
    SELECT 
        i.Exporttransid,
        COUNT(i.Docitem) AS TotalItems,
        SUM(i.Totprice) AS TotalDailySales,
        SUM(i.Totpriceorg) AS TotalInsuranceSales,
        SUM(i.Totpriceorg - i.Priceorg) AS TotalSickPart,
        SUM(i.Priceorg) AS TotalOrganPart
    FROM [Exportdrugdetailtrans] AS i
    GROUP BY i.Exporttransid
)
SELECT 
    inv.ReportDate,
    COUNT(DISTINCT inv.Exporttransid) AS TotalInvoices,
    SUM(item.TotalItems) AS TotalItems,
    SUM(item.TotalDailySales) AS TotalDailySales,
    SUM(item.TotalInsuranceSales) AS TotalInsuranceSales,
    SUM(item.TotalSickPart) AS TotalSickPart,
    SUM(item.TotalOrganPart) AS TotalOrganPart,
    SUM(ISNULL(inv.PrefExchangePrice, 0)) AS TotalExchangePrice,
    SUM(ISNULL(inv.Workmoney, 0)) AS TotalPharmacyServiceFee,
    SUM(ISNULL(inv.Payableprice, 0)) AS TotalPayablePrice,
    SUM(ISNULL(inv.Paidprice, 0)) AS TotalPaidPrice
FROM InvoiceSummary inv
INNER JOIN ItemSummary item ON inv.Exporttransid = item.Exporttransid
GROUP BY inv.ReportDate;
            ";

            using var connection = _dbConnection;
            var result = await connection.QueryFirstOrDefaultAsync<DailySalesReportViewModel>(
                new CommandDefinition(sql,
                    new { Year = year, Month = month, Day = day },
                    cancellationToken: cancellationToken));

            return result ?? new DailySalesReportViewModel
            {
                ReportDate = DateTime.Now,
                TotalInvoices = 0,
                TotalItems = 0,
                TotalDailySales = 0,
                TotalInsuranceSales = 0,
                TotalSickPart = 0,
                TotalOrganPart = 0,
                TotalExchangePrice = 0,
                TotalPharmacyServiceFee = 0,
                TotalPayablePrice = 0,
                TotalPaidPrice = 0
            };
        }

        /// <summary>
        /// گزارش فروش برای بازه زمانی (شمسی)
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
            const string sql = @"
WITH InvoiceSummary AS (
    -- خلاصه فاکتور‌ها (بدون تکرار)
    SELECT 
        e.Exporttransid,
        CAST(e.Firstcreatetime AS DATE) AS ReportDate,
        e.Docyear,
        e.Docmonth,
        e.Docday,
        e.PrefExchangePrice,
        e.Workmoney,
        e.Payableprice,
        e.Paidprice
    FROM [Exportdrugmastertrans] AS e
    WHERE e.Deletedatetime IS NULL
        AND ((e.Docyear > @FromYear) 
            OR (e.Docyear = @FromYear AND e.Docmonth > @FromMonth)
            OR (e.Docyear = @FromYear AND e.Docmonth = @FromMonth AND e.Docday >= @FromDay))
        AND ((e.Docyear < @ToYear)
            OR (e.Docyear = @ToYear AND e.Docmonth < @ToMonth)
            OR (e.Docyear = @ToYear AND e.Docmonth = @ToMonth AND e.Docday <= @ToDay))
),
ItemSummary AS (
    -- خلاصه اقلام
    SELECT 
        i.Exporttransid,
        COUNT(i.Docitem) AS TotalItems,
        SUM(i.Totprice) AS TotalDailySales,
        SUM(i.Totpriceorg) AS TotalInsuranceSales,
        SUM(i.Totpriceorg - i.Priceorg) AS TotalSickPart,
        SUM(i.Priceorg) AS TotalOrganPart
    FROM [Exportdrugdetailtrans] AS i
    GROUP BY i.Exporttransid
)
SELECT 
    inv.ReportDate,
    COUNT(DISTINCT inv.Exporttransid) AS TotalInvoices,
    SUM(item.TotalItems) AS TotalItems,
    SUM(item.TotalDailySales) AS TotalDailySales,
    SUM(item.TotalInsuranceSales) AS TotalInsuranceSales,
    SUM(item.TotalSickPart) AS TotalSickPart,
    SUM(item.TotalOrganPart) AS TotalOrganPart,
    SUM(ISNULL(inv.PrefExchangePrice, 0)) AS TotalExchangePrice,
    SUM(ISNULL(inv.Workmoney, 0)) AS TotalPharmacyServiceFee,
    SUM(ISNULL(inv.Payableprice, 0)) AS TotalPayablePrice,
    SUM(ISNULL(inv.Paidprice, 0)) AS TotalPaidPrice
FROM InvoiceSummary inv
INNER JOIN ItemSummary item ON inv.Exporttransid = item.Exporttransid
GROUP BY inv.ReportDate
ORDER BY inv.ReportDate DESC;
            ";

            using var connection = _dbConnection;
            var result = await connection.QueryAsync<DailySalesReportViewModel>(
                new CommandDefinition(sql,
                    new
                    {
                        FromYear = fromYear,
                        FromMonth = fromMonth,
                        FromDay = fromDay,
                        ToYear = toYear,
                        ToMonth = toMonth,
                        ToDay = toDay
                    },
                    cancellationToken: cancellationToken));

            return result.AsList();
        }

        /// <summary>
        /// گزارش فروش برای یک ماه (شمسی)
        /// </summary>
        public async Task<List<DailySalesReportViewModel>> GetMonthlySalesReportAsync(
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            const string sql = @"
WITH InvoiceSummary AS (
    -- خلاصه فاکتور‌ها (بدون تکرار)
    SELECT 
        e.Exporttransid,
        CAST(e.Firstcreatetime AS DATE) AS ReportDate,
        e.PrefExchangePrice,
        e.Workmoney,
        e.Payableprice,
        e.Paidprice
    FROM [Exportdrugmastertrans] AS e
    WHERE e.Deletedatetime IS NULL
        AND e.Docyear = @Year
        AND e.Docmonth = @Month
),
ItemSummary AS (
    -- خلاصه اقلام
    SELECT 
        i.Exporttransid,
        COUNT(i.Docitem) AS TotalItems,
        SUM(i.Totprice) AS TotalDailySales,
        SUM(i.Totpriceorg) AS TotalInsuranceSales,
        SUM(i.Totpriceorg - i.Priceorg) AS TotalSickPart,
        SUM(i.Priceorg) AS TotalOrganPart
    FROM [Exportdrugdetailtrans] AS i
    GROUP BY i.Exporttransid
)
SELECT 
    inv.ReportDate,
    COUNT(DISTINCT inv.Exporttransid) AS TotalInvoices,
    SUM(item.TotalItems) AS TotalItems,
    SUM(item.TotalDailySales) AS TotalDailySales,
    SUM(item.TotalInsuranceSales) AS TotalInsuranceSales,
    SUM(item.TotalSickPart) AS TotalSickPart,
    SUM(item.TotalOrganPart) AS TotalOrganPart,
    SUM(ISNULL(inv.PrefExchangePrice, 0)) AS TotalExchangePrice,
    SUM(ISNULL(inv.Workmoney, 0)) AS TotalPharmacyServiceFee,
    SUM(ISNULL(inv.Payableprice, 0)) AS TotalPayablePrice,
    SUM(ISNULL(inv.Paidprice, 0)) AS TotalPaidPrice
FROM InvoiceSummary inv
INNER JOIN ItemSummary item ON inv.Exporttransid = item.Exporttransid
GROUP BY inv.ReportDate
ORDER BY inv.ReportDate DESC;
            ";

            using var connection = _dbConnection;
            var result = await connection.QueryAsync<DailySalesReportViewModel>(
                new CommandDefinition(sql,
                    new { Year = year, Month = month },
                    cancellationToken: cancellationToken));

            return result.AsList();
        }
    }
}
