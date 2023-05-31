using System.Data;
using static LinqToDB.Common.Configuration;

namespace DMSZ_API.Roles
{
    public static class IdentityData
    {
        public static (string, string)[] Identity = new (string, string)[]
        {
            ("Superuser", "Суперадмин" ),
            ("Admin", "Админ"),
            ("Technologist", "Технолог"),
            ("Seller", "Продавец"),
            ( "ReceivingFactoryUser", "Рабочий приемно-аппартного" )
        };

        public static bool IsInRoles(string key, string where)
        {
            var res = false;

            Roles().FirstOrDefault(x => 
                x.Item1.Equals(where)).Item2.ForEach(inner =>
            {
                res |= inner.Equals(key);
            });

            return res;
        }

        public static List<(string, List<string>)> Roles()
        {
            var result = Combinations(Identity);
            var premutations = new List<(string, List<string>)>();

            foreach (var item in result)
            {
                var claimName = string.Empty;
                var roles = new List<string>();
                foreach (var inner in item)
                {
                    claimName += inner.Item1;
                    roles.Add(inner.Item2);
                }
                if (roles.Any())
                    premutations.Add((claimName, roles));
            }

            return premutations;
        }

        private static IEnumerable<T[]> Combinations<T>(IEnumerable<T> source)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            return Enumerable
              .Range(0, 1 << (data.Length))
              .Select(index => data
                 .Where((v, i) => (index & (1 << i)) != 0)
                 .ToArray());
        }
    }
}
