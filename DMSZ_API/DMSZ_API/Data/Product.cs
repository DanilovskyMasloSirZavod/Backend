using LinqToDB.Mapping;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Продукт.
    /// [Связи]
    /// Привязан к цехам (Product -> Exact).
    /// </summary>
    [Table]
    public class Product
    {
        /// <summary>
        /// Id продукта.
        /// </summary>
        [PrimaryKey, Identity]
        public Guid Id { get; set; }

        #region Конкретный цех, где производят продукт.

        /// <summary>
        /// Id цеха, где производится.
        /// </summary>
        [Column]
        public Int32 ExactId { get; set; }
        [Nullable]
        private Exact _Exact;
        /// <summary>
        /// Цех, где производится продукт.
        /// </summary>
        [Association(Storage = nameof(_Exact), ThisKey = nameof(ExactId), OtherKey = nameof(Data.Exact.Id))]
        public Exact Exact
        {
            get { return this._Exact; }
            set { this._Exact = value; }
        }

        #endregion

        /// <summary>
        /// Название продукта.
        /// </summary>
        [Column, Nullable]
        public string ProductName { get; set; }

        /// <summary>
        /// Формула для расчета выхода продукта.
        /// </summary>
        [Column, Nullable]
        public string Formula { get; set; }
    }

    #region Производство.

    /// <summary>
    /// Многие ко многим связь потерь и продукта.
    /// </summary>
    [Table]
    public class ProductPossibleLossesMap
    {
        #region Продукт.

        /// <summary>
        /// Id продукт.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid ProductId { get; set; }
        /// <summary>
        /// Продукт.
        /// </summary>
        [Association(ThisKey = nameof(ProductId), OtherKey = nameof(Data.Product.Id))]
        public Product Product { get; set; }

        #endregion

        #region Потеря.

        /// <summary>
        /// Id потерь.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid PossibleLossId { get; set; }
        /// <summary>
        /// Потеря.
        /// </summary>
        [Association(ThisKey = nameof(PossibleLossId), OtherKey = nameof(Data.PossibleLosses.ID))]
        public PossibleLosses PossibleLosses { get; set; }

        #endregion
    }

    /// <summary>
    /// Потери.
    /// </summary>
    [Table]
    public class PossibleLosses
    {
        [PrimaryKey, Identity, Column]
        public Guid ID { get; set; }

        /// <summary>
        /// Месяц.
        /// </summary>
        [Column, NotNull]
        public int Month { get; set; }

        /// <summary>
        /// Значение потерь для месяца.
        /// </summary>
        [Column, NotNull]
        public float LossValue { get; set; }

        [Column]
        public bool IsDone { get; set; }
    }

    #endregion
}
