using LinqToDB.Mapping;
using static LinqToDB.Common.Configuration;
using System.Numerics;

namespace DMSZ_API.Data
{
    /// <summary>
    /// Событие на день.
    /// </summary>
    public enum Event : int
    {
        /// <summary>
        /// Рабочий день.
        /// </summary>
        Work,
        /// <summary>
        /// Выходной день.
        /// </summary>
        DayOff,
        /// <summary>
        /// Больничный.
        /// </summary>
        Sick,
        /// <summary>
        /// Отсутствовал.
        /// </summary>
        Absent
    }

    /// <summary>
    /// Текущее расписание, которое отличается от расписания по плану.
    /// Используется для того, чтобы определять сколько было прогулов.
    /// Влияет на зарплату, а также помогает определить, кто сейчас на работе.
    /// </summary>
    [Table]
    public class Schedule
    {
        /// <summary>
        /// Id, расписания на текущий момент.
        /// </summary>
        [PrimaryKey, Identity]
        public Guid Id { get; set; }

        #region Расписание.

        /// <summary>
        /// Текущее состояние по графику.
        /// </summary>
        [Column, NotNull]
        public Event Current { get; set; }

        /// <summary>
        /// Запланированное состояние по графику.
        /// </summary>
        [Column, NotNull]
        public Event Planned { get; set; }

        #endregion

        /// <summary>
        /// Дата дня.
        /// </summary>
        [Column, NotNull]
        public DateTime Date { get; set; }

        public override string ToString() => $"ID: {Id}\nCurrent: {Consts.EventsRussian[Current]}\nPlanned: {Consts.EventsRussian[Planned]}\nDate: {Date}";
    }

    /// <summary>
    /// Связь многие ко многим, т.к. у некоторых дней может быть несколько людей.
    /// </summary>
    [Table]
    public class ScheduleEmployeeMap
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

        #region Текущий график сотрудника.

        /// <summary>
        /// Id текущего графика у сотрудника.
        /// </summary>
        [PrimaryKey, NotNull]
        public Guid ScheduleId { get; set; }
        /// <summary>
        /// Текущий график у сотрудника.
        /// </summary>
        [Association(ThisKey = nameof(ScheduleId), OtherKey = nameof(Data.Schedule.Id))]
        public Schedule Schedule { get; set; }

        #endregion

        public override string ToString() => $"EmployeeId: {EmployeeId}\nScheduleId: {ScheduleId}" +
            $"\nEmployee: {Employee}\nSchedule: {Schedule}";
    }
}