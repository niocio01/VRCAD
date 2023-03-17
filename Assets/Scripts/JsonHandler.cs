using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Data;

public class JsonHandler : MonoBehaviour
{
    // [SerializeField] private SketchEditor sketchEditor;
    public static void JsonSave(Part part)
    {
        var jsonSerializer = new JsonSerializer();
        jsonSerializer.Converters.Add(new Vec2JsonConverter());
        jsonSerializer.Converters.Add(new Constraint2JsonConverter());
        jsonSerializer.Converters.Add(new Part2JsonConverter());
        jsonSerializer.Converters.Add(new Feature2JsonConverter());
        jsonSerializer.Formatting = Formatting.Indented;
        // jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;


        StringBuilder jsonString = new StringBuilder();
        StringWriter sw = new StringWriter(jsonString);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            jsonSerializer.Serialize(writer, part);
        }

        print(jsonString);
    }
}

internal class Feature2JsonConverter : JsonConverter<Feature>
{
    public override Feature ReadJson(JsonReader reader, Type objectType, Feature existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Feature feature, JsonSerializer serializer)
    {
        serializer.Serialize(writer, feature.feature2Jsonfeature());
    }
}

internal class Vec2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return new Vector2(0, 0);
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x.ToString());
        writer.WriteValue(value.y.ToString());
        writer.WriteEndArray();
    }
}
internal class Part2JsonConverter : JsonConverter<Part>
{
    public override Part ReadJson(JsonReader reader, Type objectType, Part existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Part part, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("meta");
        serializer.Serialize(writer, part.PartInfo);
        

        writer.WritePropertyName("data");
        writer.WriteStartObject();
        writer.WritePropertyName("sketches");

        serializer.Serialize(writer, part.Sketches);


        writer.WritePropertyName("features");
        serializer.Serialize(writer, part.Features);
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}
internal class Constraint2JsonConverter : JsonConverter<SketchConstraint>
{
    public override SketchConstraint ReadJson(JsonReader reader, Type objectType, SketchConstraint existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, SketchConstraint constraint, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Type");
        writer.WriteValue(constraint.Name);
        writer.WritePropertyName("id");
        writer.WriteValue(constraint.ConstraintID);
        writer.WritePropertyName("parentId");
        writer.WriteValue(constraint.Parent.ID);
        writer.WritePropertyName("childId");
        writer.WriteValue(constraint.Child.ID);
        writer.WriteEndObject();
    }
}