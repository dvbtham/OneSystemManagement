﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Responses;

namespace OneSystemManagement.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserApiController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public UserApiController(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int pageSize = 10, int pageNumber = 1, string q = null)
        {
            var response = new ListModelResponse<UserResource>();
            var query = _userRepository.Query().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(q.ToLower())
                || x.Email.ToLower().Contains(q.ToLower())
                || x.Phone.ToLower().Contains(q.ToLower())).ToList();
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
            var response = new SingleModelResponse<UserGridResource>();

            try
            {
                var entity = await GetUserRelated(id);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }
                var resource = new UserGridResource();
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
        public async Task<IActionResult> Create([FromBody] UserSaveResource userSaveResource)
        {
            var response = new SingleModelResponse<UserGridResource>();

            if (userSaveResource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }

            try
            {
                var user = new User();
                _mapper.Map(userSaveResource, user);

                user.CreateDate = DateTime.Now;
                var entity = await _userRepository.AddAsync(user);
                var entityMap = await GetUserRelated(entity.Id);

                response.Model = _mapper.Map<User, UserGridResource>(entityMap);
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
        public async Task<IActionResult> Update(int id, [FromBody] UserSaveResource userSaveResource)
        {
            var response = new SingleModelResponse<UserGridResource>();

            try
            {
                var entity = await GetUserRelated(id);
                
                if (entity == null || !ModelState.IsValid)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(userSaveResource, entity);

                await _userRepository.UpdateAsync(entity);
                
                response.Model = _mapper.Map<User, UserGridResource>(entity);
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
            var response = new SingleModelResponse<UserGridResource>();

            try
            {
                var entity = await _userRepository.DeleteAsync(id);
               
                response.Model = _mapper.Map<User, UserGridResource>(entity);
                response.Message = "The record was deleted successfully";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
            }

            return response.ToHttpResponse();
        }

        private async Task<User> GetUserRelated(int id)
        {
            var entity = await _userRepository.Query()
                  .Include(x => x.UserRoles)
                      .ThenInclude(ur => ur.Role)
                  .SingleOrDefaultAsync(x => x.Id == id);

            return entity;
        }

        #endregion
    }
}