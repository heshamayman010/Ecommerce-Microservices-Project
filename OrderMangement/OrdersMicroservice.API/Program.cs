using OrderMangement.DataAccessLayer;
using OrderMangement.BusinessLogicLayer;
using FluentValidation.AspNetCore;
using OrderMangement.BusinessLogicLayer.HttpClients;
using OrderMangement.API.Middleware;
using Polly;
using OrderMangement.BusinessLogicLayer.Ploicies;

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

// for the part of the http client  and alsow we will add the fault tolerance using the polly 
builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();
builder.Services.AddTransient<IProductsMicroservicePolicies,ProductsMicroservicePolicies>();


builder.Services.AddHttpClient<UserMicroserviceClient>(client =>
{
  client.BaseAddress=new Uri($"http://{builder.Configuration["UserMicroServiceName"]}:{builder.Configuration["UserMicroServicePort"]}");
})
// here we will get the policy directly from the policies service 
  .AddPolicyHandler(
   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy())
   ;





builder.Services.AddHttpClient<ProductMicroserviceClient>(client =>
{
  client.BaseAddress=new Uri($"http://{builder.Configuration["ProductsMicroServiceName"]}:{builder.Configuration["ProductsMicroServicePort"]}");})
    .AddPolicyHandler(
   builder.Services.BuildServiceProvider().GetRequiredService<IProductsMicroservicePolicies>().GetBulkheadIsolationPolicy())
  .AddPolicyHandler(
   builder.Services.BuildServiceProvider().GetRequiredService<IProductsMicroservicePolicies>().GetFallbackPolicy())
  ;



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
