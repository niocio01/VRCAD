using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using System.IO;
using Editors.FeatureEdit;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Newtonsoft.Json.Linq;
using Editors.PartEdit;

public class JsonHandler : MonoBehaviour
{
    public static void PrintJson(Part part)
    {
        StringBuilder json = JsonCreate(part);
        print(json);
    }
    
    public static void JsonSave(Part part, String path)
    {
        if (!File.Exists(path))
        {
            // File.CreateText(path);
        }

        StringBuilder json = JsonCreate(part);

        StreamWriter streamWriter = new StreamWriter(path, false);
        streamWriter.Write(json);
        streamWriter.Close();
        
        Debug.Log($"Saved Part to File. :{path}");
    }

    private static StringBuilder JsonCreate(Part part)
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

        return jsonString;
    }
    
    public static Part JsonLoad(TextAsset jsonAsset) 
    {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Converters.Add(new Vec2JsonConverter());
        serializer.Converters.Add(new JsonFeature2JsonConverter());

        TextReader textReader = new StringReader(jsonAsset.ToString());
        using JsonTextReader jsonReader = new JsonTextReader(textReader);
        JsonPart jsonPart = serializer.Deserialize<JsonPart>(jsonReader);

        return jsonPart.ToPart();
    }
}

public class JsonFeature2JsonConverter : JsonConverter<JsonFeature>
{
    public override JsonFeature ReadJson(JsonReader reader, Type objectType, JsonFeature existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        string type = jsonObject["Type"].Value<string>();

        switch(type)
        {
            case "Extrude" :
            {
                return JsonExtrude.Deserialize(jsonObject);
            }

            case "Revolve":
            {
                return JsonRevolve.Deserialize(jsonObject);
            }
            default: throw new JsonException("Unknown feature type: " + type); 
        }
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