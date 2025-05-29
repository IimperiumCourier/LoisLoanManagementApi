using Loan_Backend.Domain.Interface;
using Loan_Backend.Infrastructure.Repository;
using Loan_Backend.Infrastructure.Repository.Context;
using Loan_Backend.Infrastructure.Service;
using Loan_Backend.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lois.LoanManagement.Api", Version = "v1" });

    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement();
    securityRequirement.Add(securitySchema, new[] { "Bearer" });
    c.AddSecurityRequirement(securityRequirement);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(x => { x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; })
.AddJwtBearer(opt => opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration.GetSection("JwtSettings:Issuer").Value,
    ValidAudience = builder.Configuration.GetSection("JwtSettings:Audience").Value,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value))
});

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("LoanDbContext");
builder.Services.AddDbContext<LoanAppDbContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerLoanService, CustomerLoanService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPaymentLogService, PaymentLogService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAdminRoleService, AdminRoleService>();
builder.Services.AddScoped<DataSeeder>();
builder.Services.AddScoped<MonthlyProfitAnalyticsStrategy>();
builder.Services.AddScoped<QuarterlyProfitAnalyticsStrategy>();
builder.Services.AddScoped<YearlyProfitAnalyticsStrategy>();
builder.Services.AddScoped<CustomProfitAnalyticsStrategy>();

builder.Services.AddScoped<ProfitAnalyticsStrategyFactory>();
builder.Services.AddScoped<IReportService,ReportService>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddCors(p => p.AddPolicy("corspolicy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("corspolicy");

app.UseMiddleware<CustomAuthenticationMiddleware>(builder.Configuration.GetSection("JwtSettings:SecretKey").Value);

app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LoanAppDbContext>();
    db.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedRolesFromExcelIfEmptyAsync();
    await seeder.SeedPermissionsFromExcelIfEmptyAsync();
    await seeder.InsertRolePermissionsAsync();
    await seeder.InsertAdminsFromCsv();
}

app.Run();
