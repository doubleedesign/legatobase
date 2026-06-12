using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Legatobase.Core;

public partial class LbContextBase : DbContext
{
    public LbContextBase(DbContextOptions<LbContextBase> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumTrackConnection> AlbumsTracks { get; set; }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<ArtistGroupConnection> ArtistsGroups { get; set; }

    public virtual DbSet<ArtistTrackConnection> ArtistsTracks { get; set; }

    public virtual DbSet<FileType> FileTypes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Person> Peoples { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Track> Tracks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Albums_Id").IsUnique();

            entity.Property(e => e.Barcode).HasColumnType("VARCHAR");
            entity.Property(e => e.ExternalPlaycount).HasColumnName("external_playcount");
            entity.Property(e => e.MasterId).HasColumnType("VARCHAR");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.ReleaseGroupId).HasColumnType("VARCHAR");
            entity.Property(e => e.Title).HasColumnType("VARCHAR");

            entity.HasOne(d => d.ReleaseArtist).WithMany(p => p.Albums)
                .HasForeignKey(d => d.ReleaseArtistId)
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
            entity.HasIndex(e => e.Id, "IX_Artists_Id").IsUnique();

            entity.Property(e => e.BirthDate).HasColumnType("DATE");
            entity.Property(e => e.Country).HasColumnType("VARCHAR");
            entity.Property(e => e.DeathDate).HasColumnType("DATE");
            entity.Property(e => e.Did)
                .HasColumnType("VARCHAR")
                .HasColumnName("DID");
            entity.Property(e => e.Hometown).HasColumnType("VARCHAR");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.Name).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<ArtistGroupConnection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Artists_Groups");

            entity.Property(e => e.MembershipEnd).HasColumnType("VARCHAR");
            entity.Property(e => e.MembershipStart).HasColumnType("VARCHAR");

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

            entity.HasOne(d => d.Artist).WithMany()
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Track).WithMany()
                .HasForeignKey(d => d.TrackId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FileType>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_FileTypes_Id").IsUnique();

            entity.Property(e => e.Label).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Genres_Id").IsUnique();

            entity.Property(e => e.Label).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Groups_Id").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndedDate).HasColumnType("DATE");
            entity.Property(e => e.FoundedDate).HasColumnType("DATE");
            entity.Property(e => e.Origin).HasColumnType("VARCHAR");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Group)
                .HasForeignKey<Group>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("People");

            entity.HasIndex(e => e.Id, "IX_People_Id").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BirthDate).HasColumnType("VARCHAR");
            entity.Property(e => e.DeathDate).HasColumnType("VARCHAR");
            entity.Property(e => e.Hometown).HasColumnType("VARCHAR");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Person)
                .HasForeignKey<Person>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Roles_Id").IsUnique();

            entity.Property(e => e.Label).HasColumnType("VARCHAR");
        });

        modelBuilder.Entity<Track>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Tracks_Id").IsUnique();

            entity.Property(e => e.FileLocation).HasColumnType("VARCHAR");
            entity.Property(e => e.Isrc)
                .HasColumnType("VARCHAR")
                .HasColumnName("ISRC");
            entity.Property(e => e.Iswc)
                .HasColumnType("VARCHAR")
                .HasColumnName("ISWC");
            entity.Property(e => e.Mbid)
                .HasColumnType("VARCHAR")
                .HasColumnName("MBID");
            entity.Property(e => e.ShsId).HasColumnName("SHS_ID");
            entity.Property(e => e.Title).HasColumnType("VARCHAR");

            entity.HasOne(d => d.Genre).WithMany(p => p.Tracks).HasForeignKey(d => d.GenreId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
