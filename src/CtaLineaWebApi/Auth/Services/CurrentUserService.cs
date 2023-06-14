using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;
using ZzSoft.CtaLinea.Dal.Services;

namespace CtaLineaWebApi.Auth.Services
{
	public class CurrentUserService
		: ICurrentUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IHttpContextAccessor _http;

		public CurrentUserService (
			IUserRepository userRepository,
			IHttpContextAccessor http
			)
		{
			_userRepository = userRepository;
			_http = http;
		}

		private Guid? _AssociateId = null;
		private bool _associateSet = false;
		public async  Task<Guid?> GetUserAssociateId()
		{
			if (_associateSet == true) return _AssociateId;
			var userId = _http.HttpContext.User.Identity.Name;

			if (string.IsNullOrEmpty(userId))
			{
				_AssociateId = null;
			}
			else
			{
				var user = await this._userRepository.GetUserAsync(userId)
				   .ConfigureAwait(false);

				_AssociateId = user?.AssociateId;
			}
			_associateSet = true;
			return _AssociateId;
		}

        public async Task<bool> IsAssociateRun(
			Guid runId)
        {
			bool userRun = false;
            var userId = _http.HttpContext.User.Identity.Name;

            if (string.IsNullOrEmpty(userId) == false)
            {
                userRun = await this._userRepository.IsUserAssociateRun(userId, runId)
                   .ConfigureAwait(false);
            }
            return userRun;
        }
    }
}
