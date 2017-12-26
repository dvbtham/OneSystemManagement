using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class AreaMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Areas");

                entity.Property(e => e.AreaName)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.CodeArea)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(500);
            });
        }
    }
}
