using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Abstractions;

public interface ITenantService
{
	Tenant Tenant { get; }
}
