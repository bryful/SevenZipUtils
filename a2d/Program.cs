// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System;
using FsSevenZip;

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
		foreach (string arg in args)
		{
			SevenZipUtils.ArcToDir(arg);
		}
		ret = 0;
		return ret;
	}
}