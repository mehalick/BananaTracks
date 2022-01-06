using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BananaTracks.Providers.CosmosDb;

internal class DateOnlyValueConverter : ValueConverter<DateOnly, string>
{
	public DateOnlyValueConverter() : base(i => $"{i:yyyy-MM-dd}", i => DateOnly.ParseExact(i, "yyyy-MM-dd"))
	{
	}
}
