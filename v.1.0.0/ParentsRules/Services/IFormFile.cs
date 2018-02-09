using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParentsRules.Services
{
    public interface IFormFile
    {
        // Gets the raw Content-Type header of the uploaded file.
        string ContentType { get; }

        // Gets the raw Content-Disposition header of the uploaded file.
        string ContentDisposition { get; }

        // Gets the header dictionary of the uploaded file.
        IHeaderDictionary Headers { get; }

        // Gets the file length in bytes.
        long Length { get; }

        // Gets the name from the Content-Disposition header.
        string Name { get; }

        // Gets the file name from the Content-Disposition header.
        string FileName { get; }

        // Copies the contents of the uploaded file to the target stream.
        void CopyTo(Stream target);

        // Asynchronously copies the contents of the uploaded file to the target stream.
        Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken));

        // Opens the request stream for reading the uploaded file.
        Stream OpenReadStream();

    }
}
