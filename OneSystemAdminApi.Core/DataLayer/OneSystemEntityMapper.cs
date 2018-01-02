using System.Collections.Generic;
using OneSystemAdminApi.Core.EntityMapping;

namespace OneSystemAdminApi.Core.DataLayer
{
    public class OneSystemEntityMapper : EntityMapper
    {
        public OneSystemEntityMapper()
        {
            Mappings = new List<IEntityMap>()
            {
                new AreaMap(),
                new FunctionMap(),
                new LanguageMap(),
                new RoleFunctionMap(),
                new RoleMap(),
                new UserMap(),
                new UserRoleMap(),
                new UserConfigMap()
            };
        }
    }
}
