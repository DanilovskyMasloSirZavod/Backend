using LinqToDB.Mapping;

namespace DMSZ_API.Data.Sales
{
    [Table]
    public class Check
    {
        [PrimaryKey, Identity, Column]
        public Guid ID { get; set; }

        #region Проданный товар.

        /// <summary>
        /// Id товара.
        /// </summary>
        [Column]
        public Guid SellingProductId { get; set; }
        [Nullable]
        private SellingProduct _SellingProduct;
        /// <summary>
        /// Проданный товар.
        /// </summary>
        [Association(Storage = nameof(_SellingProduct), ThisKey = nameof(SellingProductId), OtherKey = nameof(Data.Sales.SellingProduct.ID))]
        public SellingProduct SellingProduct
        {
            get { return this._SellingProduct; }
            set { this._SellingProduct = value; }
        }

        #endregion

        /// <summary>
        /// Дата формирования чека.
        /// </summary>
        [Column]
        public DateTime Date { get; set; }
        
        /// <summary>
        /// True -> продажа
        /// False -> отмена
        /// </summary>
        [Column]
        public bool IsCancel { get; set; }

    }


    /// <summary>
    /// Связть многие ко многим ChecksPointMap
    /// </summary>
    [Table]
    public class CheckPointMap
    {
        #region Сотрудник.

        /// <summary>
        /// Id сотрудника.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid EmployeeId { get; set; }
        /// <summary>
        /// Сотрудник.
        /// </summary>
        [Association(ThisKey = nameof(EmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee { get; set; }

        #endregion

        #region Точка.

        /// <summary>
        /// Id точки.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid PointId { get; set; }
        /// <summary>
        /// Точка.
        /// </summary>
        [Association(ThisKey = nameof(PointId), OtherKey = nameof(Data.Sales.PointOfSale.ID))]
        public PointOfSale PointOfSale { get; set; }

        #endregion

        #region Чек.

        /// <summary>
        /// Id чека.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid CheckId { get; set; }
        /// <summary>
        /// Чек.
        /// </summary>
        [Association(ThisKey = nameof(CheckId), OtherKey = nameof(Data.Sales.Check.ID))]
        public Check Check { get; set; }

        #endregion
    }
}
