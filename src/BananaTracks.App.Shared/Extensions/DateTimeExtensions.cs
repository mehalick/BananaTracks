using System.Globalization;
using Google.Protobuf.WellKnownTypes;

namespace BananaTracks.App.Shared.Extensions;

public static class DateTimeExtensions
{
	public static Timestamp ToTimestamp(this DateOnly date)
	{
		var dateTime = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

		return Timestamp.FromDateTime(dateTime);
	}

	public static string ToSortableDate(this Timestamp timestamp)
	{
		return timestamp.ToDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
	}
}
