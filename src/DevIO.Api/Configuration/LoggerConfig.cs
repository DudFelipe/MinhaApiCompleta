using DevIO.Api.Extensions;
using Elmah.Io.Extensions.Logging;
using HealthChecks.UI.Client;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmahIo(o =>
            {
                o.ApiKey = "a5a14fd8e590415cb57419c5f775cec1";
                o.LogId = new Guid("7265e39f-db14-48fc-a80c-2e573084ad91");
            });

            //Configurando o elmah.io como provider para os logs do asp.net
            //services.AddLogging(builder =>
            //{
            //    builder.AddElmahIo(o =>
            //    {
            //        o.ApiKey = "a5a14fd8e590415cb57419c5f775cec1";
            //        o.LogId = new Guid("7265e39f-db14-48fc-a80c-2e573084ad91");
            //    });

            //    builder.AddFilter<ElmahIoLoggerProvider>(null, LogLevel.Warning);
            //});

            //Usando o UI do Elmah para verificar os health checks
            services.AddHealthChecks()
                .AddElmahIoPublisher(options =>
                {
                    options.ApiKey = "a5a14fd8e590415cb57419c5f775cec1";
                    options.LogId = new Guid("7265e39f-db14-48fc-a80c-2e573084ad91");
                    options.HeartbeatId = "318a1bb7e1514e3b8068fa4af90d4424";
                })
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "Banco SQL Server");

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            app.UseHealthChecks("/api/hc", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return app;
        }
    }
}
