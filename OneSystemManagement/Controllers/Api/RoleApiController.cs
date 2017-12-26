//using System.Collections.Generic;
//using System.Linq;
//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using OneSystemAdminApi.Core.EntityLayer;
//using OneSystemManagement.Controllers.Resources;
//using OneSystemManagement.Persistence.Repositories;

//namespace OneSystemManagement.Controllers.Api
//{
//    [Produces("application/json")]
//    [Route("api/roles")]
//    public class RoleApiController : Controller
//    {
//        private readonly IRepository<Role> _roleRepository;
//        private readonly IMapper _mapper;
//        public RoleApiController(IRepository<Role> roleRepository, IMapper mapper)
//        {
//            _roleRepository = roleRepository;
//            _mapper = mapper;
//        }

//        public IList<Role> GetAll(string q = null)
//        {
//            var query = _roleRepository.GetAll().ToList();
//            if (!string.IsNullOrEmpty(q) && query.Any())
//            {
//                q = q.ToLower();
//                query = query.Where(x => x.CodeRole.ToLower().Contains(q)
//                || x.RoleName.ToLower().Contains(q) 
//                || x.Description.ToLower().Contains(q)).ToList();
//            }
//            return query;
//        }

//        [HttpGet("{id}")]
//        public IActionResult Get(int id)
//        {
//            var role = _roleRepository.Find(x => x.Id == id);

//            if (role == null) return NotFound("Data could not be found");
//            var resource = new RoleResource();
//            _mapper.Map(role, resource);
//            return Ok(resource);
//        }

//        [HttpPost]
//        public IActionResult Create([FromBody] RoleResource resource)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            var role = new Role();
//            _mapper.Map(resource, role);

//            _roleRepository.Insert(role);

//            return Ok(role.Id);
//        }

//        [HttpPut("{id}")]
//        public IActionResult Update(int id, [FromBody] RoleResource resource)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            var role = _roleRepository.Find(x => x.Id == id);

//            if (role == null) return NotFound("Data could not be found");

//            _mapper.Map(resource, role);

//            _roleRepository.Insert(role);

//            return Ok(role.Id);
//        }

//        [HttpDelete("{id}")]
//        public IActionResult Detete(int id)
//        {
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            var role = _roleRepository.Find(x => x.Id == id);

//            if (role == null) return NotFound("Data could not be found.");

//           _roleRepository.Delete(role);

//            return Ok(role.Id);
//        }
//    }
//}