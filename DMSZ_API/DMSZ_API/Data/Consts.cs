namespace DMSZ_API.Data
{
    /// <summary>
    /// Содержит все константы в проекте.
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// Переводчик событий на день с английского на русский.
        /// </summary>
        public static Dictionary<Event, string> EventsRussian = new()
        {
            { Event.Work, "Рабочий" },
            { Event.DayOff, "Выходной" },
            { Event.Sick, "Больничный" },
            { Event.Absent, "Отсутствовал" }
        };

        /// <summary>
        /// Строка подключения к базе данных.
        /// </summary>
        public static string ConnectionString = "Data Source = localhost\\SQLEXPRESS; Initial Catalog = DMSZ; trusted_connection=true;TrustServerCertificate=True;";
    }
}
