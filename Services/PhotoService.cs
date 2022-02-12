using AutoMapper;
using PhotoAPI.DTO_s;
using PhotoAPI.Interfaces;
using PhotoAPI.Models;
using MetadataExtractor;
using Microsoft.EntityFrameworkCore;
using PhotoAPI.Data;


namespace PhotoAPI.Services
{
    public class PhotoService : IPhoto
    {
        private readonly PhotoDbContext _dbContext;
        private readonly IMapper _mapper;

        public PhotoService(PhotoDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, PhotoQueryResponseDTO, string ErrorMessage)> GetAllPhotos(int page)
        {

            if (page < 1)
            {
                return (false, null, "Page number starts from 1");
            }
     

            //Calculating the total pages, and fetch specific number of rows for each page.
            var pageResults = 5f;
            var pageCount = Math.Ceiling(_dbContext.Photos.Count()/ pageResults);
            var photosList = await _dbContext.Photos.Skip((page - 1) * (int)pageResults).Take((int)pageResults).ToListAsync();

            if (photosList.Any())
            {
                foreach (var item in photosList)
                {
                    item.Base64 = await Helpers.Helpers.ConvertFromBytesToBase64(item.ImagePath);
                }

                var result = _mapper.Map<List<PhotoResponseDTO>>(photosList);

                var photoQueryListResponse = new PhotoQueryResponseDTO { CurrentPage = page, Pages= (int)pageCount, Photos=result };

                return (true, photoQueryListResponse, null);
            }
            return (false, null, "No Photos Found");
        }

 
        public async Task<(bool isSuccess, PhotoQueryResponseDTO, string ErrorMessage)> GetAllResizedPhotos(int size, int page)
        {

            if (page < 1)
            {
                return (false, null, "Page number starts from 1");
            }

            var pageResults = 5f;
            var pageCount = Math.Ceiling(_dbContext.Photos.Count() / pageResults);
            var photosList = await _dbContext.Photos.Skip((page - 1) * (int)pageResults).Take((int)pageResults).ToListAsync();

            if (photosList.Any())
            {
                foreach (var item in photosList)
                {
                    
                    var (isSuccess, resizedbs64, ErrorMessage) = await Helpers.Helpers.ProcessImage(item.ImagePath, size);

                    if (isSuccess)
                    {
                        item.ImageHeight = size.ToString() + " Pixels";
                        item.ImageWidth = size.ToString() + " Pixels";
                        item.Base64 = resizedbs64;
                    }
                    else
                    {
                        item.ImageHeight = size.ToString() + " Pixels";
                        item.ImageWidth = size.ToString() + " Pixels";
                        item.Base64 = ErrorMessage;
                    }
                }

                var result = _mapper.Map<List<PhotoResponseDTO>>(photosList);

                var photoQueryListResponse = new PhotoQueryResponseDTO { CurrentPage = page, Pages = (int)pageCount, Photos = result };

                return (true, photoQueryListResponse, null);
            }
            return (false, null, "No Photo Data Found");

        }

        public async Task<(bool isSuccess, PhotoResponseDTO, string ErrorMessage)> GetSinglePhotoByIdAndSize(Guid id, int size)
        {
            var photoObject = await _dbContext.Photos.FindAsync(id);
            if (photoObject != null)
            {
                if (size > 0 && size != null)
                {
                    var (isSuccess, resizedbs64, ErrorMessage) = await Helpers.Helpers.ProcessImage(photoObject.ImagePath, size);
                    if (isSuccess)
                    {
                        photoObject.Base64 = resizedbs64;
                        photoObject.ImageHeight = size.ToString() + " Pixels";
                        photoObject.ImageWidth = size.ToString() + " Pixels";

                        var result = _mapper.Map<PhotoResponseDTO>(photoObject);
                        return (true, result, null);
                    }
                    return (false, null, ErrorMessage);

                }
                else
                {
                    photoObject.Base64 = await Helpers.Helpers.ConvertFromBytesToBase64(photoObject.ImagePath);
                    photoObject.ImageHeight = size.ToString() + " Pixels";
                    photoObject.ImageWidth = size.ToString() + " Pixels";

                    var result = _mapper.Map<PhotoResponseDTO>(photoObject);
                    return (true, result, null);
                }
            }
            return (false, null, "No Photo Found");
        }


        public async Task<(bool isSuccess, string ErrorMessage)> UploadPhoto(PhotoRequestDTO photoDto)

        {
            if (photoDto != null)
            {
                //First Upload the image to some spefic folder.
                (bool isSuccess, (PhotoMetadata meta, string path), string ErrorMessage) imageUploadResult = await SaveImage(photoDto.ImageFile);

                var photoObj = _mapper.Map<Photo>(photoDto);
                photoObj.ImagePath = imageUploadResult.Item2.path;
                photoObj.ImageHeight = imageUploadResult.Item2.meta.ImageHeight;
                photoObj.ImageWidth = imageUploadResult.Item2.meta.ImageWidth;
                photoObj.ImageType = imageUploadResult.Item2.meta.ImageType;

                await _dbContext.Photos.AddAsync(photoObj);
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            return (false, "Please provide photo data");
        }

        //Image Saving and Extracting Metadata related methods =>
        private async Task<(bool isSuccess, (PhotoMetadata meta, string path), string ErrorMessage)> SaveImage(IFormFile imageFile)
        {
            PhotoMetadata photoMetadata = new();
            var imagePath = await Helpers.Helpers.WriteImageToFolder(imageFile);
            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(imagePath);

            //Depending on the image extension, following lines extracts some metadata for demo purpose.
            var jpgFileType = directories.OfType<MetadataExtractor.Formats.Jpeg.JpegDirectory>().FirstOrDefault();
            var pngFileType = directories.OfType<MetadataExtractor.Formats.Png.PngDirectory>().FirstOrDefault();

            //For Demo Purpose, only getting some specific details from the image's metadata.
            if (jpgFileType != null)
            {
                foreach (var tag in jpgFileType.Tags)
                {
                    if (tag.Name == "Image Height")
                    {
                        photoMetadata.ImageHeight = tag.Description;
                    }
                    else if (tag.Name == "Image Width")
                    {
                        photoMetadata.ImageWidth = tag.Description;
                    }
                    else
                    {
                        continue;
                    }

                }
                photoMetadata.ImageType = "jpg";
            }
            else if (pngFileType != null)
            {
                foreach (var tag in pngFileType.Tags)
                {
                    if (tag.Name == "Image Height")
                    {
                        photoMetadata.ImageHeight = tag.Description;
                    }
                    else if (tag.Name == "Image Width")
                    {
                        photoMetadata.ImageWidth = tag.Description;
                    }
                    else
                    {
                        continue;
                    }
                }
                photoMetadata.ImageType = "png";
            }
            else
            {
                return (false, (null, null), "Error while extracting the metadata");
            }

            return (true, (photoMetadata, imagePath), null);
        }
    }
}
