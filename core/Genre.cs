using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Genre
{
    public int Id { get; set; }

    public string Label { get; set; } = null!;

    public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();
}
