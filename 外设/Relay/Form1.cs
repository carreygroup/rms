using System.Diagnostics;
using System;
using AxMSCommLib;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using Microsoft.VisualBasic.Compatibility;

namespace 工程1
{
	partial class main_form : System.Windows.Forms.Form
	{
		
		#region Default Instance
		
		private static main_form defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static main_form Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new main_form();
					defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
				}
				
				return defaultInstance;
			}
		}
		
		static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
		{
			defaultInstance = null;
		}
		
		#endregion
		//UPGRADE_WARNING: 数组 RXDDATA 的下限已由 1 更改为 0。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"”
		int[] RXDDATA = new int[101];
		int RXDTIME;
		//UPGRADE_WARNING: 数组 DATA_BUFFE 的下限已由 1 更改为 0。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"”
		byte[] DATA_BUFFE = new byte[6];
		byte SET_FLAG;
		private void SET_BUFF(byte leixing, byte data1, byte data2)
		{
			DATA_BUFFE[1] = leixing;
			DATA_BUFFE[2] = data1;
			DATA_BUFFE[3] = data2;
			SET_FLAG = 1;
		}
		private void SEND_BUFF()
		{
			if (SET_FLAG == 0)
			{
				Module1.tx_frame(0x10, 0, 0);
			}
			else
			{
				SET_FLAG = 0;
				Module1.tx_frame(DATA_BUFFE[1], DATA_BUFFE[2], DATA_BUFFE[3]);
			}
		}
		
		public void closeall_Click(System.Object eventSender, System.EventArgs eventArgs)
		{
			SET_BUFF(0x13, 0, 0);
		}
		
		private void data_chuli()
		{
			short i;
			for (i = 0; i <= 7; i++)
			{
				//UPGRADE_WARNING: 未能解析对象 Bit(CByte(RXDDATA(6)), i) 的默认属性。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"”
				if (Module1.Bit((byte) (RXDDATA[6]), i) == 1)
				{
					Picture1[9 + i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(0, 255, 0));
					Label2[9 + i].Text = "  吸合";
				}
				else
				{
					Picture1[9 + i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(255, 0, 0));
					Label2[9 + i].Text = "  断开";
				}
				//UPGRADE_WARNING: 未能解析对象 Bit(CByte(RXDDATA(7)), i) 的默认属性。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"”
				if (Module1.Bit((byte) (RXDDATA[7]), i) == 1)
				{
					Picture1[1 + i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(0, 255, 0));
					Label2[1 + i].Text = "  吸合";
				}
				else
				{
					Picture1[1 + i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(255, 0, 0));
					Label2[1 + i].Text = "  断开";
				}
			}
		}
		
		public void Label2_Click(System.Object eventSender, System.EventArgs eventArgs)
		{
			short Index = Label2.GetIndex(eventSender);
			Picture1_Click(Picture1[Index], new System.EventArgs());
		}
		
		static short MSComm1_OnComm_Cont;
		public void MSComm1_OnComm(System.Object eventSender, System.EventArgs eventArgs)
		{
			short i;
			object vTemp;
			byte[] bTemp;
			if (MSComm1.CommEvent == MSCommLib.OnCommConstants.comEvReceive)
			{
				for (i = 1; i <= 8; i++)
				{
					//UPGRADE_WARNING: 未能解析对象 MSComm1.Input 的默认属性。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"”
					//UPGRADE_WARNING: 未能解析对象 vTemp 的默认属性。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"”
					vTemp = MSComm1.Input;
					//UPGRADE_WARNING: 未能解析对象 vTemp 的默认属性。 单击以获得更多信息:“ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"”
					bTemp =  (@) (vTemp));
					RXDDATA[i] = bTemp[0];
				}
				if ((RXDDATA[1] + RXDDATA[2] + RXDDATA[3] + RXDDATA[4] + RXDDATA[5] + RXDDATA[6] + RXDDATA[7]) % 256 == RXDDATA[8] && RXDDATA[1] == 0x22)
				{
					data_chuli();
					RXDTIME = 5;
				}
			}
		}
		private void open_state()
		{
			short i;
			for (i = 1; i <= 16; i++)
			{
				this.Picture1[i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(255, 255, 0));
				this.Label2[i].Text = "状态未知";
			}
			this.openall.Enabled = true;
			this.closeall.Enabled = true;
			this.Timer1.Enabled = true;
			this.chuankou_num.Enabled = false;
			this.open_comm.Text = "关闭串口";
		}
		private void close_state()
		{
			short i;
			for (i = 1; i <= 16; i++)
			{
				this.Picture1[i].BackColor = System.Drawing.ColorTranslator.FromOle(0x80000011);
				this.Label2[i].Text = "状态未知";
			}
			this.openall.Enabled = false;
			this.closeall.Enabled = false;
			this.Timer1.Enabled = false;
			this.chuankou_num.Enabled = true;
			this.open_comm.Text = "打开串口";
		}
		public void open_comm_Click(System.Object eventSender, System.EventArgs eventArgs)
		{
			//			short i;
			if (MSComm1.PortOpen == false)
			{
				this.MSComm1.CommPort = short.Parse(chuankou_num.Text);
				this.MSComm1.PortOpen = true;
				this.MSComm1.RThreshold = 8; //一次接受的字节数
				open_state();
			}
			else if (MSComm1.PortOpen == true)
			{
				this.MSComm1.PortOpen = false;
				close_state();
			}
		}
		
		public void openall_Click(System.Object eventSender, System.EventArgs eventArgs)
		{
			SET_BUFF(0x13, 0xFF, 0xFF);
		}
		
		public void Picture1_Click(System.Object eventSender, System.EventArgs eventArgs)
		{
			short Index = Picture1.GetIndex(eventSender);
			if (System.Drawing.ColorTranslator.ToOle(Picture1[Index].BackColor) == Information.RGB(0, 255, 0))
			{
				SET_BUFF(0x11, 0, (byte) Index);
			}
			else if (System.Drawing.ColorTranslator.ToOle(Picture1[Index].BackColor) == Information.RGB(255, 0, 0))
			{
				SET_BUFF(0x12, 0, (byte) Index);
			}
		}
		
		public void Timer1_Tick(System.Object eventSender, System.EventArgs eventArgs)
		{
			short i;
			if (RXDTIME > 0)
			{
				RXDTIME--;
			}
			else
			{
				for (i = 1; i <= 16; i++)
				{
					Picture1[i].BackColor = System.Drawing.ColorTranslator.FromOle(Information.RGB(255, 255, 0));
					Label2[i].Text = "状态未知";
				}
			}
			SEND_BUFF();
		}
	}
}
