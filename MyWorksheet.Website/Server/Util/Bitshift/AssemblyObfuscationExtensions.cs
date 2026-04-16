using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Crypto.Prng;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http.Features;
using System;

namespace MyWorksheet.Website.Server.Util.Bitshift;

public static class AssemblyObfuscationExtensions
{
    public static IApplicationBuilder UseAssemblyObfuscation(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder.Use((context, next) =>
        {
            var requestPath = context.Request.Path;
            var fileExtension = Path.GetExtension(requestPath.Value);

            if (ComponentsWebAssemblyApplicationBuilderExtensions.ObfuscatableExtensions.Contains(fileExtension))
            {
                var gen = new CryptoApiRandomGenerator();
                gen.AddSeedMaterial(Random.Shared.Next());
                var key = new byte[32];
                var iv = new byte[16];
                gen.NextBytes(key);
                gen.NextBytes(iv);
                var originalBodyFeature = context.Features.Get<IHttpResponseBodyFeature>();
                context.Response.Headers.Append("x-assembly-obfuscation-key", string.Join(" ", key.Select(f => f.ToString("X2"))));
                context.Response.Headers.Append("x-assembly-obfuscation-iv", string.Join(" ", iv.Select(f => f.ToString("X2"))));
                context.Response.Headers.Append("x-assembly-obfuscation-mode", "xor");
                context.Response.Headers.CacheControl = new StringValues("no-cache");

                context.Features.Set<IHttpResponseBodyFeature>(new BitshifedBody(context,
                    originalBodyFeature,
                    key,
                    iv));
            }

            return next();
        });
    }
}
