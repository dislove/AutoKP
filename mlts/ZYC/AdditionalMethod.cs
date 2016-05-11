using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace ZYC
{
	public class AdditionalMethod
	{
		public class FileTypeRegInfo
		{
			public string ExtendName;
			public string Description;
			public string IcoPath;
			public string ExePath;
			public FileTypeRegInfo()
			{
			}
			public FileTypeRegInfo(string extendName)
			{
				this.ExtendName = extendName;
			}
		}
		public class FileTypeRegister
		{
			public static void RegisterFileType(AdditionalMethod.FileTypeRegInfo regInfo)
			{
				string text = regInfo.ExtendName.Substring(1, regInfo.ExtendName.Length - 1).ToUpper() + "_FileType";
				RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(regInfo.ExtendName);
				registryKey.SetValue("", text);
				registryKey.Close();
				RegistryKey registryKey2 = Registry.ClassesRoot.CreateSubKey(text);
				registryKey2.SetValue("", regInfo.Description);
				RegistryKey registryKey3 = registryKey2.CreateSubKey("DefaultIcon");
				registryKey3.SetValue("", regInfo.IcoPath);
				RegistryKey registryKey4 = registryKey2.CreateSubKey("Shell");
				RegistryKey registryKey5 = registryKey4.CreateSubKey("Open");
				RegistryKey registryKey6 = registryKey5.CreateSubKey("Command");
				registryKey6.SetValue("", regInfo.ExePath + " %1");
				registryKey2.Close();
			}
			public static AdditionalMethod.FileTypeRegInfo GetFileTypeRegInfo(string extendName)
			{
				AdditionalMethod.FileTypeRegInfo result;
				if (!AdditionalMethod.FileTypeRegister.FileTypeRegistered(extendName))
				{
					result = null;
				}
				else
				{
					AdditionalMethod.FileTypeRegInfo fileTypeRegInfo = new AdditionalMethod.FileTypeRegInfo(extendName);
					string name = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
					RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name);
					fileTypeRegInfo.Description = registryKey.GetValue("").ToString();
					RegistryKey registryKey2 = registryKey.OpenSubKey("DefaultIcon");
					fileTypeRegInfo.IcoPath = registryKey2.GetValue("").ToString();
					RegistryKey registryKey3 = registryKey.OpenSubKey("Shell");
					RegistryKey registryKey4 = registryKey3.OpenSubKey("Open");
					RegistryKey registryKey5 = registryKey4.OpenSubKey("Command");
					string text = registryKey5.GetValue("").ToString();
					fileTypeRegInfo.ExePath = text.Substring(0, text.Length - 3);
					result = fileTypeRegInfo;
				}
				return result;
			}
			public static bool UpdateFileTypeRegInfo(AdditionalMethod.FileTypeRegInfo regInfo)
			{
				bool result;
				if (!AdditionalMethod.FileTypeRegister.FileTypeRegistered(regInfo.ExtendName))
				{
					result = false;
				}
				else
				{
					string extendName = regInfo.ExtendName;
					string name = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
					RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name, true);
					registryKey.SetValue("", regInfo.Description);
					RegistryKey registryKey2 = registryKey.OpenSubKey("DefaultIcon", true);
					registryKey2.SetValue("", regInfo.IcoPath);
					RegistryKey registryKey3 = registryKey.OpenSubKey("Shell");
					RegistryKey registryKey4 = registryKey3.OpenSubKey("Open");
					RegistryKey registryKey5 = registryKey4.OpenSubKey("Command", true);
					registryKey5.SetValue("", regInfo.ExePath + " %1");
					registryKey.Close();
					result = true;
				}
				return result;
			}
			public static bool FileTypeRegistered(string extendName)
			{
				RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extendName);
				return registryKey != null;
			}
		}
		public class MediaFileInfo
		{
			public struct AudioInfo
			{
				public string identify;
				public string Title;
				public string Artist;
				public string Album;
				public string Year;
				public string Comment;
				public char reserved1;
				public char reserved2;
				public char reserved3;
			}
			private AdditionalMethod.MediaFileInfo.AudioInfo info;
			public MediaFileInfo(string mp3FilePos)
			{
				this.info = this.GetMediaInfo(this.GetLast128(mp3FilePos));
			}
			public string GetOriginalName()
			{
				return this.FormatString(this.info.Title.Trim()) + "-" + this.FormatString(this.info.Artist.Trim());
			}
			public AdditionalMethod.MediaFileInfo.AudioInfo GetInfo()
			{
				return this.info;
			}
			private string FormatString(string str)
			{
				return str.Replace("\0", "");
			}
			private byte[] GetLast128(string FileName)
			{
				FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				Stream stream = fileStream;
				stream.Seek(-128L, SeekOrigin.End);
				byte[] array = new byte[128];
				int num = stream.Read(array, 0, 128);
				fileStream.Close();
				stream.Close();
				return array;
			}
			public AdditionalMethod.MediaFileInfo.AudioInfo GetMediaInfo(byte[] Info)
			{
				AdditionalMethod.MediaFileInfo.AudioInfo result = default(AdditionalMethod.MediaFileInfo.AudioInfo);
				string text = null;
				int num = 0;
				int num2 = 0;
				for (int i = num2; i < num2 + 3; i++)
				{
					text += (char)Info[i];
					num++;
				}
				num2 = num;
				result.identify = text;
				byte[] array = new byte[30];
				int num3 = 0;
				for (int i = num2; i < num2 + 30; i++)
				{
					array[num3] = Info[i];
					num++;
					num3++;
				}
				num2 = num;
				result.Title = this.ByteToString(array);
				num3 = 0;
				byte[] array2 = new byte[30];
				for (int i = num2; i < num2 + 30; i++)
				{
					array2[num3] = Info[i];
					num++;
					num3++;
				}
				num2 = num;
				result.Artist = this.ByteToString(array2);
				num3 = 0;
				byte[] array3 = new byte[30];
				for (int i = num2; i < num2 + 30; i++)
				{
					array3[num3] = Info[i];
					num++;
					num3++;
				}
				num2 = num;
				result.Album = this.ByteToString(array3);
				num3 = 0;
				byte[] array4 = new byte[4];
				for (int i = num2; i < num2 + 4; i++)
				{
					array4[num3] = Info[i];
					num++;
					num3++;
				}
				num2 = num;
				result.Year = this.ByteToString(array4);
				num3 = 0;
				byte[] array5 = new byte[28];
				for (int i = num2; i < num2 + 25; i++)
				{
					array5[num3] = Info[i];
					num++;
					num3++;
				}
				result.Comment = this.ByteToString(array5);
				result.reserved1 = (char)Info[++num];
				result.reserved2 = (char)Info[++num];
				result.reserved3 = (char)Info[num + 1];
				return result;
			}
			private string ByteToString(byte[] b)
			{
				Encoding encoding = Encoding.GetEncoding("GB2312");
				string text = encoding.GetString(b);
				text = text.Substring(0, (text.IndexOf("#CONTENT#") >= 0) ? text.IndexOf("#CONTENT#") : text.Length);
				return text.Replace("\0", "");
			}
		}
		private class CalUtility
		{
			private StringBuilder StrB;
			private int iCurr = 0;
			private int iCount = 0;
			public CalUtility(string calStr)
			{
				this.StrB = new StringBuilder(calStr.Trim());
				this.iCount = Encoding.Default.GetByteCount(calStr.Trim());
			}
			public string getItem()
			{
				string result;
				if (this.iCurr == this.iCount)
				{
					result = "";
				}
				else
				{
					char c = this.StrB[this.iCurr];
					bool flag = this.IsNum(c);
					if (!flag)
					{
						this.iCurr++;
						result = c.ToString();
					}
					else
					{
						string text = "";
						while (this.IsNum(c) == flag && this.iCurr < this.iCount)
						{
							c = this.StrB[this.iCurr];
							if (this.IsNum(c) != flag)
							{
								break;
							}
							text += c;
							this.iCurr++;
						}
						result = text;
					}
				}
				return result;
			}
			public bool IsNum(char c)
			{
				return (c >= '0' && c <= '9') || c == '.';
			}
			public bool IsNum(string c)
			{
				return !c.Equals("") && ((c[0] >= '0' && c[0] <= '9') || c[0] == '.');
			}
			public bool Compare(string str1, string str2)
			{
				return this.getPriority(str1) >= this.getPriority(str2);
			}
			public int getPriority(string str)
			{
				int result;
				if (str.Equals(""))
				{
					result = -1;
				}
				else
				{
					if (str.Equals("("))
					{
						result = 0;
					}
					else
					{
						if (str.Equals("+") || str.Equals("-"))
						{
							result = 1;
						}
						else
						{
							if (str.Equals("*") || str.Equals("/"))
							{
								result = 2;
							}
							else
							{
								if (str.Equals(")"))
								{
									result = 0;
								}
								else
								{
									result = 0;
								}
							}
						}
					}
				}
				return result;
			}
		}
		private interface IOper
		{
			object Oper(object o1, object o2);
		}
		private class OperAdd : AdditionalMethod.IOper
		{
			public class OperDec : AdditionalMethod.IOper
			{
				public object Oper(object o1, object o2)
				{
					decimal d = decimal.Parse(o1.ToString());
					decimal d2 = decimal.Parse(o2.ToString());
					return d - d2;
				}
			}
			public class OperRide : AdditionalMethod.IOper
			{
				public object Oper(object o1, object o2)
				{
					decimal d = decimal.Parse(o1.ToString());
					decimal d2 = decimal.Parse(o2.ToString());
					return d * d2;
				}
			}
			public class OperDiv : AdditionalMethod.IOper
			{
				public object Oper(object o1, object o2)
				{
					decimal d = decimal.Parse(o1.ToString());
					decimal d2 = decimal.Parse(o2.ToString());
					return d / d2;
				}
			}
			public object Oper(object o1, object o2)
			{
				decimal d = decimal.Parse(o1.ToString());
				decimal d2 = decimal.Parse(o2.ToString());
				return d + d2;
			}
		}
		private class OperFactory
		{
			public AdditionalMethod.IOper CreateOper(string Oper)
			{
				AdditionalMethod.IOper result;
				if (Oper.Equals("+"))
				{
					AdditionalMethod.IOper oper = new AdditionalMethod.OperAdd();
					result = oper;
				}
				else
				{
					if (Oper.Equals("-"))
					{
						AdditionalMethod.IOper oper = new AdditionalMethod.OperAdd.OperDec();
						result = oper;
					}
					else
					{
						if (Oper.Equals("*"))
						{
							AdditionalMethod.IOper oper = new AdditionalMethod.OperAdd.OperRide();
							result = oper;
						}
						else
						{
							if (Oper.Equals("/"))
							{
								AdditionalMethod.IOper oper = new AdditionalMethod.OperAdd.OperDiv();
								result = oper;
							}
							else
							{
								result = null;
							}
						}
					}
				}
				return result;
			}
		}
		public class Calculate
		{
			private ArrayList HList;
			public ArrayList Vlist;
			private AdditionalMethod.CalUtility cu;
			private AdditionalMethod.OperFactory of;
			public Calculate()
			{
			}
			public Calculate(string str)
			{
				this.HList = new ArrayList();
				this.Vlist = new ArrayList();
				this.of = new AdditionalMethod.OperFactory();
				this.cu = new AdditionalMethod.CalUtility(str);
			}
			public object Compute()
			{
				string item = this.cu.getItem();
				while (true)
				{
					if (this.cu.IsNum(item))
					{
						this.Vlist.Add(item);
					}
					else
					{
						this.Cal(item);
					}
					if (item.Equals(""))
					{
						break;
					}
					item = this.cu.getItem();
				}
				return this.Vlist[0];
			}
			public object Compute(string str)
			{
				this.HList = new ArrayList();
				this.Vlist = new ArrayList();
				this.of = new AdditionalMethod.OperFactory();
				this.cu = new AdditionalMethod.CalUtility(str);
				string item = this.cu.getItem();
				while (true)
				{
					if (this.cu.IsNum(item))
					{
						this.Vlist.Add(item);
					}
					else
					{
						this.Cal(item);
					}
					if (item.Equals(""))
					{
						break;
					}
					item = this.cu.getItem();
				}
				return this.Vlist[0];
			}
			private void Cal(string str)
			{
				if (!str.Equals("") || this.HList.Count != 0)
				{
					if (this.HList.Count > 0)
					{
						if (this.HList[this.HList.Count - 1].ToString().Equals("(") && str.Equals(")"))
						{
							this.HList.RemoveAt(this.HList.Count - 1);
							if (this.HList.Count > 0)
							{
								str = this.HList[this.HList.Count - 1].ToString();
								this.HList.RemoveAt(this.HList.Count - 1);
								this.Cal(str);
							}
						}
						else
						{
							if (this.cu.Compare(this.HList[this.HList.Count - 1].ToString(), str))
							{
								AdditionalMethod.IOper oper = this.of.CreateOper(this.HList[this.HList.Count - 1].ToString());
								if (oper != null)
								{
									this.Vlist[this.Vlist.Count - 2] = oper.Oper(this.Vlist[this.Vlist.Count - 2], this.Vlist[this.Vlist.Count - 1]);
									this.HList.RemoveAt(this.HList.Count - 1);
									this.Vlist.RemoveAt(this.Vlist.Count - 1);
									this.Cal(str);
								}
							}
							else
							{
								if (!str.Equals(""))
								{
									this.HList.Add(str);
								}
							}
						}
					}
					else
					{
						if (!str.Equals(""))
						{
							this.HList.Add(str);
						}
					}
				}
			}
		}
		public class Encryption
		{
			public static string GenerateKey()
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = (DESCryptoServiceProvider)DES.Create();
				return Encoding.ASCII.GetString(dESCryptoServiceProvider.Key);
			}
			public static void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
			{
				FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
				FileStream fileStream2 = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);
				ICryptoTransform transform = new DESCryptoServiceProvider
				{
					Key = Encoding.ASCII.GetBytes(sKey),
					IV = Encoding.ASCII.GetBytes(sKey)
				}.CreateEncryptor();
				CryptoStream cryptoStream = new CryptoStream(fileStream2, transform, CryptoStreamMode.Write);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.Close();
				fileStream.Close();
				fileStream2.Close();
			}
			public static void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
			{
				DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
				dESCryptoServiceProvider.Key = Encoding.ASCII.GetBytes(sKey);
				dESCryptoServiceProvider.IV = Encoding.ASCII.GetBytes(sKey);
				FileStream stream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
				ICryptoTransform transform = dESCryptoServiceProvider.CreateDecryptor();
				CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				StreamWriter streamWriter = new StreamWriter(sOutputFilename);
				streamWriter.Write(new StreamReader(stream2).ReadToEnd());
				streamWriter.Flush();
				streamWriter.Close();
			}
		}
		[ComVisible(false), Guid("00000001-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		internal interface IClassFactory
		{
			void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid refiid, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
			void LockServer(bool fLock);
		}
		internal static class ComHelper
		{
			private delegate int DllGetClassObject(ref Guid ClassId, ref Guid InterfaceId, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
			private class Win32NativeMethods
			{
				[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
				public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
				[DllImport("kernel32.dll")]
				public static extern bool FreeLibrary(IntPtr hModule);
				[DllImport("kernel32.dll")]
				public static extern IntPtr LoadLibrary(string lpFileName);
			}
			private class DllList
			{
				private List<IntPtr> _dllList = new List<IntPtr>();
				public void AddDllHandle(IntPtr dllHandle)
				{
					List<IntPtr> dllList;
					Monitor.Enter(dllList = this._dllList);
					try
					{
						this._dllList.Add(dllHandle);
					}
					finally
					{
						Monitor.Exit(dllList);
					}
				}
            //    protected override void Finalize()
            //    {
            //        try
            //        {
            //            foreach (IntPtr current in this._dllList)
            //            {
            //                try
            //                {
            //                    AdditionalMethod.ComHelper.Win32NativeMethods.FreeLibrary(current);
            //                }
            //                catch
            //                {
            //                }
            //            }
            //        }
            //        finally
            //        {
            //            base.Finalize();
            //        }
            //    }
            //
            }
			private static AdditionalMethod.ComHelper.DllList _dllList = new AdditionalMethod.ComHelper.DllList();
			internal static AdditionalMethod.IClassFactory GetClassFactory(string dllName, string filterPersistClass)
			{
				return AdditionalMethod.ComHelper.GetClassFactoryFromDll(dllName, filterPersistClass);
			}
			private static AdditionalMethod.IClassFactory GetClassFactoryFromDll(string dllName, string filterPersistClass)
			{
				IntPtr intPtr = AdditionalMethod.ComHelper.Win32NativeMethods.LoadLibrary(dllName);
				AdditionalMethod.IClassFactory result;
				if (intPtr == IntPtr.Zero)
				{
					result = null;
				}
				else
				{
					AdditionalMethod.ComHelper._dllList.AddDllHandle(intPtr);
					IntPtr procAddress = AdditionalMethod.ComHelper.Win32NativeMethods.GetProcAddress(intPtr, "DllGetClassObject");
					if (procAddress == IntPtr.Zero)
					{
						result = null;
					}
					else
					{
						AdditionalMethod.ComHelper.DllGetClassObject dllGetClassObject = (AdditionalMethod.ComHelper.DllGetClassObject)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(AdditionalMethod.ComHelper.DllGetClassObject));
						Guid guid = new Guid(filterPersistClass);
						Guid guid2 = new Guid("00000001-0000-0000-C000-000000000046");
						object obj;
						if (dllGetClassObject(ref guid, ref guid2, out obj) != 0)
						{
							result = null;
						}
						else
						{
							result = (obj as AdditionalMethod.IClassFactory);
						}
					}
				}
				return result;
			}
		}
		public class FilterReader : TextReader
		{
			private AdditionalMethod.IFilter _filter;
			private bool _done;
			private AdditionalMethod.STAT_CHUNK _currentChunk;
			private bool _currentChunkValid;
			private char[] _charsLeftFromLastRead;
			public override void Close()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			~FilterReader()
			{
				this.Dispose(false);
			}
			protected override void Dispose(bool disposing)
			{
				if (this._filter != null)
				{
					Marshal.ReleaseComObject(this._filter);
				}
			}
			public override int Read(char[] array, int offset, int count)
			{
				int num = 0;
				int num2 = 0;
				while (!this._done && num2 < count)
				{
					if (this._charsLeftFromLastRead != null)
					{
						int num3 = (this._charsLeftFromLastRead.Length < count - num2) ? this._charsLeftFromLastRead.Length : (count - num2);
						Array.Copy(this._charsLeftFromLastRead, 0, array, offset + num2, num3);
						num2 += num3;
						if (num3 < this._charsLeftFromLastRead.Length)
						{
							char[] array2 = new char[this._charsLeftFromLastRead.Length - num3];
							Array.Copy(this._charsLeftFromLastRead, num3, array2, 0, array2.Length);
							this._charsLeftFromLastRead = array2;
						}
						else
						{
							this._charsLeftFromLastRead = null;
						}
					}
					else
					{
						if (!this._currentChunkValid)
						{
							AdditionalMethod.IFilterReturnCode filterReturnCode = this._filter.GetChunk(out this._currentChunk);
							this._currentChunkValid = (filterReturnCode == AdditionalMethod.IFilterReturnCode.S_OK && (this._currentChunk.flags & AdditionalMethod.CHUNKSTATE.CHUNK_TEXT) != (AdditionalMethod.CHUNKSTATE)0);
							if (filterReturnCode == (AdditionalMethod.IFilterReturnCode)2147751680u)
							{
								num++;
							}
							if (num > 1)
							{
								this._done = true;
							}
						}
						if (this._currentChunkValid)
						{
							uint num4 = (uint)(count - num2);
							if (num4 < 8192u)
							{
								num4 = 8192u;
							}
							char[] array3 = new char[num4];
							AdditionalMethod.IFilterReturnCode filterReturnCode = this._filter.GetText(ref num4, array3);
							if (filterReturnCode == AdditionalMethod.IFilterReturnCode.S_OK || filterReturnCode == AdditionalMethod.IFilterReturnCode.FILTER_S_LAST_TEXT)
							{
								int num5 = (int)num4;
								if (num5 + num2 > count)
								{
									int num6 = num5 + num2 - count;
									this._charsLeftFromLastRead = new char[num6];
									Array.Copy(array3, num5 - num6, this._charsLeftFromLastRead, 0, num6);
									num5 -= num6;
								}
								else
								{
									this._charsLeftFromLastRead = null;
								}
								Array.Copy(array3, 0, array, offset + num2, num5);
								num2 += num5;
							}
							if (filterReturnCode == AdditionalMethod.IFilterReturnCode.FILTER_S_LAST_TEXT || filterReturnCode == (AdditionalMethod.IFilterReturnCode)2147751681u)
							{
								this._currentChunkValid = false;
							}
						}
					}
				}
				return num2;
			}
			public FilterReader(string fileName)
			{
				this._filter = AdditionalMethod.FilterLoader.LoadAndInitIFilter(fileName);
				if (this._filter == null)
				{
					throw new ArgumentException("no filter defined for " + fileName);
				}
			}
		}
		private static class FilterLoader
		{
			private class CacheEntry
			{
				public string DllName;
				public string ClassName;
				public CacheEntry(string dllName, string className)
				{
					this.DllName = dllName;
					this.ClassName = className;
				}
			}
			private static Dictionary<string, AdditionalMethod.FilterLoader.CacheEntry> _cache = new Dictionary<string, AdditionalMethod.FilterLoader.CacheEntry>();
			private static string ReadStrFromHKLM(string key)
			{
				return AdditionalMethod.FilterLoader.ReadStrFromHKLM(key, null);
			}
			private static string ReadStrFromHKLM(string key, string value)
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key);
				string result;
				if (registryKey == null)
				{
					result = null;
				}
				else
				{
					using (registryKey)
					{
						result = (string)registryKey.GetValue(value);
					}
				}
				return result;
			}
			private static AdditionalMethod.IFilter LoadIFilter(string ext)
			{
				string dllName;
				string filterPersistClass;
				AdditionalMethod.IFilter result;
				if (AdditionalMethod.FilterLoader.GetFilterDllAndClass(ext, out dllName, out filterPersistClass))
				{
					result = AdditionalMethod.FilterLoader.LoadFilterFromDll(dllName, filterPersistClass);
				}
				else
				{
					result = null;
				}
				return result;
			}
			internal static AdditionalMethod.IFilter LoadAndInitIFilter(string fileName)
			{
				return AdditionalMethod.FilterLoader.LoadAndInitIFilter(fileName, Path.GetExtension(fileName));
			}
			internal static AdditionalMethod.IFilter LoadAndInitIFilter(string fileName, string extension)
			{
				AdditionalMethod.IFilter filter = AdditionalMethod.FilterLoader.LoadIFilter(extension);
				AdditionalMethod.IFilter result;
				if (filter == null)
				{
					result = null;
				}
				else
				{
					IPersistFile persistFile = filter as IPersistFile;
					if (persistFile != null)
					{
						persistFile.Load(fileName, 0);
						AdditionalMethod.IFILTER_INIT grfFlags = AdditionalMethod.IFILTER_INIT.CANON_PARAGRAPHS | AdditionalMethod.IFILTER_INIT.HARD_LINE_BREAKS | AdditionalMethod.IFILTER_INIT.CANON_HYPHENS | AdditionalMethod.IFILTER_INIT.CANON_SPACES | AdditionalMethod.IFILTER_INIT.APPLY_INDEX_ATTRIBUTES | AdditionalMethod.IFILTER_INIT.FILTER_OWNED_VALUE_OK;
						AdditionalMethod.IFILTER_FLAGS iFILTER_FLAGS;
						if (filter.Init(grfFlags, 0, IntPtr.Zero, out iFILTER_FLAGS) == AdditionalMethod.IFilterReturnCode.S_OK)
						{
							result = filter;
							return result;
						}
					}
					Marshal.ReleaseComObject(filter);
					result = null;
				}
				return result;
			}
			private static AdditionalMethod.IFilter LoadFilterFromDll(string dllName, string filterPersistClass)
			{
				AdditionalMethod.IClassFactory classFactory = AdditionalMethod.ComHelper.GetClassFactory(dllName, filterPersistClass);
				AdditionalMethod.IFilter result;
				if (classFactory == null)
				{
					result = null;
				}
				else
				{
					Guid guid = new Guid("89BCB740-6119-101A-BCB7-00DD010655AF");
					object obj;
					classFactory.CreateInstance(null, ref guid, out obj);
					result = (obj as AdditionalMethod.IFilter);
				}
				return result;
			}
			private static bool GetFilterDllAndClass(string ext, out string dllName, out string filterPersistClass)
			{
				if (!AdditionalMethod.FilterLoader.GetFilterDllAndClassFromCache(ext, out dllName, out filterPersistClass))
				{
					string persistentHandlerClass = AdditionalMethod.FilterLoader.GetPersistentHandlerClass(ext, true);
					if (persistentHandlerClass != null)
					{
						AdditionalMethod.FilterLoader.GetFilterDllAndClassFromPersistentHandler(persistentHandlerClass, out dllName, out filterPersistClass);
					}
					AdditionalMethod.FilterLoader.AddExtensionToCache(ext, dllName, filterPersistClass);
				}
				return dllName != null && filterPersistClass != null;
			}
			private static void AddExtensionToCache(string ext, string dllName, string filterPersistClass)
			{
				Dictionary<string, AdditionalMethod.FilterLoader.CacheEntry> cache;
				Monitor.Enter(cache = AdditionalMethod.FilterLoader._cache);
				try
				{
					AdditionalMethod.FilterLoader._cache.Add(ext.ToLower(), new AdditionalMethod.FilterLoader.CacheEntry(dllName, filterPersistClass));
				}
				finally
				{
					Monitor.Exit(cache);
				}
			}
			private static bool GetFilterDllAndClassFromPersistentHandler(string persistentHandlerClass, out string dllName, out string filterPersistClass)
			{
				dllName = null;
				filterPersistClass = null;
				filterPersistClass = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\CLSID\\" + persistentHandlerClass + "\\PersistentAddinsRegistered\\{89BCB740-6119-101A-BCB7-00DD010655AF}");
				bool result;
				if (string.IsNullOrEmpty(filterPersistClass))
				{
					result = false;
				}
				else
				{
					dllName = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\CLSID\\" + filterPersistClass + "\\InprocServer32");
					result = !string.IsNullOrEmpty(dllName);
				}
				return result;
			}
			private static string GetPersistentHandlerClass(string ext, bool searchContentType)
			{
				string text = AdditionalMethod.FilterLoader.GetPersistentHandlerClassFromExtension(ext);
				if (string.IsNullOrEmpty(text))
				{
					text = AdditionalMethod.FilterLoader.GetPersistentHandlerClassFromDocumentType(ext);
				}
				if (searchContentType && string.IsNullOrEmpty(text))
				{
					text = AdditionalMethod.FilterLoader.GetPersistentHandlerClassFromContentType(ext);
				}
				return text;
			}
			private static string GetPersistentHandlerClassFromContentType(string ext)
			{
				string text = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\" + ext, "Content Type");
				string result;
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					string text2 = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\MIME\\Database\\Content Type\\" + text, "Extension");
					if (ext.Equals(text2, StringComparison.CurrentCultureIgnoreCase))
					{
						result = null;
					}
					else
					{
						result = AdditionalMethod.FilterLoader.GetPersistentHandlerClass(text2, false);
					}
				}
				return result;
			}
			private static string GetPersistentHandlerClassFromDocumentType(string ext)
			{
				string text = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\" + ext);
				string result;
				if (string.IsNullOrEmpty(text))
				{
					result = null;
				}
				else
				{
					string str = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\" + text + "\\CLSID");
					if (string.IsNullOrEmpty(text))
					{
						result = null;
					}
					else
					{
						result = AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\CLSID\\" + str + "\\PersistentHandler");
					}
				}
				return result;
			}
			private static string GetPersistentHandlerClassFromExtension(string ext)
			{
				return AdditionalMethod.FilterLoader.ReadStrFromHKLM("Software\\Classes\\" + ext + "\\PersistentHandler");
			}
			private static bool GetFilterDllAndClassFromCache(string ext, out string dllName, out string filterPersistClass)
			{
				string key = ext.ToLower();
				Dictionary<string, AdditionalMethod.FilterLoader.CacheEntry> cache;
				Monitor.Enter(cache = AdditionalMethod.FilterLoader._cache);
				bool result;
				try
				{
					AdditionalMethod.FilterLoader.CacheEntry cacheEntry;
					if (AdditionalMethod.FilterLoader._cache.TryGetValue(key, out cacheEntry))
					{
						dllName = cacheEntry.DllName;
						filterPersistClass = cacheEntry.ClassName;
						result = true;
						return result;
					}
				}
				finally
				{
					Monitor.Exit(cache);
				}
				dllName = null;
				filterPersistClass = null;
				result = false;
				return result;
			}
		}
		public struct FULLPROPSPEC
		{
			public Guid guidPropSet;
			public AdditionalMethod.PROPSPEC psProperty;
		}
		internal struct FILTERREGION
		{
			public int idChunk;
			public int cwcStart;
			public int cwcExtent;
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct PROPSPEC
		{
			[FieldOffset(0)]
			public int ulKind;
			[FieldOffset(4)]
			public int propid;
			[FieldOffset(4)]
			public IntPtr lpwstr;
		}
		[Flags]
		internal enum IFILTER_FLAGS
		{
			IFILTER_FLAGS_OLE_PROPERTIES = 1
		}
		[Flags]
		internal enum IFILTER_INIT
		{
			NONE = 0,
			CANON_PARAGRAPHS = 1,
			HARD_LINE_BREAKS = 2,
			CANON_HYPHENS = 4,
			CANON_SPACES = 8,
			APPLY_INDEX_ATTRIBUTES = 16,
			APPLY_CRAWL_ATTRIBUTES = 256,
			APPLY_OTHER_ATTRIBUTES = 32,
			INDEXING_ONLY = 64,
			SEARCH_LINKS = 128,
			FILTER_OWNED_VALUE_OK = 512
		}
		public struct STAT_CHUNK
		{
			public int idChunk;
			[MarshalAs(UnmanagedType.U4)]
			public AdditionalMethod.CHUNK_BREAKTYPE breakType;
			[MarshalAs(UnmanagedType.U4)]
			public AdditionalMethod.CHUNKSTATE flags;
			public int locale;
			public AdditionalMethod.FULLPROPSPEC attribute;
			public int idChunkSource;
			public int cwcStartSource;
			public int cwcLenSource;
		}
		public enum CHUNK_BREAKTYPE
		{
			CHUNK_NO_BREAK,
			CHUNK_EOW,
			CHUNK_EOS,
			CHUNK_EOP,
			CHUNK_EOC
		}
		public enum CHUNKSTATE
		{
			CHUNK_TEXT = 1,
			CHUNK_VALUE,
			CHUNK_FILTER_OWNED_VALUE = 4
		}
		internal enum IFilterReturnCode : uint
		{
			S_OK,
			E_ACCESSDENIED = 2147942405u,
			E_HANDLE,
			E_INVALIDARG = 2147942487u,
			E_OUTOFMEMORY = 2147942414u,
			E_NOTIMPL = 2147500033u,
			E_FAIL = 2147483656u,
			FILTER_E_PASSWORD = 2147751691u,
			FILTER_E_UNKNOWNFORMAT,
			FILTER_E_NO_TEXT = 2147751685u,
			FILTER_E_END_OF_CHUNKS = 2147751680u,
			FILTER_E_NO_MORE_TEXT,
			FILTER_E_NO_MORE_VALUES,
			FILTER_E_ACCESS,
			FILTER_W_MONIKER_CLIPPED = 268036u,
			FILTER_E_EMBEDDING_UNAVAILABLE = 2147751687u,
			FILTER_E_LINK_UNAVAILABLE,
			FILTER_S_LAST_TEXT = 268041u,
			FILTER_S_LAST_VALUES
		}
		[Guid("89BCB740-6119-101A-BCB7-00DD010655AF"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		internal interface IFilter
		{
			[PreserveSig]
			AdditionalMethod.IFilterReturnCode Init(AdditionalMethod.IFILTER_INIT grfFlags, int cAttributes, IntPtr aAttributes, out AdditionalMethod.IFILTER_FLAGS pdwFlags);
			[PreserveSig]
			AdditionalMethod.IFilterReturnCode GetChunk(out AdditionalMethod.STAT_CHUNK pStat);
			[PreserveSig]
			AdditionalMethod.IFilterReturnCode GetText(ref uint pcwcBuffer, [MarshalAs(UnmanagedType.LPArray)] [Out] char[] awcBuffer);
			[PreserveSig]
			int GetValue(ref IntPtr PropVal);
			[PreserveSig]
			int BindRegion(ref AdditionalMethod.FILTERREGION origPos, ref Guid riid, ref object ppunk);
		}
		public class MathEx
		{
			public int CommonMultiple(int val1, int val2)
			{
				int num = this.CommonDivisor(val1, val2);
				return val1 / num * (val2 / num) * num;
			}
			public int CommonDivisor(int val1, int val2)
			{
				if (val1 < val2)
				{
					this.Swap(ref val1, ref val2);
				}
				for (int num = val1 % val2; num != 0; num = val1 % val2)
				{
					val1 = val2;
					val2 = num;
				}
				return val2;
			}
			public void Swap(ref int val1, ref int val2)
			{
				int num = val1;
				val1 = val2;
				val2 = num;
			}
			public object MaxValue(int[] values)
			{
				int num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					int num2 = values[i];
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MaxValue(long[] values)
			{
				long num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					long num2 = values[i];
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MaxValue(float[] values)
			{
				float num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					float num2 = values[i];
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MaxValue(double[] values)
			{
				double num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					double num2 = values[i];
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MaxValue(decimal[] values)
			{
				decimal num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					decimal num2 = values[i];
					if (num < num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MinValue(int[] values)
			{
				int num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					int num2 = values[i];
					if (num > num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MinValue(long[] values)
			{
				long num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					long num2 = values[i];
					if (num > num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MinValue(float[] values)
			{
				float num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					float num2 = values[i];
					if (num > num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MinValue(double[] values)
			{
				double num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					double num2 = values[i];
					if (num > num2)
					{
						num = num2;
					}
				}
				return num;
			}
			public object MinValue(decimal[] values)
			{
				decimal num = values[0];
				for (int i = 0; i < values.Length; i++)
				{
					decimal num2 = values[i];
					if (num > num2)
					{
						num = num2;
					}
				}
				return num;
			}
		}
		private bool IsMoving = false;
		private int ctrlLastWidth = 0;
		private int ctrlLastHeight = 0;
		private int ctrlWidth;
		private int ctrlHeight;
		private int ctrlLeft;
		private int ctrlTop;
		private int cursorL;
		private int cursorT;
		private int ctrlLastLeft;
		private int ctrlLastTop;
		private int Htap;
		private int Wtap;
		private bool ctrlIsResizing = false;
		private Rectangle ctrlRectangle = default(Rectangle);
		private Control ctrl;
		private Form frm;
		public void KillProgram(string ex)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			ex = Path.GetFileNameWithoutExtension(ex) + ".exe";
			process.StandardInput.WriteLine("taskkill /im " + ex + " /f");
			process.StandardInput.WriteLine("exit");
			process.WaitForExit();
			process.Close();
			process.Dispose();
		}
		public bool IsAlphabet(string s)
		{
			char[] array = s.ToCharArray();
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				char ch = array[i];
				if (!WindowsAPI.IsCharAlpha(ch))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public bool IsNumeric(string s)
		{
			bool result;
			for (int i = 0; i < s.Length; i++)
			{
				if (!WindowsAPI.IsCharAlphaNumeric(s.Substring(i, 1)))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public bool IsLower(string s)
		{
			char[] array = s.ToCharArray();
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				char ch = array[i];
				if (!WindowsAPI.IsCharLower(ch))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public bool IsUpper(string s)
		{
			char[] array = s.ToCharArray();
			bool result;
			for (int i = 0; i < array.Length; i++)
			{
				char ch = array[i];
				if (!WindowsAPI.IsCharUpper(ch))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}
		public void LockRegedit()
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.LocalMachine;
			regeditManageMent.RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
			regeditManageMent.CreateString("regedit.exe", "Debugger", "123.exe");
			regeditManageMent.CreateString("regedit32.exe", "Debugger", "123.exe");
		}
		public void ReleaseRegedit()
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.LocalMachine;
			regeditManageMent.RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
			regeditManageMent.RegeditDeleteSubKey("regedit.exe");
			regeditManageMent.RegeditDeleteSubKey("regedit32.exe");
		}
		public void LockMSConfig()
		{
			new RegeditManageMent
			{
				RegeditKey = Registry.LocalMachine,
				RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options"
			}.CreateString("msconfig.exe", "Debugger", "123.exe");
		}
		public void ReleaseMSConfig()
		{
			new RegeditManageMent
			{
				RegeditKey = Registry.LocalMachine,
				RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options"
			}.RegeditDeleteSubKey("msconfig.exe");
		}
		public void LockCommon()
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.LocalMachine;
			regeditManageMent.RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
			regeditManageMent.CreateString("qq.exe", "Debugger", "123.exe");
			regeditManageMent.CreateString("notepad.exe", "Debugger", "123.exe");
			regeditManageMent.CreateString("KMPlayer.exe", "Debugger", "123.exe");
			regeditManageMent.CreateString("Thunder.exe", "Debugger", "123.exe");
			regeditManageMent.CreateString("iexplore.exe", "Debugger", "123.exe");
		}
		public void ReleaseCommon()
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.LocalMachine;
			regeditManageMent.RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
			regeditManageMent.RegeditDeleteSubKey("qq.exe");
			regeditManageMent.RegeditDeleteSubKey("notepad.exe");
			regeditManageMent.RegeditDeleteSubKey("KMPlayer.exe");
			regeditManageMent.RegeditDeleteSubKey("Thunder.exe");
			regeditManageMent.RegeditDeleteSubKey("iexplore.exe");
		}
		public void LockSoftWare(string name)
		{
			APIMethod aPIMethod = new APIMethod();
			name = aPIMethod.PathRemoveExtension(name) + ".exe";
			new RegeditManageMent
			{
				RegeditKey = Registry.LocalMachine,
				RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options"
			}.CreateString(name, "Debugger", "123.exe");
		}
		public void ReleaseSoftWare(string name)
		{
			APIMethod aPIMethod = new APIMethod();
			name = aPIMethod.PathRemoveExtension(name) + ".exe";
			new RegeditManageMent
			{
				RegeditKey = Registry.LocalMachine,
				RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options"
			}.RegeditDeleteSubKey(name);
		}
		public void ReleaseImage()
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.LocalMachine;
			regeditManageMent.RegeditPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options";
			string[] subNames = regeditManageMent.GetSubNames();
			string[] array = subNames;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text != "DllNXOptions" && text != "IEInstal.exe" && text != "iexplore.exe")
				{
					regeditManageMent.RegeditDeleteSubKey(text);
				}
			}
		}
		public void AddToSystemMenu(string name, string value, string path)
		{
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.ClassesRoot;
			regeditManageMent.RegeditPath = "*\\shell\\";
			regeditManageMent.CreateString(name, "", value);
			regeditManageMent.RegeditPath = "*\\shell\\" + name + "\\";
			regeditManageMent.CreateString("command", "", path + " %1");
		}
		public void AddToSystemMenuEx(string name, string value, string path)
		{
			APIMethod aPIMethod = new APIMethod();
			name = aPIMethod.PathRemoveExtension(name) + ".exe";
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.ClassesRoot;
			regeditManageMent.RegeditPath = "Applications\\";
			regeditManageMent.CreateString(name, "", "");
			regeditManageMent.RegeditPath = "Applications\\" + name + "\\";
			regeditManageMent.CreateString("shell", "", "open");
			regeditManageMent.RegeditPath = "Applications\\" + name + "\\shell\\";
			regeditManageMent.CreateString("Enqueue", "", value);
			regeditManageMent.CreateString("open", "", "");
			regeditManageMent.RegeditPath = "Applications\\" + name + "\\shell\\Enqueue\\";
			regeditManageMent.CreateString("command", "", "\"" + path + " /ADD \"%1\"");
			regeditManageMent.RegeditPath = "Applications\\" + name + "\\shell\\open\\";
			regeditManageMent.CreateString("command", "", "\"" + path + "\" \"%1\"");
		}
		public void AddToSystemMenuNew(string ex, string name, string path)
		{
			if (ex.StartsWith("."))
			{
				ex = ex.Substring(1, ex.Length - 1);
			}
			RegeditManageMent regeditManageMent = new RegeditManageMent();
			regeditManageMent.RegeditKey = Registry.ClassesRoot;
			regeditManageMent.CreateString("." + ex, "", name);
			regeditManageMent.RegeditPath = "." + ex + "\\";
			regeditManageMent.CreateString("ShellNew", "", "");
			RegeditManageMent expr_7C = regeditManageMent;
			expr_7C.RegeditPath += "ShellNew\\";
			regeditManageMent.SetStringValue("FileName", path);
			regeditManageMent.SetStringValue("NullFile", "");
		}
		public static string GetPassword(string file)
		{
			byte[] array = new byte[]
			{
				190,
				236,
				101,
				156,
				254,
				40,
				43,
				138,
				108,
				123,
				205,
				223,
				79,
				19,
				247,
				177
			};
			byte b = 12;
			string text = "";
			string result;
			try
			{
				FileStream fileStream = File.OpenRead(file);
				fileStream.Seek(20L, SeekOrigin.Begin);
				byte b2 = (byte)fileStream.ReadByte();
				fileStream.Seek(66L, SeekOrigin.Begin);
				byte[] array2 = new byte[33];
				if (fileStream.Read(array2, 0, 33) != 33)
				{
					result = "";
					return result;
				}
                //byte b3 = array2[32] ^ b;
                //for (int i = 0; i < 16; i++)
                //{
                //    byte b4 = array[i] ^ array2[i * 2];
                //    if (i % 2 == 0 && b2 == 1)
                //    {
                //        b4 ^= b3;
                //    }
                //    if (b4 > 0)
                //    {
                //        text += (char)b4;
                //    }
                //}
			}
			catch
			{
			}
			result = text;
			return result;
		}
		public static void ModifyExpandName(ref string path, string exp)
		{
			if (Path.GetExtension(path) != exp)
			{
				path = Path.ChangeExtension(path, exp);
			}
		}
		public string StringEncryption(string str, string password)
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] rgbIV = new byte[]
			{
				18,
				52,
				86,
				120,
				144,
				171,
				205,
				239
			};
			byte[] bytes = Encoding.UTF8.GetBytes(password);
			byte[] bytes2 = Encoding.UTF8.GetBytes(str);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateEncryptor(bytes, rgbIV), CryptoStreamMode.Write);
			cryptoStream.Write(bytes2, 0, bytes2.Length);
			cryptoStream.FlushFinalBlock();
			return Convert.ToBase64String(memoryStream.ToArray());
		}
		public string StringDeencryption(string str, string password)
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
			byte[] rgbIV = new byte[]
			{
				18,
				52,
				86,
				120,
				144,
				171,
				205,
				239
			};
			byte[] bytes = Encoding.Default.GetBytes(password);
			byte[] array = Convert.FromBase64String(str);
			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, dESCryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			Encoding encoding = new UTF8Encoding();
			return encoding.GetString(memoryStream.ToArray());
		}
		public string GetOriginUrlText(string url)
		{
			string result;
			try
			{
				string text = "";
				WebRequest webRequest = WebRequest.Create(url);
				WebResponse response = webRequest.GetResponse();
				Stream responseStream = response.GetResponseStream();
				StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
				do
				{
					text = text + streamReader.ReadLine() + "\r\n";
				}
				while (!streamReader.EndOfStream);
				result = text.Trim();
			}
			catch (Exception ex)
			{
				result = ex.Message;
			}
			return result;
		}
		public Bitmap PartBitmapA(Image BitmapSource, int X, int Y, int Width, int Height)
		{
			Bitmap bitmap = new Bitmap(Width, Height);
			Bitmap bitmap2 = new Bitmap(BitmapSource);
			Bitmap result;
			if (BitmapSource == null)
			{
				result = new Bitmap(0, 0);
			}
			else
			{
				try
				{
					for (int i = X; i < X + Width; i++)
					{
						for (int j = Y; j < Y + Height; j++)
						{
							Color pixel = bitmap2.GetPixel(i, j);
							bitmap.SetPixel(i - X, j - Y, pixel);
						}
					}
				}
				catch
				{
					result = null;
					return result;
				}
				result = bitmap;
			}
			return result;
		}
		public Bitmap PartBitmapB(Image BitmapSource, int X, int Y, int Width, int Height)
		{
			Bitmap bitmap = new Bitmap(Width, Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.DrawImage(BitmapSource, new Rectangle(new Point(0, 0), new Size(Width, Height)), new Rectangle(new Point(X, Y), new Size(Width, Height)), GraphicsUnit.Pixel);
			return bitmap;
		}
		public Bitmap CombineBitmap(Image Source1, Image Source2, int X, int Y)
		{
			Image image = (Image)Source1.Clone();
			Graphics graphics = Graphics.FromImage(image);
			graphics.DrawImage(Source2, X, Y);
			return new Bitmap(image);
		}
		public Bitmap ZoomImage(Image img, float scale)
		{
			int num = (int)((float)img.Size.Width * scale);
			int num2 = (int)((float)img.Size.Height * scale);
			Bitmap result;
			if (num <= 0 || num2 <= 0)
			{
				result = new Bitmap(img);
			}
			else
			{
				Bitmap bitmap = new Bitmap(num, num2);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawImage(img, new Rectangle(0, 0, num, num2), new Rectangle(0, 0, img.Size.Width, img.Size.Height), GraphicsUnit.Pixel);
				graphics.Dispose();
				result = bitmap;
			}
			return result;
		}
		public Bitmap ZoomImage(Image img, int width, int height)
		{
			Bitmap result;
			if (width <= 0 || height <= 0)
			{
				result = new Bitmap(img);
			}
			else
			{
				Bitmap bitmap = new Bitmap(width, height);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.DrawImage(img, new Rectangle(0, 0, width, height), new Rectangle(0, 0, img.Size.Width, img.Size.Height), GraphicsUnit.Pixel);
				graphics.Dispose();
				result = bitmap;
			}
			return result;
		}
		public void UniqueApplication(string text, string caption)
		{
			if (text == "")
			{
				text = "对不起，程序已经在运行了。本程序不能同时开启两个窗口";
			}
			if (caption == "")
			{
				caption = "提示";
			}
			Process currentProcess = Process.GetCurrentProcess();
			Process[] processes = Process.GetProcesses();
			Process process = null;
			Process process2 = null;
			Process[] array = processes;
			for (int i = 0; i < array.Length; i++)
			{
				Process process3 = array[i];
				if (process3.ProcessName == currentProcess.ProcessName)
				{
					if (process != null)
					{
						process2 = process3;
						break;
					}
					process = process3;
				}
			}
			if (process != null && process2 != null)
			{
				MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				if (process.Id == currentProcess.Id)
				{
					process.Kill();
				}
				else
				{
					process2.Kill();
				}
			}
		}
		public void TextBoxSetting(TextBox UserTextBox)
		{
			UserTextBox.AllowDrop = true;
			UserTextBox.DragEnter += new DragEventHandler(this.UserTextBox_DragEnter);
			UserTextBox.DragDrop += new DragEventHandler(this.UserTextBox_DragDrop);
		}
		private void UserTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
		}
		private void UserTextBox_DragDrop(object sender, DragEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			int num = this.CheckFile(path);
			if (num == -1)
			{
				MessageBox.Show("拒绝访问。", "载入文件", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				textBox.Text = File.ReadAllText(path, Encoding.Default);
			}
		}
		public void RichTextBoxSetting(RichTextBox UserRichTextBox)
		{
			UserRichTextBox.AllowDrop = true;
			UserRichTextBox.DragEnter += new DragEventHandler(this.UserRichTextBox_DragEnter);
			UserRichTextBox.DragDrop += new DragEventHandler(this.UserRichTextBox_DragDrop);
		}
		private void UserRichTextBox_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
		}
		private void UserRichTextBox_DragDrop(object sender, DragEventArgs e)
		{
			RichTextBox richTextBox = (RichTextBox)sender;
			string path = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			int num = this.CheckFile(path);
			if (num == -1)
			{
				MessageBox.Show("拒绝访问。", "载入文件", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				richTextBox.LoadFile(path, RichTextBoxStreamType.PlainText);
			}
		}
		public int CheckFile(string path)
		{
			int result;
			if (Directory.Exists(path))
			{
				result = -1;
			}
			else
			{
				if (!File.Exists(path))
				{
					result = 0;
				}
				else
				{
					result = 1;
				}
			}
			return result;
		}
		public void IsCreateNewFile(out bool create, RichTextBox UserRichTextBox, TextBox UserTextBox)
		{
			create = false;
			int length = Path.GetFileNameWithoutExtension(Application.ExecutablePath).Length;
			int length2 = Environment.CommandLine.Length;
			string text = Environment.CommandLine.Substring(length, length2 - length).Trim();
			string[] array = text.Split(new char[]
			{
				' '
			});
			string path = array[0];
			if (this.CheckFile(path) == 0)
			{
				DialogResult dialogResult = MessageBox.Show("找不到文件: " + Path.GetFileName(path) + "。\r\n要创建新文件吗?", "载入文件", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
				if (dialogResult == DialogResult.Cancel)
				{
					Application.Exit();
				}
				else
				{
					if (dialogResult == DialogResult.Yes)
					{
						File.Create(path);
						create = true;
						if (UserTextBox == null)
						{
							UserRichTextBox.LoadFile(Path.GetFullPath(path), RichTextBoxStreamType.PlainText);
						}
						if (UserRichTextBox == null)
						{
							UserTextBox.Text = File.ReadAllText(Path.GetFullPath(path), Encoding.Default);
						}
					}
				}
			}
		}
		public void ControlDrag(Control c)
		{
			c.AllowDrop = true;
			c.DragEnter += new DragEventHandler(this.c_DragEnter);
			c.DragDrop += new DragEventHandler(this.c_DragDrop);
		}
		private void c_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
		}
		private void c_DragDrop(object sender, DragEventArgs e)
		{
			Control control = (Control)sender;
			string text = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			control.Text = text;
		}
		public void FormDrag(Form form)
		{
			form.AllowDrop = true;
			form.DragEnter += new DragEventHandler(this.form_DragEnter);
			form.DragDrop += new DragEventHandler(this.form_DragDrop);
		}
		private void form_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
		}
		private void form_DragDrop(object sender, DragEventArgs e)
		{
			Form form = (Form)sender;
			string fileName = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
			APIMethod aPIMethod = new APIMethod();
			form.Icon = aPIMethod.GetFileIconEx(fileName);
		}
		public void ResizeAction(Control c, Form frm)
		{
			this.ctrl = c;
			this.frm = frm;
			this.Htap = this.frm.Height - this.frm.ClientRectangle.Height;
			this.Wtap = this.frm.Width - this.frm.ClientRectangle.Width;
			this.ctrl.MouseDown += new MouseEventHandler(this.MouseDown);
			this.ctrl.MouseMove += new MouseEventHandler(this.MouseMove);
			this.ctrl.MouseUp += new MouseEventHandler(this.MouseUp);
		}
		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (this.frm != null)
			{
				if (e.Button == MouseButtons.Left)
				{
					if (this.IsMoving)
					{
						if (this.ctrlLastLeft == 0)
						{
							this.ctrlLastLeft = this.ctrlLeft;
						}
						if (this.ctrlLastTop == 0)
						{
							this.ctrlLastTop = this.ctrlTop;
						}
						int num = Cursor.Position.X - this.cursorL + this.frm.DesktopLocation.X + this.Wtap + this.ctrl.Location.X;
						int num2 = Cursor.Position.Y - this.cursorT + this.frm.DesktopLocation.Y + this.Htap + this.ctrl.Location.Y;
						if (num < this.frm.DesktopLocation.X + this.Wtap)
						{
							num = this.frm.DesktopLocation.X + this.Wtap;
						}
						if (num2 < this.frm.DesktopLocation.Y + this.Htap)
						{
							num2 = this.frm.DesktopLocation.Y + this.Htap;
						}
						this.ctrlLeft = num;
						this.ctrlTop = num2;
						this.ctrlRectangle.Location = new Point(this.ctrlLastLeft, this.ctrlLastTop);
						this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
						this.ctrlRectangle.Location = new Point(this.ctrlLeft, this.ctrlTop);
						this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
					}
					else
					{
						int num3 = Cursor.Position.X - this.frm.DesktopLocation.X - this.Wtap - this.ctrl.Location.X;
						int num4 = Cursor.Position.Y - this.frm.DesktopLocation.Y - this.Htap - this.ctrl.Location.Y;
						if (num3 < 2)
						{
							num3 = 1;
						}
						if (num4 < 2)
						{
							num4 = 1;
						}
						this.ctrlWidth = num3;
						this.ctrlHeight = num4;
						if (this.ctrlLastWidth == 0)
						{
							this.ctrlLastWidth = this.ctrlWidth;
						}
						if (this.ctrlLastHeight == 0)
						{
							this.ctrlLastHeight = this.ctrlHeight;
						}
						if (this.ctrlIsResizing)
						{
							this.ctrlRectangle.Location = new Point(this.frm.DesktopLocation.X + this.ctrl.Left + this.Wtap, this.frm.DesktopLocation.Y + this.Htap + this.ctrl.Top);
							this.ctrlRectangle.Size = new Size(this.ctrlLastWidth, this.ctrlLastHeight);
						}
						this.ctrlIsResizing = true;
						ControlPaint.DrawReversibleFrame(this.ctrlRectangle, Color.Empty, FrameStyle.Dashed);
						this.ctrlLastWidth = this.ctrlWidth;
						this.ctrlLastHeight = this.ctrlHeight;
						this.ctrlRectangle.Location = new Point(this.frm.DesktopLocation.X + this.Wtap + this.ctrl.Left, this.frm.DesktopLocation.Y + this.Htap + this.ctrl.Top);
						this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
						ControlPaint.DrawReversibleFrame(this.ctrlRectangle, Color.Empty, FrameStyle.Dashed);
					}
				}
			}
		}
		private void MouseDown(object sender, MouseEventArgs e)
		{
			if (this.frm != null)
			{
				if (e.X < this.ctrl.Width - 10 || e.Y < this.ctrl.Height - 10)
				{
					this.IsMoving = true;
					this.ctrlLeft = this.frm.DesktopLocation.X + this.Wtap + this.ctrl.Left;
					this.ctrlTop = this.frm.DesktopLocation.Y + this.Htap + this.ctrl.Top;
					this.cursorL = Cursor.Position.X;
					this.cursorT = Cursor.Position.Y;
					this.ctrlWidth = this.ctrl.Width;
					this.ctrlHeight = this.ctrl.Height;
				}
				this.ctrlRectangle.Location = new Point(this.ctrlLeft, this.ctrlTop);
				this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
			}
		}
		private void MouseUp(object sender, MouseEventArgs e)
		{
			if (this.frm != null)
			{
				this.ctrlIsResizing = false;
				if (this.IsMoving)
				{
					this.ctrlRectangle.Location = new Point(this.ctrlLeft, this.ctrlTop);
					this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
					this.IsMoving = false;
					this.ctrl.Refresh();
				}
				else
				{
					this.ctrlRectangle.Location = new Point(this.frm.DesktopLocation.X + this.Wtap + this.ctrl.Left, this.frm.DesktopLocation.Y + this.Htap + this.ctrl.Top);
					this.ctrlRectangle.Size = new Size(this.ctrlWidth, this.ctrlHeight);
					ControlPaint.DrawReversibleFrame(this.ctrlRectangle, Color.Empty, FrameStyle.Dashed);
					this.ctrl.Width = this.ctrlWidth;
					this.ctrl.Height = this.ctrlHeight;
					this.ctrl.Refresh();
				}
			}
		}
		public static void Serialize(string path, params string[] content)
		{
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < content.Length; i++)
			{
				hashtable.Add(content[i], i.ToString());
			}
			FileStream fileStream = new FileStream(path, FileMode.Create);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			try
			{
				binaryFormatter.Serialize(fileStream, hashtable);
			}
			catch (SerializationException ex)
			{
				MessageBox.Show("Failed to serialize. Reason: " + ex.Message);
			}
			finally
			{
				fileStream.Close();
			}
		}
		public static string Deserialize(string path)
		{
			Hashtable hashtable = null;
			string text = "";
			FileStream fileStream = new FileStream(path, FileMode.Open);
			string result;
			try
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				hashtable = (Hashtable)binaryFormatter.Deserialize(fileStream);
			}
			catch (SerializationException ex)
			{
				MessageBox.Show("Failed to deserialize. Reason: " + ex.Message);
				result = "";
				return result;
			}
			finally
			{
				fileStream.Close();
			}
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			foreach (DictionaryEntry dictionaryEntry in hashtable)
			{
				arrayList.Add(dictionaryEntry.Key);
				arrayList2.Add(dictionaryEntry.Value);
			}
			for (int i = 0; i < arrayList.Count; i++)
			{
				int num = arrayList2.IndexOf(i.ToString());
				if (num != -1)
				{
					object value = arrayList[i];
					arrayList[i] = arrayList[num];
					arrayList[num] = value;
					object value2 = arrayList2[i];
					arrayList2[i] = arrayList2[num];
					arrayList2[num] = value2;
				}
				text = text + arrayList[i].ToString() + "\r\n";
			}
			result = text;
			return result;
		}
	}
}
