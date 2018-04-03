using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace EasyB2B.Helper.JWTTokenHelpers
{
    public interface IJWTTokenHelper
    {
        string BuildToken();
        string BuildToken(Claim[] claims);
        string BuildToken(Claim[] claims, DateTime expiresTime);
        string BuildToken(Claim[] claims, DateTime expiresTime, string securityAlgorithms);
        Claim[] CraeateClaims(string key, string value);
        Claim[] CraeateClaims(Dictionary<string, string> dictionary);
        string ReadToken(string token, string claimKey);
    }
}
