using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyB2B.Helper.JWTTokenHelper
{
    public interface IJWTConfig
    {
        void ConfigureJwtAuthentication(IServiceCollection services);

        void ConfigureJwtAuthorization(IServiceCollection services);
    }
}
