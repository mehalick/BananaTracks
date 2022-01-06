using BananaTracks.Domain.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Domain.Extensions;

public static class QueryExtensions
{
	public static async Task<T> SingleOrThrowAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
		where T : class
	{
		var result = await query.SingleOrDefaultAsync(cancellationToken);

		if (result is null)
		{
			throw new NotFoundException<T>();
		}

		return result;
	}
}
