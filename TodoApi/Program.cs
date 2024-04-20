using TodoApi.Models;
using TodoApi.Controllers;
using TodoApi.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// for Session access
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Repositories
builder.Services.AddSingleton<ITodoListRepository, InMemoryTodoListRepository>();
builder.Services.AddSingleton<ITodoListItemRepository, InMemoryTodoListItemRepository>();

//builder.Services.AddScoped<ILimitedDbContext, InMemoryDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.Run();
