using System.Globalization;
using System.Text;
using System.Text.Json;

namespace TreeMage.Json
{
    public class CultureInfoConverterTest
    {
        public readonly CultureInfoConverter converter;

        public CultureInfoConverterTest()
        {
            converter = new CultureInfoConverter();
        }

        #region Ctors

        [Fact]
        public void Ctor()
        {
            Exception? exception = Record.Exception(() => new CultureInfoConverter());

            Assert.Null(exception);
        }

        #endregion Ctors

        #region Methods

        [Theory]
        [InlineData("ja-JP")]
        [InlineData("en-US")]
        public void Read(string cultureName)
        {
            byte[] json = Encoding.UTF8.GetBytes($"\"{cultureName}\"");
            var reader = new Utf8JsonReader(json);
            reader.Read();
            CultureInfo? culture = converter.Read(ref reader, typeof(CultureInfo), new JsonSerializerOptions());

            Assert.NotNull(culture);
            Assert.Equal(cultureName, culture.Name);
        }

        [Theory]
        [InlineData("ja-JP")]
        [InlineData("en-US")]
        public void Write(string cultureName)
        {
            using var stream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(stream))
            {
                converter.Write(jsonWriter, CultureInfo.GetCultureInfo(cultureName), new JsonSerializerOptions());
            }

            stream.Position = 0;
            using var reader = new StreamReader(stream);
            Assert.Equal($"\"{cultureName}\"", reader.ReadLine());
        }

        #endregion Methods
    }
}
