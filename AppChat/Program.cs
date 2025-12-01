using AppChat.Data;
using AppChat.Hubs;
using AppChat.Repositories;
using AppChat.Services;
using AppChat.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration ---
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// --- DB Connection ---
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);
Console.WriteLine($"Connection string: {connectionString}");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
        options.UseSqlServer(connectionString); // For testing local
    else
        options.UseNpgsql(connectionString);  // For deploy 
});

// PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "5047";   // Comment for local testing
builder.WebHost.UseUrls($"http://*:{port}");  // Listening on any url with port from hosting

// --- Controllers + Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==============================
// Dependency Injection 3 tầng
// ==============================

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<ContactService>();

// ==============================
// JWT Authentication
// ==============================
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new Exception("JWT Key missing in configuration");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "myIssuer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "myAudience";
var clockSkewSeconds = Convert.ToDouble(builder.Configuration["Jwt:ClockSkewSeconds"] ?? "30");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.FromSeconds(clockSkewSeconds)
        };

        // JWT for SignalR
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("AUTH FAILED (SignalR): " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// ==============================
// CORS
// ==============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ==============================
// SignalR Hub
// ==============================
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 1024 * 1024 * 30;
    options.EnableDetailedErrors = true;
});


long oneGb = 1L * 1024 * 1024 * 1024;
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = oneGb;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = oneGb;
});

// ==============================
// Build App
// ==============================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

//Init Migration
using (var scope = app.Services.CreateScope())    // Comment for local testing
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseRouting();
app.UseCors("cors");

app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseAuthentication();
app.UseAuthorization();

// SignalR
app.MapHub<ChatHub>("/chatHub").RequireAuthorization();

// Controllers
app.MapControllers();

app.Run();
