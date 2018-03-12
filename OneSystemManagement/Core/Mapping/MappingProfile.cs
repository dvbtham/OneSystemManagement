using System.Linq;
using AutoMapper;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.ViewModels;

namespace OneSystemManagement.Core.Mapping
{
    public class MappingProfile : Profile
    {
        private void UserResourceToDomain()
        {
            CreateMap<SaveUserResource, User>().ForMember(u => u.Id, opt => opt.Ignore())
                .ForMember(u => u.Address, opt => opt.MapFrom(usr => usr.UserInfo.Address))
                .ForMember(u => u.Phone, opt => opt.MapFrom(usr => usr.UserInfo.Phone))
                .ForMember(u => u.Email, opt => opt.MapFrom(usr => usr.UserInfo.Email))
                .ForMember(u => u.FullName, opt => opt.MapFrom(usr => usr.UserInfo.FullName))
                .ForMember(u => u.Avatar, opt => opt.MapFrom(usr => usr.UserInfo.Avatar))
                .ForMember(u => u.ConfirmPassword, opt => opt.MapFrom(usr => usr.UserInfo.ConfirmPassword))
                .ForMember(u => u.IsAccFacebook, opt => opt.MapFrom(usr => usr.UserInfo.IsAccFacebook))
                .ForMember(u => u.IsAccGoogle, opt => opt.MapFrom(usr => usr.UserInfo.IsAccGoogle))
                .ForMember(u => u.IsAccOutlook, opt => opt.MapFrom(usr => usr.UserInfo.IsAccOutlook))
                .ForMember(u => u.IsAccTwitter, opt => opt.MapFrom(usr => usr.UserInfo.IsAccTwitter))
                .ForMember(u => u.IsActive, opt => opt.MapFrom(usr => usr.UserInfo.IsActive))
                .ForMember(u => u.IsAdmin, opt => opt.MapFrom(usr => usr.UserInfo.IsAdmin))
                .ForMember(u => u.IsMember, opt => opt.MapFrom(usr => usr.UserInfo.IsMember))
                .ForMember(u => u.IsPartner, opt => opt.MapFrom(usr => usr.UserInfo.IsPartner))
                .ForMember(u => u.LoginFailed, opt => opt.MapFrom(usr => usr.UserInfo.LoginFailed))
                .ForMember(u => u.Password, opt => opt.Ignore())
                .ForMember(u => u.QuestionAnswer, opt => opt.MapFrom(usr => usr.UserInfo.QuestionAnswer))
                .ForMember(u => u.QuestionCode, opt => opt.MapFrom(usr => usr.UserInfo.QuestionCode))
                .ForMember(u => u.UserCode, opt => opt.MapFrom(usr => usr.UserInfo.UserCode))
                .ForMember(u => u.UserIdentifier, opt => opt.MapFrom(usr => usr.UserInfo.UserIdentifier))
                .ForMember(u => u.UserRoles, opt => opt.Ignore())
                .AfterMap((usr, u) =>
                {
                    //Remove unselected Roles
                    var removedRoles = u.UserRoles.Where(f => !usr.Roles.Contains(f.IdRole)).ToList();
                    foreach (var f in removedRoles)
                        u.UserRoles.Remove(f);

                    // Add new Roles
                    var addedRoles = usr.Roles
                        .Where(id => u.UserRoles.All(x => x.IdRole != id))
                        .Select(id => new UserRole { IdRole = id }).ToList();

                    foreach (var f in addedRoles)
                        u.UserRoles.Add(f);
                });
        }
        private void FunctionResourceToDomain()
        {
            CreateMap<SaveFunctionResource, Function>()
                .ForMember(f => f.Id, opt => opt.Ignore())
                .ForMember(f => f.RoleFunctions, opt => opt.Ignore())
                .AfterMap((fr, f) =>
                {
                    //Remove unselected Roles
                    var removedRoles = f.RoleFunctions.Where(rf => !fr.Roles.Contains(rf.IdRole)).ToList();
                    foreach (var rmf in removedRoles)
                        f.RoleFunctions.Remove(rmf);

                    // Add new Roles
                    var addedRoles = fr.Roles
                        .Where(id => f.RoleFunctions.All(x => x.IdRole != id))
                        .Select(id => new RoleFunction { IdRole = id }).ToList();

                    foreach (var arf in addedRoles)
                        f.RoleFunctions.Add(arf);
                });
        }
        public MappingProfile()
        {
            //Domain to resource
            CreateMap<Area, AreaResource>();
            CreateMap<Area, AreaViewModel>()
                .ForMember(avm => avm.Functions,
                opt => opt.MapFrom(a => a.Functions.Select(f => new KeyValuePairResource
                {
                    Id = f.Id,
                    Name = f.FunctionName
                })));

            CreateMap<Role, RoleResource>();
            CreateMap<User, UserGridResource>()
                .ForMember(ug => ug.Roles,
                opt => opt.MapFrom(u => u.UserRoles.Select(ur => new KeyValuePairResource
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.RoleName
                })));

            CreateMap<Function, FunctionResource>()
                .ForMember(ug => ug.Functions,
                    opt => opt.MapFrom(u => u.Functions.Select(f => new KeyValuePairResource
                    {
                        Id = f.Id,
                        Name = f.FunctionName
                    })))
                .ForMember(ug => ug.Roles,
                    opt => opt.MapFrom(u => u.RoleFunctions.Select(rf => new KeyValuePairResource
                    {
                        Id = rf.Role.Id,
                        Name = rf.Role.RoleName
                    })));

            CreateMap<User, UserResource>()
                .ForMember(ur => ur.UserInfoResource,
                opt => opt.MapFrom(u => new UserInfoResource
                {
                    Address = u.Address,
                    Phone = u.Phone,
                    Email = u.Email,
                    FullName = u.FullName,
                    Avatar = u.Avatar,
                    ConfirmPassword = u.ConfirmPassword,
                    IsAccFacebook = u.IsAccFacebook,
                    IsAccGoogle = u.IsAccGoogle,
                    IsAccOutlook = u.IsAccOutlook,
                    IsAccTwitter = u.IsAccTwitter,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    IsConfirm = u.IsConfirm,
                    IsMember = u.IsMember,
                    IsPartner = u.IsPartner,
                    LoginFailed = u.LoginFailed,
                    Password = u.Password,
                    QuestionAnswer = u.QuestionAnswer,
                    QuestionCode = u.QuestionCode,
                    UserCode = u.UserCode,
                    UserIdentifier = u.UserIdentifier
                }))
                .ForMember(rr => rr.Roles, opt => opt.MapFrom(u => u.UserRoles.Select(ur => new KeyValuePairResource
                {
                    Id = ur.Role.Id,
                    Name = ur.Role.RoleName
                })));

            CreateMap<UserConfig, UserConfigResource>()
                .ForMember(x => x.User, opt => opt.MapFrom(uc => new UserResultResource
                {
                    Email = uc.User.Email,
                    FullName = uc.User.FullName,
                    UserCode = uc.User.UserCode,
                    Id = uc.User.Id
                }));

            CreateMap<UserConfigResource, UserConfig>()
                .ForMember(x => x.Id, opt => opt.Ignore());

            //Resource to domain
            CreateMap<AreaResource, Area>().ForMember(ar => ar.Id, opt => opt.Ignore());
            CreateMap<AreaViewModel, Area>();
            CreateMap<RoleResource, Role>().ForMember(rr => rr.Id, opt => opt.Ignore());

            UserResourceToDomain();
            FunctionResourceToDomain();


        }
    }
}
