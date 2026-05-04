using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using SistemaTurnos.Models;

namespace SistemaTurnos.Data;

public partial class MysqlDbContext : DbContext
{
    public MysqlDbContext()
    {
    }

    public MysqlDbContext(DbContextOptions<MysqlDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Turn> Turns { get; set; }

    public virtual DbSet<TurnHistory> TurnHistories { get; set; }

    public virtual DbSet<TurnStatus> TurnStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // NO debe haber código aquí. Si ves un if (!optionsBuilder.IsConfigured), BORRALO.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Priority>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("priorities");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("staff");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Password)
                .HasColumnType("text")
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'Asesor'")
                .HasColumnType("enum('Asesor','Admin')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Turn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turns");

            entity.HasIndex(e => e.PriorityId, "fk_priority_turn");

            entity.HasIndex(e => e.StaffId, "fk_staff_turn");

            entity.HasIndex(e => e.StatusId, "fk_status_turn");

            entity.HasIndex(e => e.UserId, "fk_user_turn");

            entity.HasIndex(e => e.Ticket, "ticket").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.PriorityId).HasColumnName("priority_id");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Ticket)
                .HasMaxLength(20)
                .HasColumnName("ticket");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Priority).WithMany(p => p.Turns)
                .HasForeignKey(d => d.PriorityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_priority_turn");

            entity.HasOne(d => d.Staff).WithMany(p => p.Turns)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("fk_staff_turn");

            entity.HasOne(d => d.Status).WithMany(p => p.Turns)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_status_turn");

            entity.HasOne(d => d.User).WithMany(p => p.Turns)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_turn");
        });

        modelBuilder.Entity<TurnHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turn_history");

            entity.HasIndex(e => e.TurnId, "fk_turn_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.TurnId).HasColumnName("turn_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Turn).WithMany(p => p.TurnHistories)
                .HasForeignKey(d => d.TurnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_turn_history");
        });

        modelBuilder.Entity<TurnStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turn_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Dni)
                .HasMaxLength(20)
                .HasColumnName("dni");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'Activo'")
                .HasColumnType("enum('Activo','Inactivo')")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}