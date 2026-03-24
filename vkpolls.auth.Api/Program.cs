using vkpolls.auth.Api.Middlewares;
using vkpolls.auth.Identity;
using vkpolls.auth.Infrastructure;
using vkpolls.auth.ApiClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddSmsService(builder.Configuration);
builder.Services.AddEmailService(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
