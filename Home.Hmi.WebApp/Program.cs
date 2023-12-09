using FluentModbus;
using Home.Hmi.WebApp.Data;
using Home.Services;
using Quartz;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey(nameof(PollModbusRegistresJob));
    q.AddJob<PollModbusRegistresJob>(o => o.WithIdentity(jobKey));

    q.AddTrigger(o => o
        .ForJob(jobKey)
        .WithIdentity($"{nameof(PollModbusRegistresJob)}-trigger")
        .WithSimpleSchedule(s => s
            .WithInterval(TimeSpan.FromMilliseconds(1000))
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

builder.Services.AddSingleton<HomeHeatingService>();
builder.Services.AddSingleton<ModbusTcpClient>();

//builder.Logging.AddConfiguration(
//    builder.Configuration.GetSection("Logging"));

builder.Logging.AddSerilog(
    new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
        .WriteTo.File("hmi-web-service-.log", rollingInterval: RollingInterval.Day)
        .CreateLogger());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
