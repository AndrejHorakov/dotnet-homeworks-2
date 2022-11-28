using System.Diagnostics.CodeAnalysis;
using Hw8.Calculator;
using StackExchange.Profiling;

namespace Hw8;

[ExcludeFromCodeCoverage]
public class 
    Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<ICalculator, Calculator.Calculator>();
        builder.Services.AddSingleton<IParser, Parser>();
        builder.Services.AddMiniProfiler();
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();
        app.UseMiniProfiler();
        
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Calculator}/{action=Index}");

        app.Run();
    }
}