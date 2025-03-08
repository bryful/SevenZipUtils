// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System;
using r2z;

internal partial class Program
{
	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	static extern int MessageBoxW(nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpText, [MarshalAs(UnmanagedType.LPWStr)] string lpCaption, uint uType);


	private static int Main(string[] args)
	{
		int ret = 1;
		if (SevenZipUtils.Enabled == false)
		{
			Console.WriteLine("7-Zip not found.");
			return ret;
		}
		if (args.Length <= 0)
		{
			Console.WriteLine("no prams");
			return ret;
		}

		if (Directory.Exists(args[0]) == true)
		{
			DirectoryInfo di = new DirectoryInfo(args[0]);
			FileInfo[] fis = di.GetFiles();
			foreach (FileInfo fi in fis)
			{
				if (SevenZipUtils.RarToZip(fi.FullName)) ret = 0;
			}
		}
		else if (File.Exists(args[0]) == true)
		{
			if (SevenZipUtils.RarToZip(args[0])) ret = 0;
		}
		else
		{
			Console.WriteLine("not found");
		}
		return ret;
	}
}