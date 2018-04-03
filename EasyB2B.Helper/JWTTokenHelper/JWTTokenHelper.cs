using EasyB2B.Helper.JWTTokenHelpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EasyB2B.Helper.JWTTokenHelpers
{
    public class JWTTokenHelper : IJWTTokenHelper
    {
        private readonly string _JwtKey = string.Empty;
        private readonly string _JwtIssuer = string.Empty;

        public JWTTokenHelper(IOptions<JWTTokenSetting> options)
        {
            _JwtKey = options.Value.JwtKey;
            _JwtIssuer = options.Value.JwtIssuer;
        }

        public string BuildToken()
        {
            return BuildToken(null);
        }

        public string BuildToken(Claim[] claims)
        {
            return BuildToken(claims, DefultValueHelper.DEFAULT_TOKEN_EXPRIRE_TIME);
        }

        public string BuildToken(Claim[] claims, DateTime expiresTime)
        {
            return BuildToken(claims,expiresTime,SecurityAlgorithms.HmacSha256);
        }

        public string BuildToken(Claim[] claims,DateTime expiresTime, string securityAlgorithms)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token;
            if (claims == null)
            {
                token = new JwtSecurityToken(_JwtIssuer,
              _JwtIssuer,
              expires: expiresTime,
              signingCredentials: creds);
            }
            else
            {
                token = new JwtSecurityToken(_JwtIssuer,
              _JwtIssuer,
              claims,
              expires: expiresTime,
              signingCredentials: creds);
            }
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Claim[] CraeateClaims(string key , string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    throw new NullReferenceException();

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add(key, value);
                return CraeateClaims(dictionary);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public Claim[] CraeateClaims(Dictionary<string,string> dictionary)
        {
            
            Claim[] claimArray = null;

            if (dictionary.Count == 0)
                throw new NullReferenceException();

            var target = dictionary.ToList();

            var newLength = target.Count + 1;

            claimArray = new Claim[newLength];

            for (int i = 0; i < target.Count; i++)
            {
                claimArray[i] = new Claim(target[i].Key, target[i].Value);
            }
            return claimArray;
        }

        public string ReadToken(string token, string claimKey)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenDetail = handler.ReadJwtToken(token);
            return tokenDetail.Claims.First(claim => claim.Type == claimKey).Value;
        }
        

    }

    public class JWTTokenSetting
    {
        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }
    }
}
