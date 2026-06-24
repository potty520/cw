using System;
using System.Threading.Tasks;
using FinanceApp.Data;
using FinanceApp.Models;

namespace FinanceApp.Services
{
    /// <summary>
    /// 用户认证服务
    /// </summary>
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private User? _currentUser;

        public User? CurrentUser => _currentUser;

        public AuthService(DbContext context)
        {
            _userRepository = new UserRepository(context);
        }

        public async Task<User?> LoginAsync(string userCode, string password)
        {
            var user = await _userRepository.ValidateLoginAsync(userCode, password);
            if (user != null)
            {
                _currentUser = user;
            }
            return user;
        }

        public void Logout()
        {
            _currentUser = null;
        }

        public bool HasPermission(string moduleName)
        {
            if (_currentUser == null) return false;

            // 根据角色判断权限
            return _currentUser.UserRole switch
            {
                UserRole.系统管理员 => true,
                UserRole.账套管理员 => true,
                UserRole.财务主管 => true,
                UserRole.会计 => moduleName switch
                {
                    "凭证管理" or "账簿查询" or "报表中心" or "固定资产" or "工资管理" => true,
                    _ => false
                },
                UserRole.出纳 => moduleName switch
                {
                    "收付款" or "票据管理" => true,
                    _ => false
                },
                UserRole.审计 => true,
                _ => false
            };
        }
    }
}
