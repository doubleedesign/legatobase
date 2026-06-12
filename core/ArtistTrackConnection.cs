using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class ArtistTrackConnection
{
    public int TrackId { get; set; }

    public int ArtistId { get; set; }

    public int RoleId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
