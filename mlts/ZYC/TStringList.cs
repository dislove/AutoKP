using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace ZYC
{
	public class TStringList
	{
		private int count = 0;
		private string name = "";
		private string text = "";
		private static int newcount = 0;
		private string filePath = "";
		private ArrayList arraylist = null;
		public string[] Strings;
		public int Count
		{
			get
			{
				return this.count;
			}
		}
		public string FilePath
		{
			get
			{
				return this.filePath;
			}
			set
			{
				this.filePath = value;
			}
		}
		public string FirstString
		{
			get
			{
				string result;
				if (this.arraylist == null)
				{
					result = "";
				}
				else
				{
					result = this.arraylist[0].ToString();
				}
				return result;
			}
		}
		public bool IsRepeat
		{
			get
			{
				return this.isRepeat();
			}
		}
		public string LastString
		{
			get
			{
				string result;
				if (this.arraylist == null)
				{
					result = "";
				}
				else
				{
					result = this.arraylist[this.count - 1].ToString();
				}
				return result;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		public string Text
		{
			get
			{
				this.Reset();
				return this.text;
			}
			set
			{
				this.text = value;
				this.GetStrings(this.text);
			}
		}
		public TStringList()
		{
			TStringList.newcount++;
			this.name = "StringList" + TStringList.newcount.ToString();
			this.text = "";
			this.count = 0;
			this.arraylist = new ArrayList();
		}
		public TStringList(string data)
		{
			TStringList.newcount++;
			this.name = "StringList" + TStringList.newcount.ToString();
			this.text = data;
			this.count = 1;
			this.arraylist.Add(data);
		}
		public TStringList(string[] data)
		{
			TStringList.newcount++;
			this.name = "StringList" + TStringList.newcount.ToString();
			for (int i = 0; i < data.Length; i++)
			{
				string str = data[i];
				this.text = this.text + "\r\n" + str;
			}
			this.count = data.Length;
			this.arraylist.AddRange(data);
		}
		public void Add(string data)
		{
			this.arraylist.Add(data);
			this.count++;
			this.Reset();
		}
		public void Append(TStringList StringList)
		{
			this.text = this.text + "\r\n" + StringList.text;
			this.GetStrings(this.text);
			this.count = this.arraylist.Count;
		}
		public void Append(string data)
		{
			this.text = this.text.Trim() + "\r\n" + data;
			this.GetStrings(this.text);
			this.count = this.arraylist.Count;
		}
		public void Clear()
		{
			this.arraylist.Clear();
			this.text = "";
			this.count = 0;
		}
		public void ClearBlankLine()
		{
			if (this.arraylist.Count != 0)
			{
				string text = "";
				foreach (string text2 in this.arraylist)
				{
					if (text2.Trim() != "")
					{
						text = text + text2 + "\r\n";
					}
				}
				this.text = text.TrimEnd(new char[0]);
				this.GetStrings(this.text);
				this.count = this.arraylist.Count;
			}
		}
		public bool Contain(string value)
		{
			return this.arraylist.Contains(value);
		}
		public void Delete(string data)
		{
			int num = this.arraylist.IndexOf(data);
			if (num != 0)
			{
				if (num <= this.count)
				{
					this.arraylist.Remove(data);
					this.Reset();
				}
			}
		}
		public void Delete(int index)
		{
			if (index <= this.count)
			{
				this.arraylist.RemoveAt(index);
				this.Reset();
			}
		}
		public void Delete(int first, int second)
		{
			if (first <= second)
			{
				if (first <= this.count)
				{
					if (second <= this.count)
					{
						this.arraylist.RemoveRange(first, second - first + 1);
						this.Reset();
					}
				}
			}
		}
		public void DeleteAll(string data)
		{
			while (this.arraylist.IndexOf(data) != -1)
			{
				this.arraylist.Remove(data);
			}
			this.arraylist.ToString();
			this.Reset();
		}
		public override bool Equals(object obj)
		{
			TStringList tStringList = (TStringList)obj;
			return tStringList.arraylist == this.arraylist;
		}
		public TStringList Exchange(int index1, int index2)
		{
			int num;
			if (index1 > index2)
			{
				num = index1;
			}
			else
			{
				num = index2;
			}
			TStringList result;
			if (num > this.count)
			{
				result = null;
			}
			else
			{
				TStringList tStringList = new TStringList();
				tStringList.arraylist = this.arraylist;
				string value = tStringList.arraylist[index1 - 1].ToString();
				tStringList.arraylist[index1 - 1] = tStringList.arraylist[index2 - 1];
				tStringList.arraylist[index2 - 1] = value;
				foreach (string str in tStringList.arraylist)
				{
					TStringList expr_9F = tStringList;
					expr_9F.text = expr_9F.text + str + "\r\n";
				}
				tStringList.text = tStringList.text.Trim();
				tStringList.count = tStringList.arraylist.Count;
				result = tStringList;
			}
			return result;
		}
		public TStringList Exchange(string data1, string data2)
		{
			int num = this.arraylist.IndexOf(data1);
			int num2 = this.arraylist.IndexOf(data2);
			TStringList result;
			if (num == -1)
			{
				result = null;
			}
			else
			{
				if (num2 == -1)
				{
					result = null;
				}
				else
				{
					TStringList tStringList = new TStringList();
					tStringList.arraylist = this.arraylist;
					tStringList.arraylist[num] = data2;
					tStringList.arraylist[num2] = data1;
					foreach (string str in tStringList.arraylist)
					{
						TStringList expr_91 = tStringList;
						expr_91.text = expr_91.text + str + "\r\n";
					}
					tStringList.text = tStringList.text.Trim();
					tStringList.count = tStringList.arraylist.Count;
					result = tStringList;
				}
			}
			return result;
		}
		public int FirstLineString(string data)
		{
			int result;
			if (this.arraylist.Count == 0)
			{
				result = -1;
			}
			else
			{
				for (int i = 0; i < this.count; i++)
				{
					if (this.arraylist[i].ToString().IndexOf(data) != -1)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			return result;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		public void Insert(int index, string data)
		{
			if (index <= this.count)
			{
				this.arraylist.Insert(index, data);
				this.Reset();
			}
		}
		public int LastLineString(string data)
		{
			int result;
			if (this.arraylist.Count == 0)
			{
				result = -1;
			}
			else
			{
				for (int i = this.count - 1; i > -1; i--)
				{
					if (this.arraylist[i].ToString().IndexOf(data) != -1)
					{
						result = i;
						return result;
					}
				}
				result = -1;
			}
			return result;
		}
		public int LastPos(string data)
		{
			return this.arraylist.LastIndexOf(data);
		}
		public void LoadFromTXT(string path)
		{
			if (File.Exists(path))
			{
				StreamReader streamReader = new StreamReader(path);
				this.text = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				this.GetStrings(this.text);
				this.filePath = path;
			}
		}
		public string PartialText(int index1, int index2)
		{
			string result;
			if (index1 > index2)
			{
				result = "";
			}
			else
			{
				if (index2 > this.count)
				{
					result = "";
				}
				else
				{
					string text = "";
					int i;
					for (i = index1 - 1; i < index2 - 1; i++)
					{
						text = text + this.arraylist[i].ToString() + "\r\n";
					}
					text += this.arraylist[i].ToString();
					result = text;
				}
			}
			return result;
		}
		public int Pos(string data)
		{
			int num = this.arraylist.IndexOf(data);
			return -1;
		}
		public void Reverse()
		{
			this.arraylist.Reverse();
			this.Reset();
		}
		public void SaveToTXT(string path)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.WriteLine(this.text);
			streamWriter.Close();
			streamWriter.Dispose();
		}
		public void Sort()
		{
			this.arraylist.Sort();
			this.Reset();
		}
		public void Swap(int index1, int index2)
		{
			int num;
			if (index1 > index2)
			{
				num = index1;
			}
			else
			{
				num = index2;
			}
			if (num <= this.count)
			{
				string value = this.arraylist[index1 - 1].ToString();
				this.arraylist[index1 - 1] = this.arraylist[index2 - 1];
				this.arraylist[index2 - 1] = value;
				this.Reset();
			}
		}
		public void Swap(string data1, string data2)
		{
			int num = this.arraylist.IndexOf(data1);
			int num2 = this.arraylist.IndexOf(data2);
			if (num != -1)
			{
				if (num2 != -1)
				{
					this.arraylist[num] = data2;
					this.arraylist[num2] = data1;
					this.Reset();
				}
			}
		}
		public override string ToString()
		{
			return this.Name;
		}
		public void TrimAll()
		{
			if (this.arraylist != null)
			{
				for (int i = 0; i < this.count; i++)
				{
					this.arraylist[i] = this.arraylist[i].ToString().Trim();
				}
				this.Reset();
			}
		}
		public void Unique()
		{
			if (this.arraylist != null)
			{
				this.arraylist = this.ArrayUnique(this.arraylist);
				this.count = this.arraylist.Count;
				this.Reset();
			}
		}
		public void WriteBegin(string beginstring)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.arraylist[i] = beginstring + this.arraylist[i].ToString();
			}
			this.Reset();
		}
		public void WriteEnd(string endstring)
		{
			for (int i = 0; i < this.count; i++)
			{
				this.arraylist[i] = this.arraylist[i].ToString() + endstring;
			}
			this.Reset();
		}
		private ArrayList ArrayUnique(ArrayList data)
		{
			ArrayList arrayList = new ArrayList();
			foreach (string text in data)
			{
				if (!arrayList.Contains(text))
				{
					arrayList.Add(text);
				}
			}
			data.Clear();
			data = arrayList;
			return data;
		}
		private void GetStrings(string data)
		{
			Regex regex = new Regex("\r\n");
			this.arraylist.Clear();
			this.arraylist.AddRange(regex.Split(this.text));
			this.count = this.arraylist.Count;
			this.Strings = new string[this.count];
			Array array = this.arraylist.ToArray();
			array.CopyTo(this.Strings, 0);
		}
		private bool isRepeat()
		{
			bool result;
			if (this.arraylist == null)
			{
				result = false;
			}
			else
			{
				ArrayList data = this.arraylist;
				int num = this.ArrayUnique(data).Count;
				result = (this.count != num);
			}
			return result;
		}
		private void Reset()
		{
			this.text = "";
			for (int i = 0; i < this.arraylist.Count; i++)
			{
				this.text += this.arraylist[i];
				if (i < this.arraylist.Count - 1)
				{
					this.text += "\r\n";
				}
			}
			this.text = this.text.TrimEnd(new char[0]);
			this.count = this.arraylist.Count;
			this.Strings = new string[this.count];
			Array array = this.arraylist.ToArray();
			array.CopyTo(this.Strings, 0);
		}
		public void LoadFromRTF(string path)
		{
			if (File.Exists(path))
			{
				RichTextBox richTextBox = new RichTextBox();
				richTextBox.LoadFile(path, RichTextBoxStreamType.RichText);
				this.text = richTextBox.Text;
				richTextBox.Dispose();
				this.GetStrings(this.text);
				this.filePath = path;
			}
		}
		public void SaveToRTF(string path)
		{
			RichTextBox richTextBox = new RichTextBox();
			richTextBox.AppendText(this.text);
			richTextBox.SaveFile(path);
			richTextBox.Dispose();
		}
	}
}
