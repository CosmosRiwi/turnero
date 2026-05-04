using System;
using System.Collections.Generic;

namespace SistemaTurnos.Models;

public partial class TurnHistory
{
    public int Id { get; set; }

    public int TurnId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Turn Turn { get; set; } = null!;
}
