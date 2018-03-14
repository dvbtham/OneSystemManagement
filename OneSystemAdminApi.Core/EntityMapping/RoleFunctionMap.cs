using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class RoleFunctionMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleFunction>(entity =>
            {
                entity.ToTable("RoleFunctions");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.RoleFunctions)
                    .HasForeignKey(d => d.IdFunction)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoleFunctions_Functions");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleFunctions)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoleFunctions_Roles");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.RoleFunctions)
                    .HasForeignKey(d => d.AreaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_RoleFunctions_Areas");
            });
        }
    }
}
