using System;

namespace AuthAPI.DTO
{
    public class LoginResultDTO
    {
        public  bool Sucess { get; set; }
        public  string[] Erros { get; set; }
        public UserLoginResultDTO UserLogin { get; set; }
    }

    public class UserLoginResultDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
