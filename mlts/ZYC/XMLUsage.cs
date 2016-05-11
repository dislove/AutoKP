using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
namespace ZYC
{
	public class XMLUsage
	{
		private XmlDocument xdoc = null;
		private XmlDocumentType xdoctype = null;
		private XmlNodeList xnodelist = null;
		private XmlNode xnode = null;
		private XmlElement xele = null;
		private XmlAttribute xattr = null;
		private string path = "";
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}
		public XMLUsage()
		{
			this.path = "./";
		}
		public bool RetrieveDatabaseName(string database)
		{
			return Directory.Exists(this.path + database);
		}
		public bool RetrieveTableName(string database, string tablename)
		{
			return File.Exists(string.Concat(new string[]
			{
				this.path,
				"/",
				database,
				"/",
				tablename,
				".xml"
			}));
		}
		public void CreateDatabase(string database)
		{
			if (!Directory.Exists(this.path + database))
			{
				Directory.CreateDirectory(this.path + database);
			}
		}
		public void DropDatabase(string database)
		{
			if (Directory.Exists(this.path + database))
			{
				Directory.Delete(this.path + database, true);
			}
		}
		public void CreateDocument(string database, string tablename, string body)
		{
			if (!File.Exists(string.Concat(new string[]
			{
				this.path,
				database,
				"/",
				tablename,
				".xml"
			})))
			{
				TStringList tStringList = new TStringList();
				string[] array = body.Split(new char[]
				{
					','
				});
				tStringList.Add(string.Concat(new string[]
				{
					"<!ELEMENT _",
					tablename,
					" (",
					tablename,
					"+)>"
				}));
				tStringList.Add("<!ELEMENT " + tablename + " (");
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						' '
					});
					if (this.KeyWord(array2[0]))
					{
						return;
					}
					tStringList.Strings[1] = tStringList.Strings[1] + array2[0] + "+, ";
					tStringList.Add("<!ELEMENT " + array2[0] + " (#PCDATA)>");
					tStringList.Add(string.Concat(new string[]
					{
						"<!ATTLIST ",
						array2[0],
						" type CDATA \"",
						array2[1],
						"\">"
					}));
				}
				tStringList.Strings[1] = tStringList.Strings[1].Substring(0, tStringList.Strings[1].Length - 2) + ")>";
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoctype = this.xdoc.CreateDocumentType("_" + tablename, null, null, "\r\n" + tStringList.Text.Trim() + "\r\n");
				this.xdoc.AppendChild(this.xdoc.CreateXmlDeclaration("1.0", "gb2312", "yes"));
				this.xdoc.AppendChild(this.xdoctype);
				this.xdoc.AppendChild(this.xdoc.CreateElement("_" + tablename));
				this.xnode = this.xdoc.DocumentElement;
				this.xele = this.xdoc.CreateElement(tablename);
				this.xattr = this.xdoc.CreateAttribute("SID");
				this.xattr.Value = "0";
				this.xele.SetAttributeNode(this.xattr);
				this.xnode.AppendChild(this.xele);
				for (int j = 0; j < array.Length; j++)
				{
					string[] array2 = array[j].Split(new char[]
					{
						' '
					});
					XmlElement xmlElement = this.xdoc.CreateElement(array2[0]);
					XmlText newChild = this.xdoc.CreateTextNode(" ");
					this.xele.AppendChild(xmlElement);
					this.xele.LastChild.AppendChild(newChild);
					if (array2.Length == 2)
					{
						this.xattr = this.xdoc.CreateAttribute("type");
						this.xattr.Value = array2[1];
						xmlElement.SetAttributeNode(this.xattr);
					}
				}
				this.xdoc.Save(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
			}
		}
		public void DeleteDocument(string database, string tablename)
		{
			if (this.RetrieveTableName(database, tablename))
			{
				File.Delete(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
			}
		}
		public string[] GetAllDocument(string database)
		{
			return Directory.GetFiles(this.path + database);
		}
		public void GetDataType(string path, TStringList list1, TStringList list2)
		{
			if (File.Exists(path))
			{
				this.xdoc = new XmlDocument();
				this.xdoc.Load(path);
				this.xdoctype = this.xdoc.DocumentType;
				list1.Text = this.xdoctype.OuterXml;
				list1.Delete(1, 3);
				list1.Delete(list1.Count);
				TStringList tStringList = new TStringList();
				for (int i = 0; i < list1.Count; i += 2)
				{
					tStringList.Add(list1.Strings[i + 1]);
				}
				list1.Text = tStringList.Text;
				for (int i = 0; i < tStringList.Count; i++)
				{
					int num = tStringList.Strings[i].IndexOf("type");
					list1.Strings[i] = tStringList.Strings[i].Substring(10, num - 11);
					num = tStringList.Strings[i].IndexOf("CDATA") + 7;
					int length = tStringList.Strings[i].Length;
					tStringList.Strings[i] = tStringList.Strings[i].Substring(num, length - num - 2);
				}
				list2.Text = tStringList.Text;
			}
		}
		public string GetNodeValue(string path, string nodename)
		{
			string result;
			if (!File.Exists(System.IO.Path.GetFullPath(path)))
			{
				result = "";
			}
			else
			{
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoc.Load(path);
				this.xnodelist = this.xdoc.DocumentElement.ChildNodes;
				foreach (XmlNode xmlNode in this.xnodelist)
				{
					if (xmlNode.Name == nodename)
					{
						result = xmlNode.InnerText.Trim();
						return result;
					}
					this.xele = (XmlElement)xmlNode;
					foreach (XmlNode xmlNode2 in this.xele.ChildNodes)
					{
						if (xmlNode2.Name == nodename)
						{
							result = xmlNode2.InnerText.Trim();
							return result;
						}
					}
				}
				result = "";
			}
			return result;
		}
		public void DeleteData(string database, string tablename, string nodename, bool alldelete)
		{
			if (this.RetrieveTableName(database, tablename))
			{
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoc.Load(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
				this.xnodelist = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
				foreach (XmlNode xmlNode in this.xnodelist)
				{
					XmlElement xmlElement = (XmlElement)xmlNode;
					if (xmlElement.Name == tablename)
					{
						XmlNodeList childNodes = xmlElement.ChildNodes;
						foreach (XmlNode xmlNode2 in childNodes)
						{
							if (xmlNode2.Name == nodename)
							{
								xmlNode2.ParentNode.RemoveChild(xmlNode2);
								if (alldelete)
								{
									this.xdoc.Save(string.Concat(new string[]
									{
										this.path,
										database,
										"/",
										tablename,
										".xml"
									}));
									return;
								}
							}
						}
					}
				}
				this.xdoc.Save(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
			}
		}
		public void InsertElement(string database, string tablename, string nodename)
		{
			if (this.RetrieveTableName(database, tablename))
			{
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoc.Load(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
				TStringList tStringList = new TStringList();
				TStringList tStringList2 = new TStringList();
				this.GetDataType(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}), tStringList, tStringList2);
				this.xnodelist = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
				int count = this.xnodelist.Count;
				this.xele = this.xdoc.CreateElement(tablename);
				this.xattr = this.xdoc.CreateAttribute("SID");
				this.xattr.Value = count.ToString();
				this.xele.SetAttributeNode(this.xattr);
				this.xnode = this.xdoc.SelectSingleNode("_" + tablename);
				this.xnode.AppendChild(this.xele);
				string[] array = nodename.Split(new char[]
				{
					','
				});
				for (int i = 0; i < tStringList.Count; i++)
				{
					XmlElement xmlElement = this.xdoc.CreateElement(tStringList.Strings[i]);
					XmlText newChild = this.xdoc.CreateTextNode(array[i]);
					this.xele.AppendChild(xmlElement);
					this.xele.LastChild.AppendChild(newChild);
					this.xattr = this.xdoc.CreateAttribute("type");
					this.xattr.Value = tStringList2.Strings[i];
					xmlElement.SetAttributeNode(this.xattr);
				}
				this.xdoc.Save(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
			}
		}
		public string UpdateData(string database, string tablename, string nodename)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					string[] array = nodename.Split(new char[]
					{
						' '
					});
					if (array.Length == 3)
					{
						if (array[2].Substring(0, 1) == "'")
						{
							array[2] = array[2].Substring(1, array[2].Length - 2);
						}
						foreach (XmlNode xmlNode in childNodes)
						{
							if (xmlNode.Name == array[1])
							{
								xmlNode.InnerText = array[2];
							}
							XmlElement xmlElement = (XmlElement)xmlNode;
							foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
							{
								string[] array2 = xmlNode2.Name.Split(new char[]
								{
									' '
								});
								if (array2[0] == array[1])
								{
									xmlNode2.InnerText = array[2];
								}
							}
						}
					}
					else
					{
						if (array.Length == 4)
						{
							if (array[2].Substring(0, 1) == "'")
							{
								array[2] = array[2].Substring(1, array[2].Length - 2);
							}
							if (array[3].Substring(0, 1) == "'")
							{
								array[3] = array[3].Substring(1, array[3].Length - 2);
							}
							foreach (XmlNode xmlNode in childNodes)
							{
								if (xmlNode.Name == array[1] && xmlNode.InnerText == array[3])
								{
									xmlNode.InnerText = array[2];
									this.xdoc.Save(filename);
									result = "S";
									return result;
								}
								XmlElement xmlElement = (XmlElement)xmlNode;
								foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
								{
									string[] array2 = xmlNode2.Name.Split(new char[]
									{
										' '
									});
									if (array[3] == " ")
									{
										array[3] = "";
									}
									if (array2[0] == array[1] && xmlNode2.InnerText == array[3])
									{
										xmlNode2.InnerText = array[2];
										this.xdoc.Save(filename);
										result = "S";
										return result;
									}
								}
							}
						}
					}
					this.xdoc.Save(filename);
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string GetTag(string database, string tablename, string element)
		{
			TStringList tStringList = new TStringList();
			TStringList tStringList2 = new TStringList();
			this.GetDataType(string.Concat(new string[]
			{
				this.path,
				database,
				"/",
				tablename,
				".xml"
			}), tStringList, tStringList2);
			int num = tStringList.Pos(element);
			string result;
			if (num == -1)
			{
				result = "";
			}
			else
			{
				result = tStringList2.Strings[num];
			}
			return result;
		}
		public void CreateAttribute(string database, string tablename, string element, string name, string value)
		{
			if (this.RetrieveTableName(database, tablename))
			{
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoc.Load(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
				this.xnodelist = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
				foreach (XmlNode xmlNode in this.xnodelist)
				{
					this.xele = (XmlElement)xmlNode;
					if (xmlNode.Name == element)
					{
						if (this.xele.HasAttributes)
						{
							this.xele.Attributes.RemoveAll();
						}
						this.xattr = this.xdoc.CreateAttribute(name);
						this.xattr.Value = value;
						this.xele.SetAttributeNode(this.xattr);
					}
					foreach (XmlNode xmlNode2 in this.xele.ChildNodes)
					{
						if (xmlNode2.Name == element)
						{
							XmlElement xmlElement = (XmlElement)xmlNode2;
							if (xmlElement.HasAttributes)
							{
								xmlElement.Attributes.RemoveAll();
							}
							this.xattr = this.xdoc.CreateAttribute(name);
							this.xattr.Value = value;
							xmlElement.SetAttributeNode(this.xattr);
						}
					}
				}
				this.xdoc.Save(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
			}
		}
		public void CreateCharData(string database, string tablename, string element, string value, int index)
		{
			if (this.RetrieveTableName(database, tablename))
			{
				if (this.xdoc == null)
				{
					this.xdoc = new XmlDocument();
				}
				this.xdoc.Load(string.Concat(new string[]
				{
					this.path,
					database,
					"/",
					tablename,
					".xml"
				}));
				this.xnodelist = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
				if (index <= this.xnodelist.Count - 1)
				{
					IEnumerator enumerator;
					if (index != -1)
					{
						this.xele = (XmlElement)this.xnodelist.Item(index);
						enumerator = this.xele.ChildNodes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								XmlNode xmlNode = (XmlNode)enumerator.Current;
								if (xmlNode.Name == element)
								{
									xmlNode.InnerText = value;
									this.xdoc.Save(string.Concat(new string[]
									{
										this.path,
										database,
										"/",
										tablename,
										".xml"
									}));
									return;
								}
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					enumerator = this.xnodelist.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							XmlNode xmlNode = (XmlNode)enumerator.Current;
							this.xele = (XmlElement)xmlNode;
							foreach (XmlNode xmlNode2 in this.xele.ChildNodes)
							{
								XmlElement xmlElement = (XmlElement)xmlNode2;
								if (xmlNode2.Name == element)
								{
									xmlNode2.InnerText = value;
									if (index > -1)
									{
										this.xdoc.Save(string.Concat(new string[]
										{
											this.path,
											database,
											"/",
											tablename,
											".xml"
										}));
										return;
									}
								}
							}
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
					this.xdoc.Save(string.Concat(new string[]
					{
						this.path,
						database,
						"/",
						tablename,
						".xml"
					}));
				}
			}
		}
		public string CreateElement(string database, string tablename, string element, string name)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					string[] array = element.Split(new char[]
					{
						' '
					});
					XmlElement xmlElement = (XmlElement)childNodes.Item(Convert.ToInt32(array[0]) - 1);
					if (xmlElement.Name == array[1])
					{
						xmlElement.AppendChild(this.xdoc.CreateElement(name));
					}
					else
					{
						foreach (XmlNode xmlNode in xmlElement)
						{
							string[] array2 = xmlNode.Name.Split(new char[]
							{
								' '
							});
							if (array2[0] == array[1])
							{
								xmlNode.AppendChild(this.xdoc.CreateElement(name));
								this.xdoc.Save(filename);
								result = "S";
								return result;
							}
						}
					}
					this.xdoc.Save(filename);
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string GetAttribute(string database, string tablename, string element)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					foreach (XmlNode xmlNode in childNodes)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlNode.Name == element)
						{
							xmlElement = (XmlElement)xmlNode;
							result = xmlElement.GetAttribute("type");
							return result;
						}
						foreach (XmlNode xmlNode2 in xmlElement)
						{
							string[] array = xmlNode2.Name.Split(new char[]
							{
								' '
							});
							if (array[0] == element)
							{
								XmlElement xmlElement2 = (XmlElement)xmlNode2;
								result = xmlElement2.GetAttribute("type");
								return result;
							}
						}
					}
				}
				result = "F";
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string GenerateDocument(string database, string tablename, string document)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					result = this.xdoc.DocumentType.InternalSubset;
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string ModifyDTD(string database, string tablename, string content)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					string[] array = content.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							' '
						});
						if (this.KeyWord(array2[0]))
						{
							result = "F";
							return result;
						}
					}
					for (int i = 0; i < array.Length; i++)
					{
						this.xdoc = new XmlDocument();
						this.xdoc.Load(filename);
						string text = this.xdoc.DocumentElement.OuterXml;
						string[] array3 = array[i].Split(new char[]
						{
							' '
						});
						string text2 = this.xdoc.DocumentType.OuterXml;
						int[] array4 = new int[2];
						int num = 0;
						int num2 = 0;
						string text3 = text2;
						for (int j = 0; j < text3.Length; j++)
						{
							char c = text3[j];
							num2++;
							if (c == '>')
							{
								array4[num] = num2;
								num++;
							}
							if (num == 2)
							{
								break;
							}
						}
						string text4 = text2.Substring(array4[0], array4[1] - array4[0]);
						string text5 = text4;
						text5 = text5.Replace(")>", ", " + array3[0] + "+)>");
						text2 = text2.Replace(text4, text5);
						text2 = text2.Replace("]>", "<!ELEMENT " + array3[0] + " (#PCDATA)>]>");
						text2 = text2.Replace("]>", "<!ATTLIST " + array3[0] + " type CDATA #REQUIRED>]>");
						string internalSubset = this.xdoc.DocumentType.InternalSubset;
						StreamReader streamReader = new StreamReader(filename);
						text = streamReader.ReadToEnd();
						streamReader.Dispose();
						text = text.Replace(internalSubset, "");
						text = text.Replace("<!DOCTYPE _" + tablename + "[]>", "");
						text = text.Substring(2, text.Length - 2);
						text = text.Replace("xml version=\"1.0\" encoding=\"gb2312\"?>", "");
						text = text.Replace("xml version=\"1.0\" encoding=\"gb2312\" standalone=\"yes\"?>", "");
						StreamWriter streamWriter = new StreamWriter(filename, false, Encoding.Default);
						streamWriter.WriteLine("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
						streamWriter.WriteLine(text2);
						streamWriter.WriteLine(text);
						streamWriter.Close();
						this.xdoc.Load(filename);
						XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
						foreach (XmlNode xmlNode in childNodes)
						{
							XmlElement xmlElement = this.xdoc.CreateElement(array3[0]);
							XmlText newChild = this.xdoc.CreateTextNode(" ");
							xmlNode.AppendChild(xmlElement);
							xmlNode.LastChild.AppendChild(newChild);
							if (array3.Length == 2)
							{
								this.xattr = this.xdoc.CreateAttribute("type");
								this.xattr.Value = array3[1];
								xmlElement.SetAttributeNode(this.xattr);
							}
						}
						this.xdoc.Save(filename);
					}
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public bool KeyWord(string a)
		{
			string[] array = new string[]
			{
				"CREATE",
				"DELETE",
				"DROP",
				"FROM",
				"LEFT",
				"INNER",
				"INSERT",
				"JOIN",
				"RIGHT",
				"SELECT",
				"SET",
				"TABLE",
				"UPDATE",
				"WHERE"
			};
			int num = 0;
			string[] array2 = array;
			bool result;
			for (int i = 0; i < array2.Length; i++)
			{
				string a2 = array2[i];
				if (a2 == a.ToUpper())
				{
					result = true;
					return result;
				}
				num++;
			}
			result = false;
			return result;
		}
		public string Delete(string database, string tablename, string nodename)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					nodename = nodename.Replace(" ", "");
					string[] array = new string[2];
					int i = 0;
					int j = 0;
					int[] array2 = null;
					string a = "";
					if (nodename.IndexOf(">=") != -1)
					{
						a = ">=";
						array[0] = nodename.Substring(0, nodename.IndexOf(">="));
						array[1] = nodename.Substring(nodename.IndexOf(">=") + 2, nodename.Length - nodename.IndexOf(">=") - 2);
					}
					else
					{
						if (nodename.IndexOf("<=") != -1)
						{
							a = "<=";
							array[0] = nodename.Substring(0, nodename.IndexOf("<="));
							array[1] = nodename.Substring(nodename.IndexOf("<=") + 2, nodename.Length - nodename.IndexOf("<=") - 2);
						}
						else
						{
							if (nodename.IndexOf("<>") != -1)
							{
								a = "<>";
								array[0] = nodename.Substring(0, nodename.IndexOf("<>"));
								array[1] = nodename.Substring(nodename.IndexOf("<>") + 2, nodename.Length - nodename.IndexOf("<>") - 2);
							}
							else
							{
								if (nodename.IndexOf("=") != -1)
								{
									a = "=";
									array[0] = nodename.Substring(0, nodename.IndexOf("="));
									array[1] = nodename.Substring(nodename.IndexOf("=") + 1, nodename.Length - nodename.IndexOf("=") - 1);
								}
								else
								{
									if (nodename.IndexOf(">") != -1)
									{
										a = ">";
										array[0] = nodename.Substring(0, nodename.IndexOf(">"));
										array[1] = nodename.Substring(nodename.IndexOf(">") + 1, nodename.Length - nodename.IndexOf(">") - 1);
									}
									else
									{
										if (nodename.IndexOf("<") != -1)
										{
											a = "<";
											array[0] = nodename.Substring(0, nodename.IndexOf("<"));
											array[1] = nodename.Substring(nodename.IndexOf("<") + 1, nodename.Length - nodename.IndexOf("<") - 1);
										}
									}
								}
							}
						}
					}
					foreach (XmlNode xmlNode in childNodes)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						foreach (XmlNode xmlNode2 in xmlElement)
						{
							string[] array3 = xmlNode2.Name.Split(new char[]
							{
								' '
							});
							if (array3[0] == array[0])
							{
								if (a == "=")
								{
									if (xmlNode2.InnerText == array[1])
									{
										j++;
									}
								}
								else
								{
									if (a == ">")
									{
										if (int.Parse(xmlNode2.InnerText) > int.Parse(array[1]))
										{
											j++;
										}
									}
									else
									{
										if (a == "<")
										{
											if (int.Parse(xmlNode2.InnerText) < int.Parse(array[1]))
											{
												j++;
											}
										}
										else
										{
											if (a == ">=")
											{
												if (int.Parse(xmlNode2.InnerText) >= int.Parse(array[1]))
												{
													j++;
												}
											}
											else
											{
												if (a == "<=")
												{
													if (int.Parse(xmlNode2.InnerText) <= int.Parse(array[1]))
													{
														j++;
													}
												}
												else
												{
													if (a == "<>")
													{
														if (xmlNode2.InnerText != array[1])
														{
															j++;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						i++;
					}
					int num = j;
					array2 = new int[num];
					j = (i = 0);
					foreach (XmlNode xmlNode in childNodes)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						foreach (XmlNode xmlNode2 in xmlElement)
						{
							string[] array3 = xmlNode2.Name.Split(new char[]
							{
								' '
							});
							if (array3[0] == array[0])
							{
								if (a == "=")
								{
									if (xmlNode2.InnerText == array[1])
									{
										array2[j] = i;
										j++;
									}
								}
								else
								{
									if (a == ">")
									{
										if (int.Parse(xmlNode2.InnerText) > int.Parse(array[1]))
										{
											array2[j] = i;
											j++;
										}
									}
									else
									{
										if (a == "<")
										{
											if (int.Parse(xmlNode2.InnerText) < int.Parse(array[1]))
											{
												array2[j] = i;
												j++;
											}
										}
										else
										{
											if (a == ">=")
											{
												if (int.Parse(xmlNode2.InnerText) >= int.Parse(array[1]))
												{
													array2[j] = i;
													j++;
												}
											}
											else
											{
												if (a == "<=")
												{
													if (int.Parse(xmlNode2.InnerText) <= int.Parse(array[1]))
													{
														array2[j] = i;
														j++;
													}
												}
												else
												{
													if (a == "<>")
													{
														if (xmlNode2.InnerText != array[1])
														{
															array2[j] = i;
															j++;
														}
													}
												}
											}
										}
									}
								}
							}
						}
						i++;
					}
					for (i = 0; i < array2.Length; i++)
					{
						for (j = i; j < array2.Length; j++)
						{
							if (array2[i] < array2[j])
							{
								int num2 = array2[i];
								array2[i] = array2[j];
								array2[j] = num2;
							}
						}
					}
					for (i = 0; i < array2.Length; i++)
					{
						XmlElement xmlElement = (XmlElement)childNodes.Item(array2[i]);
						this.xdoc.SelectSingleNode("_" + tablename).RemoveChild(xmlElement);
					}
					this.xdoc.Save(filename);
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string Insert(string database, string tablename, string nodename)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					int num = 0;
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					XmlElement xmlElement = this.xdoc.CreateElement(tablename);
					this.xattr = this.xdoc.CreateAttribute("SID");
					foreach (XmlNode xmlNode in childNodes)
					{
						XmlElement xmlElement2 = (XmlElement)xmlNode;
						if (int.Parse(xmlElement2.GetAttribute("SID")) > num)
						{
							num = int.Parse(xmlElement2.GetAttribute("SID"));
						}
					}
					this.xattr.Value = (num + 1).ToString();
					xmlElement.SetAttributeNode(this.xattr);
					XmlNode xmlNode2 = this.xdoc.SelectSingleNode("_" + tablename);
					xmlNode2.AppendChild(xmlElement);
					XmlElement xmlElement3 = (XmlElement)childNodes.Item(childNodes.Count - 1);
					foreach (XmlNode xmlNode3 in xmlElement3)
					{
						string[] array = xmlNode3.Name.Split(new char[]
						{
							' '
						});
						if (array[0] == "id")
						{
							int num2 = Convert.ToInt32(xmlNode3.InnerText);
						}
					}
					string text = this.xdoc.DocumentType.InternalSubset;
					int[] array2 = new int[2];
					int num3 = 0;
					int num4 = 0;
					string text2 = text;
					for (int i = 0; i < text2.Length; i++)
					{
						char c = text2[i];
						if (c == '>')
						{
							array2[num3] = num4;
							num3++;
						}
						if (num3 == 2)
						{
							break;
						}
						num4++;
					}
					text = text.Substring(array2[0] + 3, array2[1] - array2[0] - 1).Replace("+", "");
					num3 = (num4 = 0);
					text2 = text;
					for (int i = 0; i < text2.Length; i++)
					{
						char c = text2[i];
						if (c == '(' | c == ')')
						{
							array2[num3] = num4;
							num3++;
						}
						if (num3 == 2)
						{
							break;
						}
						num4++;
					}
					text = text.Substring(array2[0] + 1, array2[1] - array2[0] - 1);
					text = text.Replace(" ", "");
					string[] array3 = nodename.Split(new char[]
					{
						','
					});
					string[] array4 = text.Split(new char[]
					{
						','
					});
					for (int j = 0; j < array4.Length; j++)
					{
						if (array4[j] == "id")
						{
							this.xattr = this.xdoc.CreateAttribute("type");
							this.xattr.Value = "int";
							xmlElement3 = this.xdoc.CreateElement(array4[j]);
							XmlText newChild = this.xdoc.CreateTextNode(array3[j]);
							xmlElement.AppendChild(xmlElement3);
							xmlElement.LastChild.AppendChild(newChild);
							xmlElement3.SetAttributeNode(this.xattr);
						}
						else
						{
							this.xattr = this.xdoc.CreateAttribute("type");
							this.xattr.Value = "string";
							xmlElement3 = this.xdoc.CreateElement(array4[j]);
							XmlText newChild = this.xdoc.CreateTextNode(array3[j]);
							xmlElement.AppendChild(xmlElement3);
							xmlElement.LastChild.AppendChild(newChild);
						}
					}
					this.xdoc.Save(filename);
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string Update(string database, string tablename, string nodename)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					string[] array = nodename.Split(new char[]
					{
						'|'
					});
					string[] array2 = new string[2];
					string[] array3 = new string[2];
					string a = "";
					if (array[0].IndexOf(">=") != -1)
					{
						array2[0] = array[0].Substring(0, array[0].IndexOf(">="));
						array2[1] = array[0].Substring(array[0].IndexOf(">=") + 2, array[0].Length - array[0].IndexOf(">=") - 2);
					}
					else
					{
						if (array[0].IndexOf("<=") != -1)
						{
							array2[0] = array[0].Substring(0, array[0].IndexOf("<="));
							array2[1] = array[0].Substring(array[0].IndexOf("<=") + 2, array[0].Length - array[0].IndexOf("<=") - 2);
						}
						else
						{
							if (array[0].IndexOf("<>") != -1)
							{
								array2[0] = array[0].Substring(0, array[0].IndexOf("<>"));
								array2[1] = array[0].Substring(array[0].IndexOf("<>") + 2, array[0].Length - array[0].IndexOf("<>") - 2);
							}
							else
							{
								if (array[0].IndexOf("=") != -1)
								{
									array2[0] = array[0].Substring(0, array[0].IndexOf("="));
									array2[1] = array[0].Substring(array[0].IndexOf("=") + 1, array[0].Length - array[0].IndexOf("=") - 1);
								}
								else
								{
									if (array[0].IndexOf(">") != -1)
									{
										array2[0] = array[0].Substring(0, array[0].IndexOf(">"));
										array2[1] = array[0].Substring(array[0].IndexOf(">") + 1, array[0].Length - array[0].IndexOf(">") - 1);
									}
									else
									{
										if (array[0].IndexOf("<") != -1)
										{
											array2[0] = array[0].Substring(0, array[0].IndexOf("<"));
											array2[1] = array[0].Substring(array[0].IndexOf("<") + 1, array[0].Length - array[0].IndexOf("<") - 1);
										}
									}
								}
							}
						}
					}
					if (array.Length == 2)
					{
						if (array[1].IndexOf(">=") != -1)
						{
							a = ">=";
							array3[0] = array[1].Substring(0, array[1].IndexOf(">="));
							array3[1] = array[1].Substring(array[1].IndexOf(">=") + 2, array[1].Length - array[1].IndexOf(">=") - 2);
						}
						else
						{
							if (array[1].IndexOf("<=") != -1)
							{
								a = "<=";
								array3[0] = array[1].Substring(0, array[1].IndexOf("<="));
								array3[1] = array[1].Substring(array[1].IndexOf("<=") + 2, array[1].Length - array[1].IndexOf("<=") - 2);
							}
							else
							{
								if (array[1].IndexOf("<>") != -1)
								{
									a = "<>";
									array3[0] = array[1].Substring(0, array[1].IndexOf("<>"));
									array3[1] = array[1].Substring(array[1].IndexOf("<>") + 2, array[1].Length - array[1].IndexOf("<>") - 2);
								}
								else
								{
									if (array[1].IndexOf("=") != -1)
									{
										a = "=";
										array3[0] = array[1].Substring(0, array[1].IndexOf("="));
										array3[1] = array[1].Substring(array[1].IndexOf("=") + 1, array[1].Length - array[1].IndexOf("=") - 1);
									}
									else
									{
										if (array[1].IndexOf(">") != -1)
										{
											a = ">";
											array3[0] = array[1].Substring(0, array[1].IndexOf(">"));
											array3[1] = array[1].Substring(array[1].IndexOf(">") + 1, array[1].Length - array[1].IndexOf(">") - 1);
										}
										else
										{
											if (array[1].IndexOf("<") != -1)
											{
												a = "<";
												array3[0] = array[1].Substring(0, array[1].IndexOf("<"));
												array3[1] = array[1].Substring(array[1].IndexOf("<") + 1, array[1].Length - array[1].IndexOf("<") - 1);
											}
										}
									}
								}
							}
						}
						foreach (XmlNode xmlNode in childNodes)
						{
							if (xmlNode.Name == array2[0])
							{
								if (a == "=")
								{
									if (xmlNode.InnerText == array3[1])
									{
										xmlNode.InnerText = array2[0];
									}
								}
								else
								{
									if (a == ">")
									{
										if (int.Parse(xmlNode.InnerText) > int.Parse(array3[1]))
										{
											xmlNode.InnerText = array2[0];
										}
									}
									else
									{
										if (a == "<")
										{
											if (int.Parse(xmlNode.InnerText) < int.Parse(array3[1]))
											{
												xmlNode.InnerText = array2[0];
											}
										}
										else
										{
											if (a == ">=")
											{
												if (int.Parse(xmlNode.InnerText) >= int.Parse(array3[1]))
												{
													xmlNode.InnerText = array2[0];
												}
											}
											else
											{
												if (a == "<=")
												{
													if (int.Parse(xmlNode.InnerText) <= int.Parse(array3[1]))
													{
														xmlNode.InnerText = array2[0];
													}
												}
												else
												{
													if (a == "<>")
													{
														if (xmlNode.InnerText != array3[1])
														{
															xmlNode.InnerText = array2[0];
														}
													}
												}
											}
										}
									}
								}
							}
							XmlElement xmlElement = (XmlElement)xmlNode;
							foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
							{
								string[] array4 = xmlNode2.Name.Split(new char[]
								{
									' '
								});
								if (array4[0] == array3[0])
								{
									if (a == "=")
									{
										if (xmlNode2.InnerText == array3[1])
										{
											foreach (XmlNode xmlNode3 in xmlElement)
											{
												string[] array5 = xmlNode3.Name.Split(new char[]
												{
													' '
												});
												if (array5[0] == array2[0])
												{
													xmlNode3.InnerText = array2[1];
												}
											}
										}
									}
									else
									{
										if (a == ">")
										{
											if (int.Parse(xmlNode2.InnerText) > int.Parse(array3[1]))
											{
												foreach (XmlNode xmlNode3 in xmlElement)
												{
													string[] array5 = xmlNode3.Name.Split(new char[]
													{
														' '
													});
													if (array5[0] == array2[0])
													{
														xmlNode3.InnerText = array2[1];
													}
												}
											}
										}
										else
										{
											if (a == "<")
											{
												if (int.Parse(xmlNode2.InnerText) < int.Parse(array3[1]))
												{
													foreach (XmlNode xmlNode3 in xmlElement)
													{
														string[] array5 = xmlNode3.Name.Split(new char[]
														{
															' '
														});
														if (array5[0] == array2[0])
														{
															xmlNode3.InnerText = array2[1];
														}
													}
												}
											}
											else
											{
												if (a == ">=")
												{
													if (int.Parse(xmlNode2.InnerText) >= int.Parse(array3[1]))
													{
														foreach (XmlNode xmlNode3 in xmlElement)
														{
															string[] array5 = xmlNode3.Name.Split(new char[]
															{
																' '
															});
															if (array5[0] == array2[0])
															{
																xmlNode3.InnerText = array2[1];
															}
														}
													}
												}
												else
												{
													if (a == "<=")
													{
														if (int.Parse(xmlNode2.InnerText) <= int.Parse(array3[1]))
														{
															foreach (XmlNode xmlNode3 in xmlElement)
															{
																string[] array5 = xmlNode3.Name.Split(new char[]
																{
																	' '
																});
																if (array5[0] == array2[0])
																{
																	xmlNode3.InnerText = array2[1];
																}
															}
														}
													}
													else
													{
														if (a == "<>")
														{
															if (xmlNode2.InnerText != array3[1])
															{
																foreach (XmlNode xmlNode3 in xmlElement)
																{
																	string[] array5 = xmlNode3.Name.Split(new char[]
																	{
																		' '
																	});
																	if (array5[0] == array2[0])
																	{
																		xmlNode3.InnerText = array2[1];
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					else
					{
						if (array.Length == 1)
						{
							foreach (XmlNode xmlNode in childNodes)
							{
								if (xmlNode.Name == array2[0])
								{
									xmlNode.InnerText = array2[1];
								}
								XmlElement xmlElement = (XmlElement)xmlNode;
								foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
								{
									string[] array4 = xmlNode2.Name.Split(new char[]
									{
										' '
									});
									if (array4[0] == array2[0])
									{
										xmlNode2.InnerText = array2[1];
									}
								}
							}
						}
					}
					this.xdoc.Save(filename);
					result = "S";
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
		public string SelectElement(string database, string tablename, string nodename)
		{
			string result;
			try
			{
				string filename = string.Concat(new string[]
				{
					"./",
					database,
					"/",
					tablename,
					".xml"
				});
				string text = "";
				if (this.RetrieveTableName(database, tablename))
				{
					this.xdoc = new XmlDocument();
					this.xdoc.Load(filename);
					XmlNodeList childNodes = this.xdoc.SelectSingleNode("_" + tablename).ChildNodes;
					string[] array = nodename.Split(new char[]
					{
						'|'
					});
					string[] array2 = new string[2];
					StringBuilder stringBuilder = new StringBuilder(text);
					StringWriter stringWriter = new StringWriter(stringBuilder);
					string outerXml = this.xdoc.DocumentType.OuterXml;
					int[] array3 = new int[2];
					int num = 0;
					int num2 = 0;
					string text2 = outerXml;
					for (int i = 0; i < text2.Length; i++)
					{
						char c = text2[i];
						num2++;
						if (c == '>')
						{
							array3[num] = num2;
							num++;
						}
						if (num == 2)
						{
							break;
						}
					}
					string text3 = outerXml.Substring(array3[0], array3[1] - array3[0]);
					num2 = (num = 0);
					text2 = text3;
					for (int i = 0; i < text2.Length; i++)
					{
						char c = text2[i];
						num2++;
						if (c == '(' || c == ')')
						{
							array3[num] = num2;
							num++;
						}
						if (num == 2)
						{
							break;
						}
					}
					text3 = text3.Substring(array3[0], array3[1] - array3[0] - 1).Replace("+", "").Replace(" ", "");
					string[] array4 = null;
					string[] array5;
					if (array[0] == "*")
					{
						array4 = text3.Split(new char[]
						{
							','
						});
						array5 = new string[array4.Length];
						for (int j = 0; j < array4.Length; j++)
						{
							array5[j] = array4[j];
						}
					}
					else
					{
						if (array[0].Split(new char[]
						{
							','
						}).Length > 1)
						{
							array4 = array[0].Split(new char[]
							{
								','
							});
							array5 = new string[array4.Length];
							for (int j = 0; j < array4.Length; j++)
							{
								array5[j] = array4[j];
							}
						}
						else
						{
							array5 = new string[]
							{
								array[0]
							};
							array4 = new string[1];
						}
					}
					for (int j = 0; j < array4.Length; j++)
					{
						if (array.Length == 1)
						{
							stringWriter.WriteLine(array5[j]);
							foreach (XmlNode xmlNode in childNodes)
							{
								if (xmlNode.Name == array5[j])
								{
									stringWriter.WriteLine(xmlNode.InnerXml);
								}
								XmlElement xmlElement = (XmlElement)xmlNode;
								foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
								{
									string[] array6 = xmlNode2.Name.Split(new char[]
									{
										' '
									});
									if (array6[0] == array5[j])
									{
										stringWriter.WriteLine(xmlNode2.InnerXml);
									}
								}
							}
						}
						else
						{
							if (array.Length == 2)
							{
								string a = "";
								if (array[1].IndexOf(">=") != -1)
								{
									a = ">=";
									array2[0] = array[1].Substring(0, array[1].IndexOf(">="));
									array2[1] = array[1].Substring(array[1].IndexOf(">=") + 2, array[1].Length - array[1].IndexOf(">=") - 2);
								}
								else
								{
									if (array[1].IndexOf("<=") != -1)
									{
										a = "<=";
										array2[0] = array[1].Substring(0, array[1].IndexOf("<="));
										array2[1] = array[1].Substring(array[1].IndexOf("<=") + 2, array[1].Length - array[1].IndexOf("<=") - 2);
									}
									else
									{
										if (array[1].IndexOf("<>") != -1)
										{
											a = "<>";
											array2[0] = array[1].Substring(0, array[1].IndexOf("<>"));
											array2[1] = array[1].Substring(array[1].IndexOf("<>") + 2, array[1].Length - array[1].IndexOf("<>") - 2);
										}
										else
										{
											if (array[1].IndexOf("=") != -1)
											{
												a = "=";
												array2[0] = array[1].Substring(0, array[1].IndexOf("="));
												array2[1] = array[1].Substring(array[1].IndexOf("=") + 1, array[1].Length - array[1].IndexOf("=") - 1);
											}
											else
											{
												if (array[1].IndexOf(">") != -1)
												{
													a = ">";
													array2[0] = array[1].Substring(0, array[1].IndexOf(">"));
													array2[1] = array[1].Substring(array[1].IndexOf(">") + 1, array[1].Length - array[1].IndexOf(">") - 1);
												}
												else
												{
													if (array[1].IndexOf("<") != -1)
													{
														a = "<";
														array2[0] = array[1].Substring(0, array[1].IndexOf("<"));
														array2[1] = array[1].Substring(array[1].IndexOf("<") + 1, array[1].Length - array[1].IndexOf("<") - 1);
													}
												}
											}
										}
									}
								}
								stringWriter.WriteLine(array5[j]);
								foreach (XmlNode xmlNode in childNodes)
								{
									if (xmlNode.Name == array2[0])
									{
										if (a == "=")
										{
											if (xmlNode.InnerText == array2[1])
											{
												stringWriter.WriteLine(xmlNode.InnerText);
											}
										}
										else
										{
											if (a == ">")
											{
												if (int.Parse(xmlNode.InnerText) > int.Parse(array2[1]))
												{
													stringWriter.WriteLine(xmlNode.InnerText);
												}
											}
											else
											{
												if (a == "<")
												{
													if (int.Parse(xmlNode.InnerText) < int.Parse(array2[1]))
													{
														stringWriter.WriteLine(xmlNode.InnerText);
													}
												}
												else
												{
													if (a == ">=")
													{
														if (int.Parse(xmlNode.InnerText) >= int.Parse(array2[1]))
														{
															stringWriter.WriteLine(xmlNode.InnerText);
														}
													}
													else
													{
														if (a == "<=")
														{
															if (int.Parse(xmlNode.InnerText) <= int.Parse(array2[1]))
															{
																stringWriter.WriteLine(xmlNode.InnerText);
															}
														}
														else
														{
															if (a == "<>")
															{
																if (xmlNode.InnerText != array2[1])
																{
																	stringWriter.WriteLine(xmlNode.InnerText);
																}
															}
														}
													}
												}
											}
										}
									}
									XmlElement xmlElement = (XmlElement)xmlNode;
									foreach (XmlNode xmlNode2 in xmlElement.ChildNodes)
									{
										string[] array6 = xmlNode2.Name.Split(new char[]
										{
											' '
										});
										if (array[1] == " ")
										{
											array[1] = "";
										}
										if (array6[0] == array2[0])
										{
											if (a == "=")
											{
												if (xmlNode2.InnerText == array2[1])
												{
													foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
													{
														string[] array7 = xmlNode3.Name.Split(new char[]
														{
															' '
														});
														if (array7[0] == array5[j])
														{
															stringWriter.WriteLine(xmlNode3.InnerText);
														}
													}
												}
											}
											else
											{
												if (a == ">")
												{
													if (int.Parse(xmlNode2.InnerText) > int.Parse(array2[1]))
													{
														foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
														{
															string[] array7 = xmlNode3.Name.Split(new char[]
															{
																' '
															});
															if (array7[0] == array5[j])
															{
																stringWriter.WriteLine(xmlNode3.InnerText);
															}
														}
													}
												}
												else
												{
													if (a == "<")
													{
														if (int.Parse(xmlNode2.InnerText) < int.Parse(array2[1]))
														{
															foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
															{
																string[] array7 = xmlNode3.Name.Split(new char[]
																{
																	' '
																});
																if (array7[0] == array5[j])
																{
																	stringWriter.WriteLine(xmlNode3.InnerText);
																}
															}
														}
													}
													else
													{
														if (a == ">=")
														{
															if (int.Parse(xmlNode2.InnerText) >= int.Parse(array2[1]))
															{
																foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
																{
																	string[] array7 = xmlNode3.Name.Split(new char[]
																	{
																		' '
																	});
																	if (array7[0] == array5[j])
																	{
																		stringWriter.WriteLine(xmlNode3.InnerText);
																	}
																}
															}
														}
														else
														{
															if (a == "<=")
															{
																if (int.Parse(xmlNode2.InnerText) <= int.Parse(array2[1]))
																{
																	foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
																	{
																		string[] array7 = xmlNode3.Name.Split(new char[]
																		{
																			' '
																		});
																		if (array7[0] == array5[j])
																		{
																			stringWriter.WriteLine(xmlNode3.InnerText);
																		}
																	}
																}
															}
															else
															{
																if (a == "<>")
																{
																	if (xmlNode2.InnerText != array2[1])
																	{
																		foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
																		{
																			string[] array7 = xmlNode3.Name.Split(new char[]
																			{
																				' '
																			});
																			if (array7[0] == array5[j])
																			{
																				stringWriter.WriteLine(xmlNode3.InnerText);
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					text = stringBuilder.ToString();
					result = "Succeeded" + text + array4.Length.ToString();
				}
				else
				{
					result = "F";
				}
			}
			catch
			{
				result = "E";
			}
			return result;
		}
	}
}
