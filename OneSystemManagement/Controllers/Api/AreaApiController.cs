using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses;
using static System.String;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/area")]
    public class AreaApiController : Controller
    {
        #region ctor

        private readonly IRepository<Area> _areaRepository;
        private readonly IMapper _mapper;
        public AreaApiController(IRepository<Area> areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        protected override void Dispose(bool disposing)
        {
            _areaRepository?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int pageSize = 10, int pageNumber = 1, string q = null)
        {
            var response = new ListModelResponse<AreaResource>();
            var query = _areaRepository.Query().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            if (!IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.AreaName.ToLower().Contains(q.ToLower())
                || x.CodeArea.ToLower().Contains(q.ToLower())).ToList();
            }

            try
            {
                response.PageSize = pageSize;
                response.PageNumber = pageNumber;

                response.Model = _mapper.Map(query, response.Model);

                response.Message = Format("Total of records: {0}", response.Model.Count());
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
            var response = new SingleModelResponse<AreaResource>();

            try
            {
                var entity = await _areaRepository.GetAsync(id);

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AreaResource resource)
        {
            var response = new SingleModelResponse<AreaResource>();

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AreaResource resource)
        {
            var response = new SingleModelResponse<AreaResource>();

            try
            {
                var area = await _areaRepository.FindAsync(x => x.Id == id);

                if (area == null)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new SingleModelResponse<AreaResource>();

            try
            {
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

        #endregion

    }
}