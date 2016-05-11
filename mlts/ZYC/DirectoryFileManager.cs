using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
namespace ZYC
{
	public class DirectoryFileManager
	{
		private static string[] directories;
		private static string[] files;
		private static int count = 0;
		private static ArrayList dfcount;
		private static ArrayList list;
		private static Thread dfthread;
		public static int Count
		{
			get
			{
				return DirectoryFileManager.count;
			}
		}
		public static ThreadState DFThreadState
		{
			get
			{
				return DirectoryFileManager.dfthread.ThreadState;
			}
		}
		public static string[] Directories
		{
			get
			{
				DirectoryFileManager.directories = new string[DirectoryFileManager.dfcount.Count];
				DirectoryFileManager.ArrayTransverse(DirectoryFileManager.dfcount, DirectoryFileManager.directories);
				return DirectoryFileManager.directories;
			}
		}
		public static string[] Files
		{
			get
			{
				DirectoryFileManager.files = new string[DirectoryFileManager.dfcount.Count];
				DirectoryFileManager.ArrayTransverse(DirectoryFileManager.dfcount, DirectoryFileManager.files);
				return DirectoryFileManager.files;
			}
		}
		private static bool DirectoryExist(string path)
		{
			return Directory.Exists(path);
		}
		private static void ArrayTransverse(ArrayList al, string[] ar)
		{
			Array array = al.ToArray();
			array.CopyTo(ar, 0);
		}
		private static string ArrangeExtensionName(string ExtensionName)
		{
			Regex regex = new Regex("^\\.[a-zA-Z]{1,5}$", RegexOptions.IgnoreCase);
			string result;
			if (ExtensionName.StartsWith("."))
			{
				result = ExtensionName.ToLower();
			}
			else
			{
				result = "." + ExtensionName.ToLower();
			}
			return result;
		}
		public static int GetCurrentDirectoryAmount(string path)
		{
			int result;
			if (DirectoryFileManager.DirectoryExist(path))
			{
				result = Directory.GetDirectories(path).Length;
			}
			else
			{
				result = -1;
			}
			return result;
		}
		public static string[] GetCurrentDirectories(string Dir)
		{
			DirectoryFileManager.list = new ArrayList();
			string[] result;
			if (DirectoryFileManager.DirectoryExist(Dir))
			{
				string[] array = Directory.GetDirectories(Dir);
				for (int i = 0; i < array.Length; i++)
				{
					string value = array[i];
					DirectoryFileManager.list.Add(value);
				}
				DirectoryFileManager.directories = new string[DirectoryFileManager.list.Count];
				DirectoryFileManager.ArrayTransverse(DirectoryFileManager.list, DirectoryFileManager.directories);
				result = DirectoryFileManager.directories;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static int GetDirectoryAmount(string path)
		{
			DirectoryFileManager.dfthread = new Thread(new ParameterizedThreadStart(DirectoryFileManager.GetAllDirectory));
			DirectoryFileManager.dfthread.Priority = ThreadPriority.Lowest;
			DirectoryFileManager.count = Directory.GetDirectories(path).Length;
			DirectoryFileManager.dfcount = new ArrayList();
			DirectoryFileManager.dfthread.Start(path);
			return 0;
		}
		public static int GetAllDirectories(string path)
		{
			DirectoryFileManager.dfthread = new Thread(new ParameterizedThreadStart(DirectoryFileManager.GetAllDirectory));
			DirectoryFileManager.dfthread.Priority = ThreadPriority.Lowest;
			DirectoryFileManager.count = Directory.GetDirectories(path).Length;
			DirectoryFileManager.dfcount = new ArrayList();
			string[] array = Directory.GetDirectories(path);
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				DirectoryFileManager.dfcount.Add(value);
			}
			DirectoryFileManager.dfthread.Start(path);
			return 0;
		}
		private static void GetAllDirectory(object path)
		{
			string path2 = path.ToString();
			if (Directory.Exists(path2))
			{
				string[] array = Directory.GetDirectories(path2);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					DirectoryFileManager.dfcount.Add(text);
					path2 = text;
					if (WindowsAPI.GetFileAttributes(text) != 22)
					{
						DirectoryFileManager.count += Directory.GetDirectories(text).Length;
						DirectoryFileManager.GetAllDirectory(path2);
					}
				}
			}
		}
		public static int GetCurrentFileAmount(string path)
		{
			int result;
			if (DirectoryFileManager.DirectoryExist(path))
			{
				result = Directory.GetFiles(path).Length;
			}
			else
			{
				result = 0;
			}
			return result;
		}
		public static string[] GetCurrentAllFiles(string Dir)
		{
			DirectoryFileManager.list = new ArrayList();
			string[] result;
			if (DirectoryFileManager.DirectoryExist(Dir))
			{
				string[] array = Directory.GetFiles(Dir);
				for (int i = 0; i < array.Length; i++)
				{
					string value = array[i];
					DirectoryFileManager.list.Add(value);
				}
				DirectoryFileManager.files = new string[DirectoryFileManager.list.Count];
				DirectoryFileManager.ArrayTransverse(DirectoryFileManager.list, DirectoryFileManager.files);
				result = DirectoryFileManager.files;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static string[] GetCurrentAllFiles(string Dir, string ExtensionName)
		{
			DirectoryFileManager.list = new ArrayList();
			string[] result;
			if (DirectoryFileManager.DirectoryExist(Dir))
			{
				string[] array = Directory.GetFiles(Dir);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (text.EndsWith(ExtensionName))
					{
						DirectoryFileManager.list.Add(text);
					}
				}
				DirectoryFileManager.files = new string[DirectoryFileManager.list.Count];
				DirectoryFileManager.ArrayTransverse(DirectoryFileManager.list, DirectoryFileManager.files);
				result = DirectoryFileManager.files;
			}
			else
			{
				result = null;
			}
			return result;
		}
		public static int GetFileAmount(string path)
		{
			DirectoryFileManager.dfthread = new Thread(new ParameterizedThreadStart(DirectoryFileManager.GetAllFile));
			DirectoryFileManager.dfthread.Priority = ThreadPriority.Lowest;
			DirectoryFileManager.count = 0;
			DirectoryFileManager.dfcount = new ArrayList();
			DirectoryFileManager.dfthread.Start(path);
			return 0;
		}
		public static int GetAllFiles(string path)
		{
			DirectoryFileManager.dfthread = new Thread(new ParameterizedThreadStart(DirectoryFileManager.GetAllFile));
			DirectoryFileManager.dfthread.Priority = ThreadPriority.Lowest;
			DirectoryFileManager.count = 0;
			DirectoryFileManager.dfcount = new ArrayList();
			DirectoryFileManager.dfthread.Start(path);
			return 0;
		}
		private static void GetAllFile(object path)
		{
			string path2 = path.ToString();
			if (Directory.Exists(path2))
			{
				string[] array = Directory.GetFiles(path2);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					DirectoryFileManager.dfcount.Add(text);
					DirectoryFileManager.count++;
				}
				array = Directory.GetDirectories(path2);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					path2 = text;
					DirectoryInfo directoryInfo = new DirectoryInfo(path2);
					if (WindowsAPI.GetFileAttributes(text) != 22)
					{
						DirectoryFileManager.GetAllFile(path2);
					}
				}
			}
		}
	}
}
