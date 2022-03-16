using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReversiRestApi.Model
{
    public class BoardJsonConverter : JsonConverter<Color[,]>
    {

        public override Color[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Color[,] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for(int x = 0; x < value.GetLength(0); x++)
            {
                writer.WriteStartArray();
                for(int y = 0; y < value.GetLength(1); y++)
                {
                    writer.WriteStringValue(value[x, y].ToString());
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

    }
}
