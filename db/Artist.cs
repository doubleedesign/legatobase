using System;
using System.Collections.Generic;

namespace db;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Mbid { get; set; }

    public string? Did { get; set; }

    public string? Profile { get; set; }

    public string? Home { get; set; }

    public string? Country { get; set; }

    public DateTime? Birthdate { get; set; }

    public DateTime? Deathdate { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
