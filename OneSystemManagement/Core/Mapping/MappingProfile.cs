using AutoMapper;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Domain to resource
            CreateMap<Area, AreaResource>();
            CreateMap<Function, FunctionResource>();
            CreateMap<Role, RoleResource>();
            CreateMap<User, UserResource>();

            //Resource to domain
            CreateMap<AreaResource, Area>().ForMember(ar => ar.Id, opt => opt.Ignore());
            CreateMap<RoleResource, Role>().ForMember(rr => rr.Id, opt => opt.Ignore());
            CreateMap<UserResource, User>().ForMember(ur => ur.Id, opt => opt.Ignore());
        }
    }
}
