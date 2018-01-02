using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class UserConfigMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserConfig>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ApiCode).HasMaxLength(250);

                entity.Property(e => e.ApiKey).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserConfigs)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_UserConfig_Users");
            });
        }
    }
}
