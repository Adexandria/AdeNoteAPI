using AdeText.Models;
using AdeText.Utilities;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;


namespace AdeText.Services
{
    internal class JsonExtractorService : IJsonExtractorService
    {
        public T ExtractJson<T>(Stream json)
            where T: class, new() 
        {
            T model = new();
            var data = JsonNode.Parse(json).AsObject();
            try
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    object value;
                    var attribute = property.GetCustomAttribute<TranslationPropertyAttribute>()
                        ?? throw new Exception("Invalid attribute");

                    var jsonNode = data.FirstOrDefault(s => s.Key == attribute?.Name).Value;

                    if (jsonNode is not JsonObject)
                    {
                        continue;
                    }
                    var translations = jsonNode.AsObject();

                    var sourceType = Activator.CreateInstance(attribute.SourceType);

                    ExtractValues(translations, sourceType, sourceType.GetType().GetProperty("Languages"));

                    value = sourceType;

                    property.SetValue(model, value);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting fields: {ex.Message}");
            }
            return model;

        }

        private void ExtractValues(JsonObject currentJson, object model, PropertyInfo property)
        {
            var languages = new Dictionary<string, string>();
            foreach (var node in currentJson)
            {
                var nodeKey = node.Key;

                var currentNode = node.Value;

                string? extractedValue = string.Empty;

                if(currentNode is not JsonObject)
                {
                    continue;
                }

                var jsonObject = currentNode.AsObject();

                var scripts = jsonObject["scripts"];

                if(scripts is JsonArray jsonArray)
                {
                    var nodeValue = jsonArray[0]
                        .AsObject()
                        .FirstOrDefault(s => s.Key == "code").Value;

                    extractedValue = nodeValue.GetValue<string>();
                }
                else
                {
                    extractedValue = jsonObject["name"]?.GetValue<string>();
                }

                if (string.IsNullOrEmpty(extractedValue))
                {
                    continue;
                }


                if (languages.ContainsKey(extractedValue))
                {
                    continue;
                }
                languages.Add(extractedValue, nodeKey);
            }

            property.SetValue(model, languages);
        }
    }


}
