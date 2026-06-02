using System;
using System.Collections.Generic;

namespace Legatobase.Core;

public partial class Album
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ArtistId { get; set; }

    public int? Year { get; set; }

    public string? Barcode { get; set; }

    public string? MasterId { get; set; }

    public string? Mbid { get; set; }

    public int? ExternalPlaycount { get; set; }

    public virtual Artist Artist { get; set; } = null!;
}
