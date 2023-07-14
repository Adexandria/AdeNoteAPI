using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TasksLibrary.Architecture.Application;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("NotesDB");
var tokenSecret = configuration["TokenSecret"];
// Add services to the container.

var containerBuilder = new TaskContainerBuilder(connectionString);

builder.Services.AddScoped<ITaskApplication, TaskApplication>();
builder.Services.AddScoped<IUserIdentity, UserIdentity>();
builder.Services.AddScoped((o) => containerBuilder.Build());
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository));
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IPageService, PageService>();
builder.Services.AddScoped<ILabelService, LabelService>();
builder.Services.AddDbContext<NoteDbContext>(options => options
.UseSqlServer(connectionString));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret))
               };
           });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
