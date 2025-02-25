using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TreeMage.Json
{
    /// <summary>
    /// <see cref="CultureInfo"/>-JSON間の変換を行います。
    /// </summary>
    public class CultureInfoConverter : JsonConverter<CultureInfo>
    {
        /// <inheritdoc/>
        public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType is JsonTokenType.String) return CultureInfo.GetCultureInfo(Encoding.UTF8.GetString(reader.ValueSpan));
            throw new JsonException();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
