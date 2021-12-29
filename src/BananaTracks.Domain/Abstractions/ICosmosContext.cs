using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Domain.Abstractions;

public interface ICosmosContext
{
	DbSet<Team> Teams { get; set; }
	DbSet<User> Users { get; set; }

	Task Initialize();

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
