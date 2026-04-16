

using System;
namespace MyWorksheet.Website.Server.Services.StreamPool;

public interface ILocalFileStreamPoolService
{
    StreamingType GetLocalStream(string operation, Guid userId, int operationCounter);
    void RegisterForFinalisation(string key, NotifyFileStream fileStream);
    void Unregister(string key);
}