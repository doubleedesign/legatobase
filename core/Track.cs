using System;
using System.Collections.Generic;

namespace core;

public partial class Track
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Year { get; set; }

    public int? GenreId { get; set; }

    public string? Isrc { get; set; }

    public string? Iswc { get; set; }

    public int? ShsId { get; set; }

    public string? Mbid { get; set; }

    public int? Playcount { get; set; }

    public virtual Genre? Genre { get; set; }
}
