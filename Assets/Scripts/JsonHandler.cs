using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Data;
using Palmmedia.ReportGenerator.Core.Common;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Newtonsoft.Json.Linq;
using JsonSubTypes;
using UnityEngine.Analytics;

public class JsonHandler : MonoBehaviour
{
    public static void JsonSave(Part part)
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new Vec2JsonConverter());
        // serializer.Converters.Add(new JsonFeature2JsonConverter());
        serializer.Formatting = Formatting.Indented;

        StringBuilder jsonString = new StringBuilder();
        StringWriter sw = new StringWriter(jsonString);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, part.ToJsonPart());
        }

        print(jsonString);
    }
    public static void JsonLoad(TextAsset jsonAsset) 
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new Vec2JsonConverter());
        serializer.Converters.Add(new JsonFeature2JsonConverter());

        TextReader textReader = new StringReader(jsonAsset.ToString());
        using (JsonTextReader JsonReader = new JsonTextReader(textReader))
        {
            JsonPart Jsonpart = serializer.Deserialize<JsonPart>(JsonReader); 
            print(Jsonpart);

            Part part = Jsonpart.ToPart();
            print(part);
        }

    }
}

public class JsonFeature2JsonConverter : JsonConverter<JsonFeature>
{
    public override JsonFeature ReadJson(JsonReader reader, Type objectType, JsonFeature existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        uint id = jsonObject["id"].Value<uint>();

        JsonFeature jsonFeature;

        string type = jsonObject["Type"].Value<string>();

        switch(type)
        {
            case "extrude" :
            {
                uint sketchID = jsonObject["baseSketch"].Value<uint>();
                float height = jsonObject["extrusionHeight"].Value<float>();
                jsonFeature = new JsonExtrude(height, sketchID, id);
                return jsonFeature;
            }
        }

        return null;
    }
    public override void WriteJson(JsonWriter writer, JsonFeature feature, JsonSerializer serializer)
    {
        serializer.Serialize(writer, feature);
    }
}

internal class Vec2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartArray)
            throw new JsonException("Expected ArrayStart token");

        float x = (float)reader.ReadAsDouble();
        float y = (float)reader.ReadAsDouble();

        reader.Read();
        if(reader.TokenType != JsonToken.EndArray)
            throw new JsonException("Expected ArrayEnd token");

        return new Vector2(x, y);            
    }
    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        writer.WriteStartArray();
        writer.WriteValue(value.x.ToString());
        writer.WriteValue(value.y.ToString());
        writer.WriteEndArray();
    }
}