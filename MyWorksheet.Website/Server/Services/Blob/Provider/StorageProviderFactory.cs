using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public delegate IBlobProvider StorageProviderFactory(Guid storageInstance, StorageProviderData[] data);