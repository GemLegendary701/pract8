using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pr8.Models;

public partial class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Task__3214EC074B327B3F");

            entity.ToTable("Task");

            entity.Property(e => e.OwnerLogin).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07E0A480BA");

            entity.ToTable("User");

            entity.HasIndex(e => e.Login, "UQ__User__5E55825B2F713D45").IsUnique();

            entity.Property(e => e.Login).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
