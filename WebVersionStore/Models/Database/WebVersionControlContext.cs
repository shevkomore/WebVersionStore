using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebVersionStore.Models.Database;

public partial class WebVersionControlContext : DbContext
{
    public WebVersionControlContext()
    {
    }

    public WebVersionControlContext(DbContextOptions<WebVersionControlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Repository> Repositories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRepositoryAccess> UserRepositoryAccesses { get; set; }

    public virtual DbSet<Version> Versions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=WebVersionStoreDatabase");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Repository>(entity =>
        {
            entity.Property(e => e.RepositoryId).ValueGeneratedNever();
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(20);

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.Repositories)
                .HasForeignKey(d => d.Author)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Repositories_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login);

            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRepositoryAccess>(entity =>
        {
            entity.HasKey(e => new { e.UserLogin, e.RepositoryId }).HasName("PK__UserRepo__7415253E7FA5C71D");

            entity.ToTable("UserRepositoryAccess");

            entity.HasIndex(e => new { e.UserLogin, e.RepositoryId }, "IX_UserRepositoryAccess");

            entity.Property(e => e.UserLogin).HasMaxLength(100);

            entity.HasOne(d => d.Repository).WithMany(p => p.UserRepositoryAccesses)
                .HasForeignKey(d => d.RepositoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRepositoryAccess_Repositories");

            entity.HasOne(d => d.UserLoginNavigation).WithMany(p => p.UserRepositoryAccesses)
                .HasForeignKey(d => d.UserLogin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRepositoryAccess_Users");
        });

        modelBuilder.Entity<Version>(entity =>
        {
            entity.HasKey(e => e.VersionId).HasName("PK__Versions__16C6400FA113B4A9");

            entity.Property(e => e.VersionId).ValueGeneratedNever();
            entity.Property(e => e.Color)
                .HasMaxLength(6)
                .HasDefaultValueSql("('edda0e')")
                .IsFixedLength();
            entity.Property(e => e.DataLocation).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.ImageLocation).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(20);

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.InverseParentNavigation)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK__Versions__Parent__3A81B327");

            entity.HasOne(d => d.Repository).WithMany(p => p.Versions)
                .HasForeignKey(d => d.RepositoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Versions__Reposi__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
