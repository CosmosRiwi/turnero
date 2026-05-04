using System;
using System.Collections.Generic;

namespace SistemaTurnos.Models;

public partial class Staff
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Turn> Turns { get; set; } = new List<Turn>();
}
