using System;
using System.Collections.Generic;

namespace setup;

public partial class AlbumTrackConnection
{
    public int TrackId { get; set; }

    public int AlbumId { get; set; }

    public int? TrackNumber { get; set; }

    public int? DiscNumber { get; set; }

    public virtual Album Album { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
