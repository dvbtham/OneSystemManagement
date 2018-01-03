using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public class AreaService : IAreaService
    {
        private readonly IRepository<Area> _areaRepository;
        private readonly IMapper _mapper;
        public AreaService(IRepository<Area> areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public IActionResult GetAll(int? pageSize, int? pageNumber, string q = null)
        {
            var response = new ListModelResponse<AreaResource>
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber
            };
            var query = Queryable.Skip<Area>(_areaRepository.Query().Include(x => x.Functions), (response.PageNumber - 1) * response.PageSize)
                .Take(response.PageSize).ToList();

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.AreaName.ToLower().Contains(q.ToLower())
                                         || x.CodeArea.ToLower().Contains(q.ToLower())).ToList();
            }

            try
            {
                response.Model = _mapper.Map(query, response.Model);

                response.Message = string.Format("Total of records: {0}", Enumerable.Count<AreaResource>(response.Model));
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
            var response = new SingleModelResponse<AreaResource>();

            try
            {
                var entity = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<Area>(_areaRepository.Query().Include(x => x.Functions), x => x.Id == id);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                var resource = new AreaResource();
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

        public async Task<IActionResult> Create(AreaResource resource)
        {
            var response = new SingleModelResponse<AreaResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }

            try
            {
                var area = new Area();
                _mapper.Map(resource, area);

                var entity = await _areaRepository.AddAsync(area);

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

        public async Task<IActionResult> Update(int id, AreaResource resource)
        {
            var response = new SingleModelResponse<AreaResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }
            try
            {
                var area = await _areaRepository.FindAsync(x => x.Id == id);

                if (area == null || resource == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, area);

                await _areaRepository.UpdateAsync(area);

                response.Model = _mapper.Map(area, resource);
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
            var response = new SingleModelResponse<AreaResource>();

            try
            {
                var areaWithFunctions = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync<Area>(_areaRepository.Query().Include(x => x.Functions), x => x.Id == id);

                foreach (var function in areaWithFunctions.Functions)
                {
                    function.IdArea = null;
                }

                var entity = await _areaRepository.DeleteAsync(id);

                var resource = new AreaResource();
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

        public virtual void Dispose()
        {
            _areaRepository?.Dispose();
        }
    }
}