using MassTransit;
using Microsoft.AspNetCore.Http;
using SmilesInsurance_api.DTOs.Auth;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace SmilesInsurance_api.Services.Auth
{
    public class LoginDetailServices : ILoginDetailServices
    {
        public LoginDetailServices(IHttpContextAccessor accessor)
        {
            _loginClaim = new LoginDetailDto();
            IsLogin = false;

            if (accessor.HttpContext != null && accessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var token)) CheckToken(token.ToString()[7..]);
        }

        public LoginDetailServices(Headers header)
        {
            _loginClaim = new LoginDetailDto();
            IsLogin = false;

            if (header != null && header.TryGetHeader("Bearer", out var token)) CheckToken(token.ToString());
        }

        public LoginDetailServices(string token)
        {
            _loginClaim = new LoginDetailDto();
            IsLogin = false;

            CheckToken(token);
        }

        private JwtSecurityToken _jwtToken;

        private LoginDetailDto _loginClaim;

        public string Token { get; private set; }

        public string[] Roles { get; private set; }

        public string[] Permissions { get; private set; }

        public bool IsLogin { get; private set; }

        public bool CheckPermission(string permission)
        {
            return Permissions.Any(_ => _.Equals(permission));
        }

        public bool CheckRole(string role)
        {
            return Roles.Any(_ => _.Equals(role));
        }

        public LoginDetailDto GetClaim()
        {
            return _loginClaim;
        }

        private void CheckToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException($"'{nameof(token)}' cannot be null or whitespace.", nameof(token));

            _jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            if (string.IsNullOrWhiteSpace(_jwtToken.Subject)) throw new ArgumentException($"'Subject' cannot be null or whitespace.");

            _loginClaim = new LoginDetailDto
            {
                SubjectId = _jwtToken.Subject,
                UserId = GetClaim<int>("user_id"),
                EmployeeCode = GetClaim<string>("employee_code"),
                Firstname = GetClaim<string>("employee_firstname"),
                Lastname = GetClaim<string>("employee_lastname"),
                BranchId = GetClaim<int>("employee_branchid"),
                Branchname = GetClaim<string>("employee_branchname"),
                Token = token
            };

            IsLogin = true;
            Token = token;
            Permissions = GetClaims("permission");
            Roles = GetClaims("role");
        }

        private T GetClaim<T>(string @type)
        {
            var value = _jwtToken.Claims.Where(_ => _.Type == @type).FirstOrDefault()?.Value;

            switch (typeof(T))
            {
                case Type intType when intType == typeof(int):
                    if (string.IsNullOrWhiteSpace(value)) return (T)Convert.ChangeType(-1, typeof(T));
                    else return (T)Convert.ChangeType(int.Parse(value), typeof(T));
                case Type strType when strType == typeof(string):
                    return (T)Convert.ChangeType(value, typeof(T));

                default:
                    return (T)Convert.ChangeType(null, typeof(T));
            }
        }

        private string[] GetClaims(string @type)
        {
            if (!_jwtToken.Claims.Any(_ => _.Type == @type)) return null;
            return _jwtToken.Claims.Where(_ => _.Type == @type).Select(_ => _.Value).ToArray();
        }
    }
}