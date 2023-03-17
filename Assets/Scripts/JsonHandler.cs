using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;

public class JsonHandler : MonoBehaviour
{
    // [SerializeField] private SketchEditor sketchEditor;
    public static void JsonSave(Part part)
    {
        JsonPart jsonPart = Part.Part2JsonPart(part);
        var jsonSerializer = new JsonSerializer();
        jsonSerializer.Converters.Add(new Vec2JsonConverter());
        jsonSerializer.Converters.Add(new JsonPart2JsonConverter());
        // jsonSerializer.Converters.Add(new Sketch2JsonCopnverter());
        jsonSerializer.Formatting = Formatting.Indented;
        // jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;


        StringBuilder jsonString = new StringBuilder();
        StringWriter sw = new StringWriter(jsonString);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            jsonSerializer.Serialize(writer, jsonPart);
        }

        print(jsonString);
    }
}

public class Vec2JsonConverter : JsonConverter<Vector2>
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

public class JsonPart2JsonConverter : JsonConverter<JsonPart>
{
    public override JsonPart ReadJson(JsonReader reader, Type objectType, JsonPart existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, JsonPart value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("meta");
        serializer.Serialize(writer, value.PartInfo);
        

        writer.WritePropertyName("data");
        writer.WriteStartObject();
        writer.WritePropertyName("sketches");

        serializer.Serialize(writer, value.Sketches);

        //writer.WriteStartObject();
        //foreach (Sketch sketch in value.Sketches)
        //{
        //    writer.WritePropertyName(sketch.Name);
        //    writer.WriteStartObject();
        //    writer.WritePropertyName("points");
        //    serializer.Serialize(writer, sketch.Points);
        //    writer.WritePropertyName("lines");
        //    serializer.Serialize(writer, sketch.Lines);
        //    writer.WritePropertyName("constraints");
        //    serializer.Serialize(writer, sketch.Constraints);
        //    writer.WriteEndObject();
        //}
        //writer.WriteEndObject();


        writer.WritePropertyName("features");
        serializer.Serialize(writer, value.JsonFeatures);
        writer.WriteEndObject();

        writer.WriteEndObject();
    }
}

public class Sketch2JsonCopnverter : JsonConverter<Sketch>
{
    public override Sketch ReadJson(JsonReader reader, Type objectType, Sketch existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, Sketch value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(value.Name);
        writer.WriteEndObject();
    }
}