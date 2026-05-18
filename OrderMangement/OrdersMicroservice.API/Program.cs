using OrderMangement.DataAccessLayer;
using OrderMangement.BusinessLogicLayer;
using FluentValidation.AspNetCore;
using OrderMangement.BusinessLogicLayer.HttpClients;
using OrderMangement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Cors
builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(builder =>
  {
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
  });
});

// for the part of the http client 

builder.Services.AddHttpClient<UserMicroserviceClient>(client =>
{
  client.BaseAddress=new Uri($"http://{builder.Configuration["UserMicroServiceName"]}:{builder.Configuration["UserMicroServicePort"]}");
});

builder.Services.AddHttpClient<ProductMicroserviceClient>(client =>
{
  client.BaseAddress=new Uri($"http://{builder.Configuration["ProductsMicroServiceName"]}:{builder.Configuration["ProductsMicroServicePort"]}");
});



var app = builder.Build();

app.UseExceptionHandlingMiddleware();
app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
