using DMSZ_API.Data;
using DMSZ_API.Data.Productions;
using DMSZ_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Services;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DMSZ_API
{
    public class Utils
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Создаёт конструктор с конфигурацией.
        /// </summary>
        /// <param name="configuration">Конфигурация.</param>
        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Создаёт токен.
        /// Перед использованием токена всегда писать "Bearer *here token*"
        /// </summary>
        /// <returns>Токен.</returns>
        public string GenerateToken(CredentialsExtended credentials)
        {
            var tokenLifeTime = TimeSpan.FromMinutes(30);
            var tokenSecret = _configuration["JwtSettings:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(tokenSecret);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("login", credentials.Login),
                new(JwtRegisteredClaimNames.Name, credentials.FullName),
                new(ClaimTypes.NameIdentifier, credentials.Id.ToString()),
                new("userRole", credentials.Role),
                new("place", credentials.Place)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenLifeTime),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Генерирует отформатированный массив для графика сумм.
        /// </summary>
        /// <param name="summed"></param>
        /// <returns></returns>
        public List<object> GenerateFormattedArr(List<(DateTime, float)> summed)
        {
            var formattedArr = new List<object>();

            for (int i = 0; i < summed.Count; i++)
            {
                var item = summed[i];

                formattedArr.Add(new { x = item.Item1.ToString("MMM yyy"), y = item.Item2 });
            }

            return formattedArr;
        }

        /// <summary>
        /// Генерирует отформатированный массив для столбчатой диаграммы
        /// </summary>
        /// <param name="summed">Массив сумм</param>
        /// <param name="currentKey">Текущий ключ</param>
        /// <returns>Объект массива с суммой</returns>
        public object GenerateExactFormattedArr(List<(DateTime, float)> summed, string currentKey)
        {
            summed = summed.Where(x => x.Item2 > 0).ToList();
            //@$"{{""id"":""{1}"",""{2}"":{3}}}",
            List<object> formattedArr = new();

            for (int i = 0; i < summed.Count; i++)
            {
                var item = summed[i];
                formattedArr.Add(@$"{{""id"":""{item.Item1}"",""{currentKey}"":{item.Item2}}}");
            }

            return formattedArr;
        }

        public object GenerateExactFormattedArr(List<(DateTime, List<(string, float)>)> formattedArr)
        {
            var reslt = new List<object>();

            formattedArr.ForEach(i =>
            {
                i.Item2.ForEach(j =>
                {
                    if(j.Item2 > 0)
                        reslt.Add(@$"{{""id"":""{i.Item1}"",""{j.Item1}"":{j.Item2}}}");
                });
            });

            return reslt;
        }

        public object GenerateExactFormattedArr(List<(DateTime, Employee, decimal)> formattedArr)
        {
            var reslt = new List<object>();

            formattedArr.ForEach(i =>
            {
                reslt.Add(@$"{{""id"":""{i.Item1}"",""{i.Item2.Surname} {i.Item2.Name} {i.Item2.Patronymic}"":{i.Item3}}}");
            });

            return reslt;
        }

        /// <summary>
        /// Создаёт массив с датами и суммой практического веса для CreationsHistory
        /// </summary>
        /// <param name="list">Массив данных, для которых надо создать сумму</param>
        /// <returns>Массив дат и сумм</returns>
        public List<(DateTime, float)> GenerateSummedArr(List<CreationsHistory> list)
        {
            var to = DateTime.Now;
            var from = new DateTime(to.Year - 1, to.Month, 1);

            var summed = list.GroupBy(x => new { x.Date.Month, x.Date.Year }).Select(dat =>
            (
                dat.Select(sel => sel.Date).DistinctBy(a => a.Date.Month).FirstOrDefault(),
                dat.Sum(x => x.PracticalWeightResult)
            )).ToList();

            var dates = Enumerable.Range(0, 1 + to.Subtract(from).Days)
                .Select(offset => from.AddDays(offset))
                .GroupBy(x => new { x.Date.Month, x.Date.Year })
                .Select(dat => (
                    dat.Select(sel => sel.Date).DistinctBy(a => a.Month).FirstOrDefault(),
                    0f
                )).ToList();

            var resl = new List<(DateTime, float)>();

            foreach (var date in dates)
            {
                var re = summed.FirstOrDefault(x => x.Item1.Month.Equals(date.Item1.Month) && x.Item1.Year.Equals(date.Item1.Year));
                if (re.Item1.Equals(default) && re.Item2.Equals(default))
                {
                    resl.Add(date);
                }
                else
                {
                    resl.Add(re);
                }
            }

            var ans = resl.OrderBy(x => x.Item1).ToList();
            return ans;
        }

        public List<(DateTime, float)> GenerateChecksCountArr(List<HistoryOfSale> history)
        {
            var to = DateTime.Now;
            var from = new DateTime(to.Year - 1, to.Month, 1);

            var dates = Enumerable.Range(0, 1 + to.Subtract(from).Days).Select(offset => from.AddDays(offset))
               .GroupBy(x => new { x.Date.Month, x.Date.Year })
               .Select(dat => (dat.Select(sel => sel.Date).DistinctBy(a => a.Month).FirstOrDefault(), 0)).ToList();

            var ditinctId = history.DistinctBy(id => id.PointOfSale.ID).Select(ad => ad.PointOfSale.Addres).ToList();

            var result = new List<(DateTime, float)>();

            foreach (var id in ditinctId)
            {
                foreach (var date in dates)
                {
                    var summ = history.Where(x => x.PointOfSale.Addres.Equals(id) && !x.IsCancel && x.DateOfSell.Month == date.Item1.Month && x.DateOfSell.Year == date.Item1.Year).Count();
                    result.Add((date.Item1, summ));
                }
            }

            var ans = result.OrderBy(x => x.Item1).ToList();
            return ans;
        }

        public List<object> SummArray(List<HistoryOfSale> history, List<(DateTime, decimal)> dates)
        {
            var summsArr = new List<object>();
            dates.ForEach(item =>
            {
                var sum = history.Where(x => x.DateOfSell.Month == item.Item1.Month && x.DateOfSell.Year == item.Item1.Year && !x.IsCancel)
                    .Sum(s => s.SoldItems.Sum(x => (decimal)x.Weight * x.Price));
                summsArr.Add(new { X = item.Item1.ToString("MMM"), Y = sum });
            });

            return summsArr;
        }

        /// <summary>
        /// Сравнить по месяцу и дате
        /// </summary>
        /// <param name="x">X больше y</param>
        /// <param name="y">Y меньше x</param>
        /// <returns></returns>
        public static bool CompareByMonthAndYear(DateTime x, DateTime y)
        {
            return x.Month >= y.Month ? x.Year >= y.Year : x.Year >= y.Year;
        }

        public static bool EqualByMonthAndYear(DateTime x, DateTime y)
        {
            return x.Month == y.Month ? x.Year == y.Year : x.Year == y.Year;
        }

        public static IList CreateList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static List<object> CastObjectToList(object myObject)
        {
            List<object> list = new();
            if (myObject is IEnumerable)
            {
                var enumerator = ((IEnumerable)myObject).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
            }

            return list;
        }
    }
}
