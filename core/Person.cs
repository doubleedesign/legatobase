using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Person
{
    public int Id { get; set; }

    public string? Hometown { get; set; }

    public string? BirthDate { get; set; }

    public string? DeathDate { get; set; }

    public virtual Artist IdNavigation { get; set; } = null!;
}
