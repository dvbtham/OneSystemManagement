using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OneSystemManagement.Controllers.Resources;

namespace OneSystemManagement.Controllers.Api
{
    /// <summary>
    /// This class for management user.
    /// </summary>
    [Produces("application/json")]
    [Route("api/user")]
    public class UserApiController : BaseApiController
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userService"></param>
        public UserApiController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _userService?.Dispose();

            base.Dispose(disposing);
        }

        #region CRUD Methods

        /// <summary>
        /// Return user list with paging.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll(int? pageSize = 10, int? pageNumber = 1, string q = null)
        {
            return _userService.GetAll(pageSize, pageNumber, q);
        }

        /// <summary>
        /// Get user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            return await _userService.GetAsync(id);
        }

        /// <summary>
        /// Add new user to database.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveUserResource resource)
        {
            return await _userService.Create(resource);
        }

        /// <summary>
        /// Update existing user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveUserResource resource)
        {
            return await _userService.Update(id, resource);
        }

        /// <summary>
        /// Delete existing user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _userService.Delete(id);
        }

        /// <summary>
        /// Login by email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<bool> Login(string email, string password)
        {
            return await _userService.Login(email, password);
        }

        #endregion
    }
}