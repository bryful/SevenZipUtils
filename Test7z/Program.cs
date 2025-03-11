// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using FsSevenZip;

internal partial class Program
{
	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	static extern int MessageBoxW(nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpText, [MarshalAs(UnmanagedType.LPWStr)] string lpCaption, uint uType);
	
	
	private static int Main(string[] args)
	{
		if (args.Length <= 0)
		{
			Console.WriteLine("no prams");
			return 1;
		}
		ArcInfo[] list = SevenZipUtils.Listup(args[0]);

		int rc = 0;
		foreach (ArcInfo file in list)
		{
			Console.WriteLine(file.Json());
			if (file.IsRootDir) rc++;
		}
		Console.WriteLine("RootDirCount:" + rc);
		return 0;
	}
}




