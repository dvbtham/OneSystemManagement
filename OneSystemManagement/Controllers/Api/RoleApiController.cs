using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/role")]
    public class RoleApiController : Controller
    {
        #region ctor

        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;
        public RoleApiController(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        protected override void Dispose(bool disposing)
        {
            _roleRepository?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int pageSize = 10, int pageNumber = 1, string q = null)
        {
            var response = new ListModelResponse<RoleResource>();
            var query = _roleRepository.Query().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.RoleName.ToLower().Contains(q.ToLower())
                || x.CodeRole.ToLower().Contains(q.ToLower())).ToList();
            }

            try
            {
                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                response.Model = _mapper.Map(query, response.Model);

                response.Message = string.Format("Total of records: {0}", response.Model.Count());
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var entity = await _roleRepository.GetAsync(id);
                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }
                var resource = new RoleResource();
                _mapper.Map(entity, resource);
                response.Model = resource;
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleResource resource)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var role = new Role();
                _mapper.Map(resource, role);

                var entity = await _roleRepository.AddAsync(role);

                response.Model = _mapper.Map(entity, resource);
                response.Message = "The data was saved successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleResource resource)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var role = await _roleRepository.FindAsync(x => x.Id == id);

                if (role == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, role);
                
                await _roleRepository.UpdateAsync(role);

                response.Model = _mapper.Map(role, resource);
                response.Message = "The data was saved successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var entity = await _roleRepository.DeleteAsync(id);
                var resource = new RoleResource();
                response.Model = _mapper.Map(entity, resource);
                response.Message = "The record was deleted successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        #endregion
    }
}