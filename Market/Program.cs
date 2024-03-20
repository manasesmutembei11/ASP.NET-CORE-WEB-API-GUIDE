using Contracts;
using Market.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Market.ActionFilters;
using Service.DataShapping;
using Shared.DataTransferObjects;
using Market.Presentation.ActionFilters;
using Market.Utility;
using AspNetCoreRateLimit;
var builder = WebApplication.CreateBuilder(args);
LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(),
"/nlog.config"));

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
.Services.BuildServiceProvider()
.GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
.OfType<NewtonsoftJsonPatchInputFormatter>().First();

builder.Services.ConfigureHttpCacheHeaders();

builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();




// Add services to the container.
builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddControllers(config => {
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
    {
        Duration = 120
    });

}).AddXmlDataContractSerializerFormatters()
 .AddCustomCSVFormatter()
.AddApplicationPart(typeof(Market.Presentation.AssemblyReference).Assembly);

builder.Services.AddCustomMediaTypes();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();








// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
    app.UseHsts();


if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseIpRateLimiting();
app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseHttpCacheHeaders();



app.UseAuthorization();

app.MapControllers();

app.Run();
