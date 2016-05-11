using Microsoft.Win32;
using System;
namespace ZYC
{
	public class RegeditManageMent
	{
		private string regeditPath = "";
		private RegistryKey regeditKey = null;
		public string RegeditPath
		{
			get
			{
				return this.regeditPath;
			}
			set
			{
				this.regeditPath = value;
			}
		}
		public RegistryKey RegeditKey
		{
			get
			{
				return this.regeditKey;
			}
			set
			{
				this.regeditKey = value;
			}
		}
		public RegeditManageMent()
		{
			this.regeditPath = "";
		}
		public void SetValue(string path, RegistryKey key, string data1, string data2)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(data1, data2);
		}
		public void SetValue(string data1, string data2)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(data1, data2);
				}
			}
		}
		public void RegeditCreateSubKey(string path, RegistryKey key, string SubKeyName)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			this.regeditKey.OpenSubKey(this.regeditPath, true).CreateSubKey(SubKeyName);
		}
		public void RegeditCreateSubKey(string SubKeyName)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					this.regeditKey.OpenSubKey(this.regeditPath, true).CreateSubKey(SubKeyName);
				}
			}
		}
		public void CreateDWord(string name1, string name2, int value)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					RegistryKey registryKey = this.regeditKey.OpenSubKey(this.regeditPath, true).CreateSubKey(name1);
					registryKey.SetValue(name2, value, RegistryValueKind.DWord);
				}
			}
		}
		public void CreateString(string name1, string name2, string value)
		{
			if (this.regeditKey != null)
			{
				RegistryKey registryKey = this.regeditKey.OpenSubKey(this.regeditPath, true).CreateSubKey(name1);
				registryKey.SetValue(name2, value, RegistryValueKind.String);
			}
		}
		public void DeleteValue(string name)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(name, false);
				}
			}
		}
		public void RegeditDeleteSubKey(string path, RegistryKey key, string SubKeyName)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteSubKey(SubKeyName);
				}
			}
		}
		public void RegeditDeleteSubKey(string SubKeyName)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteSubKey(SubKeyName);
				}
			}
		}
		public void SetStringValue(string path, RegistryKey key, string KeyName, string data)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.String);
			}
			else
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
			}
		}
		public void SetStringValue(string KeyName, string data)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.String);
					}
					else
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
					}
				}
			}
		}
		public void SetBinaryValue(string path, RegistryKey key, string KeyName, byte[] data)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.Binary);
			}
			else
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
			}
		}
		public void SetBinaryValue(string KeyName, byte[] data)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.Binary);
					}
					else
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
					}
				}
			}
		}
		public void SetDWordValue(string path, RegistryKey key, string KeyName, int data)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.DWord);
			}
			else
			{
				this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
			}
		}
		public void SetDWordValue(string KeyName, int data)
		{
			if (this.regeditKey != null)
			{
				if (!(this.regeditPath == ""))
				{
					if (this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) == null)
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).SetValue(KeyName, data, RegistryValueKind.DWord);
					}
					else
					{
						this.regeditKey.OpenSubKey(this.regeditPath, true).DeleteValue(KeyName);
					}
				}
			}
		}
		public bool CheckValue(string path, RegistryKey key, string KeyName)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			return this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName) != null;
		}
		public string GetValue(string path, RegistryKey key, string KeyName)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			string result;
			if (this.CheckValue(path, key, KeyName))
			{
				result = this.regeditKey.OpenSubKey(this.regeditPath, true).GetValue(KeyName).ToString();
			}
			else
			{
				result = "";
			}
			return result;
		}
		public string[] GetValueNames()
		{
			string[] result;
			if (this.regeditKey == null)
			{
				result = null;
			}
			else
			{
				if (this.regeditPath == "")
				{
					result = null;
				}
				else
				{
					result = this.regeditKey.OpenSubKey(this.regeditPath, true).GetValueNames();
				}
			}
			return result;
		}
		public string[] GetValueNames(string path, RegistryKey key)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			string[] result;
			if (this.regeditKey == null)
			{
				result = null;
			}
			else
			{
				if (this.regeditPath == "")
				{
					result = null;
				}
				else
				{
					result = this.regeditKey.OpenSubKey(this.regeditPath, true).GetValueNames();
				}
			}
			return result;
		}
		public string[] GetSubNames()
		{
			string[] result;
			if (this.regeditKey == null)
			{
				result = null;
			}
			else
			{
				if (this.regeditPath == "")
				{
					result = null;
				}
				else
				{
					result = this.regeditKey.OpenSubKey(this.regeditPath, true).GetSubKeyNames();
				}
			}
			return result;
		}
		public string[] GetSubNames(string path, RegistryKey key)
		{
			this.regeditPath = path;
			this.regeditKey = key;
			return this.regeditKey.OpenSubKey(this.regeditPath, true).GetSubKeyNames();
		}
	}
}
