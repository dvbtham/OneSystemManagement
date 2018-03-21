using System.Linq;
using AutoMapper;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Areas.SystemAdmin.Models;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Area, AreaViewModel>();
            CreateMap<AreaViewModel, Area>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<AreaViewModel, AreaViewModel>();

            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleViewModel, Role>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<RoleViewModel, RoleViewModel>();

            CreateMap<Function, FunctionViewModel>()
                .ForMember(x => x.Functions, opt => opt.MapFrom(x => x.Functions.Select(f => new KeyValuePairResource { Id = x.FunctionProp.Id, Name = x.FunctionProp.FunctionName })))
                .ForMember(ug => ug.IsRead,
                    opt => opt.MapFrom(u => u.RoleFunctions.Select(f => f.IsRead)))
                .ForMember(ug => ug.IsWrite,
                    opt => opt.MapFrom(u => u.RoleFunctions.Select(f => f.IsWrite))); 
            CreateMap<FunctionViewModel, Function>().ForMember(x => x.Id, opt => opt.Ignore());
            CreateMap<FunctionViewModel, FunctionViewModel>();

            CreateMap<SaveUserViewModel, SaveUserViewModel>();

            CreateMap<UserConfigViewModel, UserConfigViewModel>();
            
        }
    }
}
