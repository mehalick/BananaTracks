using BananaTracks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BananaTracks.Providers.CosmosDb;

internal static class CosmosExtensions
{
	public static EntityTypeBuilder<T> SetDefaults<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
	{
		var containerName = typeof(CosmosContext)
			.GetProperties()
			.Where(i => i.PropertyType == typeof(DbSet<T>))
			.Select(i => i.Name)
			.Single();

		builder.ToContainer(containerName);

		builder.HasKey(i => i.Id);
		builder.HasNoDiscriminator();
		builder.HasPartitionKey(i => i.TenantId);

		builder.Property(i => i.Id).ToJsonProperty("id");

		builder.UseETagConcurrency();

		return builder;
	}
}
