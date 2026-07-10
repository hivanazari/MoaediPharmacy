namespace MoaediPharamcy.ViewModels
{
    /// <summary>
    /// ViewModel برای گزارش فروش روزانه
    /// شامل خلاصه فروش، سهم بیمار، سهم سازمان و مبلغ قابل پرداخت
    /// </summary>
    public class DailySalesReportViewModel
    {
        /// <summary>
        /// تاریخ گزارش
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// تعداد فاکتورهای پردازش‌شده در این روز
        /// </summary>
        public int TotalInvoices { get; set; }

        /// <summary>
        /// تعداد اقلام خریداری‌شده (Drug Items)
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// مجموع فروش روزانه (جمع تمام اقلام)
        /// = SUM(i.Totprice)
        /// </summary>
        public decimal TotalDailySales { get; set; }

        /// <summary>
        /// مجموع فروش بیمه‌ای روزانه
        /// = SUM(i.Totpriceorg)
        /// </summary>
        public decimal TotalInsuranceSales { get; set; }

        /// <summary>
        /// مجموع سهم بیمار روزانه
        /// = SUM((i.Totpriceorg - i.Priceorg))
        /// </summary>
        public decimal TotalSickPart { get; set; }

        /// <summary>
        /// مجموع سهم سازمان روزانه
        /// = SUM(i.Priceorg)
        /// </summary>
        public decimal TotalOrganPart { get; set; }

        /// <summary>
        /// مجموع سهم ارز
        /// = SUM(e.PrefExchangePrice)
        /// </summary>
        public decimal TotalExchangePrice { get; set; }

        /// <summary>
        /// مجموع تعرفه خدمات دارویی
        /// = SUM(e.Workmoney)
        /// </summary>
        public decimal TotalPharmacyServiceFee { get; set; }

        /// <summary>
        /// مجموع مبلغ قابل پرداخت روزانه
        /// = SUM(e.Payableprice)
        /// </summary>
        public decimal TotalPayablePrice { get; set; }

        /// <summary>
        /// مجموع مبلغ پرداخت‌شده
        /// = SUM(e.Paidprice)
        /// </summary>
        public decimal TotalPaidPrice { get; set; }

        /// <summary>
        /// مجموع مبلغ درخواست‌شده (معوق)
        /// = TotalPayablePrice - TotalPaidPrice
        /// </summary>
        public decimal OutstandingAmount => TotalPayablePrice - TotalPaidPrice;

        /// <summary>
        /// مجموع فروش غیر بیمه‌ای روزانه
        /// = TotalDailySales - TotalInsuranceSales
        /// </summary>
        public decimal TotalNonInsuranceSales => TotalDailySales - TotalInsuranceSales;

        /// <summary>
        /// درصد فروش بیمه‌ای
        /// </summary>
        public decimal InsuranceSalesPercentage => TotalDailySales > 0 
            ? (TotalInsuranceSales / TotalDailySales) * 100 
            : 0;

        /// <summary>
        /// درصد فروش غیر بیمه‌ای
        /// </summary>
        public decimal NonInsuranceSalesPercentage => TotalDailySales > 0 
            ? (TotalNonInsuranceSales / TotalDailySales) * 100 
            : 0;

        /// <summary>
        /// میانگین فروش به ازای هر فاکتور
        /// </summary>
        public decimal AverageSalesPerInvoice => TotalInvoices > 0 
            ? TotalDailySales / TotalInvoices 
            : 0;

        /// <summary>
        /// میانگین مبلغ قابل پرداخت به ازای هر فاکتور
        /// </summary>
        public decimal AveragePayablePerInvoice => TotalInvoices > 0 
            ? TotalPayablePrice / TotalInvoices 
            : 0;

        /// <summary>
        /// نسبت جمع‌آوری (Collection Ratio)
        /// = (TotalPaidPrice / TotalPayablePrice) * 100
        /// </summary>
        public decimal CollectionRatio => TotalPayablePrice > 0 
            ? (TotalPaidPrice / TotalPayablePrice) * 100 
            : 0;
    }
}
