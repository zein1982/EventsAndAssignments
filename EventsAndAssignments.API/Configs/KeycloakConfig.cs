using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace EventsAndAssignments.API.Configs
{
    public static class KeycloakConfig
    {
        const string _realms = "/realms/";
        public static string? KeycloakUrlWithRealm { get; set; }
        public static string? KeycloakClientId { get; set; }

        public static void Init(WebApplicationBuilder builder)
        {
            string? keycloakUrl =
                builder.Configuration["KEYCLOAK_URL"] ??
                    builder.Configuration["Security:Keycloak:Url"];

            string? keycloakRealm =
                builder.Configuration["KEYCLOAK_REALM"] ??
                    builder.Configuration["Security:Keycloak:Realm"];

            string? keycloakClientId =
                builder.Configuration["KEYCLOAK_CLIENT_ID"] ??
                    builder.Configuration["Security:Keycloak:ClientId"];

            KeycloakUrlWithRealm = keycloakUrl + _realms + keycloakRealm;
            KeycloakClientId = keycloakClientId;

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = KeycloakUrlWithRealm;
                //"https://keycloak-keycloak.apps.ocpd.sib.evraz.com/auth/realms/saml";
                o.Audience = keycloakClientId;
                //"ruk-10252-dev";
                o.AutomaticRefreshInterval = TimeSpan.FromHours(2);
                o.RefreshInterval = TimeSpan.FromMinutes(1);
            });
        }
    }
}