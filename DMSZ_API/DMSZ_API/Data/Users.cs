using LinqToDB.Mapping;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Пользователь (логин и пароль).
    /// </summary>
    public class Users
    {
        /// <summary>
        /// Логин пользователя.
        /// </summary>
        [PrimaryKey, Identity]
        public string Login { get; set; }

        [Column, NotNull]
        public string Password { get; set; }

        [NotColumn]
        public string Role { get; set; }

        #region Сотрудник.

        /// <summary>
        /// Пользователь с логином и паролем.
        /// </summary>
        [Column]
        public Guid EmployeeId { get; set; }
        [NotNull]
        private Employee _EmployeeId;
        /// <summary>
        /// Пользователь связанный логин и паролем.
        /// </summary>
        [Association(Storage = nameof(_EmployeeId), ThisKey = nameof(EmployeeId), OtherKey = nameof(Data.Employee.Id))]
        public Employee Employee
        {
            get { return this._EmployeeId; }
            set { this._EmployeeId = value; }
        }

        #endregion
    } 
}
