using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Group
{
    public int Id { get; set; }

    public string? Origin { get; set; }

    public DateTime? FoundedDate { get; set; }

    public DateTime? EndedDate { get; set; }

    public virtual Artist IdNavigation { get; set; } = null!;
}
