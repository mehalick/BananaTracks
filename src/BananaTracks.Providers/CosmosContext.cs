using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Providers;

internal class CosmosContext : DbContext, ICosmosContext
{
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

		builder.Entity<User>().SetDefaults();

		base.OnModelCreating(builder);
	}

	public async Task Initialize()
	{
		await Database.EnsureCreatedAsync();

		//var t = await Users.CountAsync();

		//if (t > 0)
		//{
		//	return;
		//}

		//await AddAsync(new User("Local"));
		//await SaveChangesAsync();
	}
}
