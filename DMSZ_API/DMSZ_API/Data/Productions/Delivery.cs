using LinqToDB.Mapping;

namespace DMSZ_API.Data.Productions
{
    /// <summary>
    /// Доставка.
    /// </summary>
    [Table]
    public class Delivery
    {
        /// <summary>
        /// Id партии, которую доставили.
        /// </summary>
        [PrimaryKey]
        public string BatchNumber { get; set; }

        #region Поставщик.

        /// <summary>
        /// Название компании или поставщика.
        /// </summary>
        [Column]
        public Guid ProviderId { get; set; }
        [Nullable]
        private Provider _Provider;
        /// <summary>
        /// Название компании или поставщика.
        /// </summary>
        [Association(Storage = nameof(_Provider), ThisKey = nameof(ProviderId), OtherKey = nameof(Productions.Provider.Id))]
        public Provider Provider
        {
            get { return _Provider; }
            set { _Provider = value; }
        }

        #endregion

        /// <summary>
        /// Дата поставки.
        /// </summary>
        [Column]
        public DateTime Date { get; set; }

        /// <summary>
        /// Вес.
        /// </summary>
        [Column]
        public float Weight { get; set; }

        /// <summary>
        /// Изобретенный продукт из данной партии.
        /// </summary>
        [NotColumn]
        public Product CreatedProduct { get; set; }
    }

    /// <summary>
    /// Поставщик.
    /// </summary>
    [Table]
    public class Provider
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [Column]
        public string ProviderName { get; set; }
        [Column]
        public DateTime DateOfCoop { get; set; }
    }


    /// <summary>
    /// Связь многие ко многим, т.к. у некоторых доставок может быть несколько людей, котрые принимают.
    /// </summary>
    [Table]
    public class DeliveryReceiverEmployeeMap
    {
        #region Приёщик.

        /// <summary>
        /// Id приёмщика.
        /// </summary>
        [PrimaryKey, NotNull]
        public Guid ReceiverEmployeeId { get; set; }
        /// <summary>
        /// Приёмщик.
        /// </summary>
        [Association(ThisKey = nameof(ReceiverEmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee { get; set; }

        #endregion

        #region Привезённая партия.

        /// <summary>
        /// Номер привезённой партии.
        /// </summary>
        [PrimaryKey, NotNull]
        public string DeliveryBatchNumber { get; set; }
        /// <summary>
        /// Привезённая партия.
        /// </summary>
        [Association(ThisKey = nameof(DeliveryBatchNumber), OtherKey = nameof(Productions.Delivery.BatchNumber))]
        public Delivery Delivery { get; set; }

        #endregion
    }

}
