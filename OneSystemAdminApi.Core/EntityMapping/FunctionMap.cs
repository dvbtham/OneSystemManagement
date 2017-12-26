using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class FunctionMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("Functions");
                entity.Property(e => e.CodeFuction)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FuctionName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Url).HasMaxLength(500);

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Functions)
                    .HasForeignKey(d => d.IdArea)
                    .HasConstraintName("FK_Functions_Areas");

                entity.HasOne(d => d.FunctionNav)
                    .WithMany(p => p.Functions)
                    .HasForeignKey(d => d.IdFunctionParent)
                    .HasConstraintName("FK_Functions_Functions");
            });
        }
    }
}
