using System;
using System.Collections.Generic;
using Dominio.Entidades.Models;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia.Contexto;
public partial class SistemaReservasContext : DbContext
{
    public SistemaReservasContext()
    {
    }

    public SistemaReservasContext(DbContextOptions<SistemaReservasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppConfiguracion> AppConfiguracions { get; set; }

    public virtual DbSet<DiaHabilitado> DiaHabilitados { get; set; }

    public virtual DbSet<Estacion> Estacions { get; set; }

    public virtual DbSet<Log> Logs { get; set; }


    public virtual DbSet<Planificacion> Planificacions { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<TokenCancelacion> TokenCancelacions { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=SistemaReservas;TrustServerCertificate=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppConfiguracion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AppConfi__3214EC07025AADBB");

            entity.ToTable("AppConfiguracion");
        });

        modelBuilder.Entity<DiaHabilitado>(entity =>
        {
            entity.HasKey(e => e.DiaHabilitadoId).HasName("PK__DiaHabil__1BFD90531A6CE197");

            entity.ToTable("DiaHabilitado");

            entity.HasIndex(e => new { e.PlanificacionId, e.Fecha }, "UQ_DiaHabilitado_Fecha").IsUnique();

            entity.HasOne(d => d.Planificacion).WithMany(p => p.DiaHabilitados)
                .HasForeignKey(d => d.PlanificacionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiaHabilitado_Planificacion");
        });

        modelBuilder.Entity<Estacion>(entity =>
        {
            entity.HasKey(e => e.EstacionId).HasName("PK__Estacion__D9998F1F4CD05F1E");

            entity.ToTable("Estacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.DiaHabilitado).WithMany(p => p.Estacions)
                .HasForeignKey(d => d.DiaHabilitadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estacion_DiaHabilitado");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Log__5E548648ACBF4BD2");

            entity.ToTable("Log");

            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

  

        modelBuilder.Entity<Planificacion>(entity =>
        {
            entity.HasKey(e => e.PlanificacionId).HasName("PK__Planific__F5D21E6356B86D82");

            entity.ToTable("Planificacion");

            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Planificacions)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Planifica__IdUsu__48CFD27E");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.ReservaId).HasName("PK__Reserva__C3993763A29252CA");

            entity.ToTable("Reserva");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pediente");
            entity.Property(e => e.FechaExpiracion).HasColumnType("datetime");
            entity.Property(e => e.FechaReserva)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdEstacionNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEstacion)
                .HasConstraintName("FK__Reserva__IdEstac__6477ECF3");

            entity.HasOne(d => d.Slot).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__SlotId__60A75C0F");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reserva__Usuario__5FB337D6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC070B8DEEB5");

            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Slot__0A124AAF7B2564E2");

            entity.ToTable("Slot");

            entity.HasIndex(e => e.PublicId, "UQ__Slot__87F1F399BA713BB5").IsUnique();

            entity.Property(e => e.PublicId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.IdDiaHabilitadoNavigation).WithMany(p => p.Slots)
                .HasForeignKey(d => d.IdDiaHabilitado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Slot__IdDiaHabil__5812160E");
        });

        modelBuilder.Entity<TokenCancelacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TokenCan__3214EC076DD5C66F");

            entity.ToTable("TokenCancelacion");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

            entity.HasOne(d => d.Reserva).WithMany(p => p.TokenCancelacions)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TokenCanc__Reser__70DDC3D8");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.TurnoId).HasName("PK__Turno__AD3E2E9482CCBD95");

            entity.ToTable("Turno");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDiaHabilitadoNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.IdDiaHabilitado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Turno_Estacion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC075AE3CC9B");

            entity.HasIndex(e => e.PublicId, "UQ__Usuarios__87F1F399214DA23E").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105346B8811F9").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HashContrasena).HasMaxLength(255);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PublicId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__IdRol__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
