namespace BananaTracks.Domain.Entities;

public class TimeOff : EntityBase
{
	public Guid TeamId { get; set; }
	public Guid UserId { get; set; }

	public string UserName { get; set; } = "";

	public TimeOffStatus Status { get; set; }
	public TimeOffType Type { get; set; }
	public DateOnly Date { get; set; }

	public DateTime? ApprovedAt { get; set; }
	public Guid? ApprovedBy { get; set; }

	private TimeOff()
	{
		// required for EF
	}

	public TimeOff(Guid tenantId) : base(tenantId)
	{
	}
}
