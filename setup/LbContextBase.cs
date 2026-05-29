using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace setup;

public partial class LbContextBase : DbContext
{
    public LbContextBase(DbContextOptions<LbContextBase> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumTrackConnection> AlbumsTracks { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<ArtistGroup> ArtistGroups { get; set; }

    public virtual DbSet<ArtistTrackConnection> ArtistsTracks { get; set; }

    public virtual DbSet<ArtistType> ArtistTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.Property(e => e.Barcode).HasColumnType("VARCHAR");
            entity.Property(e => e.ExternalPlaycount).HasColumnName("external_playcount");
            entity.Property(e => e.MasterId).HasColumnType("VARCHAR");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.Title).HasColumnType("VARCHAR");

            entity.HasOne(d => d.Artist).WithMany(p => p.Albums)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AlbumTrackConnection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Albums_Tracks");

            entity.HasOne(d => d.Album).WithMany()
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Track).WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity.Property(e => e.Birthdate).HasColumnType("DATETIME");
            entity.Property(e => e.Country).HasColumnType("VARCHAR");
            entity.Property(e => e.Deathdate).HasColumnType("DATETIME");
            entity.Property(e => e.Did)
                .HasColumnType("VARCHAR")
                .HasColumnName("DID");
            entity.Property(e => e.Home).HasColumnType("VARCHAR");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.Name).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<ArtistGroup>(entity =>
        {
            entity.HasNoKey();

            entity.HasOne(d => d.Artist).WithMany()
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Group).WithMany()
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArtistTrackConnection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Artists_Tracks");

            entity.HasOne(d => d.AristType).WithMany()
                .HasForeignKey(d => d.AristTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Artist).WithMany()
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Track).WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArtistType>(entity =>
        {
            entity.Property(e => e.Label).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.Label).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.Property(e => e.Isrc)
                .HasColumnType("VARCHAR")
                .HasColumnName("ISRC");
            entity.Property(e => e.Iswc)
                .HasColumnType("VARCHAR")
                .HasColumnName("ISWC");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.Playcount).HasColumnName("playcount");
            entity.Property(e => e.ShsId).HasColumnName("SHS_ID");
            entity.Property(e => e.Title).HasColumnType("VARCHAR");

            entity.HasOne(d => d.Genre).WithMany(p => p.Tracks).HasForeignKey(d => d.GenreId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
