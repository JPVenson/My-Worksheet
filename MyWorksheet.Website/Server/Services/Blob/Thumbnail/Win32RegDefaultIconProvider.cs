//using System;
//using System.Collections.Concurrent;
//using System.IO;

//namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail
//{
//	public class Win32RegDefaultIconProvider : IInMemoryThumbnailProvider
//	{
//		public Win32RegDefaultIconProvider()
//		{
//			StoredIcons = new ConcurrentDictionary<string, byte[]>();
//		}

//		public bool TestExtention(string extenstion)
//		{
//			throw new NotImplementedException();
//		}

//		public string[] GetSupportedMimeTypes()
//		{
//			return new string[0];
//		}

//		public byte[] CreateThumbnail(Stream memory, string extenstion, int x, int y)
//		{
//			if (string.IsNullOrWhiteSpace(extenstion))
//			{
//				extenstion = ".blablabla";
//			}

//			if (StoredIcons.ContainsKey(extenstion))
//			{
//				return StoredIcons[extenstion];
//			}

//			var icon = IconReader.GetFileIcon(extenstion, IconReader.IconSize.Large, false);
//			using (var ms = new MemoryStream())
//			{
//				icon.Save(ms);
//				byte[] iconImg;
//				StoredIcons.TryAdd(extenstion, iconImg = ms.ToArray());
//				return iconImg;
//			}
//		}

//		public ConcurrentDictionary<string, byte[]> StoredIcons { get; private set; }
//	}
//}