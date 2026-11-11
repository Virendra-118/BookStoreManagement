//using BookStoreManagement.Config;
//using BookStoreManagement.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Register services
//builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<TokenService>();


//// Register MongoDB configuration and services
//builder.Services.AddSingleton<MongoDbConfig>();
//builder.Services.AddSingleton<BookService>();
//builder.Services.AddSingleton<UserService>();
////builder.Services.AddSingleton<EmailService>();


//var app = builder.Build();

//// Configure Swagger
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();

//    app.UseHttpsRedirection();

//    app.UseAuthorization();

//    app.MapControllers();

//    app.Run();
//}


using BookStoreManagement.Config;
using BookStoreManagement.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddSingleton<MongoDbConfig>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<TokenService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    }); 
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


