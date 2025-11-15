using AppChat.Data;
using AppChat.Hubs;
using AppChat.Services;
using AppChat.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;





var builder = WebApplication.CreateBuilder(args);

// DB Connection
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);
Console.WriteLine($"Connection string: {connectionString}");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// PORT 
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//builder.WebHost.UseUrls($"http://*:{port}");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Define Services for Dependency Injection
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MessageService>();

// Define JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JWT for API resquest
        var jwtConfig = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"])),

            ClockSkew = TimeSpan.FromSeconds(Convert.ToDouble(jwtConfig["ClockSkewSeconds"] ?? "30"))
        };

        // JWT for SignalR Hub
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
            }
        };
    }
    );

// CORS Define%
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
        //WithOrigins("http://127.0.0.1:5500")
            //.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// SignalR Hub
builder.Services.AddSignalR(options =>
    {
        options.MaximumReceiveMessageSize = 1024 * 1024 * 30;
    }
);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();  // Comment for deploy test

// Init Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.UseCors("cors");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// SignalR endpoint
app.MapHub<ChatHub>("/chatHub").RequireAuthorization();

app.MapControllers();

app.Run();
