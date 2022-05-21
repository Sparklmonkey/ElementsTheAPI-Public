using ElementsTheAPI.Data;
using ElementsTheAPI.Filters;
using ElementsTheAPI.Hubs;
using ElementsTheAPI.Repositories;
using ElementsTheAPI.Services;
using ElementsTheAPI.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;
using System.Threading.Tasks;

namespace ElementsTheAPI
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

            services.AddCors();
            services.AddSignalR().AddAzureSignalR();
            services.AddControllers();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration.GetValue<string>("JwtSettings:Issuer"),
                    ValidAudience = Configuration.GetValue<string>("JwtSettings:Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("JwtSettings:Key")))
                };

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            }); 
            services.AddSingleton<IPvpRoomService, PvpRoomService>();
            services.AddSingleton<IJwtAuth>(new JwtAuth(Configuration.GetValue<string>("JwtSettings:Key")));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ElementsTheAPI", Version = "v1" });
                c.OperationFilter<AddRequiredHeaderParameter>();

            });

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddScoped<IUserDataContext, UserDataContext>();
            //services.AddScoped<IPvpHubContext, PvpHubContext>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IUserDataRepository, UserDataRepository>();
            services.AddTransient(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ElementsTheAPI v1"));
            };

            app.UseCors(builder => builder
                .WithOrigins("http://localhost", "https://v6p9d9t4.ssl.hwcdn.net", "https://sparklmonkeygames.com") //Allows Localhost and Itch.io
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            //app.UseAzureSignalR(routes =>
            //{
            //    routes.MapHub<PvpHub>("/pvpHub");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHub<PvpHub>("/pvpHub");
            });
        }
    }
}
