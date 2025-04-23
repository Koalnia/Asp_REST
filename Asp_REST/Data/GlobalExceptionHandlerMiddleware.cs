using Asp_REST.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;
namespace Asp_REST.Data
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted)
                {
                    switch (context.Response.StatusCode)
                    {
                        case StatusCodes.Status400BadRequest:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Nieprawidłowe żądanie. Sprawdź poprawność danych.");
                            break;

                        case StatusCodes.Status401Unauthorized:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Brak autoryzacji. Zaloguj się, aby uzyskać dostęp.");
                            break;

                        case StatusCodes.Status403Forbidden:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Brak uprawnień do żądanego zasobu.");
                            break;

                        case StatusCodes.Status404NotFound:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Żądany zasób nie istnieje.");
                            break;

                        case StatusCodes.Status405MethodNotAllowed:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync($"Metoda HTTP {context.Request.Method} nie jest obsługiwana dla tego zasobu.");
                            break;
                        case StatusCodes.Status409Conflict:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync($"Naruszono wartość unikalną - {context.Request.Method}");
                            break;

                        case StatusCodes.Status415UnsupportedMediaType:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Nieobsługiwany format danych.");
                            break;

                        case StatusCodes.Status429TooManyRequests:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Przekroczono limit żądań. Spróbuj ponownie później.");
                            break;

                        case StatusCodes.Status500InternalServerError:
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("Wystąpił błąd wewnętrzny serwera.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var message = "Wystąpił nieoczekiwany błąd." + exception.Message;

            
           if (exception is UserException userException)
            {

                code = HttpStatusCode.NotFound;
                message = $"Błąd związany z użytkownikiem: {userException.Message}";
            }
           else if (exception is AdvertisementException advertisementException)
            {

                code = HttpStatusCode.NotFound;
                message = $"Błąd związany z ogłoszeniem: {advertisementException.Message}";
            }
           else if (exception is DbUpdateException dbEx)
            {
                code = HttpStatusCode.Conflict;
                message = $"Błąd związany z wartoscią unikalną: {dbEx.Message}";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(message);
        }
    }
}

