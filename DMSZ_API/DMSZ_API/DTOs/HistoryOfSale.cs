using DMSZ_API.Data;
using DMSZ_API.Data.Sales;
using System.Diagnostics.CodeAnalysis;

namespace DMSZ_API.DTOs
{

    /// <summary>
    /// История одной продажи одной точки.
    /// </summary>
    public class HistoryOfSale
    {
        public HistoryOfSale(List<CheckPointMap> checksPointMap)
        {
            if (checksPointMap == null || !checksPointMap.Any()) 
                return;

            CheckId = checksPointMap.FirstOrDefault().CheckId;
            Employee = checksPointMap.FirstOrDefault().Employee;
            DateOfSell = checksPointMap.FirstOrDefault().Check.Date;
            PointOfSale = checksPointMap.FirstOrDefault().PointOfSale;
            IsCancel = checksPointMap.FirstOrDefault().Check.IsCancel;

            SoldItems = new();

            checksPointMap.ForEach(check =>
            {
                SoldItems.Add(new SoldItem(check.Check.SellingProduct));
            });
        }

        /// <summary>
        /// Id чека.
        /// </summary>
        public Guid CheckId { get; set; }

        /// <summary>
        /// Точка продажи.
        /// </summary>
        public PointOfSale PointOfSale { get; set; }

        /// <summary>
        /// Список проданых товаров.
        /// </summary>
        public List<SoldItem> SoldItems { get; set; }
        
        /// <summary>
        /// Тип операции (True -> отмена, False -> продажа)
        /// </summary>
        public bool IsCancel { get; set; }

        #region Сумма проданых товаров

        private decimal _sum = 0;
        
        /// <summary>
        /// Сумма проданых товаров.
        /// </summary>
        public decimal Sum 
        {
            get
            {
                return _sum;
            }
            set
            {        
                _sum = IsCancel ? -SoldItems.Sum(x => (x.Price * (decimal)x.Weight)) : SoldItems.Sum(x => (x.Price * (decimal)x.Weight));
            }
        }

        #endregion

        /// <summary>
        /// Продавец.
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Дата продажи.
        /// </summary>
        public DateTime DateOfSell { get; set; }
     
    }


    public class HistoryOfSaleComparator : IEqualityComparer<HistoryOfSale>
    {
        public bool Equals(HistoryOfSale? x, HistoryOfSale? y)
        {
            return x.CheckId.Equals(y.CheckId);
        }

        public int GetHashCode([DisallowNull] HistoryOfSale obj)
        {
            return obj.CheckId.GetHashCode();
        }
    }
}
