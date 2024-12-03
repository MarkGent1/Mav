using AutoFixture.Kernel;
using System.Reflection;
using System.Text.Json;

namespace Mav.Tests.Shared.Builders;

public class CustomContextDataSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo property
            && property.PropertyType == typeof(Dictionary<string, JsonElement>))
        {
            var customContextDataObject = new Dictionary<string, object>() { { "test1", "test1" }, { "test2", true }, { "test3", 123 } };
            var jsonElement = JsonDocument.Parse(JsonSerializer.Serialize(customContextDataObject)).RootElement;
            var customContextData = new Dictionary<string, JsonElement>() { { "customContextData", jsonElement } };
            return customContextData;
        }

        return new NoSpecimen();
    }
}
