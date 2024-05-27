using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BangchakStationService.Services.RabbitMQ;
using BangchakStationService.Services.UserService;
using BangchakStationService.Consumers;

var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("IdentityDbContext") ?? throw new InvalidOperationException("Connection string 'DotnetAPIAppIdentityDbContextConnection' not found.");

// Add services to the container.

// Add CORS service
builder.Services.AddCors();

// Add DbContext Service
// builder.Services.AddDbContext<MySQLDbContext>();

// add jwt service for validate token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                config =>
                {
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT_KEY").Value!)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                        ClockSkew = TimeSpan.Zero
                    };

                }
            );

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.MaxDepth = 64;
});

// custom service

// add rabbitmq to hosts service
builder.Services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>();

builder.Services.AddScoped<IUserService, UserService>();

// add rabbitmq consumers
builder.Services.AddHostedService<AuthServiceConsumer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // for upload file to folder wwwroot

app.UseAuthentication();

app.UseCors(options => {
    //options.WithOrigins("https://codingthailand.com").AllowAnyMethod();
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
