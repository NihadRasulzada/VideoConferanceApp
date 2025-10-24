using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using VideoConferanceApp.Server.Helpers;
using VideoConferanceApp.Server.Hubs;
using VideoConferanceApp.Server.Infrastructure.Data;
using VideoConferanceApp.Server.Infrastructure.Services;
using VideoConferanceApp.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.HttpContext.Request.Path;
            var token = context.Request.Query["access_token"];
            if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/meetinghub"))
                context.Token = token;

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<ITwilioService, TwilioService>();

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5129")
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials(); // Əgər cookie və ya auth header istifadə edirsənsə
    });
});

builder.Services.AddScoped<IGetUsersConnectionIdsByMeetingIdHelper, GetUsersConnectionIdsByMeetingIdHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IMeetingHubHelper, MeetingHubHelper>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapScalarApiReference();

//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");


app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MeetingHub>("/meetinghub");

app.MapControllers();

app.Run();