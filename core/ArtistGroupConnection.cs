using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class ArtistGroupConnection
{
    public int ArtistId { get; set; }

    public int GroupId { get; set; }

    public string? MembershipStart { get; set; }

    public string? MembershipEnd { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Artist Group { get; set; } = null!;
}
