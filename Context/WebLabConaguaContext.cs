using System;
using System.Collections.Generic;
using API_WebLabCon_test.Models;
using Microsoft.EntityFrameworkCore;

namespace API_WebLabCon_test.Context;

public partial class WebLabConaguaContext : DbContext
{
    public WebLabConaguaContext()
    {
    }

    public WebLabConaguaContext(DbContextOptions<WebLabConaguaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dato> Datos { get; set; }

    public virtual DbSet<Estacione> Estaciones { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=10.20.55.34;initial catalog=WebLabConagua; user=sa; password=blackhorse851.; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dato>(entity =>
        {
            entity.HasKey(e => e.IdDato).HasName("PK__datos__95283FC2AD9E6DA6");

            entity.ToTable("datos");

            entity.Property(e => e.IdDato).HasColumnName("id_dato");
            entity.Property(e => e.Dirv).HasColumnName("dirv");
            entity.Property(e => e.Estacion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("estacion");
            entity.Property(e => e.Eto).HasColumnName("eto");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Humh).HasColumnName("humh");
            entity.Property(e => e.Humr).HasColumnName("humr");
            entity.Property(e => e.Prec).HasColumnName("prec");
            entity.Property(e => e.Rad).HasColumnName("rad");
            entity.Property(e => e.Temp).HasColumnName("temp");
            entity.Property(e => e.Velv).HasColumnName("velv");

            entity.HasOne(d => d.EstacionNavigation).WithMany(p => p.Datos)
                .HasForeignKey(d => d.Estacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__datos__estacion__5165187F");
        });

        modelBuilder.Entity<Estacione>(entity =>
        {
            entity.HasKey(e => e.IdEstacion).HasName("PK__estacion__1F3B45EBD90330FB");

            entity.ToTable("estaciones");

            entity.Property(e => e.IdEstacion)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("id_estacion");
            entity.Property(e => e.Altitud).HasColumnName("altitud");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Latitud).HasColumnName("latitud");
            entity.Property(e => e.Longitud).HasColumnName("longitud");
            entity.Property(e => e.Municipio)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.NombreEstacion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre_estacion");

            entity.HasOne(d => d.MunicipioNavigation).WithMany(p => p.Estaciones)
                .HasForeignKey(d => d.Municipio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__estacione__munic__4E88ABD4");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__estado__86989FB20542BAEF");

            entity.ToTable("estado");

            entity.Property(e => e.IdEstado)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("id_estado");
            entity.Property(e => e.Nestado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nestado");
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio).HasName("PK__municipi__01C9EB9940FCD82A");

            entity.ToTable("municipio");

            entity.Property(e => e.IdMunicipio)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("id_municipio");
            entity.Property(e => e.Estado)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Nmunicipio)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nmunicipio");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Municipios)
                .HasForeignKey(d => d.Estado)
                .HasConstraintName("FK__municipio__estad__267ABA7A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
