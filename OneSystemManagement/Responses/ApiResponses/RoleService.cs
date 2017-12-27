using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Responses.ApiResponses
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IMapper _mapper;
        public RoleService(IRepository<Role> roleRepository,IUserRoleService userRoleService, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _userRoleService = userRoleService;
            _mapper = mapper;
        }

        public IActionResult GetAll(int? pageSize, int? pageNumber, string q = null)
        {
            var response = new ListModelResponse<RoleResource>
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber
            };

            var query = _roleRepository.Query()
                .Skip((response.PageNumber - 1) * response.PageSize)
                .Take(response.PageSize).ToList();

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.RoleName.ToLower().Contains(q.ToLower())
                                         || x.CodeRole.ToLower().Contains(q.ToLower())).ToList();
            }

            try
            {
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

        public async Task<IActionResult> Create(RoleResource resource)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                if (resource == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be null.";
                    return response.ToHttpResponse();
                }
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

        public async Task<IActionResult> Update(int id, RoleResource resource)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var role = await _roleRepository.FindAsync(x => x.Id == id);

                if (role == null || resource == null)
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

        public async Task<IActionResult> Delete(int id)
        {
            var response = new SingleModelResponse<RoleResource>();

            try
            {
                var rolesToDelete = await _roleRepository.Query().Include(x => x.UserRoles)
                    .SingleOrDefaultAsync(x => x.Id == id);

                if (rolesToDelete == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                await _userRoleService.Delete(rolesToDelete.UserRoles.ToList());

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

        public void Dispose()
        {
            _roleRepository?.Dispose();
        }
    }
}