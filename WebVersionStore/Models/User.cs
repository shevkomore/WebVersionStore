using System;
using System.Collections.Generic;

namespace WebVersionStore.Models;

public partial class User
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Username { get; set; } = null!;
}
