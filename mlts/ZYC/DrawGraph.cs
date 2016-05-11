using System;
using System.Drawing;
using System.Windows.Forms;
namespace ZYC
{
	public class DrawGraph
	{
		public struct PieInfo
		{
			public Color Color;
			public uint Number;
			public string Text;
			public PieInfo(Color AColor, uint ANumber, string AString)
			{
				this.Color = AColor;
				this.Number = ANumber;
				this.Text = AString;
			}
		}
		public void BrokenLineGraph(Panel panel, int[] array, float distance, int interval, Color color, Brush b)
		{
			Graphics graphics = panel.CreateGraphics();
			int num = panel.Height - 5;
			Pen pen = new Pen(color);
			Pen pen2 = new Pen(b);
			int num2 = 3;
			float num3 = (float)array[0] + 1f * distance + 1f;
			float num4 = (float)(num - array[0] * interval - 1);
			graphics.DrawEllipse(pen2, num3, num4, (float)num2, (float)num2);
			graphics.FillEllipse(b, num3, num4, (float)num2, (float)num2);
			for (int i = 1; i < array.Length; i++)
			{
				graphics.DrawLine(pen, num3, num4, (float)array[i] + (float)(i + 1) * distance, (float)(num - array[i] * interval));
				graphics.DrawEllipse(pen2, (float)array[i] + (float)(i + 1) * distance, (float)(num - array[i] * interval), (float)num2, (float)num2);
				graphics.FillEllipse(b, (float)array[i] + (float)(i + 1) * distance, (float)(num - array[i] * interval), (float)num2, (float)num2);
				num3 = (float)array[i] + (float)(i + 1) * distance + 1f;
				num4 = (float)(num - array[i] * interval - 1);
			}
		}
		public void BrokenLineGraph(Panel panel, float[] array, float distance, int interval, Color color, Brush b)
		{
			Graphics graphics = panel.CreateGraphics();
			int num = panel.Height - 5;
			Pen pen = new Pen(color);
			Pen pen2 = new Pen(b);
			int num2 = 3;
			float num3 = array[0] + 1f * distance + 1f;
			float num4 = (float)num - array[0] * (float)interval - 1f;
			graphics.DrawEllipse(pen2, num3, num4, (float)num2, (float)num2);
			graphics.FillEllipse(b, num3, num4, (float)num2, (float)num2);
			for (int i = 1; i < array.Length; i++)
			{
				graphics.DrawLine(pen, num3, num4, array[i] + (float)(i + 1) * distance, (float)num - array[i] * (float)interval);
				graphics.DrawEllipse(pen2, array[i] + (float)(i + 1) * distance, (float)num - array[i] * (float)interval, (float)num2, (float)num2);
				graphics.FillEllipse(b, array[i] + (float)(i + 1) * distance, (float)num - array[i] * (float)interval, (float)num2, (float)num2);
				num3 = array[i] + (float)(i + 1) * distance + 1f;
				num4 = (float)num - array[i] * (float)interval - 1f;
			}
		}
		public void BarGraph(Panel panel, int[] array, int width, float distance, int interval, Color color, Brush b)
		{
			Graphics graphics = panel.CreateGraphics();
			int num = panel.Height - 5;
			Pen pen = new Pen(b);
			int num2 = 5;
			for (int i = 0; i < array.Length; i++)
			{
				graphics.DrawRectangle(pen, num2, num - array[i] * interval, width, num - (num - array[i] * interval));
				graphics.FillRectangle(b, num2, num - array[i] * interval, width, num - (num - array[i] * interval));
				num2 += width + interval;
			}
		}
		public void BarGraph(Panel panel, float[] array, int width, float distance, int interval, Color color, Brush b)
		{
			Graphics graphics = panel.CreateGraphics();
			int num = panel.Height - 5;
			Pen pen = new Pen(b);
			int num2 = 5;
			for (int i = 0; i < array.Length; i++)
			{
				graphics.DrawRectangle(pen, (float)num2, (float)num - array[i] * (float)interval, (float)width, (float)num - ((float)num - array[i] * (float)interval));
				graphics.FillRectangle(b, (float)num2, (float)num - array[i] * (float)interval, (float)width, (float)num - ((float)num - array[i] * (float)interval));
				num2 += width + interval;
			}
		}
		private void DrawPies(Graphics AGraphics, Rectangle ARect, params DrawGraph.PieInfo[] APies)
		{
			if (AGraphics != null)
			{
				uint num = 0u;
				for (int i = 0; i < APies.Length; i++)
				{
					DrawGraph.PieInfo pieInfo = APies[i];
					num += pieInfo.Number;
				}
				float num2 = 0f;
				int num3 = 0;
				int num4 = ARect.Width + 10;
				for (int i = 0; i < APies.Length; i++)
				{
					DrawGraph.PieInfo pieInfo = APies[i];
					if (pieInfo.Number != 0u)
					{
						float num5 = pieInfo.Number / num * 360f;
						AGraphics.FillPie(new SolidBrush(pieInfo.Color), ARect, num2, num5);
						AGraphics.DrawRectangle(new Pen(pieInfo.Color), num4, num3, 10, 5);
						AGraphics.FillRectangle(new SolidBrush(pieInfo.Color), num4, num3, 20, 10);
						AGraphics.DrawString(pieInfo.Text, new Font("微软雅黑", 10f), Brushes.Black, new PointF((float)(num4 + 20), (float)(num3 - 5)));
						num2 += num5;
						num3 += 15;
					}
				}
			}
		}
		public void DrawPie(int width, int height, Control c, params DrawGraph.PieInfo[] APies)
		{
			Graphics graphics = c.CreateGraphics();
			this.DrawPies(graphics, new Rectangle(0, 0, width, height), APies);
			graphics.Dispose();
		}
	}
}
