using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Track
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Year { get; set; }

    public int? GenreId { get; set; }

    public string? Isrc { get; set; }

    public string? Iswc { get; set; }

    public string? Mbid { get; set; }

    public int? ShsId { get; set; }

    public int? PlayCount { get; set; }

    public string? FileLocation { get; set; }

    public int? FileSize { get; set; }

    public int? FileTypeId { get; set; }

    public int? Length { get; set; }

    public int? BitRate { get; set; }

    public int? SampleRate { get; set; }

    public virtual Genre? Genre { get; set; }
}
