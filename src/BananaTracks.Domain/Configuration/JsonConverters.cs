using System.Globalization;
using System.Text.Json;

namespace BananaTracks.Domain.Configuration;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
	private const string DateFormat = "yyyy-MM-dd";

	public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString()!;

		return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
	}

	public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
	}
}

public class DateOnlyNullableJsonConverter : JsonConverter<DateOnly?>
{
	private const string DateFormat = "yyyy-MM-dd";

	public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();

		if (string.IsNullOrEmpty(value))
		{
			return null;
		}

		return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
	}

	public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value == null ? string.Empty : value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
	}
}
