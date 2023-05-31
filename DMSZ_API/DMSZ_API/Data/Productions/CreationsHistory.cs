using LinqToDB.Mapping;

namespace DMSZ_API.Data.Productions
{
    /// <summary>
    /// История произведённых продуктов.
    /// </summary>
    [Table]
    public class CreationsHistory
    {
        /// <summary>
        /// Номер смены.
        /// </summary>
        [Column, PrimaryKey]
        public string ShiftNumber { get; set; }

        private CheeseStorageProcess _CheeseStorageProcess;
        [Association(Storage = nameof(_CheeseStorageProcess), ThisKey = nameof(ShiftNumber), OtherKey = nameof(Productions.CheeseStorageProcess.BatchUniqueId))]
        public CheeseStorageProcess CheeseStorageProcess
        {
            get { return _CheeseStorageProcess; }
            set { _CheeseStorageProcess = value; }
        }

        /// <summary>
        /// Номер привезённой партии из которой произвели продукт.
        /// </summary>
        [Column]
        public string BatchNumber { get; set; }

        /// <summary>
        /// Теоретический вес, который посчитала программа.
        /// </summary>
        [Column]
        public float TheoreticalWeightResult { get; set; }

        /// <summary>
        /// Практический вес, который получился.
        /// </summary>
        [Column]
        public float PracticalWeightResult { get; set; }

        /// <summary>
        /// Вес сколько было потрачено сырья.
        /// </summary>
        [Column]
        public float UsedRaw { get; set; }

        /// <summary>
        /// Дата производства.
        /// </summary>
        [Column]
        public DateTime Date { get; set; }

        /// <summary>
        /// Совпадение или нет.
        /// </summary>
        [NotColumn]
        public bool IsOk { get; set; }

        #region Созданный продукт.

        /// <summary>
        /// Созданный продукт.
        /// </summary>
        [Column]
        public Guid CreatedProduct { get; set; }
        [Nullable]
        private Product _Product;
        /// <summary>
        /// Созданный продукт.
        /// </summary>
        [Association(Storage = nameof(_Product), ThisKey = nameof(CreatedProduct), OtherKey = nameof(Data.Product.Id))]
        public Product Product
        {
            get { return _Product; }
            set { _Product = value; }
        }

        #endregion
    }

    /// <summary>
    /// Связь многие ко многим, т.к. над некоторыми партиями может работать несколько людей.
    /// </summary>
    [Table]
    public class CreatorEmpolyeeMap
    {
        #region Рабочий.

        /// <summary>
        /// Id рабочего.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public Guid CreatorEmployeeId { get; set; }
        /// <summary>
        /// Рабочий.
        /// </summary>
        [Association(ThisKey = nameof(CreatorEmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee { get; set; }

        #endregion

        #region Созданная партия.

        /// <summary>
        /// Номер партии.
        /// </summary>
        [PrimaryKey, NotNull, Column]
        public string ShiftNumberId { get; set; }
        /// <summary>
        /// Созданная партия.
        /// </summary>
        [Association(ThisKey = nameof(ShiftNumberId), OtherKey = nameof(Productions.CreationsHistory.BatchNumber))]
        public CreationsHistory CreationsHistory { get; set; }

        #endregion
    }

}
