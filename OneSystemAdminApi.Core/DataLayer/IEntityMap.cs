using Microsoft.EntityFrameworkCore;

namespace OneSystemAdminApi.Core.DataLayer
{
    public interface IEntityMap
    {
        void Map(ModelBuilder modelBuilder);
    }
}
