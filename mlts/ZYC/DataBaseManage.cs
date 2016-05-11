using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace ZYC
{
	public sealed class DataBaseManage
	{
		private bool databaseconnection_s = false;
		private bool issuccess_s = false;
		private bool databaseconnection_a = false;
		private bool issuccess_a = false;
		private string connectionstring = "";
		private string commandstring = "";
		private string connectionerror = "";
		private string selectcommand = "";
		private string insertcommand = "";
		private string updatecommand = "";
		private string deletecommand = "";
		private SqlConnection sqlcon = null;
		private SqlCommand sqlcom = null;
		private SqlDataAdapter sqladapter = null;
		private SqlDataReader sqlreader = null;
		private DataTable sqldt = null;
		private DataSet sqlds = null;
		private SqlCommandBuilder sqlbuilder = null;
		private OleDbConnection olecon = null;
		private OleDbCommand olecom = null;
		private OleDbDataAdapter oleadapter = null;
		private OleDbDataReader olereader = null;
		private DataTable oledt = null;
		private DataSet oleds = null;
		private OleDbCommandBuilder olebuilder = null;
		public string SQL2000
		{
			get
			{
				return "Provider=sqloledb;Data Source=MySqlServer;Initial Catalog=pubs;User Id=*****;Password=*****;";
			}
		}
		public string SQL2005
		{
			get
			{
				return "Data Source=USER-PC;Initial Catalog=Sample;Integrated Security=True";
			}
		}
		public string Access2003
		{
			get
			{
				return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=d:\\Northwind.mdb;Jet OLEDB:System Database=d:\\NorthwindSystem.mdw;User ID=*****;Password=*****;";
			}
		}
		public string Access2007
		{
			get
			{
				return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=d:\\Northwind.mdb;Jet OLEDB:System Database=d:\\NorthwindSystem.mdw;User ID=*****;Password=*****;";
			}
		}
		public string Oracle
		{
			get
			{
				return "Data Source=Oracle9i;User ID=*****;Password=*****;";
			}
		}
		public bool DatabaseConncetionSQL
		{
			get
			{
				return this.databaseconnection_s;
			}
		}
		public bool IsSuccessSQL
		{
			get
			{
				return this.issuccess_s;
			}
		}
		public bool DatabaseConncetionAccess
		{
			get
			{
				return this.databaseconnection_a;
			}
		}
		public bool IsSuccessAccess
		{
			get
			{
				return this.issuccess_a;
			}
		}
		public string ConnectionString
		{
			get
			{
				return this.connectionstring;
			}
			set
			{
				this.connectionstring = value;
			}
		}
		public string CommandString
		{
			get
			{
				return this.commandstring;
			}
			set
			{
				this.commandstring = value;
			}
		}
		public string ConnectionError
		{
			get
			{
				return this.connectionerror;
			}
		}
		public string SelectCommand
		{
			get
			{
				return this.selectcommand;
			}
			set
			{
				this.selectcommand = value;
			}
		}
		public string InsertCommand
		{
			get
			{
				return this.insertcommand;
			}
			set
			{
				this.insertcommand = value;
			}
		}
		public string UpdateCommand
		{
			get
			{
				return this.updatecommand;
			}
			set
			{
				this.updatecommand = value;
			}
		}
		public string DeleteCommand
		{
			get
			{
				return this.deletecommand;
			}
			set
			{
				this.deletecommand = value;
			}
		}
		public bool CheckDatabase(string databasename)
		{
			this.SqlConnect();
			string cmdText = "select * from master..sysdatabases where name='" + databasename + "'";
			this.sqlcom = new SqlCommand(cmdText, this.sqlcon);
			return this.sqlcom.ExecuteReader().HasRows;
		}
		public bool CheckDatabase(string databasename, string constring)
		{
			this.SqlConnect(constring);
			string cmdText = "select * from master..sysdatabases where name='" + databasename + "'";
			this.sqlcom = new SqlCommand(cmdText, this.sqlcon);
			return this.sqlcom.ExecuteReader().HasRows;
		}
		public void SqlConnect()
		{
			if (this.sqlcon == null)
			{
				this.sqlcon = new SqlConnection(this.connectionstring);
				try
				{
					this.sqlcon.Open();
					this.issuccess_s = true;
					this.databaseconnection_s = true;
				}
				catch (Exception ex)
				{
					this.issuccess_s = false;
					this.connectionerror = ex.ToString();
				}
			}
			else
			{
				if (this.sqlcon.State != ConnectionState.Open)
				{
					this.sqlcon.Open();
				}
			}
		}
		public void SqlConnect(string constring)
		{
			if (this.sqlcon == null)
			{
				this.connectionstring = constring;
				this.sqlcon = new SqlConnection(this.connectionstring);
				try
				{
					this.sqlcon.Open();
					this.issuccess_s = true;
					this.databaseconnection_s = true;
				}
				catch (Exception ex)
				{
					this.issuccess_s = false;
					this.connectionerror = ex.ToString();
				}
			}
			else
			{
				if (this.sqlcon.State != ConnectionState.Open)
				{
					this.sqlcon.Open();
				}
			}
		}
		public void SqlExecute(SqlCommand cmd)
		{
			cmd.ExecuteNonQuery();
		}
		public void SqlExecute(string cmd)
		{
			this.SqlConnect();
			this.sqlcom = new SqlCommand(cmd, this.sqlcon);
			this.sqlcom.ExecuteNonQuery();
		}
		public object SqlExecuteScalar(string cmd)
		{
			this.SqlConnect();
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			return this.sqlcom.ExecuteScalar();
		}
		public object SqlExecuteScalar(string con, string cmd)
		{
			this.SqlConnect(con);
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			return this.sqlcom.ExecuteScalar();
		}
		public SqlCommand GetSqlCommandProcedure(string cmd)
		{
			this.SqlConnect();
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = CommandType.StoredProcedure;
			return this.sqlcom;
		}
		public SqlCommand GetSqlCommandProcedure(string con, string cmd)
		{
			this.SqlConnect(con);
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = CommandType.StoredProcedure;
			return this.sqlcom;
		}
		public SqlCommand GetSqlCommand(string cmd)
		{
			this.SqlConnect();
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = CommandType.Text;
			return this.sqlcom;
		}
		public SqlCommand GetSqlCommand(string constring, string cmd)
		{
			this.SqlConnect(constring);
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = CommandType.Text;
			return this.sqlcom;
		}
		public SqlCommand GetSqlCommand(string cmd, CommandType type)
		{
			this.SqlConnect();
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = type;
			return this.sqlcom;
		}
		public SqlCommand GetSqlCommand(string con, string cmd, CommandType type)
		{
			this.SqlConnect(con);
			this.commandstring = cmd;
			this.sqlcom = new SqlCommand(this.commandstring, this.sqlcon);
			this.sqlcom.CommandType = type;
			return this.sqlcom;
		}
		public DataTable DataTableConnectSQL(string cmd)
		{
			this.sqldt = new DataTable();
			this.SqlConnect();
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqldt);
			return this.sqldt;
		}
		public DataTable DataTableConnectSQL(string constring, string cmd)
		{
			this.sqldt = new DataTable();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqldt);
			return this.sqldt;
		}
		public DataSet DataSetConnectSQL(string cmd)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(this.connectionstring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds);
			for (int i = 0; i < this.sqlds.Tables.Count; i++)
			{
				this.sqlds.Tables[i].TableName = "table" + (i + 1).ToString();
			}
			return this.sqlds;
		}
		public DataSet DataSetConnectSQL(string cmd, params string[] tablenames)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(this.connectionstring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds);
			for (int i = 0; i < this.sqlds.Tables.Count; i++)
			{
				this.sqlds.Tables[i].TableName = tablenames[i];
			}
			return this.sqlds;
		}
		public DataSet DataSetConnectSQL(string cmd, string tablename)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(this.connectionstring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds, tablename);
			return this.sqlds;
		}
		public DataSet DataSetConnectSQL(string constring, string cmd, string tablename)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds, tablename);
			return this.sqlds;
		}
		public DataSet DataSetFromXMLSql(string xmlpath)
		{
			this.sqlds = new DataSet();
			this.sqlds.ReadXml(xmlpath);
			return this.sqlds;
		}
		public DataSet DataSetFromXMLSql(string xmlpath, string xsdpath)
		{
			this.sqlds = new DataSet();
			this.sqlds.ReadXml(xmlpath);
			this.sqlds.ReadXmlSchema(xsdpath);
			return this.sqlds;
		}
		public DataRow[] DataTableSelect(DataTable dt, string where)
		{
			return dt.Select(where);
		}
		public void SqlDataAdapterUpdate(DataRow[] datarow)
		{
			this.sqladapter.Update(datarow);
		}
		public void SqlDataAdapterUpdate(DataSet dataset)
		{
			this.sqladapter.Update(dataset);
		}
		public void SqlDataAdapterUpdate(DataTable datatable)
		{
			this.sqladapter.Update(datatable);
		}
		public void SqlDataAdapterUpdate(DataSet dataset, string table)
		{
			this.sqladapter.Update(dataset, table);
		}
		private void NewSbuilderSql()
		{
			this.sqlbuilder = new SqlCommandBuilder(this.sqladapter);
		}
		public void SqlDataAdapterAutoUpdate(DataRow[] datarow)
		{
			this.NewSbuilderSql();
			this.sqladapter.Update(datarow);
		}
		public void SqlDataAdapterAutoUpdate(DataSet dataset)
		{
			this.NewSbuilderSql();
			this.sqladapter.Update(dataset);
		}
		public void SqlDataAdapterAutoUpdate(DataTable datatable)
		{
			this.NewSbuilderSql();
			this.sqladapter.Update(datatable);
		}
		public void SqlDataAdapterAutoUpdate(DataSet dataset, string table)
		{
			this.NewSbuilderSql();
			this.sqladapter.Update(dataset, table);
		}
		public SqlDataReader GetDataReaderSql()
		{
			this.sqlcom = this.GetSqlCommand(this.connectionstring, this.commandstring);
			this.sqlreader = this.sqlcom.ExecuteReader();
			return this.sqlreader;
		}
		public SqlDataReader GetDataReaderSql(string cmd)
		{
			this.sqlcom = this.GetSqlCommand(this.connectionstring, cmd);
			this.sqlreader = this.sqlcom.ExecuteReader();
			return this.sqlreader;
		}
		public SqlDataReader GetDataReaderSql(string con, string cmd)
		{
			this.sqlcom = this.GetSqlCommand(con, cmd);
			this.sqlreader = this.sqlcom.ExecuteReader();
			return this.sqlreader;
		}
		public XmlReader GetXMLReader()
		{
			this.sqlcom = this.GetSqlCommand(this.connectionstring, this.commandstring);
			return this.sqlcom.ExecuteXmlReader();
		}
		public XmlReader GetXMLReaderSql(string cmd)
		{
			this.sqlcom = this.GetSqlCommand(this.connectionstring, cmd);
			return this.sqlcom.ExecuteXmlReader();
		}
		public XmlReader GetXMLReaderSql(string con, string cmd)
		{
			this.sqlcom = this.GetSqlCommand(con, cmd);
			return this.sqlcom.ExecuteXmlReader();
		}
		public byte[] InsertImageSQL(string path)
		{
			byte[] result;
			if (!File.Exists(path))
			{
				result = null;
			}
			else
			{
				FileStream fileStream = File.OpenRead(path);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, Convert.ToInt32(fileStream.Length));
				fileStream.Close();
				result = array;
			}
			return result;
		}
		public byte[] InsertImageSQL(Image image)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Jpeg);
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Position = 0L;
			memoryStream.Read(array, 0, Convert.ToInt32(memoryStream.Length));
			memoryStream.Close();
			return array;
		}
		public Image GetImageSQL(byte[] img)
		{
			MemoryStream stream = new MemoryStream(img);
			return Image.FromStream(stream);
		}
		public void Dispose()
		{
			if (this.sqlcon != null)
			{
				this.sqlcon.Close();
				this.sqlcon.Dispose();
				this.sqlcon = null;
			}
			if (this.sqlcom != null)
			{
				this.sqlcom.Dispose();
				this.sqlcom = null;
			}
			if (this.sqladapter != null)
			{
				this.sqladapter.Dispose();
				this.sqladapter = null;
			}
			if (this.sqlreader != null)
			{
				this.sqlreader.Dispose();
				this.sqlreader = null;
			}
			if (this.sqldt != null)
			{
				this.sqldt.Dispose();
				this.sqldt = null;
			}
			if (this.sqlds != null)
			{
				this.sqlds.Dispose();
				this.sqlds = null;
			}
			if (this.olecon != null)
			{
				this.olecon.Close();
				this.olecon.Dispose();
				this.olecon = null;
			}
			if (this.olecom != null)
			{
				this.olecom.Dispose();
				this.olecom = null;
			}
			if (this.oleadapter != null)
			{
				this.oleadapter.Dispose();
				this.oleadapter = null;
			}
			if (this.olereader != null)
			{
				this.olereader.Dispose();
				this.olereader = null;
			}
			if (this.oledt != null)
			{
				this.oledt.Dispose();
				this.oledt = null;
			}
			if (this.oleds != null)
			{
				this.oleds.Dispose();
				this.oleds = null;
			}
		}
		public void AccessConnect()
		{
			this.olecon = new OleDbConnection(this.connectionstring);
			try
			{
				this.olecon.Open();
				this.issuccess_s = true;
				this.databaseconnection_s = true;
			}
			catch (Exception ex)
			{
				this.issuccess_s = false;
				this.connectionerror = ex.ToString();
			}
		}
		public void AccessConnect(string constring)
		{
			this.connectionstring = constring;
			this.olecon = new OleDbConnection(this.connectionstring);
			try
			{
				this.olecon.Open();
				this.issuccess_s = true;
				this.databaseconnection_s = true;
			}
			catch (Exception ex)
			{
				this.issuccess_s = false;
				this.connectionerror = ex.ToString();
			}
		}
		public OleDbCommand AccessCommandQuery(string cmd)
		{
			if (this.olecon == null)
			{
				this.AccessConnect();
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			return this.olecom;
		}
		public OleDbCommand AccessCommandQuery(string con, string cmd)
		{
			if (this.olecon == null)
			{
				this.AccessConnect(con);
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			return this.olecom;
		}
		public OleDbCommand AccessCommandProcedure(string cmd)
		{
			if (this.olecon == null)
			{
				this.AccessConnect();
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			this.olecom.CommandType = CommandType.StoredProcedure;
			return this.olecom;
		}
		public OleDbCommand AccessCommandProcedure(string con, string cmd)
		{
			if (this.olecon == null)
			{
				this.AccessConnect(con);
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			this.olecom.CommandType = CommandType.StoredProcedure;
			return this.olecom;
		}
		public OleDbCommand AccessCommandExecute(string cmd, CommandType type)
		{
			if (this.olecon == null)
			{
				this.AccessConnect();
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			this.olecom.CommandType = type;
			return this.olecom;
		}
		public OleDbCommand AccessCommandExecute(string con, string cmd, CommandType type)
		{
			if (this.olecon == null)
			{
				this.AccessConnect(con);
			}
			this.commandstring = cmd;
			this.olecom = new OleDbCommand(this.commandstring, this.olecon);
			this.olecom.CommandType = type;
			return this.olecom;
		}
		public DataTable DataTableConnectACCESS(string constring, string cmd)
		{
			this.oledt = new DataTable();
			this.AccessConnect(constring);
			this.AccessCommandQuery(cmd);
			this.oleadapter = new OleDbDataAdapter(this.olecom);
			this.oleadapter.Fill(this.oledt);
			return this.oledt;
		}
		public DataSet DataSetConnectACCESS(string cmd)
		{
			this.oleds = new DataSet();
			this.AccessConnect(this.connectionstring);
			this.AccessCommandQuery(cmd);
			this.oleadapter = new OleDbDataAdapter(this.olecom);
			this.oleadapter.Fill(this.oleds);
			for (int i = 0; i < this.oleds.Tables.Count; i++)
			{
				this.oleds.Tables[i].TableName = "table" + (i + 1).ToString();
			}
			return this.oleds;
		}
		public DataSet DataSetConnectACCESS(string cmd, params string[] tablenames)
		{
			this.oleds = new DataSet();
			this.AccessConnect(this.connectionstring);
			this.AccessCommandQuery(cmd);
			this.oleadapter = new OleDbDataAdapter(this.olecom);
			this.oleadapter.Fill(this.oleds);
			for (int i = 0; i < this.oleds.Tables.Count; i++)
			{
				this.oleds.Tables[i].TableName = tablenames[i];
			}
			return this.oleds;
		}
		public DataSet DataSetConnectACCESS(string cmd, string tablename)
		{
			this.oleds = new DataSet();
			this.AccessConnect(this.connectionstring);
			this.AccessCommandQuery(cmd);
			this.oleadapter = new OleDbDataAdapter(this.olecom);
			this.oleadapter.Fill(this.oleds, tablename);
			return this.oleds;
		}
		public DataSet DataSetConnectACCESS(string constring, string cmd, string tablename)
		{
			this.oleds = new DataSet();
			this.AccessConnect(constring);
			this.AccessCommandQuery(cmd);
			this.oleadapter = new OleDbDataAdapter(this.olecom);
			this.oleadapter.Fill(this.oleds, tablename);
			return this.oleds;
		}
		public DataSet DataSetFromXMLAccess(string xmlpath)
		{
			this.oleds = new DataSet();
			this.oleds.ReadXml(xmlpath);
			return this.oleds;
		}
		public DataSet DataSetFromXMLAccess(string xmlpath, string xsdpath)
		{
			this.oleds = new DataSet();
			this.oleds.ReadXml(xmlpath);
			this.oleds.ReadXmlSchema(xsdpath);
			return this.oleds;
		}
		public void AccessDataAdapterUpdate(DataRow[] datarow)
		{
			this.oleadapter.Update(datarow);
		}
		public void AccessDataAdapterUpdate(DataSet dataset)
		{
			this.oleadapter.Update(dataset);
		}
		public void AccessDataAdapterUpdate(DataTable datatable)
		{
			this.oleadapter.Update(datatable);
		}
		public void AccessDataAdapterUpdate(DataSet dataset, string table)
		{
			this.oleadapter.Update(dataset, table);
		}
		private void NewSbuilderAccess()
		{
			this.olebuilder = new OleDbCommandBuilder(this.oleadapter);
		}
		public void AccessDataAdapterAutoUpdate(DataRow[] datarow)
		{
			this.NewSbuilderAccess();
			this.oleadapter.Update(datarow);
		}
		public void AccessDataAdapterAutoUpdate(DataSet dataset)
		{
			this.NewSbuilderAccess();
			this.oleadapter.Update(dataset);
		}
		public void AccessDataAdapterAutoUpdate(DataTable datatable)
		{
			this.NewSbuilderAccess();
			this.oleadapter.Update(datatable);
		}
		public void AccessDataAdapterAutoUpdate(DataSet dataset, string table)
		{
			this.NewSbuilderAccess();
			this.oleadapter.Update(dataset, table);
		}
		public OleDbDataReader GetDataReaderAccess()
		{
			this.olecom = this.AccessCommandQuery(this.connectionstring, this.commandstring);
			this.olereader = this.olecom.ExecuteReader();
			return this.olereader;
		}
		public OleDbDataReader GetDataReaderAccess(string cmd)
		{
			this.olecom = this.AccessCommandQuery(this.connectionstring, cmd);
			this.olereader = this.olecom.ExecuteReader();
			return this.olereader;
		}
		public OleDbDataReader GetDataReaderAccess(string con, string cmd)
		{
			this.olecom = this.AccessCommandQuery(con, cmd);
			this.olereader = this.olecom.ExecuteReader();
			return this.olereader;
		}
		public void DataTableConnectSQL(string constring, string cmd, DataGrid datagrid)
		{
			this.sqldt = new DataTable();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqldt);
			datagrid.DataSource = this.sqldt;
		}
		public void DataTableConnectSQL(string constring, string cmd, DataGridView datagridview)
		{
			this.sqldt = new DataTable();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqldt);
			datagridview.DataSource = this.sqldt;
		}
		public void DataSetConnectSQL(string constring, string cmd, string tablename, DataGrid datagrid)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds, tablename);
			datagrid.DataSource = this.sqlds.Tables[0];
		}
		public void DataSetConnectSQL(string constring, string cmd, string tablename, DataGridView datagridview)
		{
			this.sqlds = new DataSet();
			this.SqlConnect(constring);
			this.GetSqlCommand(cmd);
			this.sqladapter = new SqlDataAdapter(this.sqlcom);
			this.sqladapter.Fill(this.sqlds, tablename);
			datagridview.DataSource = this.sqlds.Tables[0];
		}
	}
}
