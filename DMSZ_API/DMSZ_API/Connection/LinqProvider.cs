using DMSZ_API.Data;
using DMSZ_API.Data.Productions;
using DMSZ_API.Data.Sales;
using DMSZ_API.DTOs;
using LinqToDB;

namespace DMSZ_API.Connection
{
    /// <summary>
    /// Использовать LINQ, чтобы обращаться к бд.
    /// </summary>
    public class LinqProvider
    {
        protected string ConnectionString;

        /// <summary>
        /// Конструктор, провайдера LINQ.
        /// </summary>
        public LinqProvider(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #region Общие методы.

        public bool EmployeeInsertFile(Document documentForInsertion)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var insertResl = dataBase.Insert(documentForInsertion);
                return insertResl > 0;
            }
        }

        public bool PointInsertFile(Document documentForInsertion)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var insertResl = dataBase.Insert(documentForInsertion);
                return insertResl > 0;
            }
        }

        /// <summary>
        /// Установить роль пользователю.
        /// </summary>
        /// <param name="userRole">Пользователь с новой ролью.</param>
        /// <returns>True, если успешно обновлено, иначе false.</returns>
        public Users SetRole(UserRole userRole)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var foundPosition = dataBase.JobPosition.FirstOrDefault(x => x.PositionName.Equals(userRole.Role));

                if (foundPosition is null)
                {
                    return null;
                }

                var jobId = dataBase.Employees.FirstOrDefault(empId => empId.Id.Equals(userRole.Id)).IdJob;

                if (jobId.Equals(Guid.Empty))
                {
                    return null;
                }

                var updating = dataBase.JobInfo.Where(i => i.Id.Equals(jobId)).Set(x => x.PositionId, (Int16)foundPosition.Id).Update();

                return updating > 0 ? GetUserById(userRole.Id) : null;
            }
        }

        /// <summary>
        /// Получить все продукты и где они производятся.
        /// </summary>
        /// <returns>Список продуктов.</returns>
        public IEnumerable<Product> GetProducts()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.Products.LoadWith(x => x.Exact).ToList();
            }
        }

        /// <summary>
        /// Получить всех сотрудников и их информацию.
        /// </summary>
        /// <returns>Список сотрудников.</returns>
        public IEnumerable<Employee> GetEmployees()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.Employees
                    .LoadWithAsTable(r1 => r1.Job.Position)
                    .LoadWithAsTable(r2 => r2.Job.Place)
                    .LoadWithAsTable(r3 => r3.Job.Exact).ToList();
            }
        }

        /// <summary>
        /// Получить список график работы сотрудника.
        /// </summary>
        /// <param name="id">ID сотрудника.</param>
        /// <returns>Список дней.</returns>
        public ScheduleEmployeeMap GetSchedule(Guid id)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.ScheduleEmployeeMap
                    .LoadWithAsTable(s => s.Schedule)
                    .LoadWithAsTable(e => e.Employee)
                    .FirstOrDefault(x => x.EmployeeId.Equals(id));
            }
        }

        /// <summary>
        /// Получить список всех документов, которые принадлежат сотруднику.
        /// </summary>
        /// <param name="employeeId">Id сотрудника.</param>
        /// <returns>Список документов.</returns>
        public List<Document> GetDocuments(Guid employeeId)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.Documents.Where(x => x.EmployeeId.Equals(employeeId)).ToList();
            }
        }

        /// <summary>
        /// Получить список данных всех пользователей.
        /// </summary>
        /// <returns>Список пользователей с их данными.</returns>
        public List<Users> GetAllCredentials()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var list = dataBase.Users.LoadWithAsTable(x => x.Employee.Job.Position).ToList();

                list.ForEach(x =>
                {
                    x.Role = x.Employee.Job.Position.PositionName;
                });

                return list;
            }
        }

        public List<string> GetAllRoles()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.JobPosition.Select(x => x.PositionName).ToList();
            }
        }

        public List<string> GetExact()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.JobExact.Select(x => x.Clarification).ToList();
            }
        }

        public List<string> GetPlace()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.JobPlace.Select(x => x.WorkPlace).ToList();
            }
        }

        public Users GetUserById(Guid id)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var obj = dataBase.Users.LoadWithAsTable(x => x.Employee.Job.Position).FirstOrDefault(i => i.EmployeeId.Equals(id));
                obj.Role = obj.Employee.Job.Position.PositionName;

                return obj;
            }
        }

        #endregion

        #region Методы производства.

        /// <summary>
        /// Получить результат лаборатории.
        /// </summary>
        /// <param name="batchNumber">Id партии.</param>
        /// <returns>Результаты лаборатории.</returns>
        public List<Lab> GetLab(string batchNumber)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.LabResearches.Where(x => x.BatchNumber.Equals(batchNumber)).ToList();
            }
        }

        /// <summary>
        /// Получить результат лаборатории.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <returns>Результаты лаборатории.</returns>
        public List<Lab> GetLabDated(DateTime date)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.LabResearches.LoadWithAsTable(x => x.Employee).LoadWithAsTable(x => x.SelectedProduct.Exact)
                    .Where(a => a.Date.Date.Equals(date.Date)).ToList();
            }
        }

        public List<Delivery> GetDeliveryInTimeRange(DateTime from, DateTime to)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.Delivery.Where(x => x.Date.Date <= to.Date && x.Date.Date >= from.Date)
                    .LoadWith(with => with.Provider).OrderBy(ord => ord.Date).ToList();
            }
        }

        public List<DeliveryReceiverEmployeeMap> GetDeliveriesWithRecivers()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var obj = dataBase.Delivery.LoadWithAsTable(x => x.Provider).ToList();

                var loaded = dataBase.DeliveryReceiverEmployeeMap
                    .LoadWithAsTable(prov => prov.Delivery.Provider)
                    .LoadWithAsTable(emp => emp.Employee).ToList();

                var arr = new List<DeliveryReceiverEmployeeMap>();

                foreach (var item in obj)
                {
                    var temp = loaded.FirstOrDefault(x => x.DeliveryBatchNumber.Equals(item.BatchNumber));

                    var newObj = new DeliveryReceiverEmployeeMap()
                    {
                        Delivery = item,
                        DeliveryBatchNumber = item.BatchNumber,
                        Employee = temp is null ? null : temp.Employee,
                        ReceiverEmployeeId = temp is null ? Guid.NewGuid() : temp.ReceiverEmployeeId
                    };


                    newObj.Delivery.CreatedProduct = GetCreationsHistoryByBatchId(newObj.DeliveryBatchNumber)
                                                .Select(prod => prod.Product).FirstOrDefault();
                    arr.Add(newObj);

                }

                return arr;
            }
        }

        public List<CreationsHistory> GetCreationsHistoryByBatchId(string batchId)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.CreationsHistory.LoadWithAsTable(x => x.Product)
                    .Where(b => b.BatchNumber.Equals(batchId)).ToList();
                //.LoadWith(x => x.CreatedProduct).ToList();
            }
        }

        public List<CreationsHistory> GetCreationsHistoryByShiftId(string batchId)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.CreationsHistory.LoadWithAsTable(x => x.Product)
                    .Where(b => b.ShiftNumber.Equals(batchId)).ToList();
                //.LoadWith(x => x.CreatedProduct).ToList();
            }
        }

        public List<CreatorEmpolyeeMap> GetCreationsOfEmpolyee(Guid idOfEmployee)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return GetCreatorsOfProducts().Where(x => x.CreatorEmployeeId.Equals(idOfEmployee)).ToList();
            }
        }

        public List<CreatorEmpolyeeMap> GetCreatorsOfProducts()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var rel = dataBase.CreatorEmpolyeeMap.LoadWithAsTable(cr => cr.CreationsHistory.Product)
                    .LoadWithAsTable(emp => emp.Employee).ToList();

                var hist = GetCreationsHistory();

                rel.ForEach(x =>
                {
                    x.CreationsHistory = hist.Where(h => h.ShiftNumber.Equals(x.ShiftNumberId)).FirstOrDefault();
                });

                return rel;
            }
        }

        public List<CreationsHistory> GetCreationsHistory()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var story = dataBase.CreationsHistory.LoadWithAsTable(x => x.Product.Exact).ToList();
                return story;
            }
        }

        public List<Delivery> GetTableOfDeliveries()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {

                var delv = dataBase.Delivery.LoadWith(with => with.Provider).OrderByDescending(ord => ord.Date).ToList();

                return delv;
            }
        }

        public List<ProductPossibleLossesMap> GetPossibleLossesOfProductById(Guid productId)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.ProductPossibleLossesMap.LoadWithAsTable(prod => prod.Product.Exact)
                    .LoadWithAsTable(o => o.PossibleLosses)
                    .Where(x => x.ProductId.Equals(productId)).ToList();
            }
        }

        /// <summary>
        /// История списания, для получения процесса проходящего в сырохранилище.
        /// </summary>
        /// <returns></returns>
        public List<HistoryWriteOff> GetCheeseStorageProcessFull(bool val)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var resl = dataBase.HistoryWriteOff.LoadWithAsTable(x => x.Employee)
                    .LoadWithAsTable(x => x.CheeseStorageProcess.CreationsHistory.Product).OrderByDescending(d => d.Date).ToList();

                //DateOfRecv --> Дата получения продукции (созревание) * +
                //SummedLoss --> Общие потери (созревание) *
                //ResultMass --> Итого продукта (созревание) * +


                //MiddleDate --> Дата готовности созревания (DateOfRecv.Month + 3) [Дата получения готового продукта] * +
                //PossibleNowLoss --> Допустимые потери * +
                //Value --> Последнее списание * +
                //Date --> Дата последнего списания *
                //Employee --> Ответственный * +
                //BatchNumber --> Партия * +

                //DateOfDate --> Дата готовности (готового) продукта (MiddleDate.Month + 3) * 
                //SummedLossOfDone --> Общие потери (готово) *
                //ResultMassOfDone --> Итого продукта (готово) *


                resl.ForEach(x =>
                {
                    x.CheeseStorageProcess.MiddleDate = x.CheeseStorageProcess.DateOfRecv.AddMonths(3);
                    x.CheeseStorageProcess.DateOfDate = x.CheeseStorageProcess.MiddleDate.AddMonths(3);

                    x.IsDone = x.Date.Date >= x.CheeseStorageProcess.MiddleDate;
                });


                resl.ForEach(x =>
                {
                    x.CheeseStorageProcess.SummedLoss = resl.Where(b => b.BatchNumber.Equals(x.CheeseStorageProcess.BatchUniqueId) && !b.IsDone)
                        .Sum(s => s.Value);
                    x.CheeseStorageProcess.SummedLossOfDone = resl.Where(b => b.BatchNumber.Equals(x.CheeseStorageProcess.BatchUniqueId) && b.IsDone)
                        .Sum(s => s.Value);

                    x.CheeseStorageProcess.ResultMass = x.CheeseStorageProcess.MassWhenRecv - x.CheeseStorageProcess.SummedLoss;
                    x.CheeseStorageProcess.ResultMassOfDone = x.CheeseStorageProcess.ResultMass - x.CheeseStorageProcess.SummedLossOfDone;

                    x.CheeseStorageProcess.PossibleNowLoss = (GetPossibleLossesOfProductById(x.CheeseStorageProcess.ProductId)
                    .FirstOrDefault(i => i.PossibleLosses.Month.Equals(DateTime.Now.Month)
                        && i.PossibleLosses.IsDone.Equals(val)).PossibleLosses.LossValue); //&& i.PossibleLosses.IsDone.Equals(x.IsDone)).PossibleLosses.LossValue);

                });

                return resl.DistinctBy(x => x.CheeseStorageProcess.BatchUniqueId).ToList();
            }
        }

        #endregion

        #region Методы продаж.

        #region Not used

        /// <summary>
        /// Получить список всех точек.
        /// </summary>
        /// <returns>Список точек.</returns>
        public List<PointOfSale> GetPoints()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.PointsOfSales.ToList();
            }
        }

        /// <summary>
        /// Получить данные точки по Id.
        /// </summary>
        /// <returns>Данные точки.</returns>
        public PointOfSale GetPointById(Guid Id)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.PointsOfSales.FirstOrDefault(x => x.ID.Equals(Id));
            }
        }

        /// <summary>
        /// Получить точки по городу.
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public List<PointOfSale> GetPointsByCity(string city)
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.PointsOfSales.Where(x => x.City.Equals(city)).ToList();
            }
        }

        #endregion

        /// <summary>
        /// Получить список точек с полной информацие по ним.
        /// </summary>
        /// <returns></returns>
        public List<EmployeePointMap> GetPointsFullInfo()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var documentsOfPoints = dataBase.Documents.Where(i => i.PointId != Guid.Empty || i.PointId != null).ToList();
                var pointsWithEmployees = dataBase.EmployeePointMap.LoadWithAsTable(x => x.PointOfSale).LoadWithAsTable(emp=>emp.Employee.Job).ToList();

                pointsWithEmployees.ForEach(item =>
                {
                    item.Documents = documentsOfPoints.FindAll(i => i.PointId.Equals(item.PointId)).ToList();
                });

                return pointsWithEmployees;
            }
        }

        /// <summary>
        /// Получить полную историю продаж всех точек. 
        /// </summary>
        /// <returns></returns>
        public List<HistoryOfSale> GetHistoryOfSalesOfPoints()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var history = dataBase.CheckPointMap.LoadWithAsTable(point => point.PointOfSale)
                    .LoadWithAsTable(empl => empl.Employee).LoadWithAsTable(check => check.Check.SellingProduct.Product).ToList();

                var sales = new HashSet<HistoryOfSale>(new HistoryOfSaleComparator());

                history.ForEach(h =>
                {
                    var found = history.Where(x => x.Check.SellingProduct.CartId.Equals(h.Check.SellingProduct.CartId)).ToList();
                    sales.Add(new HistoryOfSale(found));
                });

                return sales.ToList();
            }
        }

        /// <summary>
        /// Получить список истории продаж точки.
        /// </summary>
        /// <param name="id">Id точки.</param>
        /// <returns></returns>
        public List<HistoryOfSale> GetHistoryOfSalesByPointId(Guid id)
        {
            return GetHistoryOfSalesOfPoints().Where(x => x.PointOfSale.ID.Equals(id)).ToList();
        }

        /// <summary>
        /// Получить информацию сотрудника по его id.
        /// </summary>
        /// <param name="id">Id сотрудника.</param>
        /// <returns></returns>
        public EmployeePointMap GetEmployeeInfo(Guid id)
        {
            using(var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.EmployeePointMap.LoadWithAsTable(ej => ej.Employee.Job)
                    .LoadWithAsTable(p => p.PointOfSale).FirstOrDefault(x => x.EmployeeId.Equals(id));
            }
        }

        /// <summary>
        /// Получить список статусов рекомендаций.
        /// </summary>
        /// <returns>Список статусов для рекомендаций.</returns>
        public List<RecommedStates> GetStates()
        {
            using (var dataBase = new DatabaseAggregator(ConnectionString))
            {
                return dataBase.RecommedStates.ToList();
            }
        }

        /// <summary>
        /// Получить список истории цен, также узнать что обновилось для рекомендаций.
        /// </summary>
        /// <returns>Список истории цен с рекомендациями.</returns>
        public List<HistoryOfCost> GetHistoryOfCost()
        {
            using( var dataBase = new DatabaseAggregator(ConnectionString))
            {
                var arr = dataBase.HistoryOfCosts.LoadWithAsTable(prod=>prod.Product.Exact)
                    .LoadWithAsTable(rc=>rc.RecommedStates).ToList();

                var points = dataBase.PointsOfSales.ToList();

                arr.ForEach(x =>
                {
                    x.PointOfSale = points.FirstOrDefault(p => p.ID.Equals(x.PointId));
                });

                return arr;
            }
        }

        #endregion

    }
}
