using Asp.Versioning;
using CatalogoAPI.Context;
using CatalogoAPI.DTOs.Mappings;
using CatalogoAPI.Extensions;
using CatalogoAPI.Filters;
using CatalogoAPI.Logging;
using CatalogoAPI.Models;
using CatalogoAPI.RateLimitOptions;
using CatalogoAPI.Repositories;
using CatalogoAPI.Repositories.Impl;
using CatalogoAPI.Services;
using CatalogoAPI.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adiciona o filtro de tratamento de excecoes nao tratadas como global e permite que o json ignore objetos em ciclos
builder.Services.AddControllers(options =>
    options.Filters.Add(typeof(ApiExceptionFilter))
)
.AddJsonOptions(options => 
            options.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.IgnoreCycles)
.AddNewtonsoftJson();
var valor1 = builder.Configuration["chave1"];
var valor2 = builder.Configuration["secao1:chave2"];
var chaveSecreta = builder.Configuration["JWT:SecretKey"]
                    ?? throw new ArgumentException("senha inválida");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                         new string[] {}
                    }
                });
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "Riverson"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
                      context.User.HasClaim(claim => claim.Type.Equals("id") &&
                                                     claim.Value.Equals("Riverson") ||
                                                     context.User.IsInRole("SuperAdmin"))));
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(chaveSecreta))

    };
});
var rateLimit = new MyRateLimitOptions();
builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(rateLimit);

builder.Services.AddRateLimiter(rateLimitOptions =>
{
    rateLimitOptions.AddFixedWindowLimiter("fixedwindow", options =>
    {
        options.PermitLimit = rateLimit.PermitLimit;
        options.Window = TimeSpan.FromSeconds(rateLimit.Window);
        options.QueueLimit = rateLimit.QueueLimit;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    rateLimitOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
                            RateLimitPartition.GetFixedWindowLimiter(
                                               partitionKey: httpcontext.User?.Identity?.Name ??
                                                             httpcontext.Request.Headers.Host.ToString(),
                                               factory: partition => new FixedWindowRateLimiterOptions
                                               {
                                                   AutoReplenishment = true,
                                                   PermitLimit = rateLimit.PermitLimit,
                                                   QueueLimit = rateLimit.QueueLimit,
                                                   Window = TimeSpan.FromSeconds(rateLimit.Window)
                                               })) ;
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryImpl>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRespositoryImpl>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(RepositoryImpl<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();
builder.Services.AddScoped<ITokenService, TokenServiceImpl>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddCors(options =>
{
    options.AddPolicy("DominiosPermitidos",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7022")
                                .WithMethods("GET", "POST")
                                .AllowAnyHeader();
                      });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
                                                new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddAutoMapper(typeof(DTOMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
