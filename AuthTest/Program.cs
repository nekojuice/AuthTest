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
//builder.Services.AddScoped<AuthLogOutFilter>(); // 登出Filter 掛在單一 controller attribute
builder.Services.AddControllers(options => options.Filters.Add<AuthLogOutFilter>()); // 登出Filter 掛在全域

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


// 擷取 httpcontext
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

// 全域 Auth
app.MapControllers().RequireAuthorization();

app.Run();
