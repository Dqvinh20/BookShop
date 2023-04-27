using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Core.Contracts.Services;
public interface IStorageRepository
{
    /// <summary>
    /// Upload Image
    /// </summary>
    Task<string> UploadImageAsync(string imagePath, string imageName, EventHandler<float> onProgress = null);

    /// <summary>
    /// Delete Image
    /// </summary>
    Task<bool> DeleteImageAsync(string imagePath);
}
