using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Services.LocalStorage.Entities;

public class StorageState
{
    public Version Version { get; set; }
}

public class PresentationState
{
    public bool Enabled { get; set; }
}