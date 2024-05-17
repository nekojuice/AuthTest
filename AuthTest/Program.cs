using AuthTest.Filter;
using AuthTest.JWTToken;
using AuthTest.NSwag;
using AuthTest.Request.Login;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var env = builder.Environment;

// Add services to the container.

// Auth filter
//builder.Services.AddScoped<AuthLogOutFilter>(); // �n�XFilter ���b��@ controller attribute
builder.Services.AddControllers(options => options.Filters.Add<AuthLogOutFilter>()); // �n�XFilter ���b����

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// NSwag
builder.Services.NSwagConfigSetting(env);

// Auth & JWT
builder.Services.AddSingleton<JwtHelpers>();
builder.Services.JwtConfig(config);

// memory cache
builder.Services.AddMemoryCache();


// �^�� httpcontext
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ���� Auth
app.MapControllers().RequireAuthorization();

app.Run();
