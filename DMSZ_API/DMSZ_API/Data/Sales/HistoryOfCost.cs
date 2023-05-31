using LinqToDB.Mapping;

namespace DMSZ_API.Data.Sales
{
    /// <summary>
    /// История цен (в ней есть рекомендации).
    /// </summary>
    [Table]
    public class HistoryOfCost
    {
        [Identity, Column, PrimaryKey]
        public Guid ID { get; set; }

        #region Продукт.

        /// <summary>
        /// Id продукт.
        /// </summary>
        [Nullable, Column]
        public Guid? ProductId { get; set; }
        /// <summary>
        /// Продукт.
        /// </summary>
        [Association(ThisKey = nameof(ProductId), OtherKey = nameof(Data.Product.Id))]
        public Product Product { get; set; }

        #endregion

        /// <summary>
        /// Цена.
        /// </summary>
        [Column, Nullable]
        public decimal? Cost { get; set; }

        [Column, Nullable]
        public DateTime? Date { get; set; }

        #region Статус.

        /// <summary>
        /// Id статуса.
        /// </summary>
        [Nullable, Column]
        public Guid? Status { get; set; }
        /// <summary>
        /// Статус.
        /// </summary>
        [Association(ThisKey = nameof(Status), OtherKey = nameof(Sales.RecommedStates.ID))]
        public RecommedStates RecommedStates { get; set; }

        #endregion

        /// <summary>
        /// Рекомендованная цена.
        /// </summary>
        [Nullable, Column]
        public decimal? RecommendedCost { get; set; }

        #region Точка.

        /// <summary>
        /// Id точки.
        /// </summary>
        [Nullable, Column]
        public Guid? PointId { get; set; }
        /// <summary>
        /// Точка.
        /// </summary>
        [Association(ThisKey = nameof(PointId), OtherKey = nameof(Sales.PointOfSale.ID))]
        public PointOfSale PointOfSale { get; set; }

        #endregion
    }

    /// <summary>
    /// Статус.
    /// </summary>
    [Table]
    public class RecommedStates
    {
        [PrimaryKey, Identity]
        public Guid ID { get; set; }

        /// <summary>
        /// Тип статуса.
        /// </summary>
        [Column, Nullable]
        public string StatusType { get; set; }
    }
}
