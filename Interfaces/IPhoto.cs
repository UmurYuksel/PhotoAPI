using PhotoAPI.DTO_s;
using PhotoAPI.DTO_s;

namespace PhotoAPI.Interfaces
{
    public interface IPhoto
    {

        Task<(bool IsSuccess, PhotoQueryResponseDTO, string ErrorMessage)> GetAllPhotos(int page);

        Task<(bool isSuccess, PhotoResponseDTO, string ErrorMessage)> GetSinglePhotoByIdAndSize(Guid id, int size);

        Task<(bool isSuccess, PhotoQueryResponseDTO, string ErrorMessage)> GetAllResizedPhotos(int size, int page);

        Task<(bool isSuccess, string ErrorMessage)> UploadPhoto(PhotoRequestDTO photo);


    }
}
