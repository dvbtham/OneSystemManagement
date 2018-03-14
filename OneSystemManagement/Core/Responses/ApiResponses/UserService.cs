using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneSystemAdminApi.Core.DataLayer;
using OneSystemAdminApi.Core.EntityLayer;
using OneSystemManagement.Controllers.Resources;
using OneSystemManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OneSystemManagement.Areas.SystemAdmin.Models;

namespace OneSystemManagement.Core.Responses.ApiResponses
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        public UserService(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        #region CRUD Methods

        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null, bool isPaging = false)
        {
            var response = new ListModelResponse<UserGridResource>
            {
                PageNumber = (int)pageNumber,
                PageSize = (int)pageSize
            };
            var query = _userRepository.Query().Where(x => x.IsActive)
                .Include(x => x.UserRoles).ThenInclude(ur => ur.Role)
                .ToList();

            if (isPaging)
            {
                query = query.Skip((response.PageNumber - 1) * response.PageSize)
                             .Take(response.PageSize).ToList();
            }

            if (!string.IsNullOrEmpty(q) && query.Any())
            {
                q = q.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(q.ToLower())
                                         || x.Email.ToLower().Contains(q.ToLower())
                                         || x.Phone.ToLower().Contains(q.ToLower())).ToList();
            }

            try
            {
                response.Model = _mapper.Map<IEnumerable<User>, IEnumerable<UserGridResource>>(query);

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
                var entity = await GetUserWithRelated(id);

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
        public async Task<IActionResult> Create(SaveUserResource resource)
        {
            var response = new SingleModelResponse<UserGridResource>();

            if (resource == null)
            {
                response.DidError = true;
                response.ErrorMessage = "Input cannot be null.";
                return response.ToHttpResponse();
            }

            try
            {
                var user = new User();
                _mapper.Map(resource, user);

                if (_userRepository.Query().Any(x => x.Email == resource.UserInfo.Email))
                {
                    response.DidError = true;
                    response.ErrorMessage = "This email is already register.";
                    return response.ToHttpResponse();
                }

                user.CreateDate = DateTime.Now;
                var md5Hash = MD5.Create();
                user.Password = PasswordManager.GetMd5Hash(md5Hash, resource.UserInfo.Password);
                var entity = await _userRepository.AddAsync(user);
                var entityMap = await GetUserWithRelated(entity.Id);

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
        public async Task<IActionResult> Update(int id, SaveUserResource resource)
        {
            var response = new SingleModelResponse<UserGridResource>();

            try
            {
                var entity = await GetUserWithRelated(id);

                if (entity == null || resource == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, entity);

                await _userRepository.UpdateAsync(entity);

                var entityMap = await GetUserWithRelated(entity.Id);

                response.Model = _mapper.Map<User, UserGridResource>(entityMap);
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
                var entity = await _userRepository.FindAsync(x => x.Id == id && x.IsActive);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Input could not be found.";
                    return response.ToHttpResponse();
                }

                entity.IsActive = false;

                await _userRepository.UpdateAsync(entity);

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

        public async Task<User> GetUserWithRelated(int id, bool include = true)
        {
            if (include)
            {
                var entityTrue = await _userRepository.Query().Where(x => x.IsActive)
                    .Include(x => x.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .Include(x => x.UserConfigs)
                    .SingleOrDefaultAsync(x => x.Id == id);

                return entityTrue;
            }

            var entityFalse = await _userRepository.Query().Where(x => x.IsActive).SingleOrDefaultAsync(x => x.Id == id);

            return entityFalse;

        }
        /// <summary>
        /// 10: Success
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="isAdminLogin"></param>
        /// <returns></returns>
        public async Task<int> Login(string email, string password, bool isAdminLogin = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return (int)LoginStatus.IncorrectEmailAndPass;

            var user = await _userRepository.Query().SingleOrDefaultAsync(x => x.Email == email);

            if (user == null) return (int)LoginStatus.IncorrectEmailAndPass;

            if (!user.IsActive) return (int)LoginStatus.NotActived;

            if (!user.IsConfirm) return (int)LoginStatus.NotConfirmed;

            var md5Hash = MD5.Create();
            var loginResult = PasswordManager.VerifyMd5Hash(md5Hash, password, user.Password);

            if (isAdminLogin)
            {
                if (user.IsAdmin)
                {
                    if (loginResult)
                        return (int)LoginStatus.Success;
                    return (int)LoginStatus.IncorrectEmailAndPass;
                }
                return (int)LoginStatus.NotAdmin;
            }

            if (loginResult)
                return (int)LoginStatus.Success;
            return (int)LoginStatus.IncorrectEmailAndPass;

        }

        [HttpDelete("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var response = new SingleModelResponse<UserResultResource>();

            try
            {
                var entity = await _userRepository.Query().SingleOrDefaultAsync(x => x.Email == email);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
                    return response.ToHttpResponse();
                }
                var resource = new UserResultResource();
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

        public async Task<bool> UserVerifyAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return false;

            var user = await _userRepository.Query().SingleOrDefaultAsync(x => x.Email == email);

            if (user == null) return false;

            var md5Hash = MD5.Create();

            return PasswordManager.VerifyMd5Hash(md5Hash, password, user.Password);
        }

        public async Task<IActionResult> ChangePassword(int id, ChangePasswordViewModel changePassword)
        {
            var response = new SingleModelResponse<UserResultResource>();

            try
            {
                var entity = await _userRepository.Query().FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
                    return response.ToHttpResponse();
                }
                var md5Hash = MD5.Create();
                if (!PasswordManager.VerifyMd5Hash(md5Hash, changePassword.OldPassword, entity.Password))
                {
                    response.DidError = true;
                    response.ErrorMessage = "Mật khẩu cũ không đúng";
                    return response.ToHttpResponse();
                }

                entity.Password = PasswordManager.GetMd5Hash(md5Hash, changePassword.NewPassword);

                await _userRepository.UpdateAsync(entity);

                var model = new UserResultResource();
                response.Model = _mapper.Map(entity, model);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
                response.Model = null;
            }

            return response.ToHttpResponse();
        }

        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            var response = new SingleModelResponse<UserResultResource>();

            try
            {
                var entity = await _userRepository.Query().FirstOrDefaultAsync(x => x.Id == id && x.IsActive == true);

                if (entity == null)
                {
                    response.DidError = true;
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
                    return response.ToHttpResponse();
                }
                var md5Hash = MD5.Create();

                entity.Password = PasswordManager.GetMd5Hash(md5Hash, newPassword);

                await _userRepository.UpdateAsync(entity);

                var model = new UserResultResource();
                response.Model = _mapper.Map(entity, model);
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.Message;
                response.Model = null;
            }

            return response.ToHttpResponse();
        }

        #endregion

        public virtual void Dispose()
        {
            _userRepository?.Dispose();
        }
    }
}