using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNT.IDP.Common;
using DNT.IDP.DataLayer.Context;
using DNT.IDP.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace DNT.IDP.Services
{
    public interface IUsersService
    {
        Task<bool> AreUserCredentialsValidAsync(string username, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByProviderAsync(string loginProvider, string providerKey);
        Task<User> GetUserBySubjectIdAsync(string subjectId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectIdAsync(string subjectId);
        Task<IEnumerable<UserLogin>> GetUserLoginsBySubjectIdAsync(string subjectId);
        Task<bool> IsUserActiveAsync(string subjectId);
        Task AddUserAsync(User user);
        Task AddUserLoginAsync(string subjectId, string loginProvider, string providerKey);
        Task AddUserClaimAsync(string subjectId, string claimType, string claimValue);
    }

    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;

        public UsersService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _users = _uow.Set<User>();
        }

        public async Task<bool> AreUserCredentialsValidAsync(string username, string password)
        {
            // get the user
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            return user.Password == password.GetSha256Hash();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            return _users.FirstOrDefaultAsync(u =>
                u.UserClaims.Any(c => c.ClaimType == "email" && c.ClaimValue == email));
        }

        public Task<User> GetUserByProviderAsync(string loginProvider, string providerKey)
        {
            return _users
                .FirstOrDefaultAsync(u =>
                    u.UserLogins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));
        }

        public Task<User> GetUserBySubjectIdAsync(string subjectId)
        {
            return _users.FirstOrDefaultAsync(u => u.SubjectId == subjectId);
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            return _users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<UserClaim>> GetUserClaimsBySubjectIdAsync(string subjectId)
        {
            var user = await _users.Include(x => x.UserClaims).FirstOrDefaultAsync(u => u.SubjectId == subjectId);
            return user == null ? new List<UserClaim>() : user.UserClaims.ToList();
        }

        public async Task<IEnumerable<UserLogin>> GetUserLoginsBySubjectIdAsync(string subjectId)
        {
            var user = await _users.Include(x => x.UserLogins).FirstOrDefaultAsync(u => u.SubjectId == subjectId);
            return user == null ? new List<UserLogin>() : user.UserLogins.ToList();
        }

        public async Task<bool> IsUserActiveAsync(string subjectId)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            if (user == null)
            {
                throw new ArgumentException("User with given subjectId not found.", subjectId);
            }

            return user.IsActive;
        }

        public async Task AddUserAsync(User user)
        {
            _users.Add(user);
            await _uow.SaveChangesAsync();
        }

        public async Task AddUserLoginAsync(string subjectId, string loginProvider, string providerKey)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            if (user == null)
            {
                throw new ArgumentException("User with given subjectId not found.", subjectId);
            }

            user.UserLogins.Add(new UserLogin
            {
                SubjectId = subjectId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
			await _uow.SaveChangesAsync();
        }

        public async Task AddUserClaimAsync(string subjectId, string claimType, string claimValue)
        {
            var user = await GetUserBySubjectIdAsync(subjectId);
            if (user == null)
            {
                throw new ArgumentException("User with given subjectId not found.", subjectId);
            }

            user.UserClaims.Add(new UserClaim {ClaimType = claimType, ClaimValue = claimValue});
			await _uow.SaveChangesAsync();
        }
    }
}