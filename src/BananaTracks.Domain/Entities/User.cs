namespace BananaTracks.Domain.Entities;

public class User : EntityBase
{
	[Required]
	public string Email { get; private set; } = "";

	[Required]
	public string Name { get; private set; } = "";

	public Claims Claims { get; init; } = Claims.None;

	public Guid TeamId { get; set; }

	public string TeamName { get; set; } = "";

	public DateOnly StartDate { get; set; }
	public DateOnly ResetDate { get; set; }

	private User()
	{
		// required for EF
	}

	public User(Guid tenantId, string email, string name) : base(tenantId)
	{
		Email = email.ToLowerInvariant().Trim();
		Name = name.Trim();
	}
}

public class TimeOff : EntityBase
{
	public Guid TeamId { get; set; }
	public Guid UserId { get; set; }

	public TimeOffStatus Status { get; set; } = TimeOffStatus.Pending;
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

public enum TimeOffType
{
	Planned = 1,
	Unplanned = 2
}

public enum TimeOffStatus
{
	Pending = 0,
	Approved = 1
}
