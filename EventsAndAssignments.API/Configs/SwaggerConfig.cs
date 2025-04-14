using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace EventsAndAssignments.API.Configs
{
    public static class SwaggerConfig
    {
        public static void Init(WebApplicationBuilder builder)
        {
            using StreamReader reader = File.OpenText($"{AppContext.BaseDirectory}/Recources/index.html");
            string indexPageBody = reader.ReadToEnd();

            OpenApiInfo apiInfo = new()
            {
                Title = nameof(EventsAndAssignments),
                Description = indexPageBody,
                Version = "v1",
                Contact = new OpenApiContact { Email = "example@evraz.com" }
            };

            builder.Services.AddSwaggerGen(so =>
            {
                so.SwaggerDoc("v1", apiInfo);
                so.IncludeXmlComments("doc.xml");
                // TODO: Вынести линк в переменные окружения
                so.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/protocol/openid-connect/auth/"),
                            TokenUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/protocol/openid-connect/token"),
                            Scopes = new Dictionary<string, string>()
                        }
                    },
                    OpenIdConnectUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/.well-known/openid-configuration")
                });
                so.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OpenIdConnect,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/protocol/openid-connect/auth/"),
                                TokenUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/saml/protocol/openid-connect/token"),
                                Scopes = new Dictionary<string, string>()
                            }
                        },
                        OpenIdConnectUrl = new Uri(KeycloakConfig.KeycloakUrlWithRealm + "/.well-known/openid-configuration")
                    }] = Array.Empty<string>()
                });
            });
        }
    }
}