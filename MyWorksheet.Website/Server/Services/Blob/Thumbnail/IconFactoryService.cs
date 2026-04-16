namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

//[SingletonService(typeof(IIconFactoryService))]
//public class IconFactoryService : IIconFactoryService
//{
//	public IconFactoryService()
//	{
//		ShellItemImageFactories = new ConcurrentDictionary<string, IconFactoryWin32.IShellItemImageFactory>();
//	}

//	private ConcurrentDictionary<string, IconFactoryWin32.IShellItemImageFactory> ShellItemImageFactories { get; set; }

//	public Bitmap ExtractThumbnail(string filePath, Size size, IconFactoryWin32.SIIGBF flags)
//	{
//		if (filePath == null)
//			throw new ArgumentNullException("filePath");

//		var extention = Path.GetExtension(filePath);

//		IconFactoryWin32.IShellItemImageFactory factory;
//		int hr;
//		if (!ShellItemImageFactories.TryGetValue(extention.ToLower(), out factory))
//		{
//			hr = IconFactoryWin32.SHCreateItemFromParsingName(filePath, IntPtr.Zero, typeof(IconFactoryWin32.IShellItemImageFactory).GUID, out factory);
//			if (hr != 0 || factory == null)
//				throw new Win32Exception(hr);
//			ShellItemImageFactories.TryAdd(extention.ToLower(), factory);
//		}

//		IntPtr bmp;
//		hr = factory.GetImage(size, flags, out bmp);
//		if (hr != 0)
//			throw new Win32Exception(hr);

//		return Image.FromHbitmap(bmp);
//	}
//}