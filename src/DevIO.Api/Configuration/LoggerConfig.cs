using Elmah.Io.Extensions.Logging;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLogginConfiguration(this IServiceCollection services)
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

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            app.UseElmahIo();

            return app;
        }
    }
}
