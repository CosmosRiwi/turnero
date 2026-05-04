using System;
using System.Collections.Generic;

namespace SistemaTurnos.Models;

public partial class User
{
    public int Id { get; set; }

    public string Dni { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Turn> Turns { get; set; } = new List<Turn>();
}
