using AuthAPI.Entidade;
using System;
using System.Collections.Generic;
using System.Linq;
using static AuthAPI.Entidade.LoginResult;

namespace AuthAPI.Repositorio
{
    public class UserRepository
    {
        private const int LOGIN_FAILED_LIMIT = 3;
        private readonly Dictionary<Guid, User> _users;

        public UserRepository()
        {
            _users ??= new Dictionary<Guid, User>();
        }

        public IEnumerable<User> Get()
        {
            return _users.Values;
        }

        public User Get(Guid id)
        {
            if(_users.TryGetValue(id, out var user))
                return user;

            throw new Exception("Usuario não encontrado");
        }
        public User GetbyUsername(string username)
        {
            return _users.Values.Where(u => u.Username == username).FirstOrDefault();
        }
        public User Create(User user)
        {
            user.Id = Guid.NewGuid();
            if(_users.TryAdd(user.Id, user))
                return user;

            throw new Exception("Não foi possivel cadastrar");
        }

        public bool Remove (Guid id)
        {
            return _users.Remove(id);
        }

        public User Update(Guid id, User user)
        {
            if(_users.TryGetValue (id, out var userToUpdate)) 
            {
                userToUpdate.Role = user.Role;
                userToUpdate.Username = user.Username;
                userToUpdate.Password = user.Password;

                return Get(id);
            }

            throw new Exception("Usuario não encontrado");
        }

        public LoginResult Login(string username, string password)
        {
            try {
                var user = _users.Values.Where(u => u.Username == username && u.Password == password).SingleOrDefault();

                if (user != null)
                {
                    if (user.IsLockout)
                    {
                        if (DateTime.Now <= user.LockoutDate?.AddMinutes(15))
                        {
                            return LoginResult.ErrorResult(UserBlockedException.USER_BLOCKED_EXCEPTION);

                        }
                        else
                        {
                            user.IsLockout = false;
                            user.LockoutDate = null;
                            user.FailedAttempts = 0;
                        }
                    }

                    return LoginResult.SucessResult(user);
                }

                var userExistsForUsername = _users.Values.Where(u => u.Username == username).Any();

                if (userExistsForUsername)
                {
                    user = _users.Values.Where(u => u.Username == username).SingleOrDefault();

                    user.FailedAttempts++;

                    if (user.FailedAttempts > LOGIN_FAILED_LIMIT)
                    {
                        user.IsLockout = true;
                        user.LockoutDate = DateTime.Now;

                        return LoginResult.ErrorResult(UserBlockedException.USER_BLOCKED_EXCEPTION);
                    }

                    return LoginResult.ErrorResult(InvalidPasswordException.INVALID_PASSWORD_EXCEPTION);

                }
                return LoginResult.ErrorResult(InvalidUsernameException.INVALID_USERNAME_EXCEPTION);
            }catch (Exception ex)
            {
                return LoginResult.ErrorResult(new AuthenticationException(ex));
            }
        }
    }
}
