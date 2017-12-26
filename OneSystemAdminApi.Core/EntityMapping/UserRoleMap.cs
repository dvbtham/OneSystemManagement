using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class UserRoleMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");
            });
        }
    }
}
