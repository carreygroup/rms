using System;
using System.Drawing;
using System.Windows.Forms;

namespace RMSPPC
{
	/// <summary>
	/// MyButton 的摘要说明。
	/// </summary>
	public class MyButton:Panel
	{
		Label label;
		private string mTableno,mTablename,mButtonText;
		private int mstatus;
		public MyButton(int ButtonWidth,int ButtonHeight)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.Width = ButtonWidth;
			this.Height = ButtonHeight;
			label = new Label();
			label.Parent = this;
			label.Top=8;
			label.Left =1;
			label.Width = ButtonWidth-2;
			label.Height = ButtonHeight-2;
			label.TextAlign = ContentAlignment.TopCenter;
			label.ForeColor = Color.White;
			label.Font = new Font("宋体", 9, FontStyle.Bold);
		}

		public string Tableno
		{
			get 
			{ 
				return mTableno; 
			}
			set 
			{ 
				mTableno = value;
			}
		}

		public string Tablename
		{
			get 
			{ 
				return mTablename; 
			}
			set 
			{ 
				mTablename = value;
			}
		}

		public int Status
		{
			get 
			{ 
				return mstatus; 
			}
			set 
			{ 
				mstatus = value;
			}
		}

		public string ButtonText
		{
			get 
			{ 
				return mButtonText; 
			}
			set 
			{ 
				mButtonText = value;
				label.Text =mButtonText;
			}
		}

	}
}
