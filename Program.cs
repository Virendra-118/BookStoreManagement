
using BookStoreManagement.Config;
using BookStoreManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MongoDB configuration and services
builder.Services.AddSingleton<MongoDbConfig>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();         

app.MapControllers();

app.Run();
