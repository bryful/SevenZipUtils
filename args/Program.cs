// See https://aka.ms/new-console-template for more information
using System.Runtime.InteropServices;
using System;

internal partial class Program
{
	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	static extern int MessageBoxW(nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpText, [MarshalAs(UnmanagedType.LPWStr)] string lpCaption, uint uType);


	private static int Main(string[] args)
	{
		int ret = 1;
		if (args.Length <= 0)
		{
			Console.WriteLine("no prams");
			ret = 1;
		}
		else
		{
			foreach (string arg in args)
			{
				Console.WriteLine(arg);
			}
			ret = 0;
		}
		Console.ReadKey();
		return ret;
	}
}