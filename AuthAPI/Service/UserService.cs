using AuthAPI.DTO;
using AuthAPI.Entidade;
using AuthAPI.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthAPI.Service
{
    public class UserService
    {
        private UserRepository _repository;
        private JWTTokenService _tokenService;

        public UserService(UserRepository repository, JWTTokenService tokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
        }

        public UserDTO Create(User user)
        {
            var userExist = _repository.GetbyUsername(user.Username);

            if (userExist != null)
                throw new Exception("O nome de usuário já está sendo utilizado!");

            var newUser = _repository.Create(user);

            return new UserDTO
            {
                Username = newUser.Username,
                Role = newUser.Role,
            };
        }
        
        public IEnumerable<UserDTO> Get()
        {
            var user = _repository.Get();

            return user.Select(u =>
           {
               return new UserDTO
               {
                   Role = u.Role,
                   Username = u.Username,
               };

           });
        }

        public UserDTO Get(Guid id)
        {
            var user = _repository.Get(id);

            return new UserDTO
            {
                Role = user.Role,
                Username = user.Username,
            };
        }

        public LoginResultDTO Login(string username, string password)
        {

            var loginResult = _repository.Login(username, password);
            if (loginResult.Error)
            {
                return new LoginResultDTO
                {
                    Sucess = false,
                    Erros = new string[] { $"Ocorreu um erro ao autenticar {loginResult.Exception?.Message}" }
                };
            }
            var token = _tokenService.GenerateToken(loginResult.User);


            return new LoginResultDTO
            {
                Sucess = true,
                Erros = null,
                UserLogin = new UserLoginResultDTO
                {
                    Username = loginResult.User.Username,
                    Id = loginResult.User.Id,
                    Token = token,
                    Role = loginResult.User.Role,
                }
            };
        
        }
    }
}
