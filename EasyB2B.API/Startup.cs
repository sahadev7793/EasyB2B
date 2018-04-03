using EasyB2B.DataAccess.Interfaces;
using EasyB2B.DataAccess.Providers;
using EasyB2B.Helper;
using EasyB2B.Helper.Email;
using EasyB2B.Helper.EncryDecryHelper;
using EasyB2B.Helper.JWTTokenHelper;
using EasyB2B.Helper.JWTTokenHelpers;
using EasyB2B.Models.DataContext;
using EasyB2B.Models.Helper.EmailHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Text;

namespace EasyB2B.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            

            services.AddCors(o => o.AddPolicy("PolicyAccessAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            var mvc = services.AddMvc();
            mvc.AddJsonOptions(config =>
            {
                config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                config.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.Configure<DbSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });


            services.AddDataProtection()
               .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration()
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_128_CBC,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                    });



           // services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
           //.AddJwtBearer(options =>
           //{
           //   options.TokenValidationParameters = new TokenValidationParameters
           //    {    
           //        ValidateIssuer = true,
           //        ValidateAudience = true,
           //        ValidateLifetime = true,
           //        ValidateIssuerSigningKey = true,
           //        ValidIssuer = Configuration.GetSection("Jwt:Issuer").Value,
           //        ValidAudience = Configuration.GetSection("Jwt:Issuer").Value,
           //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt:Key").Value))
           //    };
           //});

           

           // services.AddAuthorization(options => {
           //     options.AddPolicy("UserAuth", policy => {
           //         policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
           //         policy.RequireAuthenticatedUser();
           //         policy.Build();
           //     });
           // });

            

            ///             services.AddAuthentication().AddGoogle(googleOptions =>
            //                {
            //                        googleOptions.ClientId = Configuration.GetSection("Authentication:Google:ClientId").Value;
            //                        googleOptions.ClientSecret = Configuration.GetSection("Authentication:Google:ClientSecret").Value;
            //                });
            services.Configure<JWTTokenSetting>(options =>
            {
                options.JwtKey = Configuration.GetSection("Jwt:Key").Value;
                options.JwtIssuer = Configuration.GetSection("Jwt:Issuer").Value;
            });


            services.Configure<JwtTokenConfig>(options =>
            {
                options.Audience = Configuration.GetSection("JwtConfiguration:Audience").Value;
                if (!string.IsNullOrEmpty(Configuration.GetSection("JwtConfiguration:ClockSkew").Value))
                    options.ClockSkew = TimeSpan.Parse(Configuration.GetSection("JwtConfiguration:ClockSkew").Value);
                    options.Issuer = Configuration.GetSection("JwtConfiguration:Audience").Value;
            });




            services.Configure<EmailSetting>(options =>
            {
                options.SMTPHostName = Configuration.GetSection("EmailSetting:SMTPHostName").Value;
                options.Port = Convert.ToInt32(Configuration.GetSection("EmailSetting:Port").Value);
                options.UserName = Configuration.GetSection("EmailSetting:UserName").Value;
                options.Password = Configuration.GetSection("EmailSetting:Password").Value;
            });

            services.AddTransient<IUserProvider, UserProvider>();
            services.AddTransient<IOTPProvider, OTPProvider>();
            services.AddTransient<IEmailHelper, EmailHelper>();
            services.AddTransient<IJWTTokenHelper, JWTTokenHelper>();
            services.AddTransient<IEncryptDecryptHelper, EncryptDecryptHelper>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors("PolicyAccessAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseJwtBearerAuthentication();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
