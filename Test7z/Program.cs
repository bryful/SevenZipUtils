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
		string[] list = SevenZipUtils.Listup(args[0]);

		foreach (string file in list)
		{
			Console.WriteLine(file);
		}

		return 0;
	}
}




