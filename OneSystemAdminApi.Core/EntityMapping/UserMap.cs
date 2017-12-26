using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;

namespace OneSystemAdminApi.Core.EntityMapping
{
    public class UserMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.Address).HasMaxLength(450);

                entity.Property(e => e.Avatar).HasMaxLength(350);

                entity.Property(e => e.ConfirmPassword).HasMaxLength(250);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.LoginFailed).HasDefaultValueSql("((0))");

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.QuestionAnswer).HasMaxLength(500);

                entity.Property(e => e.QuestionCode).HasMaxLength(300);

                entity.Property(e => e.UserCode).HasMaxLength(20);

                entity.Property(e => e.UserIdentifier)
                    .IsRequired()
                    .HasMaxLength(268);
            });
        }
    }
}
