<Query Kind="Program">
  <Reference>C:\Kuriimu\Cetera.dll</Reference>
  <Reference>C:\Kuriimu\Kontract.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Drawing.dll</Reference>
  <Namespace>Kontract</Namespace>
  <Namespace>Kontract.Interface</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>Kontract.IO</Namespace>
  <Namespace>Kontract.Image</Namespace>
  <Namespace>Kontract.Image.Format</Namespace>
  <Namespace>Kontract.Image.Swizzle</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
</Query>

void Main(string[] argv)
{
	/// RawJTEX to PNG Importer
	/// For Vulpes-Vulpeos
	/// By IcySon55
	/// v1.0
	/// This script requires Kuriimu v1.0.14+ to be in C:\Kuriimu

	Directory.SetCurrentDirectory(Path.GetDirectoryName(Util.CurrentQueryPath));

	// Decompress
	foreach (var file in argv)
	{
		if (!File.Exists(file)) continue;

		var fileName = Path.GetFileNameWithoutExtension(file);
		var extension = Path.GetExtension(file);
		var decompName = $"{fileName}.decomp.jtex";

		if (extension != ".png") continue;
		if (!File.Exists($"{decompName}")) continue;

		CN_JTEX_Header header;
		using (var br = new BinaryReaderX(File.OpenRead(decompName)))
			header = br.ReadStruct<CN_JTEX_Header>();
		if (header == null) continue;

		using (var bw = new BinaryWriterX(File.Create(decompName)))
		{
			var settings = new ImageSettings
			{
				Width = header.Width,
				Height = header.Height,
				Format = Format[header.Format],
				Swizzle = new CTRSwizzle(header.Width, header.Height)
			};

			var imageData = Common.Save(new Bitmap(file), settings);

			bw.WriteStruct(header);
			bw.Write(imageData);
		}

		var comp = Kontract.Compression.Nintendo.Compress(File.OpenRead(decompName), Kontract.Compression.Nintendo.Method.LZ11);
		File.WriteAllBytes($"{fileName}.jtex", comp);
	}
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
class CN_JTEX_Header
{
	public int Format;
	public int virWidth;
	public int virHeight;
	public int Width;
	public int Height;
}

public static Dictionary<int, IImageFormat> Format = new Dictionary<int, IImageFormat>
{
	[2] = new RGBA(8, 8, 8, 8),
	[3] = new RGBA(8, 8, 8),
	[4] = new RGBA(4, 4, 4, 4)
};