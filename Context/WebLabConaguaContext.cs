using System;
using System.Collections.Generic;
using API_WebLabCon_test.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API_WebLabCon_test.Context
{
    public partial class WebLabConaguaContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public WebLabConaguaContext(DbContextOptions<WebLabConaguaContext> options)
         : base(options)
        {
        }

        public virtual DbSet<Dato> Datos { get; set; }

        public virtual DbSet<Estacione> Estaciones { get; set; }

        public virtual DbSet<Estado> Estados { get; set; }

        public virtual DbSet<Municipio> Municipios { get; set; }

        // Elimina el método OnConfiguring o modifícalo para que solo se use en desarrollo
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // No hacer nada si ya está configurado
            if (optionsBuilder.IsConfigured)
                return;

            // Solo para desarrollo/pruebas locales
#if DEBUG
            optionsBuilder.UseSqlServer("https://localhost:7122/api");
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mantener el mismo modelado que tenías antes
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

                entity.HasOne(d => d.EstacionNavigation)
                    .WithMany(p => p.Datos)
                    .HasForeignKey(d => d.Estacion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__datos__estacion__5165187F");
            });

            modelBuilder.Entity<Estacione>(entity =>
            {
                entity.HasKey(e => e.IdEstacion).HasName("PK__estacion__0554E71239998649");

                entity.ToTable("estaciones");

                entity.Property(e => e.IdEstacion).HasColumnName("id_estacion");
                // Nota: No hay propiedad Clave en tu modelo Estacione
                entity.Property(e => e.Longitud).HasColumnName("longitud");
                entity.Property(e => e.Latitud).HasColumnName("latitud");
                entity.Property(e => e.Altitud).HasColumnName("altitud");
                entity.Property(e => e.Municipio).HasColumnName("municipio");
                entity.Property(e => e.NombreEstacion)  // Cambiado de Nombre a NombreEstacion
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("nombre_estacion");  // Ajusta el nombre de columna si es necesario
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)  // Ajusta la longitud según tus necesidades
                    .IsUnicode(false)
                    .HasColumnName("descripcion");

                entity.HasOne(d => d.MunicipioNavigation)
                    .WithMany(p => p.Estaciones)
                    .HasForeignKey(d => d.Municipio)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__estacione__municip__4CA06362");
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.HasKey(e => e.IdEstado).HasName("PK__estados__350649776905B00D");

                entity.ToTable("estado");

                entity.Property(e => e.IdEstado).HasColumnName("id_estado");
                entity.Property(e => e.Nestado)  // Cambiado de Nombre a Nestado
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("nestado");  // Ajusta el nombre de columna si es necesario
            });

            modelBuilder.Entity<Municipio>(entity =>
            {
                entity.HasKey(e => e.IdMunicipio).HasName("PK__municipi__43249725194E9E1C");

                entity.ToTable("municipios");

                entity.Property(e => e.IdMunicipio).HasColumnName("id_municipio");
                entity.Property(e => e.Estado).HasColumnName("estado");
                entity.Property(e => e.Nmunicipio)  // Cambiado de Nombre a Nmunicipio
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("nmunicipio");  // Ajusta el nombre de columna si es necesario

                entity.HasOne(d => d.EstadoNavigation)
                    .WithMany(p => p.Municipios)
                    .HasForeignKey(d => d.Estado)
                    .HasConstraintName("FK__municipio__estado__48CFD27E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}