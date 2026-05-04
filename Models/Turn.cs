using System;
using System.Collections.Generic;

namespace SistemaTurnos.Models;

public partial class Turn
{
    public int Id { get; set; }

    public string Ticket { get; set; } = null!;

    public int UserId { get; set; }

    public int? StaffId { get; set; }

    public int StatusId { get; set; }

    public int PriorityId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Priority Priority { get; set; } = null!;

    public virtual Staff? Staff { get; set; }

    public virtual TurnStatus Status { get; set; } = null!;

    public virtual ICollection<TurnHistory> TurnHistories { get; set; } = new List<TurnHistory>();

    public virtual User User { get; set; } = null!;
}
