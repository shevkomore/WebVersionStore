﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebVersionStore.Models.Database;

public partial class UserRepositoryAccess
{
    public string UserLogin { get; set; }

    public Guid RepositoryId { get; set; }

    public bool CanView { get; set; }

    public bool CanEdit { get; set; }

    public bool CanAdd { get; set; }

    public bool CanRemove { get; set; }

    public virtual Repository Repository { get; set; }

    public virtual User UserLoginNavigation { get; set; }
}