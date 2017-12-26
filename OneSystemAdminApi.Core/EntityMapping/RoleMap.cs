using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class RoleMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(e => e.CodeRole)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(300);
            });
        }
    }
}
