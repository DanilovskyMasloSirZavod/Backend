using LinqToDB.Mapping;

namespace DMSZ_API.Data.Sales
{
    /// <summary>
    /// Точка продаж.
    /// </summary>
    [Table]
    public class PointOfSale
    {
        [PrimaryKey, Identity]
        public Guid ID { get; set; }

        [Column, Nullable]
        public string Addres { get; set; }

        [Column, Nullable]
        public string City { get; set; }
    }

    /// <summary>
    /// Связь многие ко многим.
    /// </summary>
    [Table]
    public class EmployeePointMap
    {
        #region Сотрудник.

        /// <summary>
        /// Id сотрудника.
        /// </summary>
        [PrimaryKey, NotNull]
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
        [PrimaryKey, NotNull]
        public Guid PointId { get; set; }
        /// <summary>
        /// Точка.
        /// </summary>
        [Association(ThisKey = nameof(PointId), OtherKey = nameof(Data.Sales.PointOfSale.ID))]
        public PointOfSale PointOfSale { get; set; }

        #endregion

        [NotColumn]
        public List<Document> Documents { get; set; }
    }
}
