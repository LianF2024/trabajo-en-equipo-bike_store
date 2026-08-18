using BikeStore.Domain.Entities;
using BikeStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BikeStore.Infrastructure.Data;

public sealed class BikeStoreDbContext(DbContextOptions<BikeStoreDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Bicycle> Bicycles => Set<Bicycle>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categoria", t => t.HasCheckConstraint("CK_Categoria_Nombre", "LEN(LTRIM(RTRIM([Nombre]))) >= 2"));
            entity.HasKey(x => x.Id).HasName("PK_Categoria");
            entity.Property(x => x.Id).HasColumnName("IdCategoria");
            entity.Property(x => x.Name).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasColumnName("Descripcion").HasMaxLength(250);
            entity.Property(x => x.Active).HasColumnName("Activo").HasDefaultValue(true);
            entity.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_Categoria_Nombre");
        });

        modelBuilder.Entity<Bicycle>(entity =>
        {
            entity.ToTable("Bicicleta", t =>
            {
                t.HasCheckConstraint("CK_Bicicleta_Precio", "[Precio] > 0");
                t.HasCheckConstraint("CK_Bicicleta_Stock", "[Stock] >= 0");
                t.HasCheckConstraint("CK_Bicicleta_Estado", "[Estado] IN (N'Disponible', N'Bajo stock', N'Agotado', N'Inactivo')");
            });
            entity.HasKey(x => x.Id).HasName("PK_Bicicleta");
            entity.Property(x => x.Id).HasColumnName("IdBicicleta");
            entity.Property(x => x.CategoryId).HasColumnName("IdCategoria");
            entity.Property(x => x.Brand).HasColumnName("Marca").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Model).HasColumnName("Modelo").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Price).HasColumnName("Precio").HasPrecision(10, 2);
            entity.Property(x => x.Stock).HasColumnName("Stock");
            entity.Property(x => x.Status).HasColumnName("Estado")
                .HasConversion(
                    status => status == BicycleStatus.BajoStock ? "Bajo stock" : status == BicycleStatus.Agotado ? "Agotado" : status == BicycleStatus.Inactivo ? "Inactivo" : "Disponible",
                    value => value == "Bajo stock" ? BicycleStatus.BajoStock : value == "Agotado" ? BicycleStatus.Agotado : value == "Inactivo" ? BicycleStatus.Inactivo : BicycleStatus.Disponible)
                .HasMaxLength(20)
                .HasDefaultValue(BicycleStatus.Disponible);
            entity.HasIndex(x => new { x.Brand, x.Model }).HasDatabaseName("IX_Bicicleta_Marca_Modelo");
            entity.HasIndex(x => x.Stock).HasDatabaseName("IX_Bicicleta_Stock");
            entity.HasOne(x => x.Category).WithMany(x => x.Bicycles).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Bicicleta_Categoria");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(x => x.Id).HasName("PK_Cliente");
            entity.Property(x => x.Id).HasColumnName("IdCliente");
            entity.Property(x => x.Identification).HasColumnName("Cedula").HasMaxLength(20).IsRequired();
            entity.Property(x => x.FirstNames).HasColumnName("Nombres").HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastNames).HasColumnName("Apellidos").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Phone).HasColumnName("Telefono").HasMaxLength(20);
            entity.Property(x => x.Email).HasColumnName("Correo").HasMaxLength(150);
            entity.HasIndex(x => x.Identification).IsUnique().HasDatabaseName("UQ_Cliente_Cedula");
            entity.HasIndex(x => x.LastNames).HasDatabaseName("IX_Cliente_Apellidos");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("Venta", t =>
            {
                t.HasCheckConstraint("CK_Venta_Subtotal", "[Subtotal] >= 0");
                t.HasCheckConstraint("CK_Venta_IVA", "[IVA] >= 0");
                t.HasCheckConstraint("CK_Venta_Total", "[Total] >= 0");
            });
            entity.HasKey(x => x.Id).HasName("PK_Venta");
            entity.Property(x => x.Id).HasColumnName("IdVenta");
            entity.Property(x => x.Date).HasColumnName("Fecha").HasColumnType("datetime2");
            entity.Property(x => x.CustomerId).HasColumnName("IdCliente");
            entity.Property(x => x.Subtotal).HasColumnName("Subtotal").HasPrecision(12, 2);
            entity.Property(x => x.Vat).HasColumnName("IVA").HasPrecision(12, 2);
            entity.Property(x => x.Total).HasColumnName("Total").HasPrecision(12, 2);
            entity.HasIndex(x => x.Date).HasDatabaseName("IX_Venta_Fecha");
            entity.HasOne(x => x.Customer).WithMany(x => x.Sales).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_Venta_Cliente");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.ToTable("DetalleVenta", t =>
            {
                t.HasCheckConstraint("CK_DetalleVenta_Cantidad", "[Cantidad] > 0");
                t.HasCheckConstraint("CK_DetalleVenta_Precio", "[Precio] > 0");
                t.HasCheckConstraint("CK_DetalleVenta_Subtotal", "[Subtotal] > 0");
            });
            entity.HasKey(x => x.Id).HasName("PK_DetalleVenta");
            entity.Property(x => x.Id).HasColumnName("IdDetalle");
            entity.Property(x => x.SaleId).HasColumnName("IdVenta");
            entity.Property(x => x.BicycleId).HasColumnName("IdBicicleta");
            entity.Property(x => x.Quantity).HasColumnName("Cantidad");
            entity.Property(x => x.UnitPrice).HasColumnName("Precio").HasPrecision(10, 2);
            entity.Property(x => x.Subtotal).HasColumnName("Subtotal").HasPrecision(12, 2);
            entity.HasOne(x => x.Sale).WithMany(x => x.Details).HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_DetalleVenta_Venta");
            entity.HasOne(x => x.Bicycle).WithMany(x => x.SaleDetails).HasForeignKey(x => x.BicycleId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("FK_DetalleVenta_Bicicleta");
        });
    }
}
