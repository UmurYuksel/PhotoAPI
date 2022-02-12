using PhotoAPI.DTO_s;
using PhotoAPI.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PhotoAPI.Controllers
{
    [Produces("application/json", "application/xml")]
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private readonly IPhoto _photoService;

        //DI
        public PhotosController(IPhoto photoService)
        {
            _photoService = photoService;
        }

        /// <summary>
        /// Returns Single Photo Item with Given Size in Base64 string.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="size"></param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /GetSinglePhoto
        ///     {
        ///        "id": 1,
        ///        "size": "128",
        ///     }
        ///     
        ///     *Note: If you input the size as 0, it returns the original size of the picture.
        ///
        /// </remarks>
        /// <response code="201">Returns Created Status 201</response>
        /// <response code="404">Return 404 if no data found.</response>
        /// <returns>PhotoResp</returns>
        [ProducesResponseType(typeof(PhotoResponseDTO), StatusCodes.Status200OK)]
        [HttpGet("GetSinglePhoto/{id}/{size?}")]
        public async Task<IActionResult> GetPhotoById(Guid id, int size)
        {
            var result = await _photoService.GetSinglePhotoByIdAndSize(id, size);
            if (result.isSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }


        /// <summary>
        /// Returns All Photos with their Original Size in Base64 .
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <returns>List of Photo Items</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /GetAllPhotos     
        /// 
        /// </remarks>
        /// <response code="200">List of Photo Items</response>
        /// <response code="404">If No item found</response>
        [ProducesResponseType(typeof(List<PhotoResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet("GetAllPhotos/{page}")]
        public async Task<IActionResult> GetAllPhotos(int page)
        {
            var result = await _photoService.GetAllPhotos(page);
            if (result.IsSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }

        /// <summary>
        /// Returns All Photos as Thumbnail.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <returns>Current Page, Total Page Number and List of Resized Photo Items with Src Compatible Base64</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /GetAllPhotos/Thumbnail
        ///     
        ///
        ///     {
        ///       "page":1  
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">List of Thumbnail Photo Items</response>
        /// <response code="404">If No item found</response>
        [ProducesResponseType(typeof(List<PhotoResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet("GetAllPhotos/Thumbnail/{page}")]
        public async Task<IActionResult> GetThumbnailPhotos(int page)
        {
            var result = await _photoService.GetAllResizedPhotos(128,page);
            if (result.isSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }


        /// <summary>
        /// Returns All Photos with Small Size
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <returns>Current Page, Total Page Number and List of Resized Photo Items with Src Compatible Base64</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /GetAllPhotos/Small
        ///         
        /// </remarks>
        /// <response code="200">List of Resized Photo Items</response>
        /// <response code="404">If No item found</response>
        [ProducesResponseType(typeof(List<PhotoResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet("GetAllPhotos/Small/{page}")]
        public async Task<IActionResult> GetSmallSizePhotos(int page)
        {
            var result = await _photoService.GetAllResizedPhotos(512, page);
            if (result.isSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }


        /// <summary>
        /// Returns All Photos with Large Size
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Current Page, Total Page Number and List of Resized Photo Items with Src Compatible Base64</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /GetAllPhotos/Large
        ///     
        ///
        /// </remarks>
        /// <response code="200">List of Resized Photo Items</response>
        /// <response code="404">If No item found</response>
        [ProducesResponseType(typeof(List<PhotoResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet("GetAllPhotos/Large/{page}")]
        public async Task<IActionResult> GetLargeSizePhotos(int page)
        {
            var result = await _photoService.GetAllResizedPhotos(2048, page);
            if (result.isSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }


        /// <summary>
        /// Returns All Photos with Custom Size
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns>Current Page, Total Page Number and List of Custom Resized Photo Items with Src Compatible Base64</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /GetAllPhotos/Custom/{size}
        ///
        /// </remarks>
        /// <response code="200">List of Resized Photo Items</response>
        /// <response code="404">If No item found</response>
        [ProducesResponseType(typeof(List<PhotoResponseDTO>), StatusCodes.Status200OK)]
        [HttpGet("GetAllPhotos/Custom/{size}/{page}")]
        public async Task<IActionResult> GetCustomSizePhotos(int size, int page)
        {
            var result = await _photoService.GetAllResizedPhotos(size,page);
            if (result.isSuccess)
            {
                return Ok(result.Item2);
            }
            return NotFound(result.ErrorMessage);
        }

        /// <summary>
        /// 
        ///  Endpoint for Posting New Photo Item. Avaliable for Test.
        /// 
        /// </summary>
        /// 
        /// <returns>If Success, only 201 Status</returns>
        /// <remarks>
        ///  
        ///   POST /Photos
        ///  
        ///  **This endpoint accepts images with max file size of 30_000_000 in bytes.
        ///  
        ///  **There is a IP Rate Limiting for preventing unnecessary storage space usage. Only 10 request can be sent from the same ip within 5 minutes. This rule applies for all the endpoints.
        /// 
        ///  Sample "multipart/form-data" request =>
        /// 
        ///  For successfull post request from any js based application: Do not set content-type header.
        /// 
        ///  var formData = new FormData();
        ///  
        ///  formData.append('Id', null=>using guid id for demo purpose);
        ///  
        ///  formData.append('UserId', 1);
        ///  
        ///  formData.append('MarkerId', 1);
        ///  
        ///  formData.append('MarkerId', 1);
        ///  
        ///  formData.append('ImageFile', $('input[type=file]')[0].files[0]); 
        ///  
        ///  fetch("/thisapi",
        ///  {
        ///    body: formData,
        ///    method: "post"
        ///  });
        ///     
        ///  
        /// </remarks>
        /// <response code="201">Image Uploaded Successfully</response>
        /// <response code="400">Produces 400, If there was an error while uploading the image</response>
        [HttpPost]
        [RequestSizeLimit(bytes: 30_000_000)]
        public async Task<IActionResult> Post([FromForm] PhotoRequestDTO photo)
        {
            if (photo.ImageFile.ContentType is "image/jpeg" or "image/jpg" or "image/png" or "image/x-png")
            {
                var (isSuccess, ErrorMessage) = await _photoService.UploadPhoto(photo);

                if (isSuccess)
                {
                    return StatusCode(StatusCodes.Status201Created);
                }

                return BadRequest(ErrorMessage);
            }
            return BadRequest("You can only send files in jpg or png extension.");
        }


    }
}
