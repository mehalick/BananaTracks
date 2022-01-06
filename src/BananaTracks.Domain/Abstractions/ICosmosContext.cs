using BananaTracks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Domain.Abstractions;

public interface ICosmosContext
{
	DbSet<Team> Teams { get; set; }
	DbSet<User> Users { get; set; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
