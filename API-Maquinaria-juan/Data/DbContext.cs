using API_Maquinaria_juan.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Maquinaria_juan.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entity = modelBuilder.Entity<Equipment>();

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Code)
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(e => e.Code)
                .IsUnique();



            entity.HasData(
                new Equipment
                {
                    Id = "eq-1",
                    Name = "Bomba centrífuga 01",
                    Code = "BOM-001",
                    Location = "Sala de máquinas",
                    Status = "Operativo"
                },
                new Equipment
                {
                    Id = "eq-2",
                    Name = "Compresor A2",
                    Code = "COM-002",
                    Location = "Taller",
                    Status = "En mantenimiento"
                },
                new Equipment
                {
                    Id = "eq-3",
                    Name = "Tablero eléctrico Norte",
                    Code = "TAB-003",
                    Location = "Depósito",
                    Status = "Operativo"
                }
            );
        }
    }
}
