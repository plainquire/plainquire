using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Plainquire.Integration.Tests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostFormFile(this HttpClient client, string route, IFormFile? file)
    {
        using var multipartContent = new MultipartFormDataContent();
        if (file != null)
        {
            multipartContent.Add(ReadFileBytes(file), file.FileName, file.FileName);
            multipartContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = file.FileName, FileName = file.FileName };
        }
        return await client.PostAsync(route, multipartContent);
    }

    private static ByteArrayContent ReadFileBytes(IFormFile? file)
    {
        if (file == null)
            return new ByteArrayContent(Array.Empty<byte>());

        using var fileStream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        fileStream.CopyTo(memoryStream);
        return new ByteArrayContent(memoryStream.ToArray());
    }
}