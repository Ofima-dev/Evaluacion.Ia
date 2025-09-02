using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Evaluacion.IA.Application.Services;

public interface IImageStorageService
{
    /// <summary>
    /// Guarda una imagen y retorna la URL donde fue almacenada.
    /// </summary>
    /// <param name="file">Archivo de imagen a guardar.</param>
    /// <returns>URL de la imagen almacenada.</returns>
    Task<string> SaveImageAsync(IFormFile file);

    /// <summary>
    /// Obtiene el stream de la imagen almacenada a partir de la URL relativa.
    /// </summary>
    /// <param name="url">URL relativa de la imagen (ej: /images/archivo.jpg)</param>
    /// <returns>Stream de la imagen o null si no existe.</returns>
    Task<Stream?> GetImageAsync(string url);
}