using System.Diagnostics;
using api.repository;
using ChessApi.Models.DB;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Middleware;

public class ErrorMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ConnectionRepository connRepo)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            await connRepo.GetCollection<Error>("ErrorLog").InsertOneAsync(new Error(ex));
        }
    }
}
