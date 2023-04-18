using Microsoft.OpenApi.Models;

namespace Customer.Service.Ignitions
{
    public static class SwaggerIgnition
    {
        public static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = builder.Configuration["SwaggerUI:Title"],
                    Description = builder.Configuration["SwaggerUI:Description"],
                    License = new OpenApiLicense
                    {
                        Name = builder.Configuration["SwaggerUI:Licence:Copyright"],
                        Url = new Uri(builder.Configuration["SwaggerUI:Licence:Url"]!)
                    },
                });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "customer_service.xml"));
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.OrderActionsBy(a => a.HttpMethod);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "Bearer ......",
                    In = ParameterLocation.Header,
                    Description = "This API expects a valid JWT token in a header called \"token\".<br/> This enables the token used to access the API to remain secret when accessing the API over HTTPS. The example below shows what a token header should look like. When accessing the API without the correct permissions a 403 Forbidden response will be returned.",
                    Name = "Authorization",

                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new[] { "readAccess", "writeAccess" }
                    }
                });
              
                options.DescribeAllParametersInCamelCase();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
             
            });
        }
    }
}
