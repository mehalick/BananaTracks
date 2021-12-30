using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BananaTracks.Providers;

internal class CosmosContext : DbContext, ICosmosContext
{
	public DbSet<Team> Teams { get; set; } = null!;
	public DbSet<User> Users { get; set; } = null!;

	public CosmosContext(DbContextOptions options) : base(options)
	{
		SavingChanges += OnSavingChanges;
	}

	private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
	{
		var entries = ChangeTracker.Entries<EntityBase>()
			.Where(x => x.State is EntityState.Modified)
			.Select(x => x.Entity)
			.ToList();

		foreach (var entry in entries)
		{
			entry.ModifiedAt = DateTime.UtcNow;
		}
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		//builder.ApplyConfigurationsFromAssembly(typeof(CosmosContext).Assembly);

		builder.Entity<Team>().SetDefaults();
		builder.Entity<User>().SetDefaults();

		var converter = new ValueConverter<DateOnly, string>(
			i => $"{i:yyyy-MM-dd}",
			i => DateOnly.ParseExact(i, "yyyy-MM-dd"));

		builder.Entity<User>().Property(i => i.StartDate).HasConversion(converter);
		builder.Entity<User>().Property(i => i.ResetDate).HasConversion(converter);

		base.OnModelCreating(builder);
	}
}
