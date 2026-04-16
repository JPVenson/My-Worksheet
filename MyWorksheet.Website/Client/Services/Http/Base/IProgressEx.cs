using System;

namespace MyWorksheet.Website.Client.Services.Http.Base;

public interface IProgressEx<in T> : IProgress<T>
{
    void SetMax(T value);
}