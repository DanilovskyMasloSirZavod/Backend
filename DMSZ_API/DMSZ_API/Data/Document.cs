using DMSZ_API.Data.Sales;
using LinqToDB.Mapping;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Документ.
    /// </summary>
    [Table]
    public class Document
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// Файл.
        /// </summary>
        [Column, NotNull]
        public byte[] File { get; set; }

        #region Сотрудник у которого этот файл.

        /// <summary>
        /// Id сотрудника.
        /// </summary>
        [Column, Nullable]
        public Guid? EmployeeId { get; set; }
        [Nullable]
        private Employee _Employee;
        /// <summary>
        /// Занимаемая позиция.
        /// </summary>
        [Association(Storage = nameof(_Employee), ThisKey = nameof(EmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee
        {
            get { return this._Employee; }
            set { this._Employee = value; }
        }

        #endregion

        /// <summary>
        /// Название файла.
        /// </summary>
        [Column, NotNull]
        public string FileName { get; set; }

        #region Точка продаж.

        /// <summary>
        /// Id точки.
        /// </summary>
        [Column, Nullable]
        public Guid? PointId { get; set; }
        [Nullable]
        private PointOfSale _PointOfSale;
        /// <summary>
        /// Точка.
        /// </summary>
        [Association(Storage = nameof(_PointOfSale), ThisKey = nameof(PointId), OtherKey = nameof(Data.Sales.PointOfSale.ID))]
        public PointOfSale PointOfSale
        {
            get { return this._PointOfSale; }
            set { this._PointOfSale = value; }
        }

        #endregion
    }
}
