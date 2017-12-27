using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses;
using Microsoft.EntityFrameworkCore;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/function")]
    public class FunctionApiController : Controller
    {
        private readonly IRepository<Function> _functionRepository;
        private readonly IMapper _mapper;

        public FunctionApiController(IRepository<Function> functionRepository, IMapper mapper)
        {
            _functionRepository = functionRepository;
            _mapper = mapper;
        }


        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {

            var response = new ListModelResponse<FunctionResource>
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber
            };
            var query = _functionRepository.Query()
                .Include(f => f.RoleFunctions)
                .ThenInclude(rf => rf.Role)
                .Include(x => x.Functions)
                .Include(x => x.FunctionProp)
                .Skip((response.PageNumber - 1) * response.PageSize)
                .Take(response.PageSize).ToList();

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.FuctionName.ToLower().Contains(q.ToLower())
                                         || x.CodeFuction.ToLower().Contains(q.ToLower())
                                         || x.Description.ToLower().Contains(q.ToLower())).ToList();
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

        [HttpGet("{id}")]
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveFunctionResource resource)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFunctionResource resource)
        {
            var response = new SingleModelResponse<FunctionResource>();

            try
            {
                var entity = await GetFunctionWithRelated(id);

                if (entity == null || !ModelState.IsValid)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new SingleModelResponse<FunctionResource>();

            try
            {
                var entity = await _functionRepository.DeleteAsync(id);

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

        private async Task<Function> GetFunctionWithRelated(int id)
        {
            var entity = await _functionRepository.Query()
                .Include(f => f.RoleFunctions)
                    .ThenInclude(rf => rf.Role)
                .Include(x => x.Functions)
                .Include(x => x.FunctionProp)
                  .SingleOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        #endregion
    }
}
