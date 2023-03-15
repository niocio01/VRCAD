using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Text;
using System.IO;

public class JsonHandler : MonoBehaviour
{
    [SerializeField] private SketchEditor sketchEditor;
    public void JsonSave()
    {
        var jsonSerializer = new JsonSerializer();
        jsonSerializer.Converters.Add(new Vec2JsonVonverter());
        jsonSerializer.Formatting = Formatting.Indented;
        // jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;


        StringBuilder jsonString = new StringBuilder();
        StringWriter sw = new StringWriter(jsonString);

        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            jsonSerializer.Serialize(writer, sketchEditor.Sketch.Points);
        }

        print(jsonString);
    }
}

public class Vec2JsonVonverter : JsonConverter<Vector2>
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
