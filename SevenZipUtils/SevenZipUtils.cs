using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace r2z
{
    public class SevenZipUtils
    {
		private static string _7zPath = @"C:\\Program Files\\7-Zip\\7z.exe";
		public static bool Enabled 
		{
			get { return File.Exists(_7zPath); }
		}
		public static bool RarToZip(string p)
		{
			bool ret = false;
			FileInfo fi = new FileInfo(p);
			string ext = fi.Extension.ToLower();
			if (ext != ".rar") return ret;

			string tempExtractFolder = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(p));
			Directory.CreateDirectory(tempExtractFolder);


			ProcessStartInfo extractProcess = new ProcessStartInfo
			{
				FileName = _7zPath,
				Arguments = $"x \"{p}\" -o\"{tempExtractFolder}\" -y",
				//RedirectStandardOutput = true,
				UseShellExecute = false,
				//CreateNoWindow = true
			};

			using (Process? process = Process.Start(extractProcess))
			{
				if (process == null) return ret;
				process.WaitForExit();
			}

			string outputZipFile = fi.FullName.Replace(fi.Extension, ".zip");
			ProcessStartInfo compressProcess = new ProcessStartInfo
			{
				FileName = _7zPath,
				Arguments = $"a -tzip \"{outputZipFile}\" \"{tempExtractFolder}\\*\"",
				//RedirectStandardOutput = true,
				UseShellExecute = false,
				//CreateNoWindow = true
			};
			using (Process? process = Process.Start(compressProcess))
			{
				if (process != null)
				{
					process.WaitForExit();
				}
			}
			Directory.Delete(tempExtractFolder, true);
			ret = true;
			return ret;

		}
		public static bool DirToZip(string p)
		{
			bool ret = false;
			DirectoryInfo di = new DirectoryInfo(p);
			if(di.Exists == false) return ret;


			string zipFileName = di.FullName + ".zip";

			ProcessStartInfo extractProcess = new ProcessStartInfo
			{
				FileName = _7zPath,
				Arguments = $"a  \"{zipFileName}\" \"{di.FullName}\\\"",
				//RedirectStandardOutput = true,
				UseShellExecute = false,
				//CreateNoWindow = true
			};

			using (Process? process = Process.Start(extractProcess))
			{
				if (process == null) return ret;
				process.WaitForExit();
			}
			ret = true;
			return ret;

		}
		public SevenZipUtils(string path="")
		{
			if (path != "")
			{
				_7zPath = path;
			}

		}

	}
}
