using System;
using System.Collections.Generic;

namespace core;

public partial class ArtistTrackConnection
{
    public int TrackId { get; set; }

    public int ArtistId { get; set; }

    public int AristTypeId { get; set; }

    public virtual ArtistType AristType { get; set; } = null!;

    public virtual Artist Artist { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
