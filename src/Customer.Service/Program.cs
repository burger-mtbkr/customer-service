using Customer.Service.Ignition;
using Customer.Service.Ignitions;
using Customer.Service.MIddleware;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();

// Swagger
builder.ConfigureSwagger();

builder.ConfigureAuthentication();
builder.ConfigureLogging();

builder.ConfigureDataStore();

builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();

builder.ConfigureDataProtection();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllForAssessment",
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyOrigin()
                .SetIsOriginAllowed(c => true)
                .AllowAnyMethod();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swagger, httpReq) =>
    {
        swagger.Servers = new List<OpenApiServer>
        {
            new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
        };
    });
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{app.Configuration["SwaggerUI:Title"]}");
    c.DocumentTitle = app.Configuration["SwaggerUI:Title"];
    c.EnableValidator(null);
    c.EnableDeepLinking();
    c.DisplayRequestDuration();
});



app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAllForAssessment");

app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseMiddleware<ErrorMiddleware>();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();
