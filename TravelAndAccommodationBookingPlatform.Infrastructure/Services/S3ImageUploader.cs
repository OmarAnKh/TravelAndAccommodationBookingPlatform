using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class S3ImageUploader : IImageUploader
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3ImageUploader(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
    }


    public async Task<string> UploadImagesAsync(List<IFormFile> files, ImageEntityType imageType)
    {

        if (files == null || files.Count == 0)
            throw new ArgumentException("File is null or empty");

        var key = $"{imageType.ToString().ToLower()}/{Guid.NewGuid()}";

        foreach (var file in files)
        {
            var imageKey = $"{key}/{Guid.NewGuid()}";

            var request = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = imageKey,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(request);
        }

        var imagePath = $"https://{_bucketName}.s3.amazonaws.com/{key}/";
        return imagePath;
    }


    public Task DeleteImageAsync(string imageUrl)
    {
        var path = Path.Combine("wwwroot", imageUrl.TrimStart('/'));

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}