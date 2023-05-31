using DMSZ_API.Data;
using Newtonsoft.Json.Linq;

namespace DMSZ_API.DTOs
{
    /// <summary>
    /// Информация пользователя (логин и пароль).
    /// </summary>
    public class Credentials
    {
        public string Login { get; set; }
        public string Password { get; set; }     
    }

    public class CredentialsExtended
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public Guid Id { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Place { get; set; }
        public CredentialsExtended(Credentials credentials, Users user)
        {
            Login = credentials.Login;
            Password = credentials.Password;
            Role = user.Role;
            Id = user.EmployeeId;
            FullName = $"{user.Employee?.Surname} {user.Employee?.Name} {user.Employee?.Patronymic}";
            Place = user?.Employee?.Job?.Place?.WorkPlace;
        }
    }
}
