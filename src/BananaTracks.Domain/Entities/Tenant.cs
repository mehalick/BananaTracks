namespace BananaTracks.Domain.Entities;

public class Tenant
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Name { get; set; } = "";

	public string Host { get; set; } = "";
}
