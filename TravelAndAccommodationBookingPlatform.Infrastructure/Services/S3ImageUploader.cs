using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Services;

public class S3ImageUploader : IImageUploader
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _region;

    public S3ImageUploader(IAmazonS3 s3Client, string bucketName, string region = "il-central-1")
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
        _region = region;
    }

    public async Task<string> UploadImagesAsync(List<IFormFile> files, ImageEntityType imageType)
    {
        if (files == null || files.Count == 0)
            throw new ArgumentException("File is null or empty");

        var key = $"{imageType.ToString().ToLower()}/{Guid.NewGuid()}";

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var imageKey = $"{key}/{Guid.NewGuid()}{fileExtension}";

            var request = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = imageKey,
                BucketName = _bucketName,
                ContentType = file.ContentType,
                Metadata =
                {
                    ["original-filename"] = file.FileName,
                    ["upload-date"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            var transfer = new TransferUtility(_s3Client);
            await transfer.UploadAsync(request);
        }

        var imagePath = $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}/";
        return imagePath;
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var key = uri.AbsolutePath.TrimStart('/');

            if (key.EndsWith("/"))
            {
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = key
                };

                var listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                foreach (var obj in listResponse.S3Objects)
                {
                    await _s3Client.DeleteObjectAsync(_bucketName, obj.Key);
                }
            }
            else
            {
                await _s3Client.DeleteObjectAsync(_bucketName, key);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete image: {ex.Message}", ex);
        }
    }
    public async Task<List<string>> GetImageUrlsAsync(string imagesUrl)
    {
        var uri = new Uri(imagesUrl);
        var prefix = uri.AbsolutePath.TrimStart('/'); // e.g., hotels/uuid/

        var request = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = prefix
        };

        var urls = new List<string>();
        ListObjectsV2Response response;

        do
        {
            response = await _s3Client.ListObjectsV2Async(request);

            if (response?.S3Objects != null)
            {
                urls.AddRange(response.S3Objects.Select(o =>
                    $"https://{_bucketName}.s3.{_region}.amazonaws.com/{o.Key}"));
            }

            request.ContinuationToken = response.NextContinuationToken;

        } while (response.IsTruncated ?? false);

        return urls;
    }

}