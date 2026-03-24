using System.Net;
using vkpolls.auth.Api.Models;
using vkpolls.auth.Application.Exceptions;

namespace vkpolls.auth.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            CustomProblemDetails problem = new();

            switch (ex)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.UnprocessableEntity;
                    problem = new CustomProblemDetails
                    {
                        Title = validationException.Message,
                        Status = (int)statusCode,
                        Detail = validationException.InnerException?.Message,
                        Type = nameof(ValidationException),
                        Errors = validationException.ValidationErrors
                    };
                    break;

                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new CustomProblemDetails
                    {
                        Title = notFoundException.Message,
                        Status = (int)statusCode,
                        Detail = notFoundException.InnerException?.Message,
                        Type = nameof(NotFoundException)
                    };
                    break;

                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = badRequestException.Message,
                        Status = (int)statusCode,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = badRequestException.ValidationErrors
                    };
                    break;

                case InvalidOperationException invalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = invalidOperationException.Message,
                        Status = (int)statusCode,
                        Detail = invalidOperationException.InnerException?.Message,
                        Type = nameof(InvalidOperationException)
                    };
                    break;

                case UnauthorizedAccessException unauthorizedAccessException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new CustomProblemDetails
                    {
                        Title = unauthorizedAccessException.Message,
                        Status = (int)statusCode,
                        Detail = unauthorizedAccessException.InnerException?.Message,
                        Type = nameof(UnauthorizedAccessException)
                    };
                    break;

                default:
                    problem = new CustomProblemDetails
                    {
                        Title = ex.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.InternalServerError),
                        Detail = ex.StackTrace
                    };
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem);
        }
    }
}
