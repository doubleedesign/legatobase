using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class ArtistGroup
{
    public int ArtistId { get; set; }

    public int GroupId { get; set; }

    public virtual Artist Artist { get; set; } = null!;

    public virtual Artist Group { get; set; } = null!;
}
