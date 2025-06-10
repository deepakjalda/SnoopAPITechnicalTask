using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask.Utilities
{
    public static class JsonValidator
    {
        /// <summary>
        /// Validates the given JSON string against a JSON schema loaded from a file.
        /// </summary>
        /// <param name="jsonPath">Path to the JSON file to validate.</param>
        /// <param name="schemaPath">Path to the schema file.</param>
        /// <returns>List of validation error messages (empty if valid).</returns>
        public static async Task<IList<string>> ValidateJsonFileAsync(string jsonContent, string schemaPath)
        {
            string schemaContent = await File.ReadAllTextAsync(schemaPath);

            var schema = await JsonSchema.FromJsonAsync(schemaContent);
            var token = JToken.Parse(jsonContent);

            var errors = schema.Validate(token);
            var messages = new List<string>();

            foreach (var error in errors)
            {
                messages.Add($"{error.Path}::{error.Kind} - {error.Property}");
            }

            return messages;
        }
    }
    }
