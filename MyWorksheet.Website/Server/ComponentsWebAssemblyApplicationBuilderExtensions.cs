using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Util.Bitshift;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace MyWorksheet.Website.Server;

/// <summary>
///     Extensions for mapping Blazor WebAssembly applications.
/// </summary>
public static class ComponentsWebAssemblyApplicationBuilderExtensions
{
    // Only assembly-type files go through loadBootResource on the client and can be XOR-deobfuscated.
    // .js and .wasm are loaded directly by the .NET 9+ runtime, bypassing loadBootResource entirely.
    public static readonly HashSet<string> ObfuscatableExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".dll", ".dat", ".pdb", "wasm" };

    /// <summary>
    ///     Configures the application to serve Blazor WebAssembly framework files from the path <paramref name="pathPrefix" />
    ///     . This path must correspond to a referenced Blazor WebAssembly application project.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" />.</param>
    /// <param name="pathPrefix">
    ///     The <see cref="PathString" /> that indicates the prefix for the Blazor WebAssembly
    ///     application.
    /// </param>
    /// <returns>The <see cref="IApplicationBuilder" /></returns>
    public static IApplicationBuilder UseBlazorFrameworkFilesWithBitshift(this IApplicationBuilder builder, PathString pathPrefix)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var webHostEnvironment = builder.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
        var webRootFileProvider = webHostEnvironment.WebRootFileProvider;
        var options = CreateStaticFilesOptions(webRootFileProvider);

        builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments(pathPrefix, out var rest) &&
                               rest.StartsWithSegments("/_framework") &&
                               // .js and .wasm files are loaded directly by the .NET runtime in .NET 9+ and bypass
                               // loadBootResource, so the client-side XOR deobfuscation never runs for them.
                               // Only assemblies (.dll / .dat) go through loadBootResource and can be deobfuscated.
                               ObfuscatableExtensions.Contains(Path.GetExtension(rest.Value)),
                        subBuilder =>
                        {
                            subBuilder.Use((context, next) =>
                            {
                                context.Response.Headers.Append("Blazor-Environment", webHostEnvironment.EnvironmentName);

                                return next();
                            });

                            //subBuilder.UseMiddleware<ContentEncodingNegotiator>();
                            subBuilder.UseAssemblyObfuscation();
                            subBuilder.UseStaticFiles(options);
                        });

        builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/meta_framework") && HttpMethods.IsHead(ctx.Request.Method),
                        subBuilder =>
                        {
                            subBuilder.Use((context, next) =>
                            {
                                if (!context.Request.Query.TryGetValue("name", out var filename))
                                {
                                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                                    return next();
                                }

                                var fileInfo = webRootFileProvider.GetFileInfo(filename);

                                if (fileInfo.Exists)
                                {
                                    context.Response.Headers["x-file-size"] = fileInfo.Length.ToString();
                                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                                }
                                else
                                {
                                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                                }

                                return Task.CompletedTask;
                            });
                        });

        return builder;
    }

    /// <summary>
    ///     Configures the application to serve Blazor WebAssembly framework files from the root path "/".
    /// </summary>
    /// <param name="applicationBuilder">The <see cref="IApplicationBuilder" />.</param>
    /// <returns>The <see cref="IApplicationBuilder" /></returns>
    public static IApplicationBuilder UseBlazorFrameworkFilesWithBitshift(this IApplicationBuilder applicationBuilder)
    {
        return UseBlazorFrameworkFilesWithBitshift(applicationBuilder, default);
    }

    private static StaticFileOptions CreateStaticFilesOptions(IFileProvider webRootFileProvider)
    {
        var options = new StaticFileOptions();
        options.FileProvider = webRootFileProvider;
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        AddMapping(contentTypeProvider, ".dll", MediaTypeNames.Application.Octet);
        // We unconditionally map pdbs as there will be no pdbs in the output folder for
        // release builds unless BlazorEnableDebugging is explicitly set to true.
        AddMapping(contentTypeProvider, ".pdb", MediaTypeNames.Application.Octet);
        AddMapping(contentTypeProvider, ".br", MediaTypeNames.Application.Octet);
        AddMapping(contentTypeProvider, ".dat", MediaTypeNames.Application.Octet);
        AddMapping(contentTypeProvider, ".wasm", "application/wasm");
        options.ContentTypeProvider = contentTypeProvider;

        // Static files middleware will try to use application/x-gzip as the content
        // type when serving a file with a gz extension. We need to correct that before
        // sending the file.
        options.OnPrepareResponse = fileContext =>
        {
            // At this point we mapped something from the /_framework
            fileContext.Context.Response.Headers.Append(HeaderNames.CacheControl, "no-cache");
            var requestPath = fileContext.Context.Request.Path;
            var fileExtension = Path.GetExtension(requestPath.Value);

            if (string.Equals(fileExtension, ".gz") || string.Equals(fileExtension, ".br"))
            {
                // When we are serving framework files (under _framework/ we perform content negotiation
                // on the accept encoding and replace the path with <<original>>.gz|br if we can serve gzip or brotli content
                // respectively.
                // Here we simply calculate the original content type by removing the extension and apply it
                // again.
                // When we revisit this, we should consider calculating the original content type and storing it
                // in the request along with the original target path so that we don't have to calculate it here.
                var originalPath = Path.GetFileNameWithoutExtension(requestPath.Value);

                if (contentTypeProvider.TryGetContentType(originalPath, out var originalContentType))
                {
                    fileContext.Context.Response.ContentType = originalContentType;
                }
            }

            fileContext.Context.Response.Headers.ContentLength = fileContext.File.Length;
        };

        return options;
    }

    private static void AddMapping(FileExtensionContentTypeProvider provider, string name, string mimeType)
    {
        if (!provider.Mappings.ContainsKey(name))
        {
            provider.Mappings.Add(name, mimeType);
        }
    }
}
