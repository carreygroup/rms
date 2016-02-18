using System;
using System.Windows.Forms;
using System.Drawing;

namespace RMSPPC
{
	/// <summary>
	/// TableButtonList ��ժҪ˵����
	/// </summary>
	public class TableButtonList:System.Collections.CollectionBase
	{
		private readonly System.Windows.Forms.Panel HostPanel; 
		private int ButtonWidth = 115;
		private int ButtonHeight = 60;
		private int CountBtnPerLine = 2;
		private int CurrentBtnRow = 1;
		private int CurrentBtnCol = 1;
		private Color ButtonOrignalBackColor1 = Color.DarkBlue;
		private Color ButtonOrignalBackColor2 = Color.Gold;
		private Color ButtonOrignalBackColor3 = Color.Gray;
		private Color ButtonSelectedBackColor = Color.FromArgb(255, 128, 128);
		public int CurrentBtnTop = 1;
		public int CurrentBtnLeft = 1;
		
		public TableButtonList(System.Windows.Forms.Panel host,
			                   int btnWidth,int btnHeight,int btnCountPerLine)
		{
			HostPanel = host;
            ButtonWidth = btnWidth;
            ButtonHeight = btnHeight;
            CountBtnPerLine = btnCountPerLine;
		}

		public MyButton this[ int index ]  
		{
			get  
			{
				return( (MyButton) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public void RemoveAll()
		{
			while (this.Count>0)
			{
				HostPanel.Controls.Remove(this[Count-1] as MyButton);
				this.List.RemoveAt(this.Count-1);
			}
		}

		public void AddButton(string tableno,string tablename,int status)
		{
			MyButton aButton = new MyButton(ButtonWidth,ButtonHeight);
			aButton.Tableno = tableno;
			aButton.Tablename = tablename;
			aButton.Status = status;
			switch (status)
			{
				case 0://��̨����
                    aButton.BackColor = ButtonOrignalBackColor1;
					break;
                case 1://��̨ʹ����
                    aButton.BackColor = ButtonOrignalBackColor2;
					break;
                case 2://ԭ��̨Ԥ����
                    aButton.BackColor = ButtonOrignalBackColor3;
					break;
			}			
			this.List.Add(aButton);
			HostPanel.Controls.Add(aButton);
			if (CurrentBtnCol==1) //ÿ�е�һ����ť
			{
				CurrentBtnLeft=1;
				CurrentBtnTop=(CurrentBtnRow-1)*aButton.Height+CurrentBtnRow;
				CurrentBtnCol++;
			}
			else if (CurrentBtnCol<CountBtnPerLine) //ÿ�г���һ�������һ������İ�ť
			{
				CurrentBtnLeft = (CurrentBtnCol - 1) * aButton.Width + CurrentBtnCol;
				CurrentBtnTop = (CurrentBtnRow - 1) * aButton.Height + CurrentBtnRow;
				CurrentBtnCol++;
			}
			else if (CurrentBtnCol==CountBtnPerLine) //һ�����һ����ť
			{
				CurrentBtnLeft = (CurrentBtnCol - 1) * aButton.Width + CurrentBtnCol;
				CurrentBtnTop = (CurrentBtnRow - 1) * aButton.Height + CurrentBtnRow;
				CurrentBtnCol=1;
				CurrentBtnRow++;
			}
			aButton.Left = CurrentBtnLeft;
			aButton.Top = CurrentBtnTop;
			aButton.ButtonText = aButton.Tableno+"\n"+"["+aButton.Tablename+"]";
			aButton.Click+=new EventHandler(this.MyButtonClick);
		}

		private void MyButtonClick(object sender,System.EventArgs e) 
		{ 
			//MessageBox.Show("�������Զ�����¼���Ӧ����!","��ʾ��Ϣ");
			int status=0;
			for (int i=0 ;i<=this.Count-1 ;i++)
			{
				status=(this.List[i] as MyButton).Status;
				switch (status)
				{
					case 0: //��̨����
						(this.List[i] as MyButton).BackColor=ButtonOrignalBackColor1;
						break;
					case 1: //��̨ʹ����
						(this.List[i] as MyButton).BackColor=ButtonOrignalBackColor2;
						break;
					case 2: //��̨Ԥ����
						(this.List[i] as MyButton).BackColor=ButtonOrignalBackColor3;
						break;
				}
				
			}
			(sender as MyButton).BackColor=ButtonSelectedBackColor;

			Form1.CurrentSelectedTableno=(sender as MyButton).Tableno;
		} 
	}
}
