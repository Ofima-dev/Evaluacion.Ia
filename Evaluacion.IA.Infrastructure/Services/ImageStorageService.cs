using Evaluacion.IA.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Evaluacion.IA.Infrastructure.Services;

public class ImageStorageService : IImageStorageService
{
    private readonly string _imageFolder;
    private readonly string _baseUrl;

    public ImageStorageService()
    {
        // Puedes parametrizar estas rutas según tu entorno/configuración
        _imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
        _baseUrl = "/images/"; // Ajusta según tu configuración de hosting
    }

    public Task<Stream?> GetImageAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult<Stream?>(null);

        // Quita el prefijo de la url si existe
        var fileName = url.Replace(_baseUrl, "").TrimStart('/', '\\');
        var filePath = Path.Combine(_imageFolder, fileName);

        if (!File.Exists(filePath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public async Task<string> SaveImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("El archivo de imagen es inválido.");

        if (!Directory.Exists(_imageFolder))
            Directory.CreateDirectory(_imageFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_imageFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Retorna la URL relativa
        return $"{_baseUrl}{fileName}";
    }
}