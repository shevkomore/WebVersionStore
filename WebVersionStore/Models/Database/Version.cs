using System;
using System.Collections.Generic;

namespace WebVersionStore.Models.Database;

public partial class Version
{
    public Guid RepositoryId { get; set; }

    public Guid VersionId { get; set; }

    public Guid? Parent { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Color { get; set; } = null!;

    public string? ImageLocation { get; set; }

    public string DataLocation { get; set; } = null!;

    public virtual ICollection<Version> InverseParentNavigation { get; set; } = new List<Version>();

    public virtual Version? ParentNavigation { get; set; }

    public virtual Repository Repository { get; set; } = null!;
}
