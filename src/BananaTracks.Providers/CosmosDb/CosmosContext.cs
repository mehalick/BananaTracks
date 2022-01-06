using BananaTracks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Providers.CosmosDb;

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
		builder.Entity<User>().Property(i => i.StartDate).HasConversion(new DateOnlyValueConverter());
		builder.Entity<User>().Property(i => i.ResetDate).HasConversion(new DateOnlyValueConverter());

		base.OnModelCreating(builder);
	}
}
