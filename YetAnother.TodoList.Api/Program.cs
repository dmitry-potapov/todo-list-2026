using Microsoft.EntityFrameworkCore;
using YetAnother.TodoList.Api.Extensions;
using YetAnother.TodoList.Application.Repositories;
using YetAnother.TodoList.Infrastructure;

const string allowDevFrontendCorsPolicyName = "allowDevFrontend";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    var frontendHost = builder.Configuration["FrontendHost"]!;
    
    options.AddPolicy(name: allowDevFrontendCorsPolicyName,
                      policy =>
                      {
                          policy.WithOrigins(frontendHost)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(allowDevFrontendCorsPolicyName);

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();