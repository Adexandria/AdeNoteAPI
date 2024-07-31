using AdeAuth.Infrastructure;
using AdeAuth.Services;
using AdeCache;
using AdeMessaging;
using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Authentication;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Services.BookSetting;
using AdeNote.Infrastructure.Services.EmailSettings;
using AdeNote.Infrastructure.Services.Excel;
using AdeNote.Infrastructure.Services.Export;
using AdeNote.Infrastructure.Services.LabelSettings;
using AdeNote.Infrastructure.Services.Notification;
using AdeNote.Infrastructure.Services.PageSettings;
using AdeNote.Infrastructure.Services.SmsSettings;
using AdeNote.Infrastructure.Services.Statistics;
using AdeNote.Infrastructure.Services.TicketSettings;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeNote.Infrastructure.Services.UserSettings;
using AdeNote.Infrastructure.Services.Word;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.AI;
using AdeNote.Infrastructure.Utilities.AuthorisationHandler;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Models;
using AdeText;
using DocBuilder.Services;
using AdeCache.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Excelify.Services;

namespace AdeNote.Infrastructure.Extension
{
    public static class ProgramExtension
    {

        public static void RegisterTypes(this IServiceCollection serviceCollection, ApplicationSetting applicationSettings)
        {
            serviceCollection.AddScoped<IBookRepository, BookRepository>();
            serviceCollection.AddScoped<ILabelRepository, LabelRepository>();
            serviceCollection.AddScoped<IPageRepository, PageRepository>();
            serviceCollection.AddScoped<ILabelPageRepository, LabelPageRepository>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IRefreshTokenRepository, RefreshRepository>();
            serviceCollection.AddScoped<ITicketRepository, TicketRepository>();
            serviceCollection.AddScoped<IRecoveryCodeRepository, RecoveryCodeRepository>();
            serviceCollection.AddScoped<ITicketService, TicketService>();
            serviceCollection.AddScoped<IStatisticsService, StatisticService>();
            serviceCollection.AddScoped<IBookService, BookService>();
            serviceCollection.AddScoped<IPageService, PageService>();
            serviceCollection.AddScoped<ILabelService, LabelService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();
            serviceCollection.AddScoped<IBlobService, BlobService>();
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<ISmsService, SmsService>();
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<Services.UserSettings.IUserService, Services.UserSettings.UserService>();
            serviceCollection.AddScoped<IHangfireUserRepository, HangfireUserRepository>();
            serviceCollection.AddScoped<IExcel, Services.Excel.ExcelService>();
            serviceCollection.AddScoped<IExportService, ExportService>();
            serviceCollection.AddScoped<IWordService, WordService>();
            serviceCollection.AddSingleton((_) => AdeTextFactory.BuildClient(applicationSettings.TranslateConfiguration));
            serviceCollection.AddScoped<ITextTranslation, TextTranslation>();
            serviceCollection.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();
            serviceCollection.AddScoped<IFileService, FileService>();
            serviceCollection.AddScoped((_) => DocFactory.CreateService());
            serviceCollection.AddSingleton((x) => MessagingFactory.CreateServices(applicationSettings.Messaging, x.GetRequiredService<ILoggerFactory>()));
            serviceCollection.AddSingleton<LanguageScheduler>();
            serviceCollection.AddSingleton<Scheduler>();
            serviceCollection.AddScoped<IUserIdentity, UserIdentity>();
            serviceCollection.AddSingleton((_) => new ExcelifyFactory());
            serviceCollection.AddSingleton((x) => new CacheFactory().CreateService(applicationSettings.CacheConfiguration
                .SetMemoryCache(x.GetRequiredService<IMemoryCache>())));
        }

        public static void RegisterAuthentication(this IServiceCollection serviceCollection, ApplicationSetting applicationSetting)
        {
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ClockSkew = TimeSpan.Zero,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationSetting.TokenSecret)),
                   SaveSigninToken = true
               };
           }).AddJwtBearer("SSO", options =>
           {
               options.SaveToken = true;
               options.MetadataAddress = "https://login.microsoftonline.com/organizations/v2.0/.well-known/openid-configuration";
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   NameClaimType = "name",
                   ValidAudience = applicationSetting.AzureAdConfiguration.Audience,
                   ValidIssuer = $"{applicationSetting.AzureAdConfiguration.Instance}{applicationSetting.AzureAdConfiguration.TenantId}/v2.0",
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateIssuerSigningKey = true,
                   ValidateLifetime = true
               };
           });
        }

        public static void RegisterSwaggerDocs(this IServiceCollection serviceCollection)
        {
            serviceCollection.ConfigureOptions<SwaggerOptions>();
            serviceCollection.AddSwaggerGen(c =>
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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor}");
            });
        }

        public static void RegisterAuthorization(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.Requirements.Add(new RoleRequirement(Role.User));
                });

                options.AddPolicy("Owner", policy =>
                {
                    policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                    policy.AddRequirements(new RoleRequirement(Role.Admin, Role.SuperAdmin));
                });

                options.AddPolicy("sso", new AuthorizationPolicyBuilder("SSO")
                    .RequireAuthenticatedUser().Build());
            });
        }

    }
}
