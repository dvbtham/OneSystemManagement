using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OneSystemAdminApi.Core.DataLayer
{
    public interface IEntityMapper
    {
        IEnumerable<IEntityMap> Mappings { get; }

        void MapEntities(ModelBuilder modelBuilder);
    }
}
