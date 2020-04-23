using AuthenticationSampleWebApp.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationSampleWebApp.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IStudentDbService service)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string metoda = httpContext.Request.Method.ToString();
                string sciezka = httpContext.Request.Path;
                string bodyStr = "";
                string querystring = httpContext.Request?.QueryString.ToString();

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                using (StreamWriter outputFile = new StreamWriter(Path.Combine("requestsLog.txt"), true))
                {
                    outputFile.WriteLine($"{metoda}  | {sciezka} \nbody | {bodyStr} \nquery| {querystring}\n");
                }
            }

            await _next(httpContext);
        }
    }
}
