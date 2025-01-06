using Microsoft.AspNetCore.Cors;

namespace MyBGList;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
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

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
            app.UseDeveloperExceptionPage();
        else
            app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

		app.UseCors();

        app.UseAuthorization();

        app.MapGet("/error", [EnableCors("AnyOrigin")] () => Results.Problem());

        app.MapGet("/error/test", [EnableCors("AnyOrigin")] () => { throw new Exception("test"); });

        app.MapControllers();

        app.Run();
    }
}
