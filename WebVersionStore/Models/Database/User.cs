using System;
using System.Collections.Generic;

namespace WebVersionStore.Models.Database;

public partial class User
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Username { get; set; } = null!;

    public virtual ICollection<Repository> Repositories { get; set; } = new List<Repository>();

    public virtual ICollection<UserRepositoryAccess> UserRepositoryAccesses { get; set; } = new List<UserRepositoryAccess>();
}
