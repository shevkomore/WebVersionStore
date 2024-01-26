using System;
using System.Collections.Generic;

namespace WebVersionStore.Models.Database;

public partial class Repository
{
    public Guid RepositoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Author { get; set; } = null!;

    public virtual User AuthorNavigation { get; set; } = null!;

    public virtual ICollection<UserRepositoryAccess> UserRepositoryAccesses { get; set; } = new List<UserRepositoryAccess>();

    public virtual ICollection<Version> Versions { get; set; } = new List<Version>();
}
