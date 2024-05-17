using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag.Generation.Processors.Security;
using NSwag;

namespace AuthTest.NSwag;

public static class NSwag
{
    public static void NSwagConfigSetting(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddOpenApiDocument((options, provider) =>
        {
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = $"JWTAuth Test API ({env.EnvironmentName})",
                    Description = "catcat API",
                    TermsOfService = "https://example.com/terms",
                    Contact = new OpenApiContact
                    {
                        Name = "聯絡方式 (網址無效)",
                        Url = "https://example.com/contact"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License (網址無效)",
                        Url = "https://example.com/license"
                    }
                };
            };

            // 同時註冊 SecurityDefinition (.components.securitySchemes) 與 SecurityRequirement (.security)
            options.AddSecurity(
                "Bearer",
                Enumerable.Empty<string>(),
                // OpenApiSecurityScheme 物件請勿加上 Name 與 In 屬性，否則產生出來的 OpenAPI Spec 格式會有錯誤！
                new OpenApiSecurityScheme()
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT", // for documentation purposes (OpenAPI only)
                    Description = "Copy JWT Token into the value field: {token}"
                });
            // 為了將 "Bearer" 加入到 OpenAPI Spec 裡 Operator 的 security (Security requirements) 中
            options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());

            // swagger example 套件
            //options.AddExamples(provider);
        });
    }
}