using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Base
{
	public static class UserRoles
	{
		public const string Role_View = "VIEW";
		public const string Role_Edit = "EDIT";
		public const string Role_Manage = "MANAGE";
		public const string Role_Planning = "PLANNING";
		public const string Role_Costs = "COSTS";
		public const string Role_DashBoards = "DASHBOARD";
		public const string Role_Anagraohics = "ANAGS";
		
		// speciali usati anche nelle storeproc
		public const string Role_Takss = "TASK";
		public const string Role_Users = "USERS";

		public static IList<string> All = new List<string>()
		{
			Role_View,
			Role_Edit,
			Role_Manage,
			Role_Planning,
			Role_Costs,
			Role_DashBoards,
			Role_Users,
			Role_Takss,
			Role_Anagraohics
		};
	}
}
