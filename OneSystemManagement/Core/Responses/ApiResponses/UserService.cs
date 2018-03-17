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
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
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
                response.ErrorMessage = "Bạn đang truyền vào dữ liệu rỗng";
                return response.ToHttpResponse();
            }

            try
            {
                var user = new User();
                _mapper.Map(resource, user);

                if (_userRepository.Query().Any(x => x.Email == resource.UserInfo.Email))
                {
                    response.DidError = true;
                    response.ErrorMessage = "Email đã có người sử dụng.";
                    return response.ToHttpResponse();
                }

                user.CreateDate = DateTime.Now;
                var md5Hash = MD5.Create();
                user.Password = PasswordManager.GetMd5Hash(md5Hash, resource.UserInfo.Password);
                var entity = await _userRepository.AddAsync(user);
                var entityMap = await GetUserWithRelated(entity.Id);

                response.Model = _mapper.Map<User, UserGridResource>(entityMap);
                response.Message = "Dữ liệu đã được lưu thành công.";
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
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
                    return response.ToHttpResponse();
                }

                _mapper.Map(resource, entity);

                await _userRepository.UpdateAsync(entity);

                var entityMap = await GetUserWithRelated(entity.Id);

                response.Model = _mapper.Map<User, UserGridResource>(entityMap);
                response.Message = "Dữ liệu đã được lưu thành công";
            }
            catch (Exception ex)
            {
                response.DidError = true;
                response.ErrorMessage = ex.ToString();
            }

            return response.ToHttpResponse();
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                    response.ErrorMessage = "Không tìm thấy kết quả phù hợp";
                    return response.ToHttpResponse();
                }

                entity.IsActive = false;

                await _userRepository.UpdateAsync(entity);

                response.Model = _mapper.Map<User, UserGridResource>(entity);
                response.Message = "Dữ liệu đã được xóa thành công";
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
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="isAdminLogin"></param>
        /// <returns></returns>
        public async Task<int> Login(string email, string password, bool isAdminLogin = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return (int)LoginStatus.IncorrectEmailAndPass;

            var user = await _userRepository.Query().FirstOrDefaultAsync(x => x.Email == email);

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
        /// <summary>
        /// Lấy người dùng theo email nhập vào
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Lấy tất cả vùng, chức năng của người dùng theo quyền hạn
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetAreasByEmail(string email)
        {
            var response = new SingleModelResponse<UserEx>();

            var data = _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Area)
                .ThenInclude(x => x.RoleFunctions)
                .ThenInclude(x => x.Function)
                .SingleOrDefault(x => x.Email == email);

            if (data == null)
            {
                response.Message = "Không tìm thấy dữ liệu.";
                return response.ToHttpResponse();
            }

            var dataEx = new UserEx
            {
                MyRoles = data.UserRoles.Select(usr => new MyRole
                {
                    Id = usr.Role.Id,
                    Name = usr.Role.RoleName,
                    MyAreas = usr.Role.RoleFunctions.GroupBy(grf => grf.AreaId)
                        .Select(grf => grf.First())
                        .Select(rf => new MyArea
                        {
                            Id = rf.Area.Id,
                            Name = rf.Area.AreaName,
                            MyFunctions = rf.Area.RoleFunctions
                                .Where(mfrf => mfrf.IdRole == usr.IdRole && mfrf.AreaId == rf.AreaId && rf.Area.Functions.Select(mf => mf.Id).Contains(mfrf.IdFunction))
                                .Select(kvrf => new FunctionWithRole
                                {
                                    Id = kvrf.Function.Id,
                                    Name = kvrf.Function.FunctionName,
                                    IsWrite = kvrf.IsWrite,
                                    IsRead = kvrf.IsRead
                                }).ToList()
                        }).ToList()
                }).ToList()
            };

            response.Model = dataEx;

            return response.ToHttpResponse();
        }

        /// <summary>
        /// Xác thực tài khoản
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> UserVerifyAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return false;

            var user = await _userRepository.Query().SingleOrDefaultAsync(x => x.Email == email);

            if (user == null) return false;

            var md5Hash = MD5.Create();

            return PasswordManager.VerifyMd5Hash(md5Hash, password, user.Password);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changePassword"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Đặt lại mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
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