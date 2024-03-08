using Customers.Api.Database;
using Customers.Api.Services;
using Customers.Api.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

var config = builder.Configuration;
config.AddEnvironmentVariables("CustomersApi_");

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation(c => { c.DisableDataAnnotationsValidation = true; });
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(x => x.UseNpgsql("Database:ConnectionString"));
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.MapControllers();

using var migrationScope = app.Services.CreateScope();
var db = migrationScope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();

app.Run();
