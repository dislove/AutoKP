using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
namespace ZYC
{
	public class DataGridViewPrint
	{
		private DataGridView dataGridView1;
		private PrintDocument printDocument;
		private PageSetupDialog pageSetupDialog;
		private PrintPreviewDialog printPreviewDialog;
		private string title = "";
		private int currentPageIndex = 0;
		private int rowCount = 0;
		private int pageCount = 0;
		private int titleSize = 20;
		private bool isCustomHeader = false;
		private Brush alertBrush = new SolidBrush(Color.Red);
		private string[] header = null;
		private string[] uplineHeader = null;
		private int[] upLineHeaderIndex = null;
		public bool isEveryPagePrintTitle = true;
		public int headerHeight = 30;
		public int topMargin = 30;
		public int cellTopMargin = 6;
		public int cellLeftMargin = 4;
		public char splitChar = '#';
		public string falseStr = "×";
		public string trueStr = "√";
		public int pageRowCount = 30;
		public int rowGap = 28;
		public int colGap = 5;
		public int leftMargin = 45;
		public Font titleFont = new Font("黑体", 24f, FontStyle.Bold);
		public Font font = new Font("宋体", 10f);
		public Font headerFont = new Font("黑体", 11f, FontStyle.Bold);
		public Font footerFont = new Font("Arial", 8f);
		public Font upLineFont = new Font("Arial", 9f, FontStyle.Bold);
		public Font underLineFont = new Font("Arial", 8f);
		public Brush brush = new SolidBrush(Color.Black);
		public bool isAutoPageRowCount = true;
		public int buttomMargin = 80;
		public bool needPrintPageIndex = true;
		public bool setTongji = false;
		private string sTongJi01 = "";
		private string sTongJi02 = "";
		private string sTongJi03 = "";
		private bool isTongji = false;
		private string time01;
		private string time02;
		private Font tongJiFont = new Font("宋体", 14f);
		private Font dateFont = new Font("宋体", 12f, FontStyle.Bold);
		public DataGridViewPrint(DataGridView dGView, string title, string times01, string times02, string tj01, string tj02, string tj03, bool tj)
		{
			this.title = title;
			this.sTongJi01 = tj01;
			this.sTongJi02 = tj02;
			this.sTongJi03 = tj03;
			this.time01 = times01;
			this.time02 = times02;
			this.setTongji = tj;
			this.dataGridView1 = dGView;
			this.printDocument = new PrintDocument();
			this.printDocument.PrintPage += new PrintPageEventHandler(this.printDocument_PrintPage);
		}
		public bool setTowLineHeader(string[] upLineHeader, int[] upLineHeaderIndex)
		{
			this.uplineHeader = upLineHeader;
			this.upLineHeaderIndex = upLineHeaderIndex;
			this.isCustomHeader = true;
			return true;
		}
		public bool setHeader(string[] header)
		{
			this.header = header;
			return true;
		}
		private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			int width = e.PageBounds.Width;
			int height = e.PageBounds.Height;
			this.leftMargin = 40;
			if (this.isAutoPageRowCount)
			{
				this.pageRowCount = (height - this.topMargin - this.titleSize - 25 - this.headerFont.Height - this.headerHeight - this.buttomMargin) / this.rowGap;
			}
			this.pageCount = this.rowCount / this.pageRowCount;
			if (this.rowCount % this.pageRowCount > 0)
			{
				this.pageCount++;
			}
			if (this.setTongji && this.pageCount == 1)
			{
				this.pageRowCount = (height - this.topMargin - this.titleSize - 25 - this.headerFont.Height - this.headerHeight - this.buttomMargin - 25) / this.rowGap;
				this.pageCount = this.rowCount / this.pageRowCount;
				if (this.rowCount % this.pageRowCount > 0)
				{
					this.pageCount++;
				}
			}
			string text = this.time01 + " — " + this.time02;
			int num = (int)(((float)width - e.Graphics.MeasureString(this.title, this.titleFont).Width) / 2f);
			int num2 = (int)(((float)width - e.Graphics.MeasureString(text, this.dateFont).Width) / 2f);
			int num3 = this.topMargin;
			string text2 = "";
			int num4 = this.currentPageIndex * this.pageRowCount;
			int num5 = (num4 + this.pageRowCount < this.rowCount) ? (num4 + this.pageRowCount) : this.rowCount;
			int num6 = num5 - num4;
			if (this.currentPageIndex == 0 || this.isEveryPagePrintTitle)
			{
				e.Graphics.DrawString(this.title, this.titleFont, this.brush, (float)num, (float)num3);
				e.Graphics.DrawString(text, this.dateFont, this.brush, (float)num2, (float)(num3 + 40));
				num3 += this.titleSize + 20;
			}
			try
			{
				int columnCount = this.dataGridView1.ColumnCount;
				num3 += this.rowGap;
				int num7 = this.leftMargin;
				this.DrawLine(new Point(num7, num3), new Point(num7, num3 + num6 * this.rowGap + this.headerHeight), e.Graphics);
				int num8 = -1;
				int num9 = 0;
				int num10 = -1;
				for (int i = 0; i < columnCount; i++)
				{
					int width2 = this.dataGridView1.Columns[i].Width;
					if (width2 > 0)
					{
						num10++;
						if (this.header == null || this.header[num10] == "")
						{
							text2 = this.dataGridView1.Columns[i].Name;
						}
						else
						{
							text2 = this.header[num10];
						}
						if (this.isCustomHeader)
						{
							if (this.upLineHeaderIndex[num10] != num8)
							{
								if (num9 > 0 && num8 > -1)
								{
									string text3 = this.uplineHeader[num8];
									int num11 = (int)(((float)num9 - e.Graphics.MeasureString(text3, this.upLineFont).Width) / 2f);
									if (num11 < 0)
									{
										num11 = 0;
									}
									e.Graphics.DrawString(text3, this.upLineFont, this.brush, (float)(num7 - num9 + num11), (float)(num3 + this.cellTopMargin / 2));
									this.DrawLine(new Point(num7 - num9, num3 + this.headerHeight / 2), new Point(num7, num3 + this.headerHeight / 2), e.Graphics);
									this.DrawLine(new Point(num7, num3), new Point(num7, num3 + this.headerHeight / 2), e.Graphics);
								}
								num8 = this.upLineHeaderIndex[num10];
								num9 = width2 + this.colGap;
							}
							else
							{
								num9 += width2 + this.colGap;
							}
						}
						int num12 = text2.IndexOf(this.splitChar);
						if (this.upLineHeaderIndex != null && this.upLineHeaderIndex[num10] > -1)
						{
							if (num12 > 0)
							{
								string s = text2.Substring(0, num12);
								string text4 = text2.Substring(num12 + 1, text2.Length - num12 - 1);
								int num13 = (int)((float)(width2 + this.colGap) - e.Graphics.MeasureString(text4, this.upLineFont).Width);
								int num14 = (int)((float)(this.headerHeight / 2) - e.Graphics.MeasureString("a", this.underLineFont).Height);
								num13 = 6;
								num14 = 10;
								e.Graphics.DrawString(text4, this.underLineFont, this.brush, (float)(num7 + num13 - 4), (float)(num3 + this.headerHeight / 2));
								e.Graphics.DrawString(s, this.underLineFont, this.brush, (float)(num7 + 2), (float)(num3 + this.headerHeight / 2 + this.cellTopMargin / 2 + num14 - 2));
								this.DrawLine(new Point(num7, num3 + this.headerHeight / 2), new Point(num7 + width2 + this.colGap, num3 + this.headerHeight), e.Graphics);
								num7 += width2 + this.colGap;
								this.DrawLine(new Point(num7, num3 + this.headerHeight / 2), new Point(num7, num3 + num6 * this.rowGap + this.headerHeight), e.Graphics);
							}
							else
							{
								e.Graphics.DrawString(text2, this.headerFont, this.brush, (float)num7, (float)(num3 + this.headerHeight / 2 + this.cellTopMargin / 2));
								num7 += width2 + this.colGap;
								this.DrawLine(new Point(num7, num3 + this.headerHeight / 2), new Point(num7, num3 + num6 * this.rowGap + this.headerHeight), e.Graphics);
							}
						}
						else
						{
							if (num12 > 0)
							{
								string s = text2.Substring(0, num12);
								string text4 = text2.Substring(num12 + 1, text2.Length - num12 - 1);
								int num13 = (int)((float)(width2 + this.colGap) - e.Graphics.MeasureString(text4, this.upLineFont).Width);
								int num14 = (int)((float)this.headerHeight - e.Graphics.MeasureString("a", this.underLineFont).Height);
								e.Graphics.DrawString(text4, this.headerFont, this.brush, (float)(num7 + num13 - 4), (float)(num3 + 2));
								e.Graphics.DrawString(s, this.headerFont, this.brush, (float)(num7 + 2), (float)(num3 + num14 - 4));
								this.DrawLine(new Point(num7, num3), new Point(num7 + width2 + this.colGap, num3 + this.headerHeight), e.Graphics);
								num7 += width2 + this.colGap;
								this.DrawLine(new Point(num7, num3), new Point(num7, num3 + num6 * this.rowGap + this.headerHeight), e.Graphics);
							}
							else
							{
								e.Graphics.DrawString(text2, this.headerFont, this.brush, (float)num7, (float)(num3 + this.cellTopMargin));
								num7 += width2 + this.colGap;
								this.DrawLine(new Point(num7, num3), new Point(num7, num3 + num6 * this.rowGap + this.headerHeight), e.Graphics);
							}
						}
					}
				}
				if (this.isCustomHeader)
				{
					if (num9 > 0 && num8 > -1)
					{
						string text3 = this.uplineHeader[num8];
						int num11 = (int)(((float)num9 - e.Graphics.MeasureString(text3, this.upLineFont).Width) / 2f);
						if (num11 < 0)
						{
							num11 = 0;
						}
						e.Graphics.DrawString(text3, this.upLineFont, this.brush, (float)(num7 - num9 + num11), (float)(num3 + this.cellTopMargin / 2));
						this.DrawLine(new Point(num7 - num9, num3 + this.headerHeight / 2), new Point(num7, num3 + this.headerHeight / 2), e.Graphics);
						this.DrawLine(new Point(num7, num3), new Point(num7, num3 + this.headerHeight / 2), e.Graphics);
					}
				}
				int x = num7;
				this.DrawLine(new Point(this.leftMargin, num3), new Point(x, num3), e.Graphics);
				num3 += this.headerHeight;
				for (int j = num4; j < num5; j++)
				{
					num7 = this.leftMargin;
					for (int i = 0; i < columnCount; i++)
					{
						if (this.dataGridView1.Columns[i].Width > 0)
						{
							text2 = this.dataGridView1.Rows[j].Cells[i].Value.ToString();
							if (text2 == "False")
							{
								text2 = this.falseStr;
							}
							if (text2 == "True")
							{
								text2 = this.trueStr;
							}
							e.Graphics.DrawString(text2, this.font, this.brush, (float)(num7 + this.cellLeftMargin), (float)(num3 + this.cellTopMargin));
							num7 += this.dataGridView1.Columns[i].Width + this.colGap;
							num3 += this.rowGap * (text2.Split(new char[]
							{
								'\r',
								'\n'
							}).Length - 1);
						}
					}
					this.DrawLine(new Point(this.leftMargin, num3), new Point(x, num3), e.Graphics);
					num3 += this.rowGap;
				}
				this.DrawLine(new Point(this.leftMargin, num3), new Point(x, num3), e.Graphics);
				this.currentPageIndex++;
				if (this.setTongji && this.currentPageIndex == this.pageCount)
				{
					this.isTongji = true;
				}
				if (this.isTongji)
				{
					int num15 = (int)(((float)width - e.Graphics.MeasureString(this.sTongJi01, this.dateFont).Width) / 2f);
					e.Graphics.DrawString(this.sTongJi01, this.tongJiFont, this.brush, (float)num15, (float)(num3 + 25));
					e.Graphics.DrawString(this.sTongJi02, this.tongJiFont, this.brush, (float)num15, (float)(num3 + 50));
					e.Graphics.DrawString(this.sTongJi03, this.tongJiFont, this.brush, (float)(num15 + 340), (float)(num3 + 50));
				}
				if (this.needPrintPageIndex)
				{
					if (this.pageCount != 1)
					{
						e.Graphics.DrawString(string.Concat(new string[]
						{
							"共 ",
							this.pageCount.ToString(),
							" 页,当前第 ",
							this.currentPageIndex.ToString(),
							" 页"
						}), this.footerFont, this.brush, (float)(width - 200), (float)(height - this.buttomMargin / 2 - this.footerFont.Height));
					}
				}
				if (this.currentPageIndex < this.pageCount)
				{
					e.HasMorePages = true;
				}
				else
				{
					e.HasMorePages = false;
					this.currentPageIndex = 0;
				}
			}
			catch
			{
			}
		}
		private void DrawLine(Point sp, Point ep, Graphics gp)
		{
			Pen pen = new Pen(Color.Black);
			gp.DrawLine(pen, sp, ep);
		}
		public PrintDocument GetPrintDocument()
		{
			return this.printDocument;
		}
		public void Print()
		{
			this.rowCount = 0;
			try
			{
				if (this.dataGridView1.DataSource.GetType().ToString() == "System.Data.DataSet")
				{
					this.rowCount = ((DataSet)this.dataGridView1.DataSource).Tables[0].Rows.Count;
				}
				else
				{
					if (this.dataGridView1.DataSource.GetType().ToString() == "System.Data.DataView")
					{
						this.rowCount = ((DataView)this.dataGridView1.DataSource).Count;
					}
				}
				this.pageSetupDialog = new PageSetupDialog();
				this.pageSetupDialog.AllowOrientation = true;
				this.pageSetupDialog.Document = this.printDocument;
				this.pageSetupDialog.Document.DefaultPageSettings.Landscape = true;
				this.pageSetupDialog.ShowDialog();
				this.printPreviewDialog = new PrintPreviewDialog();
				this.printPreviewDialog.Document = this.printDocument;
				this.printPreviewDialog.Height = 600;
				this.printPreviewDialog.Width = 800;
				this.printPreviewDialog.ClientSize = new Size(1024, 768);
				this.printPreviewDialog.PrintPreviewControl.Zoom = 1.0;
				this.printPreviewDialog.ShowDialog();
			}
			catch
			{
			}
		}
	}
}
