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
    public interface IUserConfigService : IDisposable
    {
        IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null);
        Task<IActionResult> GetAsync(int id);
        Task<IActionResult> Create(UserConfigResource resource);
        Task<IActionResult> Update(int id, UserConfigResource resource);
        Task<IActionResult> Delete(int id);
    }
    public class UserConfigService : IUserConfigService
    {
        private readonly IRepository<UserConfig> _userConfigRepository;
        private readonly IMapper _mapper;
        public UserConfigService(IRepository<UserConfig> areaRepository, IMapper mapper)
        {
            _userConfigRepository = areaRepository;
            _mapper = mapper;
        }
        public IActionResult GetAll(int? pageSize, int? pageNumber, string q = null)
        {
            var response = new ListModelResponse<UserConfigResource>
            {
                PageSize = (int)pageSize,
                PageNumber = (int)pageNumber
            };
            var query = _userConfigRepository.Query().Include(x => x.User)
                .Skip((response.PageNumber - 1) * response.PageSize)
                .Take(response.PageSize).ToList();

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.ApiCode.ToLower().Contains(q)
                                         || x.ApiKey.ToLower().Contains(q))
                                         .ToList();

                if (query.Count(x => x.User != null) > 0)
                {
                    query = query.Where(x => x.ApiCode.ToLower().Contains(q)
                                             || x.ApiKey.ToLower().Contains(q) 
                                             || x.User.FullName.ToLower().Contains(q))
                                             .ToList();
                }
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
            var response = new SingleModelResponse<UserConfigResource>();

            try
            {
                var entity = await _userConfigRepository.Query().FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                var resource = new UserConfigResource();
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

        public async Task<IActionResult> Create(UserConfigResource resource)
        {
            var response = new SingleModelResponse<UserConfigResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }

            try
            {
                var entity = new UserConfig();
                _mapper.Map(resource, entity);

                var result = await _userConfigRepository.AddAsync(entity);

                response.Model = _mapper.Map(result, resource);
                response.Message = "The data was saved successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        public async Task<IActionResult> Update(int id, UserConfigResource resource)
        {
            var response = new SingleModelResponse<UserConfigResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }
            try
            {
                var userConfig = await _userConfigRepository.FindAsync(x => x.Id == id);

                if (userConfig == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, userConfig);

                await _userConfigRepository.UpdateAsync(userConfig);

                response.Model = _mapper.Map(userConfig, resource);
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
            var response = new SingleModelResponse<UserConfigResource>();

            try
            {
                var entity = await _userConfigRepository.DeleteAsync(id);

                var resource = new UserConfigResource();
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
            _userConfigRepository?.Dispose();
        }
    }
}
