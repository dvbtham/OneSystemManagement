using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public class FunctionService : IFunctionService
    {
        private readonly IRepository<Function> _functionRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRoleFunctionService _roleFunctionService;
        private readonly IMapper _mapper;

        public FunctionService(IRepository<Function> functionRepository,
            IRepository<Role> roleRepository, IRoleFunctionService roleFunctionService,
            IMapper mapper)
        {
            _functionRepository = functionRepository;
            _roleRepository = roleRepository;
            _roleFunctionService = roleFunctionService;
            _mapper = mapper;
        }

        #region CRUD Methods

        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null, bool isPaging = false)
        {
            var response = new ListModelResponse<FunctionResource>
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber
            };

            var query = _functionRepository.Query()
                .Include(f => f.RoleFunctions)
                .ThenInclude(rf => rf.Role)
                .Include(x => x.Functions).Include(x => x.FunctionProp)
                .Include(x => x.Area).ToList();

            if (isPaging)
            {
                query = query.Skip((response.PageNumber - 1) * response.PageSize)
                    .Take(response.PageSize).ToList();
            }

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.FunctionName.ToLower().Contains(q)
                                         || x.CodeFunction.ToLower().Contains(q)
                                         || x.Description.ToLower().Contains(q)
                                         || x.Area.AreaName.ToLower().Contains(q)).ToList();
            }

            try
            {
                response.Model = _mapper.Map<IEnumerable<Function>, IEnumerable<FunctionResource>>(query);

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
            var response = new SingleModelResponse<FunctionResource>();

            try
            {
                var entity = await GetFunctionWithRelated(id);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }
                var resource = new FunctionResource();
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

        public async Task<IActionResult> Create(SaveFunctionResource resource)
        {
            var response = new SingleModelResponse<FunctionResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }

            try
            {
                var function = new Function();
                _mapper.Map(resource, function);

                var entity = await _functionRepository.AddAsync(function);
                var entityMap = await GetFunctionWithRelated(entity.Id);

                response.Model = _mapper.Map<Function, FunctionResource>(entityMap);
                response.Message = "The data was saved successfully.";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        public async Task<IActionResult> Update(int id, SaveFunctionResource resource)
        {
            var response = new SingleModelResponse<FunctionResource>();

            try
            {
                var entity = await GetFunctionWithRelated(id);

                if (entity == null || resource == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, entity);

                await _functionRepository.UpdateAsync(entity);

                var entityMap = await GetFunctionWithRelated(entity.Id);

                response.Model = _mapper.Map<Function, FunctionResource>(entityMap);
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
            var response = new SingleModelResponse<FunctionResource>();

            try
            {
                var entity = await GetFunctionWithRelated(id);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                await _roleFunctionService.Delete(entity.RoleFunctions.ToList());

                await _functionRepository.DeleteAsync(entity.Id);

                response.Model = _mapper.Map<Function, FunctionResource>(entity);
                response.Message = "The record was deleted successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        public async Task<IActionResult> FunctionListByRole(int roleId)
        {
            var response = new ListModelResponse<FunctionResource>();

            try
            {
                var role = _roleRepository.Query()
                    .Include(x => x.RoleFunctions)
                        .ThenInclude(x => x.Function)
                    .SingleOrDefault(x => x.Id == roleId);

                var functions = role?.RoleFunctions.Select(x => x.Function).ToList();

                response.Model = _mapper.Map<IList<Function>, IList<FunctionResource>>(functions);
                response.Message = string.Format("Total of records: {0}", response.Model.Count());
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        public async Task<Function> GetFunctionWithRelated(int id, bool include = true)
        {
            if (!include)
            {
                var entityFalse = await _functionRepository.Query()
                    .SingleOrDefaultAsync(x => x.Id == id);

                return entityFalse;
            }

            var entity = await _functionRepository.Query()
                .Include(f => f.RoleFunctions)
                .ThenInclude(rf => rf.Role)
                .Include(x => x.Functions).Include(x => x.FunctionProp)
                .SingleOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        #endregion

        public virtual void Dispose()
        {
            _functionRepository?.Dispose();
        }
    }
}