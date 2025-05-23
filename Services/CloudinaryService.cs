using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(string cloudinaryUrl)
    {
        if (string.IsNullOrEmpty(cloudinaryUrl))
            throw new ArgumentException("CLOUDINARY_URL não está configurada");

        var uri = new Uri(cloudinaryUrl);

        var account = new Account(
            uri.Host, // cloud_name
            uri.UserInfo.Split(':')[0], // api_key
            uri.UserInfo.Split(':')[1]  // api_secret
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        using var stream = image.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(image.FileName, stream),
            Transformation = new Transformation().Quality("auto").FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.AbsoluteUri;
        }
        else
        {
            throw new Exception($"Erro no upload da imagem: {uploadResult.Error?.Message}");
        }
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        // Extrair public_id da URL
        var uri = new Uri(imageUrl);
        var segments = uri.Segments;
        var fileName = segments[^1]; // pega o último segmento (ex: imagem.jpg)
        var publicId = Path.GetFileNameWithoutExtension(fileName); // remove extensão

        var deletionParams = new DeletionParams(publicId);
        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

        if (deletionResult.Result != "ok")
        {
            throw new Exception("Erro ao deletar imagem do Cloudinary");
        }
    }
}
