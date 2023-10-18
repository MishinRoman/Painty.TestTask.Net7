using Microsoft.AspNetCore.Identity;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Painty.TestTask.Net7;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Painty.TestTask.Net7.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.Extensions.Options;
using System.Text;

namespace Painty.TestTask.Net7
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

            builder.Services.AddIdentityCore<AppUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

            })
.AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;


                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(500);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;


            });
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => { 
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
            };
        }
                
                );


            builder.Services.AddCors();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Description = "Please enter a valid token",
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },

                        },
                        new string[]{}
                    }
                });
            });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.UseHttpsRedirection();
app.UseCors(options =>
{
    if (app.Environment.IsDevelopment())
    {
        options.AllowAnyOrigin();

    }
});



app.UseStaticFiles();


app.MapControllers();

app.Run();
        }
    }
}