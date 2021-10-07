using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Error;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate __next;
        private readonly ILogger<ExceptionMiddleware> __logger;
        private readonly IHostEnvironment __env;
        public ExceptionMiddleware(RequestDelegate nextt, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            __env = env;
            __logger = logger;
            __next = nextt;
        }

        public async Task Invoke(HttpContext context)
        {
            try{
                await __next(context);
            }
            catch(Exception ex)
            {
                __logger.LogError(ex.Message);
                context.Response.ContentType ="application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                APIException exceptionResponse = __env.IsDevelopment() ? new APIException(context.Response.StatusCode,ex.Message, ex.StackTrace?.ToString(), 2) :
                new APIException(context.Response.StatusCode, "internal custom error");

                var Options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                var json = JsonSerializer.Serialize(exceptionResponse, Options);

                await context.Response.WriteAsync(json);
            }
        }

        private object JsonSerializeOptions()
        {
            throw new NotImplementedException();
        }
    }
}