//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using OneSystemAdminApi.Core.EntityLayer;
//using OneSystemManagement.Persistence.Repositories;

//namespace OneSystemManagement.Controllers.Api
//{
//    [Produces("application/json")]
//    [Route("api/UserApi")]
//    public class UserApiController : Controller
//    {
//        private readonly IRepository<User> _userRepository;
//        private readonly IMapper _mapper;
//        public UserApiController(IRepository<User> userRepository, IMapper mapper)
//        {
//            _userRepository = userRepository;
//            _mapper = mapper;
//        }

//        public IList<User> GetAll(string q)
//        {
//            var query = _userRepository.GetAll().ToList();
//            if (!string.IsNullOrEmpty(q) && query.Any())
//            {
//                q = q.ToLower();
//                query = query.Where(x => x.FullName.ToLower().Contains(q)
//                                         || x.Phone.ToLower().Contains(q)
//                                         || x.UserCode.ToLower().Contains(q)).ToList();
//            }
//            return query;
//        }
//    }
//}