using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ImageGallery.WebApi.DomainClasses;
using ImageGallery.WebApi.Models;
using ImageGallery.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.WebApi.WebApp.Controllers
{
    [Route("api/images")]
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly IImagesService _imagesService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;

        public ImagesController(
            IImagesService imagesService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            _imagesService = imagesService ?? throw new ArgumentNullException(nameof(imagesService));
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        public async Task<IActionResult> GetImages()
        {
            var ownerId = this.User.Claims.FirstOrDefault(claim => claim.Type == "sub").Value;
            var imagesFromRepo = await _imagesService.GetImagesAsync(ownerId);
            var imagesToReturn = _mapper.Map<IEnumerable<ImageModel>>(imagesFromRepo);
            return Ok(imagesToReturn);
        }

        [HttpGet("{id}", Name = "GetImage")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var imageFromRepo = await _imagesService.GetImageAsync(id);
            if (imageFromRepo == null)
            {
                return NotFound();
            }

            var imageToReturn = _mapper.Map<ImageModel>(imageFromRepo);
            return Ok(imageToReturn);
        }

        [HttpPost]
        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> CreateImage([FromBody] ImageForCreationModel imageForCreation)
        {
            if (imageForCreation == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                // return 422 - Unprocessable Entity when validation fails
                return new UnprocessableEntityObjectResult(ModelState);
            }

            // Automapper maps only the Title in our configuration
            var imageEntity = _mapper.Map<Image>(imageForCreation);

            // Create an image from the passed-in bytes (Base64), and
            // set the filename on the image

            // get this environment's web root path (the path
            // from which static content, like an image, is served)
            var webRootPath = _hostingEnvironment.WebRootPath;

            // create the filename
            string fileName = $"{Guid.NewGuid().ToString()}.jpg";

            // the full file path
            var filePath = Path.Combine($"{webRootPath}/images/{fileName}");

            // write bytes and auto-close stream
            System.IO.File.WriteAllBytes(filePath, imageForCreation.Bytes);

            // fill out the filename
            imageEntity.FileName = fileName;

            // set the ownerId on the imageEntity
            var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            imageEntity.OwnerId = ownerId;

            // add and save.
            await _imagesService.AddImageAsync(imageEntity);
            var imageToReturn = _mapper.Map<Image>(imageEntity);
            return CreatedAtRoute("GetImage", new { id = imageToReturn.Id }, imageToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var imageFromRepo = await _imagesService.GetImageAsync(id);
            if (imageFromRepo == null)
            {
                return NotFound();
            }

            await _imagesService.DeleteImageAsync(imageFromRepo);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(Guid id, [FromBody] ImageForUpdateModel imageForUpdate)
        {
            if (imageForUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                // return 422 - Unprocessable Entity when validation fails
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var imageFromRepo = await _imagesService.GetImageAsync(id);
            if (imageFromRepo == null)
            {
                return NotFound();
            }

            _mapper.Map(imageForUpdate, imageFromRepo);
            await _imagesService.UpdateImageAsync(imageFromRepo);
            return NoContent();
        }
    }
}