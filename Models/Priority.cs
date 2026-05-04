using System;
using System.Collections.Generic;

namespace SistemaTurnos.Models;

public partial class Priority
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Turn> Turns { get; set; } = new List<Turn>();
}
