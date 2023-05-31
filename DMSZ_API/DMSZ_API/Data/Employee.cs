using DMSZ_API.Data.Sales;
using LinqToDB.Mapping;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Сотрудники.
    /// [Связи]
    /// У одного человека, много документов.
    /// У многих сотрудников, много разных текущих графиков работы.
    /// </summary>
    [Table]
    public class Employee
    {
        /// <summary>
        /// Id сотрудника.
        /// </summary>
        [PrimaryKey, Identity]
        public Guid Id { get; set; }

        /// <summary>
        /// Имя.
        /// </summary>
        [Column]
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// Фамилия.
        /// </summary>
        [Column]
        [NotNull]
        public string Surname { get; set; }

        /// <summary>
        /// Отчество.
        /// </summary>
        [Column]
        [Nullable]
        public string Patronymic { get; set; }

        /// <summary>
        /// Номер телефона.
        /// </summary>
        [Column]
        [Nullable]
        public string Phone { get; set; }

        /// <summary>
        /// Дата принятия на работу.
        /// </summary>
        [Column]
        public DateTime DateOfEmployment { get; set; }

        #region Информация должности.

        /// <summary>
        /// Id информации должности.
        /// </summary>
        [Column]
        public Guid IdJob { get; set; }
        [Nullable]
        private Job _Job;
        /// <summary>
        /// Информация должности.
        /// </summary>
        [Association(Storage = nameof(_Job), ThisKey = nameof(IdJob), OtherKey = nameof(Data.Job.Id))]
        public Job Job
        {
            get { return this._Job; }
            set { this._Job = value; }
        }

        #endregion

    }

}
