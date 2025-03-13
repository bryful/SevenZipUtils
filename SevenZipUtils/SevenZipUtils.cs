using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FsSevenZip
{
    public class SevenZipUtils
    {
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		static extern int MessageBoxW(nint hWnd, [MarshalAs(UnmanagedType.LPWStr)] string lpText, [MarshalAs(UnmanagedType.LPWStr)] string lpCaption, uint uType);


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
			DirectoryInfo di = new DirectoryInfo(tempExtractFolder);
			DirectoryInfo[] di2 = di.GetDirectories();
			FileInfo[] fi2 = di.GetFiles();
			if ((di2.Length == 1) && (fi2.Length == 0))
			{
				tempExtractFolder = di2[0].FullName;
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

			DirectoryInfo[] di2 = di.GetDirectories();
			FileInfo[] fi2 = di.GetFiles();

			string targetFolder = di.FullName;
			if ((di2.Length == 1) && (fi2.Length == 0))
			{
				targetFolder = di2[0].FullName;
			}
			targetFolder += "\\*";
			string zipFileName = di.FullName + ".zip";

			ProcessStartInfo extractProcess = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"a  \"{zipFileName}\" \"{targetFolder}\"",
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
		public static bool ArcToDir(string p)
		{
			bool ret = false;
			FileInfo fi = new FileInfo(p);
			string ext = fi.Extension.ToLower();
			if ((ext != ".rar")&&(ext != ".zip")) return ret;

			ArcInfo[] list = Listup(p);
			int rc = 0;
			int fc = 0;
			foreach (ArcInfo file in list)
			{
				if (file.IsRootDir) rc++;
				if(file.IsDir==false) fc++;
			}
			string? tmpDir = Path.Combine(Path.GetDirectoryName(fi.FullName), Path.GetFileNameWithoutExtension(fi.FullName)); ;
			
			if (((rc==1)&&(fc==0))||((rc==0)&&(fc>0)))
			{
				tmpDir = Path.GetDirectoryName(tmpDir);
			}
			//MessageBoxW((nint)0, tmpDir, "tmpDir", 0);
			/*
			if(Directory.Exists(tmpDir))
			{
				Directory.Delete(tmpDir, true);
			}
			*/
			ProcessStartInfo extractProcess = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"x \"{p}\" -o\"{tmpDir}\" -y",
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

		private bool IsItemLine(string s)
		{
			bool ret = false;
			s = s.Trim();
			if (s.Length<=0x12) return ret;
			try
			{
				DateTime dt = DateTime.Parse(s.Substring(0, 0x12));
			}
			catch {
				return ret;
			}
			ret = true;
			//0123456789ABCDEF012
			//2024-05-18 09:35:57


			return ret;

		}
		public static ArcInfo[] Listup(string archiveFile)
		{
			FileInfo fi = new FileInfo(archiveFile);	
			if (fi.Exists==false)
			{
				return new ArcInfo[0];
			}
			List<ArcInfo> fileList = new List<ArcInfo>();
			ProcessStartInfo processInfo = new ProcessStartInfo
			{
				FileName = sevenZipPath,
				Arguments = $"l \"{archiveFile}\"",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			using (Process? process = Process.Start(processInfo))
			{
				if (process != null)
				{
					while (!process.StandardOutput.EndOfStream)
					{
						string? line = process.StandardOutput.ReadLine();
						if (line != null)
						{
							ArcInfo ai = new ArcInfo(line);
							if(ai.Enabled)
							{
								fileList.Add(ai);
							}
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

	public class ArcInfo
	{
		private bool _enabled = false;
		private DateTime _dt = new DateTime();
		private string _Attr = ".....";
		private long _Size = 0;
		private long _CompressedSize = 0;
		private string _Name = "";
		private bool _IsDir = false;
		private bool _IsRootDir = false;
		public string Name { get { return _Name; } }
		public long Size { get { return _Size; } }
		public long CompressedSize { get { return _CompressedSize; } }
		public bool IsDir { get { return _IsDir; } }
		public bool IsRootDir { get { return _IsRootDir; } }
		public string Attr { get { return _Attr; } }
		public bool Enabled { get { return _enabled; } }
		
		public void Clear()
		{
			_enabled = false;
			_dt = new DateTime();
			_Attr = ".....";
			_Size = 0;
			_CompressedSize = 0;
			_Name = "";
			_IsDir = false;
			_IsRootDir = false;
		}
		public ArcInfo() 
		{
		}
		public ArcInfo(string line)
		{
			Parse(line);
		}
		public bool Parse(string s)
		{
			//   Date      Time    Attr         Size   Compressed  Name
			//------------------------------------------------------------------------
			//2024-05-18 09:35:57 D...A            0            0  多重クミ
			//000000000011111111112222222222333333333344444444445555
			//012345678901234567890123456789012345678901234567890123
			Clear();
			bool ret = false;
			if (s.Length < 53) return ret;
			try
			{
				_dt = DateTime.Parse(s.Substring(0, 19));
				_Attr = s.Substring(20, 5);
				if ((_Attr[0] != 'D') && (_Attr[0] != '.'))
				{
					Clear();
					return ret;
				}
				if ((_Attr[4] != 'A') && (_Attr[0] != '.'))
				{
					Clear();
					return ret;
				}

				_Size = long.Parse(s.Substring(25, 13).Trim());
				_CompressedSize = long.Parse(s.Substring(38, 13).Trim());
				_Name = s.Substring(53);
				_IsDir = (_Attr[0] == 'D');
				_IsRootDir = (_Name.IndexOf('\\') < 0);
				ret = true;
				_enabled = true;
			}
			catch
			{
				ret = false;
				Clear();
			}
			return ret;
		}
		public string Json()
		{
			string ret = "";
			ret += $"{{";
			ret += $"\"Name\":\"{_Name}\",";
			ret += $"\"DateTime\":\"{_dt.ToString()}\",";
			ret += $"\"Attr\":\"{_Attr}\",";
			ret += $"\"Size\":\"{_Size}\",";
			ret += $"\"CompressedSize\":\"{_CompressedSize}\"";
			ret += $"}}";

			return ret;
		}
	}
}
