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
	sealed class Module1
	{
		//查询
		static byte[] tx_frame_Byteout = new byte[8];
		static byte tx_frame_i;
		//		static byte[] tx_frame_Byteout = new byte[8];
		//		static byte tx_frame_i;
		public static object tx_frame(byte leixing, byte data1, byte data2)
		{
			short sum;
			tx_frame_Byteout[0] = 0x55;
			tx_frame_Byteout[1] =  (@) ((double.Parse(main_form.Default.add_Renamed.Text))% 256));
			tx_frame_Byteout[2] = leixing;
			tx_frame_Byteout[3] = 0;
			tx_frame_Byteout[4] = 0;
			tx_frame_Byteout[5] = data1;
			tx_frame_Byteout[6] = data2;
			sum = 0;
			for (tx_frame_i = 0; tx_frame_i <= 6; tx_frame_i++)
			{
				sum =sum + tx_frame_Byteout[tx_frame_i];
			}
			tx_frame_Byteout[7] =sum % 256;
			if (main_form.Default.MSComm1.PortOpen == true)
			{
				main_form.Default.MSComm1.InBufferCount = 0;
				main_form.Default.MSComm1.Output = Microsoft.VisualBasic.Compatibility.VB6.Support.CopyArray(tx_frame_Byteout);
			}
		}
		
		//计算一个字节中的某一个位是 1 还是 0
		public static object Bit(byte Data0, short temp)
		{
			object returnValue;
			short[] bittemp = new short[8];
			short i;
			short temp1;
			short Data;
			Data = Data0;
			temp1 = temp;
			for (i = 0; i <= temp1; i++)
			{
				if (Data % 2 == 1) //'判断第一位是0或1
				{
					bittemp[i] = 1;
				}
				else
				{
					bittemp[i] = 0;
				}
				Data =Conversion.Int(Data / 2);
			}
			returnValue = bittemp[temp1];
			return returnValue;
		}
	}
}
