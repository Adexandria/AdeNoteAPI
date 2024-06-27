using AdeAuth.Services;
using AdeNote.Db;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.AI;
using AdeNote.Infrastructure.Utilities.HealthChecks;
using AdeNote.Models;
using AdeText;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using DocBuilder.Services;
using Excelify.Services;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.AddEnvironmentVariables().Build();
var azureAd = configuration.GetSection("AzureAd");
var textClientConfiguration = configuration.GetSection("TextTranslationConfiguration").Get<TranslateConfiguration>();
var hangfireUserConfiguration = configuration.GetSection("HangfireUser").Get<HangFireUserConfiguration>();
var defaultConfiguration = configuration.GetSection("DefaultConfiguration").Get<DefaultConfiguration>();
// Gets the connection string from appsettings
var connectionString = configuration.GetConnectionString("NotesDB");


// Gets the token secret from appsettings
var tokenSecret = configuration["TokenSecret"];


builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

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
builder.Services.ConfigureOptions<SwaggerOptions>();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<ILabelPageRepository, LabelPageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository,RefreshRepository>();
builder.Services.AddScoped<ITicketRepository,TicketRepository>();
builder.Services.AddScoped<IRecoveryCodeRepository,RecoveryCodeRepository>();
builder.Services.AddScoped<ITicketService,TicketService>();
builder.Services.AddScoped<IStatisticsService, StatisticService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IHangfireUserRepository, HangfireUserRepository>();
builder.Services.AddScoped<IExcel, AdeNote.Infrastructure.Services.ExcelService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IWordService,WordService>();
builder.Services.AddScoped<ITextTranslation, TextTranslation>();
builder.Services.AddScoped<IAuthorizationHandler,RoleRequirementHandler>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped((_) => DocFactory.CreateService());
builder.Services.AddSingleton((_) => AuthFactory.CreateService().PasswordManager);
builder.Services.AddSingleton((_) => AuthFactory.CreateService().TokenProvider);
builder.Services.AddSingleton((_) => AuthFactory.CreateService().MfaService);
builder.Services.AddSingleton((_) => AdeTextFactory.BuildClient(textClientConfiguration));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IUserIdentity, UserIdentity>();
builder.Services.AddSingleton((_) => new ExcelifyFactory());
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("NotesDB")));


builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("CheckDatabase")
    .AddCheck<APIHealthCheck>("CheckAPI")
    ;

builder.Services.AddHangfireServer();
builder.Services.AddDbContext<NoteDbContext>(options => options
.UseSqlServer(connectionString));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ClockSkew = TimeSpan.Zero,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
                   SaveSigninToken = true
               };
           }).AddJwtBearer("SSO", options =>
           {
               options.SaveToken = true;
               options.MetadataAddress = "https://login.microsoftonline.com/organizations/v2.0/.well-known/openid-configuration";
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   NameClaimType = "name",
                   ValidAudience = azureAd.GetValue<string>("Audience"),
                   ValidIssuer = $"{azureAd.GetValue<string>("Instance")}{azureAd.GetValue<string>("TenantId")}/v2.0",
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidateLifetime = true
               };
           });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.Requirements.Add(new RoleRequirement(Role.User));
    });

    options.AddPolicy("Owner", policy =>
    {
        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        policy.AddRequirements(new RoleRequirement(Role.Admin,Role.SuperAdmin));
    });

    options.AddPolicy("sso",new AuthorizationPolicyBuilder("SSO").RequireAuthenticatedUser().Build());
});
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
}
);



new Language(builder.Services.BuildServiceProvider()).GetLanguages();

var app = builder.Build();

// Configure the HTTP request pipeline.
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();



app.UseSwagger();
app.UseSwaggerUI(setupAction =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    {
        setupAction.SwaggerEndpoint(
                       $"/swagger/{description.GroupName}/swagger.json",
                       description.GroupName.ToUpperInvariant());
    }
});
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire", new DashboardOptions 
{ 
  Authorization = new[] {
      new DashboardAuthenticationFilter(builder.Services.BuildServiceProvider())
  },
  DashboardTitle = "AdeNote API",
  DarkModeEnabled = true,
  FaviconPath = "icon/download.ico",
  AppPath = "swagger/index.html"
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.CreateTables();
app.SeedHangFireUser(hangfireUserConfiguration);
app.SeedSuperAdmin(defaultConfiguration);
app.Run();

