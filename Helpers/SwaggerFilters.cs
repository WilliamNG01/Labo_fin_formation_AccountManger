using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Labo_fin_formation.Helpers
{
    public class SwaggerFilters
    {
        public class EmptyStringSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (schema.Type == "string")
                {
                    schema.Example = new OpenApiString("");
                }
            }
        }

        public class DefaultBooleanSchemaFilter : ISchemaFilter
        {
            public void Apply(OpenApiSchema schema, SchemaFilterContext context)
            {
                if (schema.Type == "boolean")
                {
                    schema.Default = new OpenApiBoolean(false);
                }
            }
        }
    }
}
