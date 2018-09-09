using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ImageGallery.WebApi.Services
{
    public class MustOwnImageRequirement : IAuthorizationRequirement
    {
    }

    public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
    {
        private readonly IImagesService _imagesService;
        private readonly ILogger<MustOwnImageHandler> _logger;

        public MustOwnImageHandler(
            IImagesService imagesService,
            ILogger<MustOwnImageHandler> logger)
        {
            _imagesService = imagesService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, MustOwnImageRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
            {
                context.Fail();
                return;
            }

            var imageId = filterContext.RouteData.Values["id"].ToString();
            if (!Guid.TryParse(imageId, out Guid imageIdAsGuid))
            {
                _logger.LogError($"`{imageId}` is not a Guid.");
                context.Fail();
                return;
            }

            var subClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sub");
            if (subClaim == null)
            {
                _logger.LogError($"User.Claims don't have the `sub` claim.");
                context.Fail();
                return;
            }

            var ownerId = subClaim.Value;
            if (!await _imagesService.IsImageOwnerAsync(imageIdAsGuid, ownerId))
            {
                _logger.LogError($"`{ownerId}` is not the owner of `{imageIdAsGuid}` image.");
                context.Fail();
                return;
            }

            // all checks out
            context.Succeed(requirement);
        }
    }
}