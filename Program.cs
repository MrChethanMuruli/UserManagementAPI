using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
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
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Middleware pipeline configuration
app.UseMiddleware<ErrorHandlingMiddleware>();       // 1. Error handling
app.UseMiddleware<TokenAuthenticationMiddleware>(); // 2. Authentication
app.UseMiddleware<RequestResponseLoggingMiddleware>(); // 3. Logging

app.MapControllers();

app.Run();