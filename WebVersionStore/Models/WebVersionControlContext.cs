using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebVersionStore.Models;

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



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Repository>(entity =>
        {
            entity.Property(e => e.RepositoryId).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Login);

            entity.Property(e => e.Login).HasMaxLength(40);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(20);
        });

        modelBuilder.Entity<UserRepositoryAccess>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserRepositoryAccess");

            entity.HasIndex(e => new { e.UserLogin, e.RepositoryId }, "IX_UserRepositoryAccess");

            entity.Property(e => e.CanAdd).HasDefaultValueSql("((0))");
            entity.Property(e => e.CanEdit).HasDefaultValueSql("((0))");
            entity.Property(e => e.CanRemove).HasDefaultValueSql("((0))");
            entity.Property(e => e.CanView).HasDefaultValueSql("((0))");
            entity.Property(e => e.UserLogin).HasMaxLength(40);

            entity.HasOne(d => d.Repository).WithMany()
                .HasForeignKey(d => d.RepositoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRepositoryAccess_Repositories");

            entity.HasOne(d => d.UserLoginNavigation).WithMany()
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
