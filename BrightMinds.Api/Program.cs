using Blendd.Infrastructure.DataAccess;
using BrightMinds.Api.Extensions;
using BrightMinds.Api.MiddleWares;
using BrightMinds.Core.Models;
using BrightMinds.Services.ApplicationServicesConfigrations;
using BrightMinds.Services.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;


namespace BrightMinds.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });
            builder.Services.AddValidationErrorResponseHelper();
            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 400000000; // 100 MB
            });
            builder.Services.AddApplicationDbContext(builder.Configuration.GetConnectionString("DefaultConnection")
                , builder.Configuration.GetConnectionString("Redis"));

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {

            })
                      .AddEntityFrameworkStores<AppDbContext>()
                      .AddDefaultTokenProviders();
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddCors(
              option => {
                  option.AddPolicy("MyPolicy", options => {
                      options.AllowAnyHeader().
                       AllowAnyMethod()
                     .AllowAnyOrigin();
                  });
              });





            Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Information()
     .WriteTo.File(
         path: "Logs/log-.txt",
         rollingInterval: RollingInterval.Day,
         retainedFileCountLimit: 7,   
         outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
     )
     .CreateLogger();



            builder.Host.UseSerilog();  


            var app = builder.Build();
            await app.UpdateDatabase();
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            // }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseShiftMiddleware();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}

// T-SQL Script to drop all columns inside a database
/*
-- 1. Drop all foreign key constraints
DECLARE @sql NVARCHAR(MAX) = ''

SELECT @sql += 'ALTER TABLE ' + QUOTENAME(TABLE_SCHEMA) 
             + '.' + QUOTENAME(TABLE_NAME) 
             + ' DROP CONSTRAINT ' + QUOTENAME(CONSTRAINT_NAME) + ';'
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'

EXEC sp_executesql @sql
SET @sql = ''

-- 2. Drop all tables (including __EFMigrationsHistory)
SELECT @sql += 'DROP TABLE ' + QUOTENAME(TABLE_SCHEMA) 
             + '.' + QUOTENAME(TABLE_NAME) + ';'
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'

EXEC sp_executesql @sql
 */
