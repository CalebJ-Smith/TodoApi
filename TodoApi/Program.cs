using TodoApi.Models;
using TodoApi.Controllers;
using TodoApi.DataAccessLayer;
using static TodoApi.DataAccessLayer.InMemoryTodoListRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// for Session access
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

// Repositories
builder.Services.AddSingleton<ITodoListRepository, InMemoryTodoListRepository>();
builder.Services.AddSingleton<ITodoListItemRepository, InMemoryTodoListItemRepository>();

// generic stuff
var myDictionary = 
builder.Services.AddSingleton<IDictionary<long, TodoListItem>>(serviceProvider => new Dictionary<long, TodoListItem>());
builder.Services.AddSingleton<IDictionary<long, IDictionary<long, TodoListItem>>>(
        serviceProvider => new Dictionary<long, IDictionary<long, TodoListItem>>());
builder.Services.AddSingleton<IDictionary<long, TodoListWrapper>>(sp => new Dictionary<long, TodoListWrapper>());
builder.Services.AddSingleton<IDictionary<string, IDictionary<long, TodoListWrapper>>>(sp => new Dictionary<string, IDictionary<long, TodoListWrapper>>());
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
