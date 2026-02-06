using Microsoft.OpenApi.Models;
using SmartMicrobus.Core.DTO.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartMicrobus.API.Filters
{
    public class AddInternalServerErrorResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {


            if (operation.Responses == null)
                operation.Responses = new OpenApiResponses();

            if (!operation.Responses.ContainsKey("500"))
            {
                operation.Responses.Add("500", new OpenApiResponse
                {
                    Description = "Unexpected server error occurred.",
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(
                                typeof(ApiResponse),
                                context.SchemaRepository)
                        }
                    }
                });
            }
        }
    }
}
