using Microsoft.OpenApi.Models;
using System.Reflection;
using NodaTime;
using ProductDatabaseAPI.Services;
using ProductDatabaseAPI.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using ProductDatabaseAPI.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace ProductDatabaseAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration["PostgreSql:ConnectionString"];
        var dbPassword = builder.Configuration["PostgreSql:DbPassword"];
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString) { Password = dbPassword };


        // Add services to the container.

        builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
            
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionStringBuilder.ConnectionString));

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllers();

        builder.Services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);
        builder.Services.AddTransient<ProductService>();
        builder.Services.AddTransient<AuthenticationService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products APIs", Version = "v1" });
            // add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { }}
    });
        });

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value ?? "");
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false
                };
            });

        //builder.Services.AddCors(options => options.AddPolicy("FrontEnd", policy =>
        //{
        //    policy.WithOrigins("http://localhost:1234").AllowAnyMethod().AllowAnyHeader();
        //}));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseDeveloperExceptionPage();
        app.UseSwagger(c => { c.RouteTemplate = "/swagger/{documentName}/swagger.json"; });
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1"));

        app.UseAuthentication();
        app.UseAuthorization();
        //app.UseCors("FrontEnd");

        app.MapControllers();
        app.Run();
    }
}

