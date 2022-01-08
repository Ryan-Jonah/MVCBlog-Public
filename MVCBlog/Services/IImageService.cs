using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    public interface IImageService
    {
        //Gather image from a form input
        Task<byte[]> EncodeImageAsync(IFormFile file);

        //Gather image from within the project
        Task<byte[]> EncodeImageAsync(string fileName);

        string DecodeImage(byte[] imageData, string contentType);

        //Image file extension
        string ContentType(IFormFile file);

        //Image Size
        int Size(IFormFile file);
    }
}
