using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

namespace MyBGList;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(cfg =>
            {
                cfg.WithOrigins(builder.Configuration["AllowedOrigins"]);
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
            });

            options.AddPolicy(name: "AnyOrigin", cfg =>
            {
                cfg.AllowAnyOrigin();
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
            });
        });

        builder.Services.AddApiVersioning(options =>
                {
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                });

        builder.Services.AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

        builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                            "v1",
                            new OpenApiInfo { Title = "MyBGList", Version = "v1.0" });
                    options.SwaggerDoc(
                            "v2",
                            new OpenApiInfo { Title = "MyBGList", Version = "v2.0" });
                });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint(
                                $"/swagger/v1/swagger.json",
                                $"MyBGList v1");
                        options.SwaggerEndpoint(
                                $"/swagger/v2/swagger.json",
                                $"MyBGLIst v2");
                    });
        }

        if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();

        app.MapGet("/v{version:ApiVersion}/error",
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [EnableCors("AnyOrigin")]
        [ResponseCache(NoStore = true)] () =>
        Results.Problem());

        app.MapGet("/v{version:ApiVersion}/error/test",
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [EnableCors("AnyOrigin")]
        [ResponseCache(NoStore = true)] () =>
                {
                    throw new Exception("test");
                });

        app.MapGet("/v{version:ApiVersion}/cod/test",
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        [EnableCors("AnyOrigin")]
        [ResponseCache(NoStore = true)] () =>
                Results.Text("<script>" +
                    "window.alert('Your client supports JavaScript!" +
                    "\\r\\n\\r\\n" +
                    $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
                    "\\r\\n" +
                    "Client time (UTC): ' + new Date().toISOString());" +
                    "</script>" +
                    "<noscript>Your client does not support JavaScript</noscript>",
                    "text/html"));

        app.MapControllers();

        app.Run();
    }
}
