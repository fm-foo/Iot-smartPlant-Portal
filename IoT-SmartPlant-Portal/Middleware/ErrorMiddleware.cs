using IoT_SmartPlant_Portal.Middleware.Errors;
using IoT_SmartPlant_Portal.Middleware.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Middleware {
    public class ErrorMiddleware {

        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception ex) {
                await HandleExceptionAsync(context, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex) {
            ErrorResponse errorResponse = null;

            switch (ex) {
                case CustomException md:
                    errorResponse = new ErrorResponse((int)md.Code, md.Message);
                    context.Response.StatusCode = (int)md.Code;
                    break;
                case Exception e:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new ErrorResponse(context.Response.StatusCode, e.Message);
                    break;
            }

            context.Response.ContentType = "application/json";
            if (errorResponse != null) {
                var result = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(result);
            }
        }

    }
}
