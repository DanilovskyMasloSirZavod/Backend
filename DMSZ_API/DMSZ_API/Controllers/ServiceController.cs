using DMSZ_API.Connection;
using DMSZ_API.Data;
using DMSZ_API.Data.Productions;
using DMSZ_API.Data.Sales;
using DMSZ_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace DMSZ_API.Controllers
{
    /// <summary>
    /// Контроллер сервиса ДМСЗ.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly LinqProvider _databaseContext;
        private readonly IConfiguration _configuration;
        private readonly Utils _utils;

        /// <summary>
        /// Конструктор, для контроллера.
        /// </summary>
        /// <param name="logger">Логгер информации.</param>
        /// <param name="configuration">Конфигурационный файл.</param>
        public ServiceController(ILogger<ServiceController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _databaseContext = new LinqProvider(Consts.ConnectionString);
            _utils = new(_configuration);
        }

        #region Общие методы.

        /// <summary>
        /// Пройти регистрацию.
        /// </summary>
        /// <param name="credentials">Логин и пароль пользователя.</param>
        /// <returns>Результаты входа.</returns>
        [EnableCors]
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult Login(Credentials credentials)
        {
            if (credentials is null || string.IsNullOrEmpty(credentials.Login) || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest(new { ru = "Логин или пароль не могут быть пустыми.", en = "Login or password can`t be empty." });
            }

            var credentialsOfAllUsers = GetAllCredentials() as ObjectResult;

            if (credentialsOfAllUsers is null
                || credentialsOfAllUsers.StatusCode.Equals(NoContent().StatusCode))
            {
                return NoContent(); // Ошибка в запросе.
            }

            var listOfUsers = (credentialsOfAllUsers.Value as List<Users>);

            var foundUser = listOfUsers!.FirstOrDefault(x =>
                x.Login.Equals(credentials.Login)
                && x.Password.Equals(credentials.Password));

            if (foundUser is null)
            {
                return NotFound(new { ru = "Логин или пароль не верны.", en = "Login or password are incorrect." });
            }

            var foundEmployee = _databaseContext.GetEmployees().First(x => x.Id.Equals(foundUser.EmployeeId));
            foundUser.Employee = foundEmployee;

            return Ok(_utils.GenerateToken(new CredentialsExtended(credentials, foundUser)));
        }

        /// <summary>
        /// Установить новую роль пользователю.
        /// </summary>
        /// <returns>User, если удалось, иначе BadRequest.</returns>
        [HttpPut]
        [Authorize(Policy = "SuperuserAdmin")]
        [Produces("application/json")]
        public IActionResult SetRole(UserRole newUserRole)
        {
            _logger.LogInformation("Asked to set new role.");
            var res = _databaseContext.SetRole(newUserRole);

            return (res is null) ? BadRequest(new { ru = "Токен отсутвует.", en = "No token was provided." }) : Ok(res);
        }

        /// <summary>
        /// Получить список всех ролей.
        /// </summary>
        /// <returns>Список ролей.</returns>
        [HttpGet]
        [Authorize(Policy = "SuperuserAdmin")]
        [Produces("application/json")]
        public IActionResult GetRoles()
        {
            var roles = _databaseContext.GetAllRoles();

            _logger.LogInformation("Asked to get roles.");

            return (roles.Any()) ? Ok(roles) : NotFound(new { ru = "Роли отсутвуют.", en = "Roles are empty." });
        }

        /// <summary>
        /// Получить список всех мест работы.
        /// </summary>
        /// <returns>Список мест работы.</returns>
        [HttpGet]
        [Authorize(Policy = "SuperuserAdmin")]
        [Produces("application/json")]
        public IActionResult GetExacts()
        {
            var roles = _databaseContext.GetExact();

            _logger.LogInformation("Asked to get exact.");

            return (roles.Any()) ? Ok(roles) : NotFound(new { ru = "Места работы отсутвуют.", en = "Exacts are empty." });
        }

        /// <summary>
        /// Получить список всех принадлежностей.
        /// </summary>
        /// <returns>Список принадлежностей.</returns>
        [HttpGet]
        [Authorize(Policy = "SuperuserAdmin")]
        [Produces("application/json")]
        public IActionResult GetPlaces()
        {
            var roles = _databaseContext.GetPlace();

            _logger.LogInformation("Asked to get place.");

            return (roles.Any()) ? Ok(roles) : NotFound(new { ru = "Принадлежности отсутвуют.", en = "Places are empty." });
        }

        /// <summary>
        /// Получить список всех продуктов и, кто их производит.
        /// </summary>
        /// <returns>Список продуктов.</returns>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetProducts()
        {
            var products = _databaseContext.GetProducts();
            _logger.LogInformation("Asked to get products.");
            return (products.Any()) ? Ok(products) : NotFound(new { ru = "База данных продуктов пуста.", en = "Database of products is empty." });
        }

        /// <summary>
        /// Получить список всех сотрудников и их информации.
        /// </summary>
        /// <returns>Список сотрудников.</returns>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetEmployees()
        {
            var employees = _databaseContext.GetEmployees();
            _logger.LogInformation("Asked to get employees.");
            return (employees.Any()) ? Ok(employees) : NotFound(new { ru = "База данных сотрудников пуста.", en = "Database of employees is empty." });
        }

        /// <summary>
        /// Получить сотрудника по id.
        /// </summary>
        /// <returns>Сотрудника.</returns>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetEmployee(Guid id)
        {
            var employee = _databaseContext.GetEmployees().FirstOrDefault(x => x.Id.Equals(id));
            _logger.LogInformation("Asked to get employee.");
            return (employee is not null) ? Ok(employee) : NotFound(new { ru = "Сотрудника с таким id не удалось найти.", en = "Employee with id was not found." });
        }

        /// <summary>
        /// Получить список график работы сотрудника.
        /// </summary>
        /// <param name="id">ID сотрудника.</param>
        /// <returns>Список дней.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetSchedule(Guid id)
        {
            var schedule = _databaseContext.GetSchedule(id);
            _logger.LogInformation($"Asked to get schedule for employee: {id}\n{schedule}.");
            return (schedule is not null) ? Ok(schedule) : NotFound(new { ru = "У сотрудника не создан график.", en = "No created schedule for employee." });
        }

        /// <summary>
        /// Получить список документов, которые принадлежат сотруднику.
        /// </summary>
        /// <param name="employeeId">ID сотрудника.</param>
        /// <returns>Список документов.</returns>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetDocuments(Guid employeeId)
        {
            var documentsOfEmployee = _databaseContext.GetDocuments(employeeId);
            _logger.LogInformation($"Asked to get documents of employee.");
            return (documentsOfEmployee.Any()) ? Ok(documentsOfEmployee) : NotFound(new { ru = "У сотрудника нет документов.", en = "No documents of employee." });
        }

        /// <summary>
        /// Получить список данных всех пользователей.
        /// </summary>
        /// <returns>Список пользователей с их данными.</returns>
        [HttpGet]
        [Authorize(Policy = "SuperuserAdmin")]
        [Produces("application/json")]
        public IActionResult GetAllCredentials()
        {
            var creadialsOfAllUsers = _databaseContext.GetAllCredentials();
            _logger.LogInformation($"Asked to get credentials of all employees.");
            return (creadialsOfAllUsers.Any()) ? Ok(creadialsOfAllUsers) : NotFound(new { ru = "В базе данных нет сотрудников.", en = "No employees in database." });
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult AddEmployeeFile(Guid employeeID)
        {
            var file = new Document { Id = Guid.NewGuid(), EmployeeId = employeeID, PointId = null, FileName = "Договор аренды.docx" };
            file.File = System.IO.File.ReadAllBytes("C:\\Users\\nikor\\Desktop\\Договор аренды.docx");

            var fileAdd = _databaseContext.EmployeeInsertFile(file);
            return fileAdd ? Ok() : BadRequest();
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult AddPointFile(Guid pointId, string path, string fileName)
        {
            var file = new Document { Id = Guid.NewGuid(), PointId = pointId, EmployeeId = null, FileName = fileName };
            file.File = System.IO.File.ReadAllBytes(path);

            var fileAdd = _databaseContext.PointInsertFile(file);
            return fileAdd ? Ok() : BadRequest();
        }

        #endregion

        #region Производство.

        /// <summary>
        /// Статистика производства сотрудника.
        /// </summary>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetStatsOfEmployee(Guid employeeID)
        {
            var creations = _databaseContext.GetCreationsOfEmpolyee(employeeID).GroupBy(item => new { item.CreationsHistory.Date.Month, item.CreationsHistory.Date.Year }).Select(obj => new
            {
                Id = new DateTime(obj.Key.Year, obj.Key.Month, 1),
                Sum = obj.Sum(x => x.CreationsHistory.PracticalWeightResult)
            }).OrderBy(x => x.Id);

            var reslt = new { data = creations, keys = new string[] { "sum" } };

            return creations.Any() ? Ok(reslt) : NotFound(new { ru = "В базе данных нет истории производства.", en = "No history of creations." });
        }

        /// <summary>
        /// Получить список результатов лабаратории по id партии.
        /// </summary>
        /// <param name="batchId">ID партии.</param>
        /// <returns>Результаты лаборатории.</returns>
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetLabResultsById(string batchId)
        {
            var labResults = _databaseContext.GetLab(batchId);
            _logger.LogInformation($"Asked to get lab results.");
            return (labResults.Any()) ? Ok(labResults) : NotFound(new { ru = "Нет результатов лаборатории по данной партии.", en = "No information for selected batch." });
        }

        /// <summary>
        /// Получить список результатов лабаратории по дате.
        /// </summary>
        /// <param name="date">Дата.</param>
        /// <returns>Результаты лаборатории.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetLabResultsByDate(string dateString)
        {
            var date = DateTime.Parse(dateString);
            Console.WriteLine(date);
            var labResults = _databaseContext.GetLabDated(date);
            _logger.LogInformation($"Asked to get lab results.");
            return (labResults.Any()) ? Ok(labResults) : NotFound(new { ru = "Нет результатов лаборатории по данной дате.", en = "No information for selected date." });
        }

        /// <summary>
        /// Получить список результатов лабаратории по текущей дате.
        /// </summary>
        /// <returns>Результаты лаборатории.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetLabResultsByCurrentDate()
        {
            var labResults = _databaseContext.GetLabDated(DateTime.Now);
            _logger.LogInformation($"Asked to get lab results.");
            return (labResults.Any()) ? Ok(labResults) : NotFound(new { ru = "Нет результатов лаборатории по текущей дате.", en = "No information for current date." });
        }

        /// <summary>
        /// Получить список доставок, до текущей даты.
        /// </summary>
        /// <returns>Список доставок.</returns>
        [HttpGet]
        [Authorize(Policy = "Superuser")]
        [Produces("application/json")]
        public IActionResult GetDeliveryToCurrentDate()
        {
            _logger.LogInformation($"Asked to get deliveries.");
            var from = DateTime.Parse($"01.{DateTime.Now.Date.Month}.{DateTime.Now.Date.Year - 1}");
            var inRange = _databaseContext.GetDeliveryInTimeRange(from, DateTime.Now);
            return (inRange.Any()) ? Ok(inRange) : NotFound(new { ru = "В базе данных нет подходящих данных поставок.", en = "No deliveries were found that satisfy params." });
        }

        /// <summary>
        /// Получить просумированный список доставок до текщей даты.
        /// </summary>
        /// <returns>Просумированный список.</returns>
        [HttpGet]
        [Authorize(Policy = "Superuser")]
        [Produces("application/json")]
        public IActionResult GetDeliverySummedToCurrent()
        {
            _logger.LogInformation($"Asked to get summed deliveries.");
            var response = GetDeliveryToCurrentDate() as ObjectResult;

            if (response is null
                || response.StatusCode.Equals(NotFound().StatusCode))
            {
                return NotFound(new { ru = "В базе данных нет подходящих данных поставок.", en = "No deliveries were found that satisfy params." });
            }

            var res = response.Value as List<Delivery>;

            var summed = res.GroupBy(x => new { x.Date.Month, x.Date.Year }).Select(dat => new
            {
                x = dat.Select(sel => sel.Date).DistinctBy(a => a.Date.Month).ToList(),
                y = dat.Sum(x => x.Weight)
            }).ToList();

            var formattedArray = new List<object>();

            for (int i = 0; i < summed.Count; i++)
            {
                var item = summed[i];

                for (int j = 0; j < item.x.Count; j++)
                {
                    formattedArray.Add(new { x = item.x[j].ToString("MMM yyy"), y = item.y });
                }
            }

            var formattedResl = new { id = "Сумма", data = formattedArray };

            return (formattedResl.data.Any()) ? Ok(formattedResl) : NotFound(new { ru = "В базе данных нет подходящих данных поставок.", en = "No deliveries were found that satisfy params." });
        }

        /// <summary>
        /// Получить список произведенных продуктов.
        /// </summary>
        /// <returns>Список произведенных продуктов</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetProductions()
        {
            _logger.LogInformation($"Asked to get productions.");
            var response = _databaseContext.GetCreationsHistory();

            var formattedReslArr = new List<object>
            {
                new { id = "Всего", data = _utils.GenerateFormattedArr(_utils.GenerateSummedArr(response)) }
            };

            foreach (var item in response)
            {
                var obj = response.Where(x => x.Product.ProductName.Equals(item.Product.ProductName)).ToList();
                formattedReslArr.Add(new { id = item.Product.ProductName, data = _utils.GenerateFormattedArr(_utils.GenerateSummedArr(obj)) });
            }

            return response.Any() ? Ok(formattedReslArr) : NotFound(new { ru = "В базе данных нет истории производства.", en = "No history of creations." });
        }

        /// <summary>
        /// Получить список произведенных продуктов для определенного цеха.
        /// </summary>
        /// <returns>Список произведенных продуктов для определенного цеха</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetProductionsOfExact(string exact)
        {
            _logger.LogInformation($"Asked to get productions.");
            var now = DateTime.Now;
            var response = _databaseContext.GetCreationsHistory().Where(i => i.Product.Exact.Clarification.Equals(exact)
                && (i.Date.Date.Month >= now.Month
                    && (i.Date.Year >= now.Date.Year - 1 && i.Date.Year <= now.Date.Year))).OrderBy(x => x.Date).ToList();

            var formattedReslArr = new List<object>();
            var products = new List<string>();

            foreach (var item in response)
            {
                if (products.Contains(item.Product.ProductName)) continue;
                var obj = response.Where(x => x.Product.ProductName.Equals(item.Product.ProductName)).ToList();
                var r = _utils.GenerateExactFormattedArr(_utils.GenerateSummedArr(obj), item.Product.ProductName);
                formattedReslArr.Add(r);
                products.Add(item.Product.ProductName);
            }

            var newFormattedArr = new List<object>();

            foreach (var item in formattedReslArr)
            {
                var innerList = item as List<object>;
                foreach (var inner in innerList)
                {
                    newFormattedArr.Add(inner);
                }
            }

            var reslt = new { data = newFormattedArr, keys = products };

            return response.Any() ? Ok(reslt) : NotFound(new { ru = "В базе данных нет истории производства.", en = "No history of creations." });
        }

        /// <summary>
        /// Получить доставки с данными получивших молоко сотрудников.
        /// </summary>
        /// <returns>Список Доставок с данными получивших молоко сотрудников.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetDeliveriesWithRecivers()
        {
            var resl = _databaseContext.GetDeliveriesWithRecivers();
            return resl.Any() ? Ok(resl) : NotFound(new { ru = "В базе данных нет подходящих данных поставок.", en = "No deliveries were found that satisfy params." });
        }

        /// <summary>
        /// История производства.
        /// </summary>
        /// <returns>История производства</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetCreationsHistory()
        {
            var sql = _databaseContext.GetCreationsHistory();
            return sql.Any() ? Ok(sql) : NotFound(new { ru = "Пока нет данных истории производства.", en = "No history of created product." });
        }

        /// <summary>
        /// История производства с проведенным сравнением
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetCreationsHistoryCalculated()
        {
            var res = _databaseContext.GetCreationsHistory();
            res.ForEach(x =>
            {
                x.IsOk = (x.TheoreticalWeightResult - x.PracticalWeightResult) <= 0.1;
            });

            return res.Any() ? Ok(res) : NotFound(new { ru = "Пока нет данных истории производства.", en = "No history of created product." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetCreatorsOfProductsAndHistory()
        {
            var sql = _databaseContext.GetCreatorsOfProducts();
            return sql.Any() ? Ok(sql) : NotFound(new { ru = "Пока нет данных истории производства.", en = "No history of created product." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetCreatorsByBatchId(string batchId)
        {
            var employees = _databaseContext.GetCreatorsOfProducts().Where(x => x.ShiftNumberId.Equals(batchId)).ToList();
            return employees.Any() ? Ok(employees) : NotFound(new { ru = "Пока нет данных истории производства.", en = "No history of created product." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetTopCreatedProducts()
        {
            var createdProds = _databaseContext.GetCreationsHistory().GroupBy(item => item.CreatedProduct).Select(obj => new
            {
                Name = obj.Select(sel => sel.Product.ProductName).Distinct().FirstOrDefault(),
                Sum = obj.Sum(x => x.PracticalWeightResult)
            }).OrderByDescending(sum => sum.Sum).ToList();

            return createdProds.Any() ? Ok(createdProds) : NotFound(new { ru = "В базе данных нет истории производства.", en = "No history of creations." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetCreationsHistoriesFull(bool val)
        {
            var resl = _databaseContext.GetCheeseStorageProcessFull(val);

            return resl.Any() ? Ok(resl) : NotFound(new { ru = "В базе данных нет истории производства или истории процессов.", en = "No history of creations or processes." });
        }

        #endregion

        #region Продажи.

        #region Для страницы "Сотрудники".

        /// <summary>
        /// Получить список продавцов и принесённую ими выручку за всё время. (Таблица список продавцов)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetEmployeesSellingStats()
        {
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var result = history.GroupBy(x => new { x.Employee, x.DateOfSell }).Select(selection => new
            {
                PointOfSale = history.FirstOrDefault(e => e.Employee.Id.Equals(selection.Key.Employee.Id)).PointOfSale,
                Employee = selection.Key.Employee,
                DateOfSell = selection.Key.DateOfSell,
                SumSold = history.Where(e => e.Employee.Id.Equals(selection.Key.Employee.Id) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Price * (decimal)x.Weight)),
                SumWeight = history.Where(e => e.Employee.Id.Equals(selection.Key.Employee.Id) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Weight)),
                City = history.FirstOrDefault(e => e.Employee.Id.Equals(selection.Key.Employee.Id)).PointOfSale.City,
            }).DistinctBy(item => item.Employee.Id).OrderByDescending(sold => sold.SumSold).ToList();

            return (result.Any()) ? Ok(result) : NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
        }

        /// <summary>
        /// Получить список продавцов и принесённую ими выручку за промежуток времени. (Таблица топ продавцов)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetTopOfEmployeesSellingStats(string totime)
        {
            var to = DateTime.Parse(totime);
            var from = new DateTime(to.Year - 1, to.Month - 1, 1);
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var result = history.GroupBy(x => new { x.Employee, x.DateOfSell }).Select(selection => new
            {
                DateOfSell = selection.Key.DateOfSell,
                Employee = selection.Key.Employee,
                SumSold = selection.Where(e => Utils.CompareByMonthAndYear(to, e.DateOfSell) && Utils.CompareByMonthAndYear(e.DateOfSell, from) && e.Employee.Id.Equals(selection.Key.Employee.Id) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Price * (decimal)x.Weight)),
                SumWeight = selection.Where(e => Utils.CompareByMonthAndYear(to, e.DateOfSell) && Utils.CompareByMonthAndYear(e.DateOfSell, from) && e.Employee.Id.Equals(selection.Key.Employee.Id) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Weight)),
                City = selection.FirstOrDefault(e => Utils.CompareByMonthAndYear(to, e.DateOfSell) && Utils.CompareByMonthAndYear(e.DateOfSell, from) && e.Employee.Id.Equals(selection.Key.Employee.Id)).PointOfSale.City,
            }).ToList();//.DistinctBy(item => item.Employee.Id).OrderByDescending(sold => sold.SumSold).ToList();

            return (result.Any()) ? Ok(result) : NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetEmployeesBarChart()
        {
            var to = DateTime.Now;
            var from = new DateTime(to.Year - 1, to.Month - 1, 1);
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var distinct = history.DistinctBy(x => x.Employee.Id).Select(x => x.Employee).ToList();

            var keys = new List<string>();

            distinct.ForEach(x =>
            {
                keys.Add($"{x.Surname} {x.Name} {x.Patronymic}");
            });

            var result = history.GroupBy(x => new { x.Employee, x.DateOfSell }).Select(selection =>
            (
                selection.Key.DateOfSell,
                selection.Key.Employee,
                selection.Where(e => Utils.CompareByMonthAndYear(to, e.DateOfSell) && Utils.CompareByMonthAndYear(e.DateOfSell, from) && e.Employee.Id.Equals(selection.Key.Employee.Id) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Price * (decimal)x.Weight))
            )).OrderBy(h => h.Item1).ToList();

            var formatted = _utils.GenerateExactFormattedArr(result);

            return result.Any() ? Ok(new { Data = formatted, Keys = keys }) : NotFound(new { ru = "Нет истории выручки у сотрдуников.", en = "No history of employees sales." });
        }

        #endregion

        #region Для нажатия на сотрудника

        /// <summary>
        /// Получить информацию сотрудника по продажам (его базовая информация и документы)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetEmployeeInfo(Guid employeeId)
        {
            var employee = _databaseContext.GetEmployeeInfo(employeeId);

            return (employee is not null) ? Ok(employee) : NotFound(new { ru = "Нет информации сотрудника с таким id.", en = "No information of employee with id." });
        }

        /// <summary>
        /// Получить информацию продаж сотрудника по id (его статистика для bar chart) 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetEmployeeSellingStatsById(Guid id)
        {
            var to = DateTime.Now;
            var obj = (GetTopOfEmployeesSellingStats(to.ToString()) as ObjectResult).Value;

            var array = Utils.CastObjectToList(obj);

            foreach (var item in array)
            {
                var employee = (Employee)Utils.GetPropValue(item, "Employee");
                if (employee.Id.Equals(id))
                {
                    return Ok(item);
                }
            }

            return NotFound(new { ru = "Нет информации сотрудника с таким id.", en = "No information of employee with id." });
        }

        #endregion

        #region Для страницы "Точки"

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetTopPoints()
        {
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            if (!history.Any())
            {
                return NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
            }

            var distinct = history.Select(x => x.PointOfSale.ID).Distinct().ToList();

            var resl = new List<(decimal, PointOfSale)>();
            distinct.ForEach(x =>
            {
                var SumSold = history.Where(i => i.PointOfSale.ID.Equals(x) && !i.IsCancel).Sum(s => s.SoldItems.Sum(j => j.Price * (decimal)j.Weight));
                var PointOfSale = history.FirstOrDefault(i => i.PointOfSale.ID.Equals(x)).PointOfSale;
                resl.Add((SumSold, PointOfSale));
            });

            var formatted = resl.OrderByDescending(x => x.Item1).Select(x => new { SumSold = x.Item1, PointOfSale = x.Item2 });

            return formatted.Any() ? Ok(formatted) : NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
        }

        #region Композиты

        /// <summary>
        /// Получить список точек и принесённую ими выручку за всё время. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetPointsSellingStats()
        {
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var result = history.GroupBy(x => new { x.PointOfSale, x.DateOfSell }).Select(selection => new
            {
                DateOfSell = selection.Key.DateOfSell,
                PointOfSale = selection.Key.PointOfSale,
                SumSold = selection.Where(e => e.PointOfSale.ID.Equals(selection.Key.PointOfSale.ID) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => (decimal)x.Weight * x.Price)),
                SumWeight = selection.Where(e => e.PointOfSale.ID.Equals(selection.Key.PointOfSale.ID) && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Weight)),
                City = selection.Key.PointOfSale.City,
            }).ToList();//.DistinctBy(item => item.PointOfSale.ID).OrderByDescending(sold => sold.SumSold).ToList();

            return (result.Any()) ? Ok(result) : NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
        }


        /// <summary>
        /// Получить список точек и принесённую ими выручку за промежуток времени.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetTopOfPointsSellingStats(DateTime from, DateTime to)
        {
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var result = history.GroupBy(x => new { x.PointOfSale, x.DateOfSell }).Select(selection => new
            {
                PointOfSale = selection.Key.PointOfSale,
                SoldDate = selection.Key.DateOfSell,
                SumSold = selection.Where(e => e.Employee.Id.Equals(selection.Key.PointOfSale.ID)
                    && (Utils.CompareByMonthAndYear(selection.Key.DateOfSell, from) && Utils.CompareByMonthAndYear(to, selection.Key.DateOfSell))).Sum(s => s.Sum),
                SumWeight = selection.Where(e => e.Employee.Id.Equals(selection.Key.PointOfSale.ID)
                    && (Utils.CompareByMonthAndYear(selection.Key.DateOfSell, from) && Utils.CompareByMonthAndYear(to, selection.Key.DateOfSell))
                        && !e.IsCancel).Sum(s => s.SoldItems.Sum(x => x.Weight)),
                City = selection.FirstOrDefault(e => e.Employee.Id.Equals(selection.Key.PointOfSale.ID)).PointOfSale.City,
            }).DistinctBy(item => item.PointOfSale.ID).OrderByDescending(sold => sold.SumSold).ToList();

            return (result.Any()) ? Ok(result) : NotFound(new { ru = "Нет истории продаж.", en = "No history of sales." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetPointHistory(Guid pointId)
        {
            var temp = _databaseContext.GetHistoryOfSalesByPointId(pointId);

            var result = temp.GroupBy(x => x.CheckId).Select(selection => new
            {
                Id = selection.Key,
                DateOfSell = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).DateOfSell,
                PointOfSale = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).PointOfSale,
                SumSold = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).SoldItems.Sum(x => (decimal)x.Weight * x.Price),
                SoldItems = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).SoldItems,
                IsCancel = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).IsCancel,
                Employee = selection.FirstOrDefault(dat => dat.CheckId.Equals(selection.Key)).Employee
            }).OrderByDescending(i => i.DateOfSell);

            return result.Any() ? Ok(result) : NotFound(new { ru = "Нет истории продаж у выбранной точки.", en = "Selected point of sale has no history of sales." });
        }

        #endregion

        #endregion

        #region Для нажатия на точку

        /// <summary>
        /// Получить всю информацию по точке.
        /// </summary>
        /// <param name="id">Id точки.</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetPointFullInfoById(Guid id)
        {
            var pointInfo = _databaseContext.GetPointsFullInfo().Where(x => x.PointId.Equals(id));

            if (pointInfo is null || !pointInfo.Any())
            {
                return NotFound(new { ru = "Нет информации по точке с таким id.", en = "No information about point of sale with this id." });
            }

            var result = new
            {
                PointOfSale = pointInfo.FirstOrDefault().PointOfSale,
                Documnts = pointInfo.FirstOrDefault().Documents,
                Employees = pointInfo.Select(emp => emp.Employee).ToList()
            };

            return (result is not null) ? Ok(result) : NotFound(new { ru = "Нет информации по точке с таким id.", en = "No information about point of sale with this id." });
        }

        /// <summary>
        /// Получить список истории продаж точки.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetHistoryOfSalesByPointId(Guid pointId)
        {
            var history = _databaseContext.GetHistoryOfSalesByPointId(pointId);

            return (history.Any()) ? Ok(history) : NotFound(new { ru = "Нет точки с таким id.", en = "No point of sales with this id." });
        }

        #endregion

        #region Для страницы "Статистика"

        #region Для секции "Точки"

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetGain()
        {
            _logger.LogInformation($"Asked to get gain.");
            var to = DateTime.Now;
            var from = new DateTime(to.Year - 1, to.Month, 1);
            var history = _databaseContext.GetHistoryOfSalesOfPoints();

            var dates = Enumerable.Range(0, 1 + to.Subtract(from).Days).Select(offset => from.AddDays(offset))
                .GroupBy(x => new { x.Date.Month, x.Date.Year })
                .Select(dat => (dat.Select(sel => sel.Date).DistinctBy(a => a.Month).FirstOrDefault(), 0m)).ToList();

            var ditinctId = history.DistinctBy(id => id.PointOfSale.ID).Select(ad => ad.PointOfSale.Addres).ToList();

            var result = new List<(string, DateTime, decimal)>();

            foreach (var id in ditinctId)
            {
                foreach (var date in dates)
                {
                    var summ = history.Where(x => x.PointOfSale.Addres.Equals(id) && !x.IsCancel && x.DateOfSell.Month == date.Item1.Month && x.DateOfSell.Year == date.Item1.Year)
                        .Sum(s => s.SoldItems.Sum(x => (decimal)x.Weight * x.Price));
                    result.Add((id, date.Item1, summ));
                }
            }

            var formatted = new List<object>
            {
                new {Id = "Всего", Data = _utils.SummArray(history, dates)}
            };

            ditinctId.ForEach(item =>
            {
                var selected = result.Where(i => i.Item1 == item).Select(a => new { X = a.Item2.ToString("MMM"), Y = a.Item3 });
                formatted.Add(new { Id = item, Data = selected });
            });

            return Ok(new { Graph = formatted, Keys = ditinctId });
        }

        /// <summary>
        /// Получить список произведенных продуктов для определенного цеха.
        /// </summary>
        /// <returns>Список произведенных продуктов для определенного цеха</returns>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetChecks()
        {
            _logger.LogInformation($"Asked to get checks.");
            var now = DateTime.Now;
            var response = _databaseContext.GetHistoryOfSalesOfPoints().Where(i => (i.DateOfSell.Date.Month >= now.Month
                    && (i.DateOfSell.Year >= now.Date.Year - 1 && i.DateOfSell.Year <= now.Date.Year))).OrderBy(x => x.DateOfSell).ToList();

            var formattedReslArr = new List<object>();
            var points = new List<string>();

            foreach (var item in response)
            {
                if (points.Contains(item.PointOfSale.Addres)) continue;
                var obj = response.Where(x => x.PointOfSale.Addres.Equals(item.PointOfSale.Addres)).ToList();
                var r = _utils.GenerateExactFormattedArr(_utils.GenerateChecksCountArr(obj), item.PointOfSale.Addres);
                formattedReslArr.Add(r);
                points.Add(item.PointOfSale.Addres);
            }

            var newFormattedArr = new List<object>();

            foreach (var item in formattedReslArr)
            {
                var innerList = item as List<object>;
                foreach (var inner in innerList)
                {
                    newFormattedArr.Add(inner);
                }
            }

            var reslt = new { data = newFormattedArr, keys = points };

            return response.Any() ? Ok(reslt) : NotFound(new { ru = "В базе данных нет истории продаж.", en = "No history of sells." });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetTopSellingPoints()
        {
            var histroy = (GetPointsSellingStats() as ObjectResult).Value;

            if (histroy is null)
            {
                return NotFound(new { ru = "Нет иcтории продаж.", en = "No history of sells." });
            }

            var originalArr = Utils.CastObjectToList(histroy);

            var arr = originalArr.DistinctBy(x => ((PointOfSale)Utils.GetPropValue(x, "PointOfSale")).Addres)
                .Select(i => ((PointOfSale)Utils.GetPropValue(i, "PointOfSale")).Addres).ToList();

            var result = new List<object>();
            arr.ForEach(item =>
            {
                var summing = originalArr.Where(x =>
                    ((PointOfSale)Utils.GetPropValue(x, "PointOfSale")).Addres
                        .Equals(item)).Sum(s => (decimal)Utils.GetPropValue(s, "SumSold"));

                var point = (PointOfSale)Utils.GetPropValue(
                    originalArr.FirstOrDefault(c => ((PointOfSale)Utils.GetPropValue(c, "PointOfSale")).Addres.Equals(item)),
                    "PointOfSale");

                result.Add(new { SumSold = summing, PointOfSale = point });
            });

            return result.Any() ? Ok(result.OrderByDescending(x => (decimal)Utils.GetPropValue(x, "SumSold"))) : NotFound(new { ru = "Нет иcтории продаж.", en = "No history of sells." });
        }
        #endregion

        #region Для секции "Товары"

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetAmmountWeights()
        {
            _logger.LogInformation($"Asked to get weights.");
            var now = DateTime.Now;
            var response = _databaseContext.GetHistoryOfSalesOfPoints().Where(i => (i.DateOfSell.Date.Month >= now.Month
                    && (i.DateOfSell.Year >= now.Date.Year - 1 && i.DateOfSell.Year <= now.Date.Year))).OrderBy(x => x.DateOfSell).ToList();

            var distinct = new List<string>();

            response.ForEach(x =>
            {
                var temp = x.SoldItems.DistinctBy(i => i.Product.ProductName).Select(j => j.Product).ToList();
                temp.ForEach(h =>
                {
                    if (!distinct.Contains(h.ProductName))
                        distinct.Add(h.ProductName);
                });
            });

            var to = DateTime.Now;
            var from = new DateTime(to.Year - 1, to.Month, 1);

            var dates = Enumerable.Range(0, 1 + to.Subtract(from).Days).Select(offset => from.AddDays(offset))
               .GroupBy(x => new { x.Date.Month, x.Date.Year })
               .Select(dat => (dat.Select(sel => sel.Date).DistinctBy(a => a.Month).FirstOrDefault(), new List<(string, float)>())).ToList();

            for (var i = 0; i < dates.Count; i++)
            {
                var found = response.FindAll(x => x.DateOfSell.Date.Month == dates[i].Item1.Month && x.DateOfSell.Date.Year == dates[i].Item1.Year);

                var newList = new Dictionary<string, float>();
                found.ForEach(j =>
                {
                    distinct.ForEach(k =>
                    {
                        var val = j.SoldItems.Where(x => x.Product.ProductName.Equals(k)).Sum(s => s.Weight);

                        if (newList.ContainsKey(k))
                        {
                            newList[k] += val;
                        }
                        else newList.TryAdd(k, val);
                    });
                });

                dates[i].Item2.AddRange(newList.Select(kv => (kv.Key, kv.Value)).ToList());
            }

            var reslt = new { data = _utils.GenerateExactFormattedArr(dates), keys = distinct };

            return response.Any() ? Ok(reslt) : NotFound(new { ru = "В базе данных нет истории продаж.", en = "No history of sells." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetTopProducts()
        {
            _logger.LogInformation($"Asked to get top products.");
            var now = DateTime.Now;
            var response = _databaseContext.GetHistoryOfSalesOfPoints().Where(i => (i.DateOfSell.Date.Month >= now.Month
                    && (i.DateOfSell.Year >= now.Date.Year - 1 && i.DateOfSell.Year <= now.Date.Year))).OrderBy(x => x.DateOfSell).ToList();

            var distinct = new List<string>();

            response.ForEach(x =>
            {
                var temp = x.SoldItems.DistinctBy(i => i.Product.ProductName).Select(j => j.Product).ToList();
                temp.ForEach(h =>
                {
                    if (!distinct.Contains(h.ProductName))
                        distinct.Add(h.ProductName);
                });
            });

            var result = new Dictionary<string, float>();

            response.ForEach(j =>
            {
                distinct.ForEach(k =>
                {
                    var val = j.SoldItems.Where(x => x.Product.ProductName.Equals(k)).Sum(s => s.Weight);

                    if (result.ContainsKey(k))
                    {
                        result[k] += val;
                    }
                    else result.TryAdd(k, val);
                });
            });

            return Ok(result.Select(x => new
            {
                ProductName = x.Key,
                Weight = x.Value,
            }).OrderByDescending(i => i.Weight));
        }

        #endregion

        #endregion

        #region Для страницы "Рекомендация цен"

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetPointsNames()
        {
            var points = _databaseContext.GetPoints().Select(x => x.Addres);
            return points.Any() ? Ok(points) : NotFound(new { ru = "Нет торговых точек.", en = "No points of sales." });
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public IActionResult GetRecommendations()
        {
            var found = _databaseContext.GetHistoryOfCost();
            return found.Any() ? Ok(found) : NotFound(new { ru = "Нет истории цен, поэтому рекомендации тоже будут отсутсвовать.", en = "No history of costs, therefore no recomendations will be provided." });
        }

        #endregion

        #endregion
    }
}
