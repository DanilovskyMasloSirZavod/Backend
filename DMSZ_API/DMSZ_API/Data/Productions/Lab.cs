using LinqToDB.Mapping;

namespace DMSZ_API.Data.Productions
{
    /// <summary>
    /// Типы сортов.
    /// </summary>
    public enum SortType : int
    {
        /// <summary>
        /// Несортовое
        /// </summary>
        OffGrade,
        /// <summary>
        /// Второй сорт.
        /// </summary>
        SecondGrade,
        /// <summary>
        /// Первый сорт.
        /// </summary>
        FirstGrade,
        /// <summary>
        /// Высший сорт.
        /// </summary>
        HighestGrade
    }

    /// <summary>
    /// Лабаратория, которая проверяет качество.
    /// </summary>
    [Table]
    public class Lab
    {
        /// <summary>
        /// Id, партии.
        /// </summary>
        [PrimaryKey]
        public string BatchNumber { get; set; }

        /// <summary>
        /// Сорт.
        /// </summary>
        [Column]
        public SortType Sort { get; set; }

        [Column]
        public DateTime Date { get; set; }

        /// <summary>
        /// Жирность.
        /// </summary>
        [Column]
        public float Richness { get; set; }

        /// <summary>
        /// Соматика.
        /// </summary>
        [Column]
        public int Somatics { get; set; }

        /// <summary>
        /// Id, ответственного лаборанта. 
        /// </summary>
        [Column]
        public Guid LabEmployeeId { get; set; }
        [Nullable]
        private Employee _Employee;
        /// <summary>
        /// Ответсвенный лаборант.
        /// </summary>
        [Association(Storage = nameof(_Employee), ThisKey = nameof(LabEmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee
        {
            get { return _Employee; }
            set { _Employee = value; }
        }

        #region Выбранный продукт для производства.

        /// <summary>
        /// Id выбранного продукта.
        /// </summary>
        [Column]
        public Guid SelectedProductId { get; set; }
        [Nullable]
        private Product _SelectedProduct;
        /// <summary>
        /// Выбранный продукт.
        /// </summary>
        [Association(Storage = nameof(_SelectedProduct), ThisKey = nameof(SelectedProductId), OtherKey = nameof(Product.Id))]
        public Product SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; }
        }

        #endregion
    }
}
