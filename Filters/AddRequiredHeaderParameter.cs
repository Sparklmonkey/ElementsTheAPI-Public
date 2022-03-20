using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AddRequiredHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "x-elementsrevival-apikey",
            In = ParameterLocation.Header,
            Description = "ApiKey",
            Schema = new OpenApiSchema { Type = "string" },
            Required = true
        });

        //operation.Parameters.Add(new OpenApiParameter
        //{
        //    Name = "authorize",
        //    In = ParameterLocation.Header,
        //    Description = "token",
        //    Schema = new OpenApiSchema { Type = "string" },
        //    Required = true
        //});
    }
}

