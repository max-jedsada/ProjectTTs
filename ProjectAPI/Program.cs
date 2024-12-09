using Project.ServicesRegister;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Project.API;
using Project.Context;
using Quartz;
using Manage.Quartz.Job;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);
var CurrentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddDbContext<ProjectContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("project", new OpenApiInfo { Title = "Project", Version = "v1" });

});

builder.Services.AddSwaggerGenNewtonsoftSupport();

builder.Services.InjectServices();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Store.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redisConn");
});

builder.Services.AddQuartz(Quartz =>
{
    var jobKey = new JobKey("JobSchedule");
    Quartz.AddJob<JobSchedule>(opts => opts.WithIdentity(jobKey));
    Quartz.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("JobSchedule-trigger")
        //.StartAt(DateTime.Now.AddDays(1))
        .StartNow()
        //.WithSchedule(
        //    SimpleScheduleBuilder
        //    .RepeatMinutelyForever(5)
        //    .RepeatForever()
        //)
        //.WithCronSchedule("0 0/5 * * * ?")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();
var _env = app.Environment.EnvironmentName;
if (_env == "Local" || _env == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/project/swagger.json", "Project.API HOME v1");
    });
}

app.Use(async (context, next) =>
{
    LogContext.PushProperty("UserId", context.User?.Identity?.Name ?? "anonymous");
    LogContext.PushProperty("CorrelationId", Guid.NewGuid());
    LogContext.PushProperty("StatusCode", context.Response.StatusCode);
    await next.Invoke();
});

app.UseStatusCodePages();

app.UseCors("AllowAll");
app.UseCors(x => x
     .SetIsOriginAllowed(origin => true)
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials());

app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.UseStaticFiles();

app.UseDefaultFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();
