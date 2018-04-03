using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Helper.JWTTokenHelper
{
    
    public class JwtTokenConfig
    {
        public SecurityKey IssuerSigningKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public bool ValidateLifetime { get; set; }
        public TimeSpan ClockSkew { get; set; }
    }

    public  class JWTConfig : IJWTConfig
    {
        private readonly JwtTokenConfig _jwtTokenConfig;

        public JWTConfig(IOptions<JwtTokenConfig> options)
        {
            _jwtTokenConfig = new JwtTokenConfig();
            _jwtTokenConfig.IssuerSigningKey = options.Value.IssuerSigningKey;
            _jwtTokenConfig.Audience = options.Value.Audience;
            _jwtTokenConfig.Issuer = options.Value.Issuer;
            _jwtTokenConfig.ValidateIssuerSigningKey = options.Value.ValidateIssuerSigningKey;
            _jwtTokenConfig.ValidateLifetime = options.Value.ValidateLifetime;
            _jwtTokenConfig.ClockSkew = options.Value.ClockSkew;
        }
        public void ConfigureJwtAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });
        }

        public void ConfigureJwtAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                   IssuerSigningKey = _jwtTokenConfig.IssuerSigningKey,
                    ValidAudience = _jwtTokenConfig.Audience,
                    ValidIssuer = _jwtTokenConfig .Issuer,
                    ValidateIssuerSigningKey = _jwtTokenConfig.ValidateIssuerSigningKey,
                    ValidateLifetime = _jwtTokenConfig.ValidateLifetime,
                    ClockSkew = _jwtTokenConfig.ClockSkew
                };
            });
        }
    }
}





