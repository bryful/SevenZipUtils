using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FsSevenZip
{
    public class SevenZipUtils
    {
		private static string sevenZipPath = @"C:\\Program Files\\7-Zip\\7z.exe";
		public static bool Enabled 
		{
			get { return File.Exists(sevenZipPath); }
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
				FileName = sevenZipPath,
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
				FileName = sevenZipPath,
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
				FileName = sevenZipPath,
				Arguments = $"a  \"{zipFileName}\" \"{di.FullName}\\*\"",
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
		public static bool Init(string p)
		{
			bool ret = false;
			sevenZipPath = p;
			if (File.Exists(sevenZipPath) == false)
			{
				sevenZipPath = @"7z.exe";
				if (File.Exists(sevenZipPath) == false)
				{
					sevenZipPath = @"C:\\Program Files\\7-Zip\\7z.exe";

				}
				if (File.Exists(sevenZipPath) == false)
				{
					sevenZipPath = @"";

				}
			}
			ret= File.Exists(sevenZipPath);
			return ret;
		}
		public SevenZipUtils(string path="")
		{
			Init(path);
		}


		public static string[] Listup(string archiveFile)
		{
			FileInfo fi = new FileInfo(archiveFile);	
			if (fi.Exists==false)
			{
				return new string[0];
			}
			List<string> fileList = new List<string>();
			ProcessStartInfo processInfo = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"l \"{archiveFile}\"",
				//RedirectStandardOutput = true,
				UseShellExecute = false,
				//CreateNoWindow = true
			};
			using (Process? process = Process.Start(processInfo))
			{
				if (process != null)
				{
					while (!process.StandardOutput.EndOfStream)
					{
						string? line = process.StandardOutput.ReadLine();
						if (!string.IsNullOrWhiteSpace(line) && line.Contains(".") && !line.StartsWith("----"))
						{
							fileList.Add(line.Trim());
						}
					}
					process.WaitForExit();
				}
			}

			return fileList.ToArray();
		}
		public static bool ExtractZip(string zipFilePath, string outputFolder)
		{
			bool ret = false;
			if (!File.Exists(zipFilePath))
			{
				return ret;
			}

			Directory.CreateDirectory(outputFolder);
			ProcessStartInfo processInfo = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"x \"{zipFilePath}\" -o\"{outputFolder}\" -y",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process? process = Process.Start(processInfo))
			{
				if(process != null)
				{
					process.WaitForExit();
					ret = true;
				}	
			}
			return ret;
		}
		public static bool CompressZipFromFolder(string outputZipFile, string targetFolder)
		{
			bool ret = false;
			if (!Directory.Exists(targetFolder))
			{
				return ret;
			}

			ProcessStartInfo processInfo = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"a -tzip \"{outputZipFile}\" \"{targetFolder}\\*\"",
				//RedirectStandardOutput = true,
				UseShellExecute = false,
				//CreateNoWindow = true
			};

			using (Process? process = Process.Start(processInfo))
			{
				if (process != null)
				{
					process.WaitForExit();
					ret = true;
				}
			}
			return ret;
		}

	}
}
