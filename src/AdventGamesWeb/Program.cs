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
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
#else
        policy.WithOrigins(Constants.Client_Origins).SetIsOriginAllowedToAllowWildcardSubdomains()
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

var environemntVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (Constants.AllowedSwaggerEnvironments.Contains(environemntVariable))
{
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
}

//TODO: this is temporary
MapJsonOptions(builder);

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

if (app.Environment.IsDevelopment() || Constants.AllowedSwaggerEnvironments.Contains(environemntVariable))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
LoadJsonOptions(app);
BanHackers(app);

app.Run();

static void MapJsonOptions(WebApplicationBuilder builder)
{
    var environemntVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    builder.Configuration.AddJsonFile($"Jsons/{environemntVariable}/GamePrizeOptions.{environemntVariable}.json", optional: false, reloadOnChange: false);
    builder.Services.Configure<GamePrizeOptions>(builder.Configuration.GetSection("GamePrizeOptions"));

    builder.Configuration.AddJsonFile($"Jsons/{environemntVariable}/CompanyOptions.{environemntVariable}.json", optional: false, reloadOnChange: false);
    builder.Services.Configure<CompanyOptions>(builder.Configuration.GetSection("CompanyOptions"));

    builder.Configuration.AddJsonFile($"Jsons/{environemntVariable}/SeasonOptions.{environemntVariable}.json", optional: false, reloadOnChange: false);
    builder.Services.Configure<SeasonOptions>(builder.Configuration.GetSection("SeasonOptions"));

    builder.Configuration.AddJsonFile($"Jsons/{environemntVariable}/GameScheduleOptions.{environemntVariable}.json", optional: false, reloadOnChange: false);
    builder.Services.Configure<GameScheduleOptions>(builder.Configuration.GetSection("GameScheduleOptions"));
}

static void LoadJsonOptions(WebApplication app)
{
    var gmePrizeRepository = (IGamePrizeRepository)app.Services.GetRequiredService(typeof(IGamePrizeRepository));
    gmePrizeRepository?.LoadJson();

    var companyRepository = (ICompanyRepository)app.Services.GetRequiredService(typeof(ICompanyRepository));
    companyRepository?.LoadJson();

    var seasonRepository = (ISeasonRepository)app.Services.GetRequiredService(typeof(ISeasonRepository));
    seasonRepository?.LoadJson();

    var gameScheduleRepository = (IGameScheduleRepository)app.Services.GetRequiredService(typeof(IGameScheduleRepository));
    gameScheduleRepository?.LoadJson();
}

static void BanHackers(WebApplication app)
{
    var gameScoreRepository = (IGameScoreRepository)app.Services.GetRequiredService(typeof(IGameScoreRepository));
    gameScoreRepository?.BanHackers();

    var gameProfileRepository = (IGameProfileRepository)app.Services.GetRequiredService(typeof(IGameProfileRepository));
    gameProfileRepository?.BanHackers();
}