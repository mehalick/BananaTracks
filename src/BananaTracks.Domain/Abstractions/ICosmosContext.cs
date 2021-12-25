using Microsoft.EntityFrameworkCore;

namespace BananaTracks.Domain.Abstractions;

public interface ICosmosContext
{
	DbSet<User> Users { get; set; }

	Task Initialize();
}
