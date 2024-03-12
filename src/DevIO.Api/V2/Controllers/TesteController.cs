using DevIO.Api.Controllers;
using DevIO.Api.V1.Controllers;
using DevIO.Business.Intefaces;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/teste")]
    public class TesteController : MainController
    {
        private readonly ILogger _logger;

        public TesteController(INotificador notificador,
                               IUser appUser,
                               ILogger<TesteController> logger) : base(notificador, appUser)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Valor()
        {
            //try
            //{
            //    var i = 0;
            //    var result = 42 / i;
            //}
            //catch(DivideByZeroException e)
            //{
            //    e.Ship(HttpContext); //Envia o erro para o elmah.io
            //}

            throw new Exception("Error");

            _logger.LogTrace("Log de Trace");
            _logger.LogDebug("Log de Debug");
            _logger.LogInformation("Log de informação");
            _logger.LogWarning("Log de aviso");
            _logger.LogError("Log de erro");
            _logger.LogCritical("Log de problema critico");

            return "Sou a V2";
        }
    }
}
