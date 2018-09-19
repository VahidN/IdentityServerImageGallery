using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNT.IDP.DataLayer.Context;
using DNT.IDP.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace DNT.IDP.Services
{
    public interface IUserClaimsService
    {
        Task<UserClaim> GetUserClaimAsync(string subjectId, string claimType);
        Task<List<UserClaim>> GetUserClaimsAsync(string subjectId, IList<string> claimTypes);
        Task AddOrUpdateUserClaimValueAsync(string subjectId, string claimType, string claimValue);
        Task AddOrUpdateUserClaimValuesAsync(
            string subjectId,
            IEnumerable<(string ClaimType, string ClaimValue)> userClaims);
    }

    public class UserClaimsService : IUserClaimsService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserClaim> _userClaims;

        public UserClaimsService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userClaims = _uow.Set<UserClaim>();
        }

        public Task<UserClaim> GetUserClaimAsync(string subjectId, string claimType)
        {
            return _userClaims.FirstOrDefaultAsync(userClaim =>
                userClaim.ClaimType == claimType && userClaim.SubjectId == subjectId);
        }

        public Task<List<UserClaim>> GetUserClaimsAsync(string subjectId, IList<string> claimTypes)
        {
            return _userClaims.Where(
                    userClaim => userClaim.SubjectId == subjectId && claimTypes.Contains(userClaim.ClaimType))
                .ToListAsync();
        }

        public async Task AddOrUpdateUserClaimValuesAsync(
            string subjectId,
            IEnumerable<(string ClaimType, string ClaimValue)> userClaims)
        {
            foreach (var userClaim in userClaims)
            {
                var dbRecord = await _userClaims.FirstOrDefaultAsync(dbClaim =>
                    dbClaim.ClaimType == userClaim.ClaimType &&
                    dbClaim.SubjectId == subjectId);
                if (dbRecord == null)
                {
                    _userClaims.Add(new UserClaim
                    {
                        ClaimType = userClaim.ClaimType,
                        ClaimValue = userClaim.ClaimValue,
                        SubjectId = subjectId
                    });
                }
                else
                {
                    dbRecord.ClaimValue = userClaim.ClaimValue;
                }
            }

            await _uow.SaveChangesAsync();
        }

        public async Task AddOrUpdateUserClaimValueAsync(string subjectId, string claimType, string claimValue)
        {
            var dbRecord = await _userClaims.FirstOrDefaultAsync(dbClaim =>
                dbClaim.ClaimType == claimType &&
                dbClaim.SubjectId == subjectId);
            if (dbRecord == null)
            {
                _userClaims.Add(new UserClaim
                {
                    ClaimType = claimType,
                    ClaimValue = claimValue,
                    SubjectId = subjectId
                });
            }
            else
            {
                dbRecord.ClaimValue = claimValue;
            }
            await _uow.SaveChangesAsync();
        }
    }
}