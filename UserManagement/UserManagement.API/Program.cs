
using UserManagement.Infrastructure;
using UserManagement.Core;
using UserManagement.API.Middlewares;
using System.Text.Json.Serialization;
using eCommerce.Core.Mappers;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCoreServices();
builder.Services.AddInfrastructureServices();

builder.Services.AddControllers().AddJsonOptions(options => {
  options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  // this is only used for the converting from the json to the enum value for the gender for example 
});
builder.Services.AddAutoMapper(typeof(AutoMapperHelper).Assembly);

// we must use this for the enabling the validators 
builder.Services.AddFluentValidationAutoValidation();
// for the part of the swagger 
//----------------
//Add API explorer services

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add cors services

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(builder => {
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
  });
});
//-----------------




var app = builder.Build();
app.UseExceptionHandlingMiddleware();

app.UseRouting();

// swagger 
app.UseSwagger(); 
app.UseSwaggerUI(); 
app.UseCors();

//----


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();








