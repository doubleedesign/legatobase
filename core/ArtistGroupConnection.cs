using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class ArtistGroupConnection
{
    public int RelationId { get; set; }

    public int ArtistId { get; set; }

    public int GroupId { get; set; }

    public string? MembershipStart { get; set; }

    public string? MembershipEnd { get; set; }

    public virtual Person Artist { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;
}
