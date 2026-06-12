using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Mbid { get; set; }

    public string? Did { get; set; }

    public string? Profile { get; set; }

    public string? Hometown { get; set; }

    public string? Country { get; set; }

    public DateTime? BirthDate { get; set; }

    public DateTime? DeathDate { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual Group? Group { get; set; }

    public virtual Person? Person { get; set; }
}
