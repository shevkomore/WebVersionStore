using System;
using System.Collections.Generic;

namespace WebVersionStore.Models;

public partial class Repository
{
    public Guid RepositoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Version> Versions { get; set; } = new List<Version>();
}
