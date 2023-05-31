using DMSZ_API.Data;
using DMSZ_API.Data.Productions;
using DMSZ_API.Data.Sales;
using LinqToDB;

namespace DMSZ_API.Connection
{
    /// <summary>
    /// Аггрегатор таблиц.
    /// </summary>
    public class DatabaseAggregator : LinqToDB.Data.DataConnection
    {
        /// <summary>
        /// Конструктор, для создания таблиц и их связей.
        /// </summary>
        /// <param name="connectionString">Строка, для подключения к бд.</param>
        public DatabaseAggregator(string connectionString) :
            base(ProviderName.SqlServer2022, connectionString)
        {
        }

        // Таблицы:

        #region Общие таблицы.

        /// <summary>
        /// Таблица документов.
        /// </summary>
        public ITable<Document> Documents => this.GetTable<Document>();

        /// <summary>
        /// Таблица сотрудников.
        /// </summary>
        public ITable<Employee> Employees => this.GetTable<Employee>();

        /// <summary>
        /// Таблица информации о должности.
        /// </summary>
        public ITable<Job> JobInfo => this.GetTable<Job>();

        /// <summary>
        /// Таблица названий занимаемых позиций.
        /// </summary>
        public ITable<Position> JobPosition => this.GetTable<Position>();

        /// <summary>
        /// Таблица места работы.
        /// </summary>
        public ITable<Place> JobPlace => this.GetTable<Place>();

        /// <summary>
        /// Таблица описания работы.
        /// </summary>
        public ITable<Exact> JobExact => this.GetTable<Exact>();

        /// <summary>
        /// Таблица продуктов, производимых на производстве.
        /// </summary>
        public ITable<Product> Products => this.GetTable<Product>();

        /// <summary>
        /// Таблица графиков работы (для проверки отклонений от графика).
        /// </summary>
        public ITable<Schedule> Schedules => this.GetTable<Schedule>();

        /// <summary>
        /// Вспомогательная таблица для связи многие,
        /// ко многим, которая объединяет график и рабочих.
        /// </summary>
        public ITable<ScheduleEmployeeMap> ScheduleEmployeeMap => this.GetTable<ScheduleEmployeeMap>();

        /// <summary>
        /// Таблица пользователей (логины и пароли)
        /// </summary>
        public ITable<Users> Users => this.GetTable<Users>();

        #endregion

        #region Таблицы производства.

        /// <summary>
        /// Таблица истории производства.
        /// </summary>
        public ITable<CreationsHistory> CreationsHistory => this.GetTable<CreationsHistory>();

        /// <summary>
        /// Вспомогательная таблица для связи многие,
        /// ко многим, которая объединяет партии и рабочих.
        /// </summary>
        public ITable<CreatorEmpolyeeMap> CreatorEmpolyeeMap => this.GetTable<CreatorEmpolyeeMap>();

        /// <summary>
        /// Таблица поставок.
        /// </summary>
        public ITable<Delivery> Delivery => this.GetTable<Delivery>();

        /// <summary>
        /// Вспомогательная таблица для связи многие,
        /// ко многим, которая объединяет поставки и приёмщиков.
        /// </summary>
        public ITable<DeliveryReceiverEmployeeMap> DeliveryReceiverEmployeeMap => this.GetTable<DeliveryReceiverEmployeeMap>();

        /// <summary>
        /// Таблица исследований лабаратории, которая проверяет качество.
        /// </summary>
        public ITable<Lab> LabResearches => this.GetTable<Lab>();

        /// <summary>
        /// Таблица многие ко многим связь потерь и продукта.
        /// </summary>
        public ITable<ProductPossibleLossesMap> ProductPossibleLossesMap => this.GetTable<ProductPossibleLossesMap>();

        /// <summary>
        /// Таблица потерь.
        /// </summary>
        public ITable<PossibleLosses> PossibleLosses => this.GetTable<PossibleLosses>();

        /// <summary>
        /// Таблица процесса на сырохранилище.
        /// </summary>
        public ITable<CheeseStorageProcess> CheeseStorageProcess => this.GetTable<CheeseStorageProcess>();

        /// <summary>
        /// Таблица истории списания товара.
        /// </summary>
        public ITable<HistoryWriteOff> HistoryWriteOff => this.GetTable<HistoryWriteOff>();

        /// <summary>
        /// Таблица поставщиков.
        /// </summary>
        public ITable<Provider> Provider => this.GetTable<Provider>();

        #endregion

        #region Таблицы продаж.

        /// <summary>
        /// Таблица точек продаж.
        /// </summary>
        public ITable<PointOfSale> PointsOfSales => this.GetTable<PointOfSale>();

        /// <summary>
        /// Таблица многие ко многим EmployeePointMap.
        /// </summary>
        public ITable<EmployeePointMap> EmployeePointMap => this.GetTable<EmployeePointMap>();

        /// <summary>
        /// Таблица продаваемых товаров.
        /// </summary>
        public ITable<SellingProduct> SellingProducts => this.GetTable<SellingProduct>();

        /// <summary>
        /// Таблица чеков.
        /// </summary>
        public ITable<Check> Checks => this.GetTable<Check>();

        /// <summary>
        /// Таблица многие ко многим CheckPointMap.
        /// </summary>
        public ITable<CheckPointMap> CheckPointMap => this.GetTable<CheckPointMap>();

        /// <summary>
        /// Таблица истории цен.
        /// </summary>
        public ITable<HistoryOfCost> HistoryOfCosts => this.GetTable<HistoryOfCost>();

        /// <summary>
        /// Таблица состояний рекомендаций.
        /// </summary>
        public ITable<RecommedStates> RecommedStates => this.GetTable<RecommedStates>();
        #endregion
    }
}