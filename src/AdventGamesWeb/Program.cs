using AdventGamesCore;
using AdventGamesWeb;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
// Add http logging
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Request | HttpLoggingFields.Request;
});

#endif

// Add cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
#if DEBUG
        policy.WithOrigins("http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod();
#else
        policy.WithOrigins(Constants.Client_Origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
#endif
    });
});

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});
//builder.Services.AddAuthorization();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add http context accessor
builder.Services.AddHttpContextAccessor();

// Add mediator
builder.Services.AddMediatR(typeof(SignupCommand).GetTypeInfo().Assembly);

// Add validators
builder.Services.AddValidators();

// Add services
builder.Services.AddCoreServices();

// Add repositories
builder.Services.AddRepositories();

// Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Advent Games Web API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
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
                }
            },
            new string[]{}
        }
    });
});

builder.Configuration.AddJsonFile("gameprizes.json", optional: false, reloadOnChange: false);
builder.Services.Configure<GamePrizesOptions>(builder.Configuration.GetSection("GamePrizesOptions"));

// Add serilog
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

#if !DEBUG
builder.Logging.ClearProviders();
#endif

builder.Logging.AddSerilog(logger);

// App build
var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

#if DEBUG
app.UseHttpLogging();
#endif

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(configure => configure.MapEndpoints());

//TODO: this is temporary

var service = (IGamePrizeRepository)app.Services.GetRequiredService(typeof(IGamePrizeRepository));
service?.LoadGamePrizesFromJson();

app.Run();