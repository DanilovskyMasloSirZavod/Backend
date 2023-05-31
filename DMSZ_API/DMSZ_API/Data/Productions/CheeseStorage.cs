using LinqToDB.Mapping;

namespace DMSZ_API.Data.Productions
{
    /// <summary>
    /// История списаний.
    /// </summary>
    [Table]
    public class HistoryWriteOff
    {
        [PrimaryKey, Identity, NotNull, Column]
        public Guid ID { get; set; }

        #region Ответственный человек.

        /// <summary>
        /// Ответственный человек id.
        /// </summary>
        [Column]
        public Guid Responsible { get; set; }
        [NotNull]
        private Employee _Employee;
        /// <summary>
        /// Ответственный человек.
        /// </summary>
        [Association(Storage = nameof(_Employee), ThisKey = nameof(Responsible), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee
        {
            get { return _Employee; }
            set { _Employee = value; }
        }

        #endregion

        #region Процесс сырохранилища.

        /// <summary>
        /// Процесс сырохранилища id.
        /// </summary>
        [Column]
        public string BatchNumber { get; set; }
        [NotNull]
        private CheeseStorageProcess _CheeseStorageProcess;
        /// <summary>
        /// Процесс сырохранилища.
        /// </summary>
        [Association(Storage = nameof(_CheeseStorageProcess), ThisKey = nameof(BatchNumber), OtherKey = nameof(Productions.CheeseStorageProcess.BatchUniqueId))]
        public CheeseStorageProcess CheeseStorageProcess
        {
            get { return _CheeseStorageProcess; }
            set { _CheeseStorageProcess = value; }
        }

        #endregion

        /// <summary>
        /// Дата, когда были внесены данные о потерях продукта.
        /// </summary>
        [NotNull, Column]
        public DateTime Date { get; set; }

        /// <summary>
        /// Значение новых потерь.
        /// </summary>
        [NotNull, Column]
        public float Value { get; set; }

        /// <summary>
        /// True -> готово,
        /// False -> созревает.
        /// </summary>
        [NotColumn]
        public bool IsDone { get; set; }


    }

    /// <summary>
    /// Процессы на сырохранилище.
    /// </summary>
    [Table]
    public class CheeseStorageProcess
    {
        [PrimaryKey, Identity, Column, NotNull]
        public string BatchUniqueId { get; set; }

        private CreationsHistory _CreationsHistory;
        [Association(Storage = nameof(_CreationsHistory), ThisKey = nameof(BatchUniqueId), OtherKey = nameof(Productions.CreationsHistory.ShiftNumber))]
        public CreationsHistory CreationsHistory
        {
            get { return _CreationsHistory; }
            set { _CreationsHistory = value; }
        }

        /// <summary>
        /// Id продукта.
        /// </summary>
        [Column]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Дата получения продукции.
        /// </summary>
        [Column, NotNull]
        public DateTime DateOfRecv { get; set; }

        /// <summary>
        /// Дата готовности созревания
        /// </summary>
        [NotColumn]
        public DateTime MiddleDate { get; set; }

        /// <summary>
        /// Дата готовности во вкладке готово.
        /// </summary>
        [Column, NotNull]
        public DateTime DateOfDate { get; set; }

        /// <summary>
        /// Масса полученного продукта (в тоннах).
        /// </summary>
        [Column, NotNull]
        public float MassWhenRecv { get; set; }

        /// <summary>
        /// Масса продукта на выходе с учетом вычитаний (созревание).
        /// </summary>
        [Column]
        public float ResultMass { get; set; }

        /// <summary>
        /// Общие потери.
        /// </summary>
        [NotColumn]
        public float SummedLoss { get; set; }

        /// <summary>
        /// Допустимые потери на текущий момент.
        /// </summary>
        [NotColumn]
        public float PossibleNowLoss { get; set; }

        /// <summary>
        /// True -> готово,
        /// False -> созревает.
        /// </summary>
        [NotColumn]
        public bool IsDone { get; set; }

        /// <summary>
        /// Сумма потерь готового продукта.
        /// </summary>
        [NotColumn]
        public float SummedLossOfDone { get; set; }

        /// <summary>
        /// Масса продукта на выходе с учетом вычитаний (готово).
        /// </summary>
        [NotColumn]
        public float ResultMassOfDone { get; set; }
    }

}
