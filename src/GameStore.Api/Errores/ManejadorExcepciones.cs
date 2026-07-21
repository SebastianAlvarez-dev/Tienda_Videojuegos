using GameStore.Domain.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Errores;

public sealed class ManejadorExcepciones : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ExcepcionDominio errorDominio)
        {
            return false;
        }

        var estado = errorDominio.Codigo switch
        {
            "JUEGO_INVALIDO" => StatusCodes.Status400BadRequest,
            "JUEGO_NO_ENCONTRADO" => StatusCodes.Status404NotFound,
            "JUEGO_YA_EXISTE" or "STOCK_INSUFICIENTE" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        httpContext.Response.StatusCode = estado;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = estado,
            Title = "Error de negocio",
            Detail = errorDominio.Message,
            Extensions = { ["codigo"] = errorDominio.Codigo }
        }, cancellationToken);
        return true;
    }
}
