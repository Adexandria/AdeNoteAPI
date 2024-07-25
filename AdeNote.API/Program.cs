using AdeAuth.Infrastructure;
using AdeNote.Db;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Middlewares;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.AuthenticationFilter;
using AdeNote.Infrastructure.Utilities.HealthChecks;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.AddEnvironmentVariables().Build();
var applicationSettings = configuration.ExtractApplicationSetting();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddApiVersioning(c =>
{
    c.DefaultApiVersion = new ApiVersion(1, 0);
    c.AssumeDefaultVersionWhenUnspecified = true;
    c.ReportApiVersions = true;
    c.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("NotesDB")));

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("CheckDatabase")
    .AddCheck<APIHealthCheck>("CheckAPI");

builder.Services.RegisterTypes(applicationSettings);
builder.Services.RegisterAuthentication(applicationSettings);
builder.Services.RegisterSwaggerDocs();
builder.Services.RegisterAuthorization();

builder.Services.AddHangfireServer();

builder.Services.AddDbContext<NoteDbContext>(options => options
.UseSqlServer(applicationSettings.ConnectionString));

//AuthContainerBuilder
 //   .UseIdentityService<NoteDbContext>((s) => s.UseSqlServer(applicationSettings.ConnectionString));

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

app.UseSwagger();
app.UseSwaggerUI(setupAction =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        setupAction.SwaggerEndpoint(
                       $"/swagger/{description.GroupName}/swagger.json",
                       description.GroupName.ToUpperInvariant());
    }
    setupAction.EnableDeepLinking();
    setupAction.DisplayRequestDuration();
    setupAction.EnableValidator();
    setupAction.ShowExtensions();
});
app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire",new DashboardOptions 
{ 
  Authorization = new[] {
      new DashboardAuthenticationFilter(applicationSettings.HangFireUserConfiguration, loggerFactory)
  },
  DashboardTitle = "AdeNote API",
  DarkModeEnabled = true,
  FaviconPath = "icon/download.ico",
  AppPath = "/swagger/index.html"
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.CreateTables();
app.SeedHangFireUser(applicationSettings.HangFireUserConfiguration);
app.SeedSuperAdmin(applicationSettings.DefaultConfiguration);
 app.SetUpRabbitConfiguration(configuration);
app.ScheduleService(configuration);
app.Run();

