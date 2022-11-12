using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CtaLineaWebApi.Auth.Model;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Auth.Services
{
    public class UserService
        : IUserService
    {
        private readonly JwtOptions _appSettings;
        private readonly IJwtService _jwtService;
        private readonly ILogger _logger;
        private readonly MD5 _md5;
        private readonly IUserRepository _userRepository;

        #region costanti per la generazione della password
        private readonly char[] _UppercaseLetters;
        private readonly char[] _LowercaseLetters;
        private readonly char[] _Digits;
        private readonly char[] _Specials = new char[] { '-', '+', '_', '/', '$', '&', '%', '!', '|', '@', '#', '?' };
        private const int Pwd_MinLenght = 8;
        private const int Pwd_MaxLEngth = 32;
        #endregion

        public UserService(
            IOptions<JwtOptions> appSettings,
            IUserRepository userRepository,
            IJwtService jwtService,
            ILogger<UserService> logger
            )
        {
            _appSettings = appSettings.Value;
            _jwtService = jwtService;
            _logger = logger;
            _md5 = MD5.Create();
            _userRepository = userRepository;

            _UppercaseLetters = this.GetCharSet('A', 26).ToArray();
            _LowercaseLetters = this.GetCharSet('a', 26).ToArray();
            _Digits = this.GetCharSet('0', 10).ToArray();
        }

        public async Task<AuthenticateResponse?> AuthenticateAsync(
            AuthenticateRequest model
            )
        {
            AuthenticateResponse? result = null;

            var user = await  this._userRepository.GetUserAsync(model.Username)
                .ConfigureAwait (false);
            if (user == null)
            {
                return null;
            }
            if (user.ExpirationDate != null
                && user.ExpirationDate.Value <= DateTime.Now)
            {
                // utente scaduto
                return null;
            }
            // controlla se l'hash della password coincide
            if (user.PasswordHash != 
                this. GetPasswordHash(model.Password))
            {
                // la password è sbagliata
                return null;
            }

            // authentication successful so generate jwt token
            var jwtResult = _jwtService.GenTokenkey(
                user.UserName,
                user.Description,
                user.Email,
                user.AssociateId,
                user.Roles,
                user.MustChangePAssword
                );

            result = new AuthenticateResponse(
                user,
                jwtResult.Token,
                jwtResult.Expiration);

            return result;
        }

        public async Task<ChangePasswordResult> ChangePAsswordASync(
            string userName,
            string oldPAssword,
            string newPAssowrd)
        {
            await Task.CompletedTask;

            var result = new ChangePasswordResult(
                true, string.Empty);

            return result;
        }

        private async Task<string> GenerateRandomPAsswordAsync()
        {
            var sb = new StringBuilder();

            // estrae dall'array delle lettere maiuscole
            var upper = this.ExtractCharacters(_UppercaseLetters, 6);
            // estrae dall'array delle lettere minuscole
            var lower = this.ExtractCharacters(_LowercaseLetters, 6);
            // estrae dall'array dei numeri
            var dicits = this.ExtractCharacters(_Digits, 2);
            // estrae dall'array dei symboli
            var symbols = this.ExtractCharacters(_Digits, 1);

            var rnd = new Random();
            var source = upper + lower + dicits + symbols;
            while (source.Length > 0)
            {
                var index = rnd.Next(source.Length - 1);
                sb.Append(source[index]);
                source = source.Remove(index, 1);
            }

            await Task.CompletedTask;

            return sb.ToString();
        }

        private IEnumerable<char> GetCharSet(char start, int num)
        {
            for (int i = 0; i < num; ++i)
            {
                yield return (char)(start + i);
            }
        }
        private string ExtractCharacters(char[] array, int number)
        {
            var sb = new StringBuilder(number);
            var rnd = new Random();

            for (int i = 0; i < number; ++i)
            {
                char ch = array[rnd.Next(0, array.Length - 1)];
                sb.Append(ch);
            }

            return sb.ToString();
        }

        private async Task<ChangePasswordResult> VerifyPasswordAsync(string password)
        {
            string msg = string.Empty;
            bool isValid = false;
            int ucaseCount = 1;
            int lcaseCount = 1;
            int digitCount = 1;
            int specialsCount = 1;

            if (password.Length < Pwd_MinLenght) msg = "Password troppo corta";
            else if (password.Length > Pwd_MaxLEngth) msg = "Password troppo lunga";
            else
            {
                for (int i = 0; i < password.Length; ++i)
                {
                    var ch = password[i]; ;
                    if (_UppercaseLetters.Contains(ch)) --ucaseCount;
                    if (_LowercaseLetters.Contains(ch)) --lcaseCount;
                    if (_Digits.Contains(ch)) --digitCount;
                    if (_Specials.Contains(ch)) --specialsCount;
                }

                isValid = (ucaseCount <= 0
                    && lcaseCount <= 0
                    && digitCount <= 0
                    && specialsCount <= 0);
            }

            var result = new ChangePasswordResult(
                isValid, msg);

            await Task.CompletedTask;
            return result;
        }
        private string GetPasswordHash(string password)
        {
            var tmpSource = ASCIIEncoding.UTF8.GetBytes(password);
            var tmpHash = _md5.ComputeHash(tmpSource);
            var hashPwd = Convert.ToBase64String(tmpHash);

            return hashPwd;
        }
    }
}