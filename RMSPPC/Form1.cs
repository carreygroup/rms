using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace RMSPPC
{
	/// <summary>
	/// Form1 的摘要说明。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		public struct foodinfo 
		{            
			public string m_guid;
			public string m_tableno;
			public string m_billno;
			public string m_foodcode;
			public string m_fooaname;
			public string m_foodtypename;
			public string m_foodunit;
			public string m_quantity;
			public string m_operandis;
			public string m_tastes;
			public double m_price;
			public double m_sub_price;
            public int m_sign; //sign = 0 未发送 sign = 1 已经发送
			public string m_opid;
			public int m_suitsign; //是否为套餐名 value = 1 为套餐 value = 0 为单品

			public foodinfo(string guid) 
			{
				m_guid=guid;
				m_tableno="";
				m_billno="";
				m_foodcode="";
				m_fooaname="";
				m_foodtypename="";
				m_foodunit="";
				m_quantity="";
				m_operandis="";
				m_tastes="";
				m_price=0;
				m_sub_price=0;
				m_sign=0;
				m_opid="";
				m_suitsign=0;
			}
		}

        string WebUrl;
		ArrayList NeedSendFoods = new ArrayList();
		const string ProgramPath = "\\Program Files\\RMSPPC"; //应用程序路径
		const string LocalDatabaseFile = ProgramPath + "\\RMS.sdf"; //SQL Mobile 3.0 数据库文件
		const string LocalConnString = "Data Source=" + LocalDatabaseFile;

		public double MaxCancelCount = 0;        

		private string TableTypeNameXmlData="";
		private string BillTypeXmlData = "";
		private string FoodOperandisXmlData = "";
		private string FoodTastesXmlData = "";
		private string DeptNameXmlData="";
		private string FoodTypeNameXmlData="";

		private string CurrentLogOnUserID = "";
		int tmp;

		private Panel PriorPanel;
		private TableButtonList TableList;

		public static string CurrentSelectedTableno="";
		// 0
		int TableItemCountOfPage = 20;
		int CurrentTablePageNo = 1;
		// 1
		int FoodItemCountOfPage = 6;
		int CurrentFoodPageNo = 1;
		// 2
		int FoodOperandiItemCountOfPage = 100;
		int CurrentFoodOperandiPageNo = 1;
		// 3
		int FoodTasteItemCountOfPage = 100;
		int CurrentFoodTastePageNo = 1;

		//14
		int SuitItemCountOfPage = 7;
		int CurrentSuitPageNo = 1;

		bool LoggedOff = false;
		bool ShowConfirmPanel = false; //是否需要显示验证窗口

        //控制输入法开关
        public static uint SIPF_OFF = 0x00;//关闭
        public static uint SIPF_ON = 0x01;//打开
        [DllImport("coredll.dll")]
        public extern static void SipShowIM(uint dwFlag);

		private System.Windows.Forms.Panel pnlLogOn;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel pnlTableList;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numPsnCount;
		private System.Windows.Forms.Button btnOpenTable;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button btnSelectFood;
		private System.Windows.Forms.Button btnNeedSendFoods;
		private System.Windows.Forms.Button btnAllOrderdFoods;
		private System.Windows.Forms.Panel pnlFoodList;
		private System.Windows.Forms.Label lblTableNo1;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblTableNo2;
		private System.Windows.Forms.Label lblFoodName;
		private System.Windows.Forms.ListView lvFoodOperandis;
		private System.Windows.Forms.ListView lvFoodTastes;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Panel pnlOrderFood;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn1;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn4;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn5;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle2;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn6;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn7;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn8;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn9;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn10;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn11;
		private System.Windows.Forms.DataGrid dgFoodlist;
		private System.Windows.Forms.Panel pnlNeedSendFoods;
		private System.Windows.Forms.Label lblTableNo3;
		private System.Windows.Forms.CheckBox chkSelAll2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ColumnHeader columnHeader17;
		private System.Windows.Forms.ListView lvNeedSendFoods;
		private System.Windows.Forms.Button button13;
		private System.Windows.Forms.Button button14;
		private System.Windows.Forms.Label lblTableNo4;
		private System.Windows.Forms.CheckBox chkSelAll1;
		private System.Windows.Forms.ComboBox cbAllBillList;
		private System.Windows.Forms.ListView lvAllOrderedFoods;
		private System.Windows.Forms.Label lblAllFoodsPrice;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.ColumnHeader columnHeader13;
		private System.Windows.Forms.ColumnHeader columnHeader14;
		private System.Windows.Forms.ColumnHeader columnHeader15;
		private System.Windows.Forms.ColumnHeader columnHeader18;
		private System.Windows.Forms.Panel pnlAllOrderedFoods;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button button16;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button10;
		private System.Windows.Forms.Panel pnlAbout;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.ColumnHeader columnHeader16;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Button btnSendFoods;
		private System.Windows.Forms.Button btnDelNeedSendFoods;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label lblFoodUnit;
		private System.Windows.Forms.Button button11;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.ComboBox cbBilllist;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Button button12;
		private System.Windows.Forms.Button button15;
		private System.Windows.Forms.Panel pnlNeedConfirm;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Button button17;
		private System.Windows.Forms.ComboBox cbFoodQuery;
		private System.Windows.Forms.TextBox txtFoodQuery;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.ComboBox cbTableTypeName;
		private System.Windows.Forms.Panel pnlFoods;
		private System.Windows.Forms.Panel pnlSuits;
		private System.Windows.Forms.Button button18;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.DataGrid dgSuitList;
		private System.Windows.Forms.DataGridTableStyle dataGridTableStyle3;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn12;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn13;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn14;
		private System.Windows.Forms.DataGridTextBoxColumn dataGridTextBoxColumn15;
		private System.Windows.Forms.ComboBox cbDept;
		private System.Windows.Forms.ComboBox cbFoodType;
		private System.Windows.Forms.ColumnHeader columnHeader19;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Button button19;
        private System.Windows.Forms.TextBox txtFoodPrice;
        private Label label10;
        private Label label16;
		private System.Windows.Forms.MainMenu mainMenu1;

		public Form1()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
            
			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}
		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}
		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.pnlLogOn = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button16 = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label37 = new System.Windows.Forms.Label();
            this.pnlTableList = new System.Windows.Forms.Panel();
            this.button19 = new System.Windows.Forms.Button();
            this.cbTableTypeName = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button11 = new System.Windows.Forms.Button();
            this.btnAllOrderdFoods = new System.Windows.Forms.Button();
            this.btnNeedSendFoods = new System.Windows.Forms.Button();
            this.btnSelectFood = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.numPsnCount = new System.Windows.Forms.NumericUpDown();
            this.btnOpenTable = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn5 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn1 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn2 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn3 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn4 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.pnlFoodList = new System.Windows.Forms.Panel();
            this.button18 = new System.Windows.Forms.Button();
            this.cbBilllist = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.button8 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.lblTableNo1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlFoods = new System.Windows.Forms.Panel();
            this.cbFoodType = new System.Windows.Forms.ComboBox();
            this.cbDept = new System.Windows.Forms.ComboBox();
            this.cbFoodQuery = new System.Windows.Forms.ComboBox();
            this.txtFoodQuery = new System.Windows.Forms.TextBox();
            this.button17 = new System.Windows.Forms.Button();
            this.dgFoodlist = new System.Windows.Forms.DataGrid();
            this.dataGridTableStyle2 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn6 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn7 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn8 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn11 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn9 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn10 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.pnlSuits = new System.Windows.Forms.Panel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label38 = new System.Windows.Forms.Label();
            this.dgSuitList = new System.Windows.Forms.DataGrid();
            this.dataGridTableStyle3 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTextBoxColumn12 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn13 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn14 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.dataGridTextBoxColumn15 = new System.Windows.Forms.DataGridTextBoxColumn();
            this.pnlOrderFood = new System.Windows.Forms.Panel();
            this.txtFoodPrice = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.lblFoodUnit = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.lvFoodTastes = new System.Windows.Forms.ListView();
            this.lvFoodOperandis = new System.Windows.Forms.ListView();
            this.lblFoodName = new System.Windows.Forms.Label();
            this.lblTableNo2 = new System.Windows.Forms.Label();
            this.pnlNeedSendFoods = new System.Windows.Forms.Panel();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btnSendFoods = new System.Windows.Forms.Button();
            this.btnDelNeedSendFoods = new System.Windows.Forms.Button();
            this.chkSelAll2 = new System.Windows.Forms.CheckBox();
            this.lblTableNo3 = new System.Windows.Forms.Label();
            this.lvNeedSendFoods = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
            this.pnlAllOrderedFoods = new System.Windows.Forms.Panel();
            this.label23 = new System.Windows.Forms.Label();
            this.cbAllBillList = new System.Windows.Forms.ComboBox();
            this.label22 = new System.Windows.Forms.Label();
            this.chkSelAll1 = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button14 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.lblAllFoodsPrice = new System.Windows.Forms.Label();
            this.lvAllOrderedFoods = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
            this.lblTableNo4 = new System.Windows.Forms.Label();
            this.pnlAbout = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button10 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList();
            this.pnlNeedConfirm = new System.Windows.Forms.Panel();
            this.label36 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.button15 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.pnlLogOn.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlTableList.SuspendLayout();
            this.pnlFoodList.SuspendLayout();
            this.pnlFoods.SuspendLayout();
            this.pnlSuits.SuspendLayout();
            this.pnlOrderFood.SuspendLayout();
            this.pnlNeedSendFoods.SuspendLayout();
            this.pnlAllOrderedFoods.SuspendLayout();
            this.pnlAbout.SuspendLayout();
            this.pnlNeedConfirm.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem3);
            this.mainMenu1.MenuItems.Add(this.menuItem7);
            this.mainMenu1.MenuItems.Add(this.menuItem9);
            // 
            // menuItem1
            // 
            this.menuItem1.MenuItems.Add(this.menuItem5);
            this.menuItem1.Text = "文件";
            // 
            // menuItem5
            // 
            this.menuItem5.Text = "退出";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "关于";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Text = "开输入法";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Text = "关输入法";
            this.menuItem9.Click += new System.EventHandler(this.menuItem9_Click);
            // 
            // pnlLogOn
            // 
            this.pnlLogOn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.pnlLogOn.Controls.Add(this.panel2);
            this.pnlLogOn.Controls.Add(this.button16);
            this.pnlLogOn.Controls.Add(this.txtUsername);
            this.pnlLogOn.Controls.Add(this.txtPassword);
            this.pnlLogOn.Controls.Add(this.label7);
            this.pnlLogOn.Controls.Add(this.label3);
            this.pnlLogOn.Controls.Add(this.label4);
            this.pnlLogOn.Controls.Add(this.button1);
            this.pnlLogOn.Controls.Add(this.pictureBox2);
            this.pnlLogOn.Controls.Add(this.label37);
            this.pnlLogOn.Location = new System.Drawing.Point(0, 0);
            this.pnlLogOn.Name = "pnlLogOn";
            this.pnlLogOn.Size = new System.Drawing.Size(240, 270);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 56);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(16, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(208, 20);
            this.label2.Text = "RMS PPC v3.0T";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 20);
            this.label1.Text = "餐厅管家";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(152, 192);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(72, 24);
            this.button16.TabIndex = 2;
            this.button16.Text = "退出";
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(128, 96);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(104, 21);
            this.txtUsername.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(128, 136);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(104, 21);
            this.txtPassword.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(128, 120);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 16);
            this.label7.Text = "密  码：";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(128, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 16);
            this.label3.Text = "用户名：";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(152, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 24);
            this.button1.TabIndex = 8;
            this.button1.Text = "登录";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(12, 64);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(96, 149);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            // 
            // label37
            // 
            this.label37.Location = new System.Drawing.Point(0, 220);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(240, 28);
            // 
            // pnlTableList
            // 
            this.pnlTableList.BackColor = System.Drawing.Color.Gray;
            this.pnlTableList.Controls.Add(this.button19);
            this.pnlTableList.Controls.Add(this.cbTableTypeName);
            this.pnlTableList.Controls.Add(this.textBox1);
            this.pnlTableList.Controls.Add(this.comboBox1);
            this.pnlTableList.Controls.Add(this.button11);
            this.pnlTableList.Controls.Add(this.btnAllOrderdFoods);
            this.pnlTableList.Controls.Add(this.btnNeedSendFoods);
            this.pnlTableList.Controls.Add(this.btnSelectFood);
            this.pnlTableList.Controls.Add(this.button5);
            this.pnlTableList.Controls.Add(this.button4);
            this.pnlTableList.Controls.Add(this.numPsnCount);
            this.pnlTableList.Controls.Add(this.btnOpenTable);
            this.pnlTableList.Controls.Add(this.label5);
            this.pnlTableList.Controls.Add(this.button3);
            this.pnlTableList.Controls.Add(this.button2);
            this.pnlTableList.Controls.Add(this.panel1);
            this.pnlTableList.Location = new System.Drawing.Point(0, 0);
            this.pnlTableList.Name = "pnlTableList";
            this.pnlTableList.Size = new System.Drawing.Size(240, 270);
            this.pnlTableList.Visible = false;
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(72, 224);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(24, 24);
            this.button19.TabIndex = 0;
            this.button19.Text = "改";
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // cbTableTypeName
            // 
            this.cbTableTypeName.Location = new System.Drawing.Point(0, 224);
            this.cbTableTypeName.Name = "cbTableTypeName";
            this.cbTableTypeName.Size = new System.Drawing.Size(48, 22);
            this.cbTableTypeName.TabIndex = 1;
            this.cbTableTypeName.SelectedIndexChanged += new System.EventHandler(this.cbTableTypeName_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(144, 200);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(48, 21);
            this.textBox1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Location = new System.Drawing.Point(96, 200);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(48, 22);
            this.comboBox1.TabIndex = 3;
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(96, 224);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(48, 24);
            this.button11.TabIndex = 4;
            this.button11.Text = "加单";
            this.button11.Click += new System.EventHandler(this.button11_Click_1);
            // 
            // btnAllOrderdFoods
            // 
            this.btnAllOrderdFoods.Location = new System.Drawing.Point(144, 248);
            this.btnAllOrderdFoods.Name = "btnAllOrderdFoods";
            this.btnAllOrderdFoods.Size = new System.Drawing.Size(96, 24);
            this.btnAllOrderdFoods.TabIndex = 5;
            this.btnAllOrderdFoods.Text = "已点全部菜品";
            this.btnAllOrderdFoods.Click += new System.EventHandler(this.btnAllOrderdFoods_Click);
            // 
            // btnNeedSendFoods
            // 
            this.btnNeedSendFoods.Location = new System.Drawing.Point(48, 248);
            this.btnNeedSendFoods.Name = "btnNeedSendFoods";
            this.btnNeedSendFoods.Size = new System.Drawing.Size(96, 24);
            this.btnNeedSendFoods.TabIndex = 6;
            this.btnNeedSendFoods.Text = "未发送菜品";
            this.btnNeedSendFoods.Click += new System.EventHandler(this.btnNeedSendFoods_Click);
            // 
            // btnSelectFood
            // 
            this.btnSelectFood.Location = new System.Drawing.Point(192, 200);
            this.btnSelectFood.Name = "btnSelectFood";
            this.btnSelectFood.Size = new System.Drawing.Size(48, 48);
            this.btnSelectFood.TabIndex = 7;
            this.btnSelectFood.Text = "点菜";
            this.btnSelectFood.Click += new System.EventHandler(this.btnSelectFood_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(168, 224);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(24, 24);
            this.button5.TabIndex = 8;
            this.button5.Text = "并";
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(144, 224);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(24, 24);
            this.button4.TabIndex = 9;
            this.button4.Text = "转";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // numPsnCount
            // 
            this.numPsnCount.Location = new System.Drawing.Point(48, 200);
            this.numPsnCount.Name = "numPsnCount";
            this.numPsnCount.Size = new System.Drawing.Size(48, 22);
            this.numPsnCount.TabIndex = 10;
            this.numPsnCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnOpenTable
            // 
            this.btnOpenTable.Location = new System.Drawing.Point(48, 224);
            this.btnOpenTable.Name = "btnOpenTable";
            this.btnOpenTable.Size = new System.Drawing.Size(24, 24);
            this.btnOpenTable.TabIndex = 11;
            this.btnOpenTable.Text = "开";
            this.btnOpenTable.Click += new System.EventHandler(this.btnOpenTable_Click);
            // 
            // label5
            // 
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(0, 256);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 20);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(24, 200);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 24);
            this.button3.TabIndex = 13;
            this.button3.Text = ">";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(0, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 24);
            this.button2.TabIndex = 14;
            this.button2.Text = "<";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 192);
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn5);
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn1);
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn2);
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn3);
            this.dataGridTableStyle1.GridColumnStyles.Add(this.dataGridTextBoxColumn4);
            this.dataGridTableStyle1.MappingName = "t";
            // 
            // dataGridTextBoxColumn5
            // 
            this.dataGridTextBoxColumn5.Format = "";
            this.dataGridTextBoxColumn5.FormatInfo = null;
            this.dataGridTextBoxColumn5.HeaderText = "序号";
            this.dataGridTextBoxColumn5.MappingName = "R";
            this.dataGridTextBoxColumn5.Width = 30;
            // 
            // dataGridTextBoxColumn1
            // 
            this.dataGridTextBoxColumn1.Format = "";
            this.dataGridTextBoxColumn1.FormatInfo = null;
            this.dataGridTextBoxColumn1.HeaderText = "台号";
            this.dataGridTextBoxColumn1.MappingName = "tableno";
            this.dataGridTextBoxColumn1.Width = 45;
            // 
            // dataGridTextBoxColumn2
            // 
            this.dataGridTextBoxColumn2.Format = "";
            this.dataGridTextBoxColumn2.FormatInfo = null;
            this.dataGridTextBoxColumn2.HeaderText = "名称";
            this.dataGridTextBoxColumn2.MappingName = "tablename";
            this.dataGridTextBoxColumn2.Width = 55;
            // 
            // dataGridTextBoxColumn3
            // 
            this.dataGridTextBoxColumn3.Format = "";
            this.dataGridTextBoxColumn3.FormatInfo = null;
            this.dataGridTextBoxColumn3.HeaderText = "就餐人数";
            this.dataGridTextBoxColumn3.MappingName = "peoplenumber";
            this.dataGridTextBoxColumn3.Width = 55;
            // 
            // dataGridTextBoxColumn4
            // 
            this.dataGridTextBoxColumn4.Format = "";
            this.dataGridTextBoxColumn4.FormatInfo = null;
            this.dataGridTextBoxColumn4.HeaderText = "状态";
            this.dataGridTextBoxColumn4.MappingName = "status";
            this.dataGridTextBoxColumn4.Width = 30;
            // 
            // pnlFoodList
            // 
            this.pnlFoodList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnlFoodList.Controls.Add(this.button18);
            this.pnlFoodList.Controls.Add(this.cbBilllist);
            this.pnlFoodList.Controls.Add(this.label21);
            this.pnlFoodList.Controls.Add(this.label20);
            this.pnlFoodList.Controls.Add(this.label17);
            this.pnlFoodList.Controls.Add(this.button8);
            this.pnlFoodList.Controls.Add(this.button7);
            this.pnlFoodList.Controls.Add(this.button6);
            this.pnlFoodList.Controls.Add(this.lblTableNo1);
            this.pnlFoodList.Controls.Add(this.label6);
            this.pnlFoodList.Controls.Add(this.pnlFoods);
            this.pnlFoodList.Controls.Add(this.pnlSuits);
            this.pnlFoodList.Location = new System.Drawing.Point(0, 0);
            this.pnlFoodList.Name = "pnlFoodList";
            this.pnlFoodList.Size = new System.Drawing.Size(240, 270);
            this.pnlFoodList.Visible = false;
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(48, 248);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(48, 22);
            this.button18.TabIndex = 0;
            this.button18.Text = "套餐";
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // cbBilllist
            // 
            this.cbBilllist.Location = new System.Drawing.Point(96, 27);
            this.cbBilllist.Name = "cbBilllist";
            this.cbBilllist.Size = new System.Drawing.Size(136, 22);
            this.cbBilllist.TabIndex = 1;
            this.cbBilllist.SelectedIndexChanged += new System.EventHandler(this.cbConsumeBilllist_SelectedIndexChanged);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(96, 48);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(136, 16);
            this.label21.Text = "label21";
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(8, 48);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(96, 16);
            this.label20.Text = "当前单据类别：";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(8, 30);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 16);
            this.label17.Text = "选择点菜单据：";
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(192, 248);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(48, 22);
            this.button8.TabIndex = 5;
            this.button8.Text = "点菜";
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(144, 248);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(48, 22);
            this.button7.TabIndex = 6;
            this.button7.Text = ">";
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(96, 248);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 22);
            this.button6.TabIndex = 7;
            this.button6.Text = "<";
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // lblTableNo1
            // 
            this.lblTableNo1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTableNo1.Location = new System.Drawing.Point(0, 8);
            this.lblTableNo1.Name = "lblTableNo1";
            this.lblTableNo1.Size = new System.Drawing.Size(240, 20);
            this.lblTableNo1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 256);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 20);
            // 
            // pnlFoods
            // 
            this.pnlFoods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.pnlFoods.Controls.Add(this.cbFoodType);
            this.pnlFoods.Controls.Add(this.cbDept);
            this.pnlFoods.Controls.Add(this.cbFoodQuery);
            this.pnlFoods.Controls.Add(this.txtFoodQuery);
            this.pnlFoods.Controls.Add(this.button17);
            this.pnlFoods.Controls.Add(this.dgFoodlist);
            this.pnlFoods.Location = new System.Drawing.Point(0, 64);
            this.pnlFoods.Name = "pnlFoods";
            this.pnlFoods.Size = new System.Drawing.Size(240, 184);
            // 
            // cbFoodType
            // 
            this.cbFoodType.Location = new System.Drawing.Point(96, 0);
            this.cbFoodType.Name = "cbFoodType";
            this.cbFoodType.Size = new System.Drawing.Size(88, 22);
            this.cbFoodType.TabIndex = 0;
            this.cbFoodType.SelectedIndexChanged += new System.EventHandler(this.cbFoodType_SelectedIndexChanged);
            // 
            // cbDept
            // 
            this.cbDept.Location = new System.Drawing.Point(0, 0);
            this.cbDept.Name = "cbDept";
            this.cbDept.Size = new System.Drawing.Size(88, 22);
            this.cbDept.TabIndex = 1;
            this.cbDept.SelectedIndexChanged += new System.EventHandler(this.cbDept_SelectedIndexChanged);
            // 
            // cbFoodQuery
            // 
            this.cbFoodQuery.Items.Add("菜品编码");
            this.cbFoodQuery.Items.Add("菜品名称");
            this.cbFoodQuery.Items.Add("拼音码");
            this.cbFoodQuery.Location = new System.Drawing.Point(0, 24);
            this.cbFoodQuery.Name = "cbFoodQuery";
            this.cbFoodQuery.Size = new System.Drawing.Size(88, 22);
            this.cbFoodQuery.TabIndex = 2;
            this.cbFoodQuery.SelectedIndexChanged += new System.EventHandler(this.cbFoodQuery_SelectedIndexChanged);
            // 
            // txtFoodQuery
            // 
            this.txtFoodQuery.Location = new System.Drawing.Point(96, 24);
            this.txtFoodQuery.Name = "txtFoodQuery";
            this.txtFoodQuery.Size = new System.Drawing.Size(88, 21);
            this.txtFoodQuery.TabIndex = 3;
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(192, 24);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(40, 21);
            this.button17.TabIndex = 4;
            this.button17.Text = "查询";
            this.button17.Click += new System.EventHandler(this.button17_Click_1);
            // 
            // dgFoodlist
            // 
            this.dgFoodlist.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgFoodlist.Location = new System.Drawing.Point(0, 48);
            this.dgFoodlist.Name = "dgFoodlist";
            this.dgFoodlist.Size = new System.Drawing.Size(240, 136);
            this.dgFoodlist.TabIndex = 5;
            this.dgFoodlist.TableStyles.Add(this.dataGridTableStyle2);
            // 
            // dataGridTableStyle2
            // 
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn6);
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn7);
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn8);
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn11);
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn9);
            this.dataGridTableStyle2.GridColumnStyles.Add(this.dataGridTextBoxColumn10);
            this.dataGridTableStyle2.MappingName = "t";
            // 
            // dataGridTextBoxColumn6
            // 
            this.dataGridTextBoxColumn6.Format = "";
            this.dataGridTextBoxColumn6.FormatInfo = null;
            this.dataGridTextBoxColumn6.HeaderText = "序号";
            this.dataGridTextBoxColumn6.MappingName = "R";
            this.dataGridTextBoxColumn6.Width = 30;
            // 
            // dataGridTextBoxColumn7
            // 
            this.dataGridTextBoxColumn7.Format = "";
            this.dataGridTextBoxColumn7.FormatInfo = null;
            this.dataGridTextBoxColumn7.HeaderText = "菜品编码";
            this.dataGridTextBoxColumn7.MappingName = "foodcode";
            // 
            // dataGridTextBoxColumn8
            // 
            this.dataGridTextBoxColumn8.Format = "";
            this.dataGridTextBoxColumn8.FormatInfo = null;
            this.dataGridTextBoxColumn8.HeaderText = "名称";
            this.dataGridTextBoxColumn8.MappingName = "foodname";
            this.dataGridTextBoxColumn8.Width = 100;
            // 
            // dataGridTextBoxColumn11
            // 
            this.dataGridTextBoxColumn11.Format = "";
            this.dataGridTextBoxColumn11.FormatInfo = null;
            this.dataGridTextBoxColumn11.HeaderText = "价格";
            this.dataGridTextBoxColumn11.MappingName = "price";
            this.dataGridTextBoxColumn11.Width = 45;
            // 
            // dataGridTextBoxColumn9
            // 
            this.dataGridTextBoxColumn9.Format = "";
            this.dataGridTextBoxColumn9.FormatInfo = null;
            this.dataGridTextBoxColumn9.HeaderText = "单位";
            this.dataGridTextBoxColumn9.MappingName = "unit";
            this.dataGridTextBoxColumn9.Width = 45;
            // 
            // dataGridTextBoxColumn10
            // 
            this.dataGridTextBoxColumn10.Format = "";
            this.dataGridTextBoxColumn10.FormatInfo = null;
            this.dataGridTextBoxColumn10.HeaderText = "类别";
            this.dataGridTextBoxColumn10.MappingName = "typename";
            this.dataGridTextBoxColumn10.Width = 60;
            // 
            // pnlSuits
            // 
            this.pnlSuits.Controls.Add(this.numericUpDown1);
            this.pnlSuits.Controls.Add(this.label38);
            this.pnlSuits.Controls.Add(this.dgSuitList);
            this.pnlSuits.Location = new System.Drawing.Point(0, 64);
            this.pnlSuits.Name = "pnlSuits";
            this.pnlSuits.Size = new System.Drawing.Size(248, 184);
            this.pnlSuits.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(96, 160);
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 22);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(8, 164);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(96, 16);
            this.label38.Text = "选择套餐数量：";
            // 
            // dgSuitList
            // 
            this.dgSuitList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dgSuitList.Location = new System.Drawing.Point(0, 0);
            this.dgSuitList.Name = "dgSuitList";
            this.dgSuitList.Size = new System.Drawing.Size(240, 160);
            this.dgSuitList.TabIndex = 2;
            this.dgSuitList.TableStyles.Add(this.dataGridTableStyle3);
            // 
            // dataGridTableStyle3
            // 
            this.dataGridTableStyle3.GridColumnStyles.Add(this.dataGridTextBoxColumn12);
            this.dataGridTableStyle3.GridColumnStyles.Add(this.dataGridTextBoxColumn13);
            this.dataGridTableStyle3.GridColumnStyles.Add(this.dataGridTextBoxColumn14);
            this.dataGridTableStyle3.GridColumnStyles.Add(this.dataGridTextBoxColumn15);
            this.dataGridTableStyle3.MappingName = "t";
            // 
            // dataGridTextBoxColumn12
            // 
            this.dataGridTextBoxColumn12.Format = "";
            this.dataGridTextBoxColumn12.FormatInfo = null;
            this.dataGridTextBoxColumn12.HeaderText = "序号";
            this.dataGridTextBoxColumn12.MappingName = "R";
            this.dataGridTextBoxColumn12.Width = 40;
            // 
            // dataGridTextBoxColumn13
            // 
            this.dataGridTextBoxColumn13.Format = "";
            this.dataGridTextBoxColumn13.FormatInfo = null;
            this.dataGridTextBoxColumn13.HeaderText = "编码";
            this.dataGridTextBoxColumn13.MappingName = "suitcode";
            this.dataGridTextBoxColumn13.Width = 40;
            // 
            // dataGridTextBoxColumn14
            // 
            this.dataGridTextBoxColumn14.Format = "";
            this.dataGridTextBoxColumn14.FormatInfo = null;
            this.dataGridTextBoxColumn14.HeaderText = "套餐名称";
            this.dataGridTextBoxColumn14.MappingName = "suitname";
            this.dataGridTextBoxColumn14.Width = 80;
            // 
            // dataGridTextBoxColumn15
            // 
            this.dataGridTextBoxColumn15.Format = "";
            this.dataGridTextBoxColumn15.FormatInfo = null;
            this.dataGridTextBoxColumn15.HeaderText = "价格";
            this.dataGridTextBoxColumn15.MappingName = "standardsum";
            this.dataGridTextBoxColumn15.Width = 40;
            // 
            // pnlOrderFood
            // 
            this.pnlOrderFood.BackColor = System.Drawing.Color.Silver;
            this.pnlOrderFood.Controls.Add(this.txtFoodPrice);
            this.pnlOrderFood.Controls.Add(this.textBox4);
            this.pnlOrderFood.Controls.Add(this.lblFoodUnit);
            this.pnlOrderFood.Controls.Add(this.label9);
            this.pnlOrderFood.Controls.Add(this.button9);
            this.pnlOrderFood.Controls.Add(this.lvFoodTastes);
            this.pnlOrderFood.Controls.Add(this.lvFoodOperandis);
            this.pnlOrderFood.Controls.Add(this.lblFoodName);
            this.pnlOrderFood.Controls.Add(this.lblTableNo2);
            this.pnlOrderFood.Location = new System.Drawing.Point(0, 0);
            this.pnlOrderFood.Name = "pnlOrderFood";
            this.pnlOrderFood.Size = new System.Drawing.Size(240, 270);
            this.pnlOrderFood.Visible = false;
            // 
            // txtFoodPrice
            // 
            this.txtFoodPrice.Location = new System.Drawing.Point(160, 48);
            this.txtFoodPrice.Name = "txtFoodPrice";
            this.txtFoodPrice.Size = new System.Drawing.Size(64, 21);
            this.txtFoodPrice.TabIndex = 0;
            this.txtFoodPrice.Text = "0";
            this.txtFoodPrice.Visible = false;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(59, 48);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(35, 21);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = "1";
            // 
            // lblFoodUnit
            // 
            this.lblFoodUnit.Location = new System.Drawing.Point(96, 50);
            this.lblFoodUnit.Name = "lblFoodUnit";
            this.lblFoodUnit.Size = new System.Drawing.Size(48, 16);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(0, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 16);
            this.label9.Text = "点菜数量：";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(144, 232);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(72, 24);
            this.button9.TabIndex = 4;
            this.button9.Text = "确定";
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // lvFoodTastes
            // 
            this.lvFoodTastes.CheckBoxes = true;
            this.lvFoodTastes.Location = new System.Drawing.Point(125, 72);
            this.lvFoodTastes.Name = "lvFoodTastes";
            this.lvFoodTastes.Size = new System.Drawing.Size(110, 152);
            this.lvFoodTastes.TabIndex = 5;
            this.lvFoodTastes.View = System.Windows.Forms.View.List;
            // 
            // lvFoodOperandis
            // 
            this.lvFoodOperandis.CheckBoxes = true;
            this.lvFoodOperandis.Location = new System.Drawing.Point(5, 72);
            this.lvFoodOperandis.Name = "lvFoodOperandis";
            this.lvFoodOperandis.Size = new System.Drawing.Size(110, 152);
            this.lvFoodOperandis.TabIndex = 6;
            this.lvFoodOperandis.View = System.Windows.Forms.View.List;
            // 
            // lblFoodName
            // 
            this.lblFoodName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.lblFoodName.Location = new System.Drawing.Point(0, 27);
            this.lblFoodName.Name = "lblFoodName";
            this.lblFoodName.Size = new System.Drawing.Size(240, 20);
            this.lblFoodName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblTableNo2
            // 
            this.lblTableNo2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTableNo2.Location = new System.Drawing.Point(0, 5);
            this.lblTableNo2.Name = "lblTableNo2";
            this.lblTableNo2.Size = new System.Drawing.Size(240, 20);
            this.lblTableNo2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlNeedSendFoods
            // 
            this.pnlNeedSendFoods.BackColor = System.Drawing.Color.Silver;
            this.pnlNeedSendFoods.Controls.Add(this.label27);
            this.pnlNeedSendFoods.Controls.Add(this.label26);
            this.pnlNeedSendFoods.Controls.Add(this.comboBox2);
            this.pnlNeedSendFoods.Controls.Add(this.label25);
            this.pnlNeedSendFoods.Controls.Add(this.label18);
            this.pnlNeedSendFoods.Controls.Add(this.btnSendFoods);
            this.pnlNeedSendFoods.Controls.Add(this.btnDelNeedSendFoods);
            this.pnlNeedSendFoods.Controls.Add(this.chkSelAll2);
            this.pnlNeedSendFoods.Controls.Add(this.lblTableNo3);
            this.pnlNeedSendFoods.Controls.Add(this.lvNeedSendFoods);
            this.pnlNeedSendFoods.Location = new System.Drawing.Point(0, 0);
            this.pnlNeedSendFoods.Name = "pnlNeedSendFoods";
            this.pnlNeedSendFoods.Size = new System.Drawing.Size(240, 270);
            this.pnlNeedSendFoods.Visible = false;
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(112, 48);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(112, 16);
            this.label27.Text = "label27";
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(64, 44);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(48, 16);
            this.label26.Text = "类别：";
            // 
            // comboBox2
            // 
            this.comboBox2.Location = new System.Drawing.Point(112, 24);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(112, 22);
            this.comboBox2.TabIndex = 2;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(64, 24);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(100, 16);
            this.label25.Text = "单据：";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(0, 256);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(88, 20);
            // 
            // btnSendFoods
            // 
            this.btnSendFoods.Location = new System.Drawing.Point(160, 232);
            this.btnSendFoods.Name = "btnSendFoods";
            this.btnSendFoods.Size = new System.Drawing.Size(72, 24);
            this.btnSendFoods.TabIndex = 5;
            this.btnSendFoods.Text = "发送";
            this.btnSendFoods.Click += new System.EventHandler(this.button17_Click);
            // 
            // btnDelNeedSendFoods
            // 
            this.btnDelNeedSendFoods.Location = new System.Drawing.Point(88, 232);
            this.btnDelNeedSendFoods.Name = "btnDelNeedSendFoods";
            this.btnDelNeedSendFoods.Size = new System.Drawing.Size(72, 24);
            this.btnDelNeedSendFoods.TabIndex = 6;
            this.btnDelNeedSendFoods.Text = "删除";
            this.btnDelNeedSendFoods.Click += new System.EventHandler(this.button11_Click);
            // 
            // chkSelAll2
            // 
            this.chkSelAll2.Location = new System.Drawing.Point(8, 40);
            this.chkSelAll2.Name = "chkSelAll2";
            this.chkSelAll2.Size = new System.Drawing.Size(48, 20);
            this.chkSelAll2.TabIndex = 8;
            this.chkSelAll2.Text = "全选";
            this.chkSelAll2.CheckStateChanged += new System.EventHandler(this.chkSelAll2_CheckStateChanged);
            // 
            // lblTableNo3
            // 
            this.lblTableNo3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTableNo3.Location = new System.Drawing.Point(0, 5);
            this.lblTableNo3.Name = "lblTableNo3";
            this.lblTableNo3.Size = new System.Drawing.Size(240, 20);
            this.lblTableNo3.Text = "1号桌台未发送菜品";
            this.lblTableNo3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lvNeedSendFoods
            // 
            this.lvNeedSendFoods.CheckBoxes = true;
            this.lvNeedSendFoods.Columns.Add(this.columnHeader1);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader2);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader3);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader4);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader5);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader8);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader6);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader7);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader17);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader16);
            this.lvNeedSendFoods.Columns.Add(this.columnHeader19);
            this.lvNeedSendFoods.Location = new System.Drawing.Point(0, 64);
            this.lvNeedSendFoods.Name = "lvNeedSendFoods";
            this.lvNeedSendFoods.Size = new System.Drawing.Size(240, 160);
            this.lvNeedSendFoods.TabIndex = 7;
            this.lvNeedSendFoods.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "菜品名称";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "数量";
            this.columnHeader2.Width = 45;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "单位";
            this.columnHeader3.Width = 40;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "单价";
            this.columnHeader4.Width = 60;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "合计";
            this.columnHeader5.Width = 60;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "做法";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "口味";
            this.columnHeader6.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "单据号";
            this.columnHeader7.Width = 80;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "guid";
            this.columnHeader17.Width = 0;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "foodcode";
            this.columnHeader16.Width = 0;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "suitsign";
            this.columnHeader19.Width = 0;
            // 
            // pnlAllOrderedFoods
            // 
            this.pnlAllOrderedFoods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.pnlAllOrderedFoods.Controls.Add(this.label23);
            this.pnlAllOrderedFoods.Controls.Add(this.cbAllBillList);
            this.pnlAllOrderedFoods.Controls.Add(this.label22);
            this.pnlAllOrderedFoods.Controls.Add(this.chkSelAll1);
            this.pnlAllOrderedFoods.Controls.Add(this.label19);
            this.pnlAllOrderedFoods.Controls.Add(this.label8);
            this.pnlAllOrderedFoods.Controls.Add(this.button14);
            this.pnlAllOrderedFoods.Controls.Add(this.button13);
            this.pnlAllOrderedFoods.Controls.Add(this.lblAllFoodsPrice);
            this.pnlAllOrderedFoods.Controls.Add(this.lvAllOrderedFoods);
            this.pnlAllOrderedFoods.Controls.Add(this.lblTableNo4);
            this.pnlAllOrderedFoods.Location = new System.Drawing.Point(0, 0);
            this.pnlAllOrderedFoods.Name = "pnlAllOrderedFoods";
            this.pnlAllOrderedFoods.Size = new System.Drawing.Size(240, 270);
            this.pnlAllOrderedFoods.Visible = false;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(120, 44);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(112, 16);
            this.label23.Text = "label23";
            // 
            // cbAllBillList
            // 
            this.cbAllBillList.Location = new System.Drawing.Point(120, 24);
            this.cbAllBillList.Name = "cbAllBillList";
            this.cbAllBillList.Size = new System.Drawing.Size(112, 22);
            this.cbAllBillList.TabIndex = 1;
            this.cbAllBillList.SelectedIndexChanged += new System.EventHandler(this.cbAllBillList_SelectedIndexChanged);
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(80, 44);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(48, 16);
            this.label22.Text = "类型：";
            // 
            // chkSelAll1
            // 
            this.chkSelAll1.Location = new System.Drawing.Point(8, 40);
            this.chkSelAll1.Name = "chkSelAll1";
            this.chkSelAll1.Size = new System.Drawing.Size(48, 20);
            this.chkSelAll1.TabIndex = 3;
            this.chkSelAll1.Text = "全选";
            this.chkSelAll1.CheckStateChanged += new System.EventHandler(this.chkSelAll1_CheckStateChanged);
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(80, 27);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(48, 16);
            this.label19.Text = "单据：";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(0, 256);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 20);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(168, 232);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(64, 24);
            this.button14.TabIndex = 6;
            this.button14.Text = "催菜";
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(104, 232);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(64, 24);
            this.button13.TabIndex = 7;
            this.button13.Text = "传菜";
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // lblAllFoodsPrice
            // 
            this.lblAllFoodsPrice.Location = new System.Drawing.Point(16, 216);
            this.lblAllFoodsPrice.Name = "lblAllFoodsPrice";
            this.lblAllFoodsPrice.Size = new System.Drawing.Size(216, 16);
            this.lblAllFoodsPrice.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lvAllOrderedFoods
            // 
            this.lvAllOrderedFoods.CheckBoxes = true;
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader9);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader10);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader11);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader13);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader14);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader15);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader18);
            this.lvAllOrderedFoods.Columns.Add(this.columnHeader12);
            this.lvAllOrderedFoods.Location = new System.Drawing.Point(0, 64);
            this.lvAllOrderedFoods.Name = "lvAllOrderedFoods";
            this.lvAllOrderedFoods.Size = new System.Drawing.Size(240, 152);
            this.lvAllOrderedFoods.TabIndex = 9;
            this.lvAllOrderedFoods.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "菜品名称";
            this.columnHeader9.Width = 140;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "数量";
            this.columnHeader10.Width = 45;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "单位";
            this.columnHeader11.Width = 40;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "合计";
            this.columnHeader13.Width = 60;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "做法";
            this.columnHeader14.Width = 100;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "口味";
            this.columnHeader15.Width = 100;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "barcode";
            this.columnHeader18.Width = 0;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "transfered";
            this.columnHeader12.Width = 0;
            // 
            // lblTableNo4
            // 
            this.lblTableNo4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblTableNo4.Location = new System.Drawing.Point(0, 5);
            this.lblTableNo4.Name = "lblTableNo4";
            this.lblTableNo4.Size = new System.Drawing.Size(232, 20);
            this.lblTableNo4.Text = "1号桌台已点全部菜品";
            this.lblTableNo4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlAbout
            // 
            this.pnlAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pnlAbout.Controls.Add(this.label16);
            this.pnlAbout.Controls.Add(this.label10);
            this.pnlAbout.Controls.Add(this.pictureBox1);
            this.pnlAbout.Controls.Add(this.button10);
            this.pnlAbout.Controls.Add(this.label12);
            this.pnlAbout.Controls.Add(this.label13);
            this.pnlAbout.Location = new System.Drawing.Point(0, 0);
            this.pnlAbout.Name = "pnlAbout";
            this.pnlAbout.Size = new System.Drawing.Size(240, 270);
            this.pnlAbout.Visible = false;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(23, 189);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(180, 16);
            this.label16.Text = "网址：www.carreygroup.com";
            this.label16.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(24, 162);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(180, 16);
            this.label10.Text = "北京凯睿瑞驰科技有限公司";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(88, 106);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 40);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(144, 232);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(72, 24);
            this.button10.TabIndex = 7;
            this.button10.Text = "确定";
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(16, 60);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(208, 20);
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(0, 27);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(240, 20);
            this.label13.Text = "餐厅管家";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.Add(this.toolBarButton1);
            this.toolBar1.Buttons.Add(this.toolBarButton2);
            this.toolBar1.Buttons.Add(this.toolBarButton3);
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.ImageIndex = 0;
            // 
            // toolBarButton2
            // 
            this.toolBarButton2.ImageIndex = 1;
            // 
            // toolBarButton3
            // 
            this.toolBarButton3.ImageIndex = 2;
            this.imageList1.Images.Clear();
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            this.imageList1.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            // 
            // pnlNeedConfirm
            // 
            this.pnlNeedConfirm.BackColor = System.Drawing.Color.RosyBrown;
            this.pnlNeedConfirm.Controls.Add(this.label36);
            this.pnlNeedConfirm.Controls.Add(this.panel3);
            this.pnlNeedConfirm.Controls.Add(this.textBox3);
            this.pnlNeedConfirm.Controls.Add(this.textBox2);
            this.pnlNeedConfirm.Controls.Add(this.label35);
            this.pnlNeedConfirm.Controls.Add(this.label34);
            this.pnlNeedConfirm.Controls.Add(this.label33);
            this.pnlNeedConfirm.Controls.Add(this.label32);
            this.pnlNeedConfirm.Controls.Add(this.label31);
            this.pnlNeedConfirm.Controls.Add(this.label30);
            this.pnlNeedConfirm.Controls.Add(this.label29);
            this.pnlNeedConfirm.Controls.Add(this.label28);
            this.pnlNeedConfirm.Controls.Add(this.button15);
            this.pnlNeedConfirm.Controls.Add(this.button12);
            this.pnlNeedConfirm.Location = new System.Drawing.Point(0, 0);
            this.pnlNeedConfirm.Name = "pnlNeedConfirm";
            this.pnlNeedConfirm.Size = new System.Drawing.Size(240, 270);
            this.pnlNeedConfirm.Visible = false;
            // 
            // label36
            // 
            this.label36.Location = new System.Drawing.Point(0, 256);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(80, 20);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Location = new System.Drawing.Point(16, 136);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(208, 3);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(80, 176);
            this.textBox3.Name = "textBox3";
            this.textBox3.PasswordChar = '*';
            this.textBox3.Size = new System.Drawing.Size(100, 21);
            this.textBox3.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(80, 152);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 3;
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(32, 176);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(64, 16);
            this.label35.Text = "密码：";
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(32, 152);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(64, 16);
            this.label34.Text = "用户ID：";
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular);
            this.label33.Location = new System.Drawing.Point(8, 16);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(224, 20);
            this.label33.Text = "确认/验证赠送操作";
            this.label33.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label32
            // 
            this.label32.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label32.Location = new System.Drawing.Point(8, 112);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(224, 16);
            this.label32.Text = "label32";
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label31.Location = new System.Drawing.Point(8, 96);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(224, 16);
            this.label31.Text = "label31";
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label30.Location = new System.Drawing.Point(8, 80);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(224, 16);
            this.label30.Text = "label30";
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label29.Location = new System.Drawing.Point(8, 64);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(224, 16);
            this.label29.Text = "label29";
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.label28.Location = new System.Drawing.Point(8, 48);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(224, 16);
            this.label28.Text = "label28";
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(152, 208);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(72, 24);
            this.button15.TabIndex = 12;
            this.button15.Text = "取消";
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(80, 208);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(72, 24);
            this.button12.TabIndex = 13;
            this.button12.Text = "确定";
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.pnlLogOn);
            this.Controls.Add(this.pnlNeedConfirm);
            this.Controls.Add(this.pnlTableList);
            this.Controls.Add(this.pnlAbout);
            this.Controls.Add(this.pnlAllOrderedFoods);
            this.Controls.Add(this.pnlFoodList);
            this.Controls.Add(this.pnlOrderFood);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.pnlNeedSendFoods);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form1_Closing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.pnlLogOn.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlTableList.ResumeLayout(false);
            this.pnlFoodList.ResumeLayout(false);
            this.pnlFoods.ResumeLayout(false);
            this.pnlSuits.ResumeLayout(false);
            this.pnlOrderFood.ResumeLayout(false);
            this.pnlNeedSendFoods.ResumeLayout(false);
            this.pnlAllOrderedFoods.ResumeLayout(false);
            this.pnlAbout.ResumeLayout(false);
            this.pnlNeedConfirm.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>

		static void Main() 
		{
			Application.Run(new Form1());
		}

		//获取数据
		//queryfield1 = tableno
		//queryfield2 = billno
		//queryfield3 = 菜品编码 菜品名称 类别名称 拼音码 tabletypename

		private string GetServerData(int target, Label progress, int PageNo, DataGrid dg, 
			                         string queryfield1,string queryfield2,string queryfield3)
		{
			if (progress != null)
			{
				progress.Text = "";
				progress.Refresh();
			}            
			string xmlstring = "";
            ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar1 = null;
			switch (target)
			{                    
				case 0: //获取桌台数据
					ar1 = rs.BeginGetData(target, TableItemCountOfPage, PageNo, "", "",queryfield3, null, null);
					break;
				case 1: //获取菜品数据
					ar1 = rs.BeginGetData(target, FoodItemCountOfPage, PageNo, "", queryfield2, "", null, null);
					break;
				case 2: //获取菜品做法数据
					ar1 = rs.BeginGetData(target, FoodOperandiItemCountOfPage, PageNo, "", "","", null, null);
					break;
				case 3: //获取菜品口味数据
					ar1 = rs.BeginGetData(target, FoodTasteItemCountOfPage, PageNo, "","","", null, null);
					break;
				case 4: //返回桌台消费单据记录
                case 5: //返回桌台所有单据记录 包含退菜单、赠送单等
					ar1 = rs.BeginGetData(target, 0, 1, queryfield1, "","", null, null);
					break;
				case 6: //返回桌台所有已点单据和菜品
					ar1 = rs.BeginGetData(target, 0, 1, queryfield1, queryfield2,"", null, null);
					break;
				case 7: //返回所有单据类别
					ar1 = rs.BeginGetData(target, 0, 1, "", "","", null, null);
					break;
				case 8: //菜品编码 查询
				case 9: //菜品名称 查询
				case 10: //类别名称 查询
				case 11: //拼音码 查询
					ar1 = rs.BeginGetData(target, FoodItemCountOfPage, PageNo, "", queryfield2,queryfield3, null, null);
					break;
				case 12: //菜品类别
					ar1 = rs.BeginGetData(target, 0, 1, queryfield1, "", "" , null, null);
					break;
				case 13: //桌台类别
					ar1 = rs.BeginGetData(target, 0, 1, "", "", "" , null, null);
					break;
				case 14: //套餐
					ar1 = rs.BeginGetData(target,SuitItemCountOfPage, PageNo, "", "", "", null, null);
					break;
				case 15: //部门
					ar1 = rs.BeginGetData(target,SuitItemCountOfPage, PageNo, "", "", "", null, null);
					break;
			}            
			int p1 = 1;
			while (!ar1.IsCompleted)
			{
				p1++;
				if (progress != null)
				{
					progress.Text = p1.ToString();
					progress.Refresh();
				}                
			}
			xmlstring = rs.EndGetData(ar1);
			switch (target)
			{
				case 2:  //获取菜品做法数据
					FoodOperandisXmlData= xmlstring;
					break;
				case 3:  //获取菜品口味数据
					FoodTastesXmlData = xmlstring;
					break;
				case 7:  //返回所有单据类别数据
					BillTypeXmlData = xmlstring;
					break;
				case 13: //返回桌台类别数据
					TableTypeNameXmlData = xmlstring;
					break;
			}
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);			 
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			if (dg != null)
			{
				if (xmlstring=="<ds /?")
					dg.DataSource =null;
				else
				{
					if (objDs.Tables.Count > 0)
					{
						dg.DataSource = objDs.Tables[0];
					}
					else
						dg.DataSource =null;
				}				
			}            
			return xmlstring;
		}

		private void LoadBillTypeNameData()
		{
			TextReader txtReader;
			txtReader = new StringReader(BillTypeXmlData);			 
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			comboBox1.Items.Clear();
			for (int i=0 ;i<=objDs.Tables[0].Rows.Count-1;i++)
			{
				comboBox1.Items.Add(objDs.Tables[0].Rows[i]["name"].ToString());
			}
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
            //MessageBox.Show("Hi!");
            WebUrl = "http://192.168.1.166/ROSS.asmx";
			if (txtUsername.Text!="")
			{
				Cursor.Current = Cursors.WaitCursor;
				try
				{
					label37.Text = "连接 "+WebUrl+"....";
					label37.Refresh();

                    ROSS.Service rs = new ROSS.Service();
                    
                	rs.Url = WebUrl;
                    
					IAsyncResult ar = rs.BeginLogOn(txtUsername.Text, txtPassword.Text,false, null, null);
					int p = 1;
					while (!ar.IsCompleted)
					{
						p++;
						label4.Text = p.ToString();
						label4.Refresh();
					}
					int rtn = rs.EndLogOn(ar);
					if (rtn==0)
					{
						MessageBox.Show("登录失败！请检查用户名和密码输入是否正确。","提示", MessageBoxButtons.OK ,MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						label37.Text ="";
						label37.Refresh();
					}
					else if (rtn==1)
					{
						LoggedOff=false;
						MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
						label37.Text ="";
						label37.Refresh();
						CurrentLogOnUserID = txtUsername.Text;
						this.Refresh();
						//获取桌台列表
						//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
						LoadTableTypeName(label4);
						cbTableTypeName.Text = "全部";
						LoadTableListToPanel(label4,1,cbTableTypeName.Text);
						//获取单据类别记录
						if (BillTypeXmlData=="") 
						{
							GetServerData(7, label4, 0, null, "", "", "");
						}
						LoadBillTypeNameData();
						LoadDeptNameData(label4);
						LoadFoodTypeNameData(label4,cbDept.Text);
						Cursor.Current = Cursors.Default;
						pnlLogOn.Hide();
						pnlTableList.Show();
						pnlTableList.BringToFront();
						PriorPanel=pnlTableList;
					}
					else
					{
						MessageBox.Show("服务器验证失败！请检查服务器配置是否正确。（"+rtn.ToString()+"）", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						label37.Text ="";
						label37.Refresh();
					}
				}
                catch (Exception err)
				{
                    MessageBox.Show("Debug: "+err.Message);

					Cursor.Current = Cursors.Default;
					MessageBox.Show("连接服务器失败，请检查设备和服务器的网络配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					label37.Text ="";
					label37.Refresh();
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}
			}			
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			if (!(TableList.Count < TableItemCountOfPage))
			{
				Cursor.Current = Cursors.WaitCursor;
				//获取后一页桌台列表
				//GetServerData(0, label5, CurrentTablePageNo + 1, dgTablelist, "", "");
				if (LoadTableListToPanel(label5, CurrentTablePageNo + 1,cbTableTypeName.Text))
				{
					CurrentTablePageNo = CurrentTablePageNo + 1;
				}				
				Cursor.Current = Cursors.Default;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			if (CurrentTablePageNo != 1)
			{
				Cursor.Current = Cursors.WaitCursor;
				//获取前一页桌台列表
				if (LoadTableListToPanel(label5, CurrentTablePageNo - 1,cbTableTypeName.Text ))
				{
					CurrentTablePageNo = CurrentTablePageNo - 1;
				}				
				Cursor.Current = Cursors.Default;
			}
		}

		private void btnOpenTable_Click(object sender, System.EventArgs e)
		{			
			//确定选中了一个桌台
			if (CurrentSelectedTableno!="")
			{
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				if (MessageBox.Show("确定要对 "+currentTableno+" 号桌台进行开台操作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
                    ROSS.Service rs = new ROSS.Service();
					rs.Url = WebUrl;
					IAsyncResult ar = rs.BeginOpenTable(currentTableno, Convert.ToInt32(numPsnCount.Value), CurrentLogOnUserID, null, null);
					int p = 1;
					int rtn;
					while (!ar.IsCompleted)
					{
						p++;
						label5.Text = p.ToString();
						label5.Refresh();
					}
					//0 = 成功 1 = 使用中 2 = 预订中 3 = 加单失败 存在同号单据 4 = 加单失败 未知 5 = 未知错误
					rtn=rs.EndOpenTable(ar); 
					switch (rtn)
					{
						case 0: //成功
							Cursor.Current = Cursors.WaitCursor;
							//清空桌台未发送菜品记录
							DelAllNeedSendFoods(currentTableno);
							MessageBox.Show("开台操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
							//重新获取当前页桌台列表
							//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
							LoadTableListToPanel(label4, CurrentTablePageNo,cbTableTypeName.Text );
							Cursor.Current = Cursors.Default;
							break;
						case 1: //使用中
							MessageBox.Show(currentTableno+" 号桌台为使用中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 2: //预订中
							MessageBox.Show(currentTableno+" 号桌台为预订中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 3: //加单失败 存在同号单据
							MessageBox.Show("开台成功，但首张单据添加失败，原因为已经存在同号单据，请手动添加单据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 4: //4 = 加单失败 未知
							MessageBox.Show("开台成功，但首张单据添加失败，请手动添加单据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 5: //5 = 未知错误
							MessageBox.Show("开台失败！稍候请重新尝试开台操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
					}
				}
			} 
		}

		private int GetServerTableStatus(string tableno)
		{
            ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginGetTableStatus(tableno, null, null);
			int p = 1;
			int rtn;
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndGetTableStatus(ar);
			return rtn;
		}

		private void LoadDeptNameData(Label progress)
		{
			string xmlstring;
			if (DeptNameXmlData=="")
			{
				xmlstring = GetServerData(15, progress, 1, null, "", "", "");			
			}
			else
			{
				xmlstring = DeptNameXmlData;
			}
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);			 
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			cbDept.Items.Clear();
			if (xmlstring!="<ds />")
			{
				for (int i=0 ;i<=objDs.Tables[0].Rows.Count-1;i++)
				{
					cbDept.Items.Add(objDs.Tables[0].Rows[i][0].ToString());
				}
				cbDept.SelectedIndex=0;
			}			
		}

		private void LoadFoodTypeNameData(Label progress, string deptname)
		{
			string xmlstring;
			if (FoodTypeNameXmlData=="")
			{
				xmlstring = GetServerData(12, progress, 1, null, deptname, "", "");	
			}
			else
			{
				xmlstring = FoodTypeNameXmlData;
			}
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);			 
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			cbFoodType.Items.Clear();
			if (xmlstring!="<ds />")
			{
				for (int i=0 ;i<=objDs.Tables[0].Rows.Count-1;i++)
				{
					cbFoodType.Items.Add(objDs.Tables[0].Rows[i][0].ToString());
				}
				cbFoodType.SelectedIndex=0;
			}			
		}

		private void btnSelectFood_Click(object sender, System.EventArgs e)
		{
			//确定选中了一个桌台
			if (CurrentSelectedTableno!="")
			{
				//确保当前选中桌台是使用状态
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				int currentTableStatus = GetServerTableStatus(currentTableno);//Convert.ToInt32(dgTablelist[dgTablelist.CurrentRowIndex, 4].ToString());
                string rtn;
				switch (currentTableStatus)
				{
					case 0: //空闲
						if (MessageBox.Show(currentTableno+" 号桌台当前为空闲，点菜前需要将其更新为开台状态，确定要对当前选中桌台进行开台操作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
                            ROSS.Service rs = new ROSS.Service();
							rs.Url = WebUrl;
							IAsyncResult ar = rs.BeginOpenTable(currentTableno, Convert.ToInt32(numPsnCount.Value), CurrentLogOnUserID, null, null);
							int p = 1;
							int rtn1;
							while (!ar.IsCompleted)
							{
								p++;
								label5.Text = p.ToString();
								label5.Refresh();
							}
							rtn1=rs.EndOpenTable(ar); //0 = 成功 1 = 使用中 2 = 预订中 3 = 未知
							switch (rtn1)
							{
								case 0: //成功
									Cursor.Current = Cursors.WaitCursor;
									//清空桌台未发送菜品记录
									DelAllNeedSendFoods(currentTableno);
									MessageBox.Show("开台操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
									//重新获取当前页桌台列表
									//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
									LoadTableListToPanel(label4,CurrentTablePageNo,cbTableTypeName.Text );
									Cursor.Current = Cursors.Default;
									break;
								case 1: //使用中
									MessageBox.Show(currentTableno+" 号桌台为使用中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
								case 2: //预订中
									MessageBox.Show(currentTableno+" 号桌台为预订中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
								case 3: //未知
									MessageBox.Show("未知错误，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
							}
						}  
						break;
					case 1: //使用中
						Cursor.Current = Cursors.WaitCursor;
						lblTableNo1.Text = "请为 " + currentTableno + " 号桌台点菜";
						//获取当前桌台所有单据列表 4 = 消费单据 5 = 所有单据
						string xmlBillList = GetServerData(5, label5, 1, null, currentTableno, "", "");
						TextReader txtReader;
						txtReader = new StringReader(xmlBillList);
						XmlTextReader xmlReader = new XmlTextReader(txtReader);
						DataSet objDs = new DataSet("ds");
						objDs.ReadXml(xmlReader);
						int RecCount = objDs.Tables[0].Rows.Count;
						cbBilllist.Items.Clear();
						for (int i = 0; i <= RecCount - 1; i++)
						{
							cbBilllist.Items.Add(objDs.Tables[0].Rows[i][0].ToString());
						}
						cbBilllist.SelectedIndex = 0;
						//返回当前单据号的类型
                        ROSS.Service rs1 = new ROSS.Service();
						rs1.Url = WebUrl;
						IAsyncResult ar1 = rs1.BeginGetBillTypeName(cbBilllist.Text, null, null);
						int p1 = 1;
						while (!ar1.IsCompleted)
						{
							p1++;
						}
						rtn = rs1.EndGetBillTypeName(ar1);
						label21.Text =rtn;
						cbFoodQuery.SelectedIndex = 0;
						//LoadDeptNameData();
                        //LoadFoodTypeNameData(label5,cbDept.Text);
						//获取菜品信息
						//GetServerData(1, label5, CurrentFoodPageNo, dgFoodlist, "","吧台咖啡" , "");
						//cbFoodQuery.SelectedIndex=0;
						Cursor.Current = Cursors.Default;
						pnlFoodList.Show();
						pnlFoodList.BringToFront();
						PriorPanel=pnlTableList;
						break;
					case 2: //预订中
						MessageBox.Show(currentTableno+" 号桌台的当前状态为预订中，请将其状态更新为开台！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
				}                
			}
			else
				MessageBox.Show("没有选中桌台，请首先选中需要操作的桌台！！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}

		private void button6_Click(object sender, System.EventArgs e)
		{
			if (pnlFoods.Visible) //单品
			{
				if (CurrentFoodPageNo != 1)
				{
					Cursor.Current = Cursors.WaitCursor;
					//获取前一页菜品信息
					//GetServerData(1, label6, CurrentFoodPageNo - 1, dgFoodlist, "", "", "");
					switch (cbFoodQuery.SelectedIndex)
					{
						case 0: //菜品编码
							if (txtFoodQuery.Text =="")
								GetServerData(1, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , "");
							else
							    GetServerData(8, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						case 1: //菜品名称
							if (txtFoodQuery.Text =="")
								GetServerData(1, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , "");
							else
								GetServerData(9, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						case 3: //拼音码
							if (txtFoodQuery.Text =="")
								GetServerData(1, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , "");
							else
								GetServerData(11, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						default:
							GetServerData(1, label6, CurrentFoodPageNo - 1, dgFoodlist, "", cbFoodType.Text , "");
							break;
					}
					CurrentFoodPageNo = CurrentFoodPageNo - 1;
					Cursor.Current = Cursors.Default;
				}
			}
			else //套餐
			{
				if (CurrentSuitPageNo != 1)
				{
					Cursor.Current = Cursors.WaitCursor;
					GetServerData(14, label6, CurrentSuitPageNo-1, dgSuitList, "", "", "");
					CurrentSuitPageNo=CurrentSuitPageNo-1;
					Cursor.Current = Cursors.Default;
				}
			}
			
		}

		private void button7_Click(object sender, System.EventArgs e)
		{
			if (pnlFoods.Visible) //单品
			{
				if (!(dgFoodlist.VisibleRowCount < FoodItemCountOfPage))
				{
					Cursor.Current = Cursors.WaitCursor;
					//获取后一页菜品信息
					//GetServerData(1, label6, CurrentFoodPageNo + 1, dgFoodlist, "", "", "");
					switch (cbFoodQuery.SelectedIndex)
					{
						case 0: //菜品编码
							if (txtFoodQuery.Text =="")
								GetServerData(1, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , "");
							else
								GetServerData(8, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						case 1: //菜品名称
							if (txtFoodQuery.Text =="")
                                GetServerData(1, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , "");
							else
								GetServerData(9, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						case 3: //拼音码
							if (txtFoodQuery.Text =="")
								GetServerData(1, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , "");
							else
							    GetServerData(11, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , txtFoodQuery.Text);
							break;
						default:
							GetServerData(1, label6, CurrentFoodPageNo + 1, dgFoodlist, "", cbFoodType.Text , "");
							break;
					}	
					CurrentFoodPageNo = CurrentFoodPageNo + 1;
					Cursor.Current = Cursors.Default;
				}
			}
			else //套餐
			{
				if (!(dgSuitList.VisibleRowCount < SuitItemCountOfPage))
				{
					Cursor.Current = Cursors.WaitCursor;
					GetServerData(14, label6, CurrentSuitPageNo+1, dgSuitList, "", "", "");
					CurrentSuitPageNo=CurrentSuitPageNo+1;
					Cursor.Current = Cursors.Default;
				}				
			}
			
		}

		private void LoadFoodOperandis()
		{
			//获取菜品做法
			string xmlstring;
			if (FoodOperandisXmlData=="")
			{
				xmlstring = GetServerData(2, label6, CurrentFoodOperandiPageNo, null, "", "", "");				
			}
			else
			{
				xmlstring = FoodOperandisXmlData;
			}
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);            
			int itemcount = objDs.Tables[0].Rows.Count;
			ListViewItem[] lvitems=new ListViewItem[itemcount];
			lvFoodOperandis.Items.Clear();
			for (int i = 0; i <= itemcount - 1; i++)
			{
				lvitems[i] = new ListViewItem();
				lvitems[i].Text = objDs.Tables[0].Rows[i][0].ToString();
				lvFoodOperandis.Items.Add(lvitems[i]);
			}
		}

		private void LoadFoodTastes()
		{
			//获得菜品口味
			string xmlstring;
			if (FoodTastesXmlData=="")
			{
				xmlstring = GetServerData(3, label6, CurrentFoodTastePageNo, null, "", "", "");				
			}
			else
			{
				xmlstring = FoodTastesXmlData;
			}
			
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			int itemcount = objDs.Tables[0].Rows.Count;
			ListViewItem[] lvitems = new ListViewItem[itemcount];
			lvFoodTastes.Items.Clear();
			for (int i = 0; i <= itemcount - 1; i++)
			{
				lvitems[i] = new ListViewItem();
				lvitems[i].Text = objDs.Tables[0].Rows[i][0].ToString();
				lvFoodTastes.Items.Add(lvitems[i]);
			}
		}

		private bool CurrentTableExistFood(string tableno,string foodname)
		{
            ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginCurrentTableExistFood(tableno,foodname,null, null);
			int p = 1;
			while (!ar.IsCompleted)
			{
				p++;
			}
			return rs.EndCurrentTableExistFood(ar);
		}	

		private double GetCurrentTableAllowCancelFoodQty(string tableno,string foodname)
		{
            ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginGetCurrentTableAllowCancelFoodQty(tableno,foodname,null, null);
			int p = 1;
			while (!ar.IsCompleted)
			{
				p++;
			}
			return rs.EndGetCurrentTableAllowCancelFoodQty(ar);
		}	

		private void AddFoodToCancelBill(string tableno,string billno)
		{
			//先检测当前桌台有无该菜品，有则允许减菜，并返回已经点菜的数量
			if (CurrentTableExistFood(tableno,dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString()))
			{
				MaxCancelCount = GetCurrentTableAllowCancelFoodQty(tableno, dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString());
				lvFoodOperandis.Enabled = false;
				lvFoodTastes.Enabled = false;
				lblTableNo2.Text = lblTableNo1.Text;
				lblFoodName.Text = dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString();
				lblFoodUnit.Text = dgFoodlist[dgFoodlist.CurrentRowIndex, 4].ToString();
				pnlOrderFood.Show();
				pnlOrderFood.BringToFront();
				PriorPanel=pnlFoodList;
			}
			else
				MessageBox.Show(tableno+" 号桌台没有选中的菜品，退菜操作无法进行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
		}

		private void AddFoodToConsumeBill(string tableno,string billno)
		{
			lblTableNo2.Text = lblTableNo1.Text;
			lblFoodName.Text = dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString();
			lblFoodUnit.Text = dgFoodlist[dgFoodlist.CurrentRowIndex, 4].ToString();
			txtFoodPrice.Text = dgFoodlist[dgFoodlist.CurrentRowIndex, 3].ToString();
			if (IsCustomFood(dgFoodlist[dgFoodlist.CurrentRowIndex, 1].ToString()))
			{
				txtFoodPrice.Visible =true;
			}
			LoadFoodOperandis();
			LoadFoodTastes();
			pnlOrderFood.Show();
			pnlOrderFood.BringToFront();
			PriorPanel=pnlFoodList;
		}

		private void button8_Click(object sender, System.EventArgs e)
		{
			if (pnlFoods.Visible) //单品
			{
				Cursor.Current = Cursors.WaitCursor;
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				//检测当前单据类型
				if (IsCancelBill(cbBilllist.Text)) //取消单
				{
					//验证有无退菜权利
					if (AllowCancelFood(CurrentLogOnUserID))
					{
						AddFoodToCancelBill(currentTableno,cbBilllist.Text);
					}
					else
                        MessageBox.Show("您没有退菜权利，无法完成向取消单点菜的操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
				else if (IsPresentBill(cbBilllist.Text)) //赠送单
				{
					//验证有无赠送权利
					//string rtn;
					//rtn = AllowPresentFood(CurrentLogOnUserID);
					//string[] rtnrec=rtn.Split(',');
					//if (rtnrec[0]=="True") //允许赠送
					//{
						//if (Convert.ToDouble(rtnrec[2])>Convert.ToDouble(rtnrec[1])) //达到赠送限额
						//{
                    //	MessageBox.Show("达到或者超过赠送限额，无法完成向赠送单点菜的操作！","提示",MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
						//}
                    //else //没达到限额，输入验证用户密码，通过后执行点菜
						//{
							//ShowConfirmPanel = true;
							AddFoodToConsumeBill(currentTableno,cbBilllist.Text);
						//}					
					//}
					//else
                            //	MessageBox.Show("您没有赠菜权利，无法完成向赠送单点菜的操作！","提示",MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
				else //消费单
				{
					AddFoodToConsumeBill(currentTableno,cbBilllist.Text);
				}
				Cursor.Current = Cursors.Default;
			} //套餐
			else
			{
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				Cursor.Current = Cursors.WaitCursor;
				//保存未发送菜品信息
				AddFoodToNeedSend(currentTableno,cbBilllist.Text,dgSuitList[dgSuitList.CurrentRowIndex, 1].ToString(),
					dgSuitList[dgSuitList.CurrentRowIndex, 2].ToString(),
					"套餐",
					"套",
					Convert.ToDouble(dgSuitList[dgSuitList.CurrentRowIndex, 3].ToString()),
					"",
					"",
					numericUpDown1.Value.ToString(),
					0,CurrentLogOnUserID,1);
				Cursor.Current = Cursors.Default;
				MessageBox.Show("套餐 "+dgFoodlist[dgFoodlist.CurrentRowIndex, 5].ToString()+" 已经被成功保存到未发送菜品列表中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				pnlFoodList.Show();
				pnlFoodList.BringToFront();
				PriorPanel=pnlFoodList;
			}			
		}

		private string NewGuid()
		{
            ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginGetNewGuid(null, null);
			int p = 1;
			while (!ar.IsCompleted)
			{
				p++;
			}
			return rs.EndGetNewGuid(ar);
		}

		private bool AddFoodToNeedSend(string tableno,string billno,string foodcode,
			                           string foodname,string foodtypename,string foodunit,
			                           double price,string operandis,string tastes,
			                           string quantity,int sign,string opid,int suitsign)
		{
			string guidstring = NewGuid();
			//保存未发送菜品信息
			foodinfo fi = new foodinfo(guidstring);
			fi.m_tableno = tableno;
			fi.m_billno = billno;
			fi.m_foodcode = foodcode;
			fi.m_fooaname = foodname;
			fi.m_foodtypename = foodtypename;
			fi.m_foodunit =foodunit;
			fi.m_price = price;
			fi.m_operandis = operandis;
			fi.m_tastes = tastes;
			fi.m_quantity = quantity;
			fi.m_sub_price = price * Convert.ToDouble(quantity);
			fi.m_sign = 0;
			fi.m_opid = CurrentLogOnUserID;
			fi.m_suitsign=suitsign; 
			NeedSendFoods.Add(fi);
			return true;
		}

		private void button9_Click(object sender, System.EventArgs e)
		{
			//如果 ShowConfirmPanel = true，需要显示验证窗口，通过后进行下一步
			if (ShowConfirmPanel)
			{
				double ordercount = 0;
				try
				{
					ordercount = Convert.ToDouble(textBox4.Text);
				}
				catch
				{
					MessageBox.Show("菜品数量输入有误，请核对后重新输入。");
					return;
				}
				double foodprice = 0;
				try
				{
					foodprice = Convert.ToDouble(txtFoodPrice.Text);
				}
				catch
				{
					MessageBox.Show("菜品价格输入有误，请核对后重新输入。");
					return;
				}	
				label28.Text = "菜品名称："+dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString();
				label29.Text = "数量："+ordercount.ToString();
				label30.Text = "单位："+dgFoodlist[dgFoodlist.CurrentRowIndex, 4].ToString();
				label31.Text = "单价："+foodprice.ToString();
				label32.Text = "合计："+Convert.ToString(ordercount*Convert.ToDouble(dgFoodlist[dgFoodlist.CurrentRowIndex, 3].ToString()));
				pnlNeedConfirm.Show();
				pnlNeedConfirm.BringToFront();
			}
			else //无需显示
			{
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				string operandis="", tastes="";
				Cursor.Current = Cursors.WaitCursor;
				for (int i = 0; i <= lvFoodOperandis.Items.Count - 1; i++)
				{
					if (lvFoodOperandis.Items[i].Checked)
					{
						if (operandis.Trim() != "")
							operandis = operandis + "," + lvFoodOperandis.Items[i].Text;
						else
							operandis = lvFoodOperandis.Items[i].Text;
					}
				}
				for (int i = 0; i <= lvFoodTastes.Items.Count - 1; i++)
				{
					if (lvFoodTastes.Items[i].Checked)
					{
						if (tastes.Trim() != "")
							tastes = tastes + "," + lvFoodTastes.Items[i].Text;
						else
							tastes = lvFoodTastes.Items[i].Text;
					}
				}
				//保存未发送菜品信息
				AddFoodToNeedSend(currentTableno,cbBilllist.Text,dgFoodlist[dgFoodlist.CurrentRowIndex, 1].ToString(),
					               dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString(),
					               dgFoodlist[dgFoodlist.CurrentRowIndex, 5].ToString(),
					               dgFoodlist[dgFoodlist.CurrentRowIndex, 4].ToString(),
					               Convert.ToDouble(dgFoodlist[dgFoodlist.CurrentRowIndex, 3].ToString()),
					               operandis,
					               tastes,
					               textBox4.Text,
					               0,CurrentLogOnUserID,0);
				Cursor.Current = Cursors.Default;
				MessageBox.Show("菜品 "+dgFoodlist[dgFoodlist.CurrentRowIndex, 5].ToString()+" 已经被成功保存到未发送菜品列表中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				pnlFoodList.Show();
				pnlFoodList.BringToFront();
				PriorPanel=pnlFoodList;
			}			
		}

		//回首页
		private void BackToStart()
		{
			pnlTableList.Show();
			pnlTableList.BringToFront();
		}

		//注销登录
		private void ReLogOn()
		{
			pnlLogOn.Show();
			pnlLogOn.BringToFront();
		}

		private void LoadNeedSendFoodsToListView(string tableno,string billno)
		{
			//获取未发送菜品内容 sign = 0 未发送 sign = 1 已经发送
			string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
			foodinfo tmp=new foodinfo("");
			lvNeedSendFoods.Items.Clear();
			for (int i=0;i<=NeedSendFoods.Count-1;i++)
			{
				tmp=(foodinfo)NeedSendFoods[i];
				//当前桌台存在未发送菜品记录将该记录插入listview
				if (tmp.m_tableno==tableno && tmp.m_sign==0 && tmp.m_billno==billno) 
				{
					ListViewItem lvitem = new ListViewItem();
					lvitem.Text = tmp.m_fooaname; //foodname
					lvitem.SubItems.Add(tmp.m_quantity); //quantity
					lvitem.SubItems.Add(tmp.m_foodunit); //foodunit
					lvitem.SubItems.Add(tmp.m_price.ToString("#,##0.00")); //price
					lvitem.SubItems.Add(tmp.m_sub_price.ToString("#,##0.00")); //subprice
					lvitem.SubItems.Add(tmp.m_operandis); //operandis
					lvitem.SubItems.Add(tmp.m_tastes); //tastes
					lvitem.SubItems.Add(tmp.m_billno); //bilno
					lvitem.SubItems.Add(tmp.m_guid); //guid
					lvitem.SubItems.Add(tmp.m_foodcode); //foodcode
					lvitem.SubItems.Add(tmp.m_suitsign.ToString()); //suitsign
					lvNeedSendFoods.Items.Add(lvitem);
				}
			}
		}

		private void btnNeedSendFoods_Click(object sender, System.EventArgs e)
		{
			if (CurrentSelectedTableno!="")
			{
				//确保当前选中桌台是空闲状态
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				int currentTableStatus = GetServerTableStatus(currentTableno);//Convert.ToInt32(dgTablelist[dgTablelist.CurrentRowIndex, 4].ToString());

				switch (currentTableStatus)
				{
					case 0: //空闲
						MessageBox.Show(currentTableno+" 号桌台的当前状态为空闲，不能查看未发送菜品。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
					case 1: //使用中
						Cursor.Current = Cursors.WaitCursor;
						LoadTableBills(currentTableno,comboBox2);
						LoadNeedSendFoodsToListView(currentTableno,comboBox2.Text);
						//返回单据类别
                        ROSS.Service rs = new ROSS.Service();
						rs.Url = WebUrl;
						IAsyncResult ar = rs.BeginGetBillTypeName(comboBox2.Text, null, null);
						int p = 1;
						while (!ar.IsCompleted)
						{
							p++;
						}
						string rtn = rs.EndGetBillTypeName(ar);
						label27.Text =rtn;	
						chkSelAll2.Checked = false;
						lblTableNo3.Text = currentTableno + " 号桌台未发送菜品";
						label18.Text = "";
						Cursor.Current = Cursors.Default;
						pnlNeedSendFoods.Show();
						pnlNeedSendFoods.BringToFront();
						PriorPanel=pnlTableList;						
						break;
					case 2: //预订中
						MessageBox.Show(currentTableno+" 号桌台的当前状态为预订中，不能查看未发送菜品。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
				}
			}
		}

		//获取桌台所有单据
		private void LoadTableBills(string tableno,ComboBox cb)
		{
			string xmlstring = GetServerData(5, label5, 1, null, tableno, "","");
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			int itemcount = objDs.Tables[0].Rows.Count;

			cb.Items.Clear();
			for (int i = 0; i <= itemcount - 1; i++)
			{
				cb.Items.Add(objDs.Tables[0].Rows[i][0].ToString());
			}
			cb.SelectedIndex = 0;
		}

		//获取桌台所有已点菜品
		private void LoadBillFoodsToListView(Label progress,string tableno,string billno)
		{
			string xmlstring = GetServerData(6, progress, 1, null, tableno, billno,"");
			TextReader txtReader;
			txtReader = new StringReader(xmlstring);
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet ds = new DataSet("ds");
			ds.ReadXml(xmlReader);
			lvAllOrderedFoods.Items.Clear();
			lblAllFoodsPrice.Text = "";
			int itemcount = 0;
			try
			{
				itemcount = ds.Tables[0].Rows.Count;
			}
			catch
			{
				itemcount = 0;
			}
			
			double quantity = 0, subprice = 0, totalprice = 0;
			for (int i = 0; i <= itemcount - 1; i++)
			{
				ListViewItem lvitem = new ListViewItem();
				lvitem.Text = ds.Tables[0].Rows[i][0].ToString(); //foodname
				quantity =Convert.ToDouble(ds.Tables[0].Rows[i][1].ToString());
				lvitem.SubItems.Add(quantity.ToString("#,##0.00")); //quantity
				lvitem.SubItems.Add(ds.Tables[0].Rows[i][2].ToString()); //foodunit
				subprice = Convert.ToDouble(ds.Tables[0].Rows[i][3].ToString());
				lvitem.SubItems.Add(subprice.ToString("#,##0.00")); //totalprice
				lvitem.SubItems.Add(ds.Tables[0].Rows[i][4].ToString()); //operandis
				lvitem.SubItems.Add(ds.Tables[0].Rows[i][5].ToString()); //tastes
				lvitem.SubItems.Add(ds.Tables[0].Rows[i][6].ToString()); //barcode
				lvitem.SubItems.Add(ds.Tables[0].Rows[i][7].ToString()); //transfered
				lvAllOrderedFoods.Items.Add(lvitem);
				totalprice += subprice;
			}
			lblAllFoodsPrice.Text = "合计：￥" + totalprice.ToString("#,##0.00");
		}

		private void btnAllOrderdFoods_Click(object sender, System.EventArgs e)
		{
			if (CurrentSelectedTableno!="")
			{
				//确保当前选中桌台是空闲状态
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				int currentTableStatus = GetServerTableStatus(currentTableno);//Convert.ToInt32(dgTablelist[dgTablelist.CurrentRowIndex, 4].ToString());

				switch (currentTableStatus)
				{
					case 0: //空闲
						MessageBox.Show(currentTableno+" 号桌台的当前状态为空闲，不能查看已点全部菜品。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
					case 1: //使用中
						Cursor.Current = Cursors.WaitCursor;
						LoadTableBills(currentTableno,cbAllBillList);
						LoadBillFoodsToListView(label5, currentTableno, cbAllBillList.Text);
						chkSelAll1.Checked = false;
						lblTableNo4.Text = CurrentSelectedTableno+" 号桌台已点全部菜品";//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString()			
						Cursor.Current = Cursors.Default;
						//返回单据类别
                        ROSS.Service rs = new ROSS.Service();
						rs.Url = WebUrl;
						IAsyncResult ar = rs.BeginGetBillTypeName(cbAllBillList.Text, null, null);
						int p = 1;
						while (!ar.IsCompleted)
						{
							p++;
						}
						string rtn = rs.EndGetBillTypeName(ar);
						label23.Text =rtn;	
						pnlAllOrderedFoods.Show();
						pnlAllOrderedFoods.BringToFront();
						PriorPanel=pnlTableList;
						break;
					case 2: //预订中
						MessageBox.Show(currentTableno+" 号桌台的当前状态为预订中，不能查看已点全部菜品。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
				}
			}
		}

		private void chkSelAll2_CheckStateChanged(object sender, System.EventArgs e)
		{
			for (int i = 0; i <= lvNeedSendFoods.Items.Count - 1; i++)
			{
				lvNeedSendFoods.Items[i].Checked = chkSelAll2.Checked;
			}
		}

		private void chkSelAll1_CheckStateChanged(object sender, System.EventArgs e)
		{
			for (int i = 0; i <= lvAllOrderedFoods.Items.Count - 1; i++)
			{
				lvAllOrderedFoods.Items[i].Checked = chkSelAll1.Checked;
			}
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			//Application.Exit();
			this.Close();
		}

		private void cbAllBillList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (cbAllBillList.Focused)
			{
				Cursor.Current = Cursors.WaitCursor;
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				LoadBillFoodsToListView(label8, currentTableno, cbAllBillList.Text);
				//返回单据类别
                ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginGetBillTypeName(cbAllBillList.Text, null, null);
				int p = 1;
				while (!ar.IsCompleted)
				{
					p++;
				}
				string rtn = rs.EndGetBillTypeName(ar);
				label23.Text =rtn;	
				Cursor.Current = Cursors.Default;
			} 
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
			pnlAbout.Show();
			pnlAbout.BringToFront();
			label12.Text= this.Text+" 正式版";
		}

        private void menuItem7_Click(object sender, System.EventArgs e)
        {
            SipShowIM(SIPF_ON);
        }

        private void menuItem9_Click(object sender, System.EventArgs e)
        {
            SipShowIM(SIPF_OFF);
        }

		private void button10_Click(object sender, System.EventArgs e)
		{
			PriorPanel.Show();
			PriorPanel.BringToFront();
		}

		//将PPC所点菜品写入桌面SQL Server服务器
		private bool AddFoodToServer(Label progress, string tableno,string billno,
			string foodcode,double quantity,string operandis,
			string tastes)
		{
			bool rtn = false;
			if (progress != null)
			{
				progress.Text = "";
			}
			//检测单据类别
			if (IsCancelBill(billno)) //取消单据
			{
				ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginAddFoodToCancelBill(tableno, billno, foodcode,
					quantity, CurrentLogOnUserID,
					null,null);
				while (!ar.IsCompleted)
				{
					tmp++;
					if (progress != null)
					{
						progress.Text = tmp.ToString();
						progress.Refresh();
					}
				}
				rtn = rs.EndAddFoodToCancelBill(ar);
			}
			else //消费或者赠送单据
			{
				ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginAddFoodToConsumeBill(tableno, billno, foodcode,
					quantity, operandis, tastes, CurrentLogOnUserID,null,null);
				while (!ar.IsCompleted)
				{
					tmp++;
					if (progress != null)
					{
						progress.Text = tmp.ToString();
						progress.Refresh();
					}
				}
				rtn = rs.EndAddFoodToConsumeBill(ar);
			}			
			return rtn;
		}

		//将PPC所点套餐写入桌面SQL Server服务器
		private bool AddSuitToServer(Label progress, string tableno,string billno,
			string suitcode,double quantity)
		{
			bool rtn = false;
			if (progress != null)
			{
				progress.Text = "";
			}
			//检测单据类别
			if (!IsCancelBill(billno)) //不是取消单据
			{
				ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginAddSuitToBill(tableno,suitcode,billno,quantity,CurrentLogOnUserID,null,null);
				while (!ar.IsCompleted)
				{
					tmp++;
					if (progress != null)
					{
						progress.Text = tmp.ToString();
						progress.Refresh();
					}
				}
				rtn = rs.EndAddSuitToBill(ar);
			}		
			return rtn;
		}

		//更新未发送菜品状态
		private void UpdateSelectedFoods(string guid)
		{
			foodinfo tmp=new foodinfo("");
			for (int i=0;i<=NeedSendFoods.Count-1;i++)
			{
				tmp=(foodinfo)NeedSendFoods[i];
				if (tmp.m_guid==guid) 
				{
					tmp.m_sign = 1;
					NeedSendFoods[i] = tmp;
				}
			}
		}

		private void DelSelectedFoods(string guid)
		{
			foodinfo tmp=new foodinfo("");
			for (int i=0;i<=NeedSendFoods.Count-1;i++)
			{
				tmp=(foodinfo)NeedSendFoods[i];
				if (tmp.m_guid==guid) 
				{
					NeedSendFoods.RemoveAt(i);
				}
			}
		}

		private void DelAllNeedSendFoods(string tableno)
		{
			foodinfo tmp=new foodinfo("");
			for (int i=0;i<=NeedSendFoods.Count-1;i++)
			{
				tmp=(foodinfo)NeedSendFoods[i];
				if (tmp.m_tableno==tableno) 
				{
					NeedSendFoods.RemoveAt(i);
				}
			}
		}

		private void button11_Click(object sender, System.EventArgs e)
		{
			string guid = "";
			string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
			if (MessageBox.Show("确定要删除选中的菜品吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				Cursor.Current = Cursors.WaitCursor;
				for (int i = 0; i <= lvNeedSendFoods.Items.Count - 1; i++)
				{
					//检测是否选中listview项
					if (lvNeedSendFoods.Items[i].Checked) //
					{
						guid = lvNeedSendFoods.Items[i].SubItems[8].Text;
						//删除sqlce数据库记录
						DelSelectedFoods(guid);
					}
				}
				//重新获取未发送菜品数据
				LoadNeedSendFoodsToListView(currentTableno,comboBox2.Text);
				chkSelAll2.Checked = false;
				Cursor.Current = Cursors.Default;
			}
		}

		private void button17_Click(object sender, System.EventArgs e)
		{
			int itemCount= lvNeedSendFoods.Items.Count;
			if (itemCount>0)
			{
				if (MessageBox.Show("确定要发送选中的菜品吗？","提示",MessageBoxButtons.YesNo ,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.Yes)
				{
					string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
					Cursor.Current = Cursors.WaitCursor;
					for (int i = 0; i <= itemCount - 1; i++)
					{
						if (lvNeedSendFoods.Items[i].Checked)
						{
							if (lvNeedSendFoods.Items[i].SubItems[10].Text=="1")
							{
								AddSuitToServer(label8,currentTableno,comboBox2.Text,
									             lvNeedSendFoods.Items[i].SubItems[9].Text,
									             Convert.ToDouble(lvNeedSendFoods.Items[i].SubItems[1].Text));
							}
							else
							{
								AddFoodToServer(label18, currentTableno,
									comboBox2.Text, //lvNeedSendFoods.Items[i].SubItems[7].Text, 
									lvNeedSendFoods.Items[i].SubItems[9].Text,
									Convert.ToDouble(lvNeedSendFoods.Items[i].SubItems[1].Text),
									lvNeedSendFoods.Items[i].SubItems[5].Text,
									lvNeedSendFoods.Items[i].SubItems[6].Text);
							}							
							//更新本地未发送菜品记录 sign = 1
							UpdateSelectedFoods(lvNeedSendFoods.Items[i].SubItems[8].Text);
						}				
					}
					Cursor.Current = Cursors.Default;
					label18.Text = "发送成功！";
					//刷新列表
					LoadNeedSendFoodsToListView(currentTableno,comboBox2.Text);
					chkSelAll2.Checked = false;
				}				
			}	
			else
			{
				MessageBox.Show("没有未发送菜品记录！","提示",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1);
			}
		}

		private void button11_Click_1(object sender, System.EventArgs e)
		{
			//确定选中了一个桌台
			if ((CurrentSelectedTableno!="")&&(comboBox1.Text !=""))
			{
				//当前选中桌台状态
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				int currentTableStatus = GetServerTableStatus(currentTableno);//Convert.ToInt32(dgTablelist[dgTablelist.CurrentRowIndex, 4].ToString());

				switch (currentTableStatus)
				{
					case 0: //空闲
						if (MessageBox.Show("确定要对当前 "+currentTableno+" 号桌台进行开台操作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							Cursor.Current = Cursors.WaitCursor;
							ROSS.Service rs = new ROSS.Service();
							rs.Url = WebUrl;
							IAsyncResult ar = rs.BeginOpenTable(currentTableno, Convert.ToInt32(numPsnCount.Value), CurrentLogOnUserID, null, null);
							int p = 1;
							int rtn;
							while (!ar.IsCompleted)
							{
								p++;
								label5.Text = p.ToString();
								label5.Refresh();
							}
							rtn=rs.EndOpenTable(ar); //0 = 成功 1 = 使用中 2 = 预订中 3 = 未知
							Cursor.Current = Cursors.Default;
							switch (rtn)
							{
								case 0: //成功
									Cursor.Current = Cursors.WaitCursor;
									//清空桌台未发送菜品记录
									DelAllNeedSendFoods(currentTableno);
									MessageBox.Show("开台操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
									//重新获取当前页桌台列表
									//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
									LoadTableListToPanel(label4,CurrentTablePageNo,cbTableTypeName.Text );
									Cursor.Current = Cursors.Default;
									break;
								case 1: //使用中
									MessageBox.Show(currentTableno+" 号桌台为使用中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
								case 2: //预订中
									MessageBox.Show(currentTableno+" 号桌台为预订中，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
								case 3: //未知
									MessageBox.Show("未知错误，开台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
									break;
							}
						}
						break;
					case 1: //使用中
						if (MessageBox.Show("确定要对当前 "+currentTableno+" 号桌台进行加单操作吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
						{
							Cursor.Current = Cursors.WaitCursor;
							ROSS.Service rs = new ROSS.Service();
							rs.Url = WebUrl;
							IAsyncResult ar = rs.BeginAddBill(currentTableno,comboBox1.Text , CurrentLogOnUserID, null, null);
							int p = 1;
							while (!ar.IsCompleted)
							{
								p++;
								label5.Text = p.ToString();
								label5.Refresh();
							}
							int rtn = rs.EndAddBill(ar);
							Cursor.Current = Cursors.Default;
							// 0 = 成功 1 = 已存在该单据号 2 = 加单失败  
							if (rtn==0)
                            {
								MessageBox.Show("加单操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
							}
							else if (rtn==1)
							{
								MessageBox.Show("加单失败，"+currentTableno+" 号桌台已存在该单据号！请联系系统管理员。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
							}
							else
							{
								MessageBox.Show("加单失败！请联系系统管理员。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
							}
						}
						break;
					case 2: //预订中
						MessageBox.Show(currentTableno+" 号桌台的当前为预订中，无法进行加单操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						break;
				}
			} 
			else
				MessageBox.Show("没有选中桌台或者没有选择加单的单据类型，无法进行加单操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						
		}

		private void cbConsumeBilllist_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (pnlFoodList.Visible)
			{
				Cursor.Current = Cursors.WaitCursor;
				ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginGetBillTypeName(cbBilllist.Text, null, null);
				int p = 1;
				while (!ar.IsCompleted)
				{
					p++;
				}
				string rtn = rs.EndGetBillTypeName(ar);
				label21.Text =rtn;
				Cursor.Current = Cursors.Default;
			}			
		}

		private void MoveTable(string srcTableno,string dstTableno)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginMoveTable(srcTableno, dstTableno, null, null);
			int p = 1;
			int rtn;
			while (!ar.IsCompleted)
			{
				p++;
				label5.Text = p.ToString();
				label5.Refresh();
			}
			//0 = 成功 
			//1 = 原桌台空闲 
			//2 = 原桌台预订中 
			//3 = 目标桌台使用中 
			//4 = 目标桌台预订中 
			//5 = 原桌台不存在
			//6 = 目标桌台不存在
			rtn=rs.EndMoveTable(ar);
			switch (rtn)
			{
				case 0: //成功
					MessageBox.Show("转台操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					//重新获取当前页桌台列表
					//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
					LoadTableListToPanel(label4,CurrentTablePageNo,cbTableTypeName.Text );
					break;
				case 1: //原桌台空闲
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)为空闲，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 2: //原桌台预订中
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)为预订中，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 3: //目标桌台使用中
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)为使用中，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 4: //目标桌台预订中
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)为预订中，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 5: //原桌台不存在
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)不存在，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 6: //目标桌台不存在
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)不存在，转台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
			}
		}

		private void CombineTable(string srcTableno,string dstTableno)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginCombineTable(srcTableno, dstTableno, null, null);
			int p = 1;
			int rtn;
			while (!ar.IsCompleted)
			{
				p++;
				label5.Text = p.ToString();
				label5.Refresh();
			}
			//0 = 成功 
			//1 = 原桌台空闲 
			//2 = 原桌台预订中 
			//3 = 目标桌台空闲 
			//4 = 目标桌台预订中 
			//5 = 原桌台不存在
			//6 = 目标桌台不存在
			rtn=rs.EndCombineTable(ar);
			switch (rtn)
			{
				case 0: //成功
					MessageBox.Show("并台操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					//重新获取当前页桌台列表
					//GetServerData(0, label4, CurrentTablePageNo, dgTablelist, "", "");
					LoadTableListToPanel(label4,CurrentTablePageNo,cbTableTypeName.Text );
					break;
				case 1: //原桌台空闲
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)为空闲，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 2: //原桌台预订中
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)为预订中，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 3: //目标桌台空闲
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)为空闲，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 4: //目标桌台预订中
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)为预订中，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 5: //原桌台不存在
					MessageBox.Show("原桌台("+srcTableno+" 号桌台)不存在，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
				case 6: //目标桌台不存在
					MessageBox.Show("目标桌台("+dstTableno+" 号桌台)不存在，并台操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					break;
			}
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			//确定选中了一个桌台
			if (CurrentSelectedTableno!="")
			{
				Cursor.Current = Cursors.WaitCursor;
				string srcTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				string dstTableno = textBox1.Text;
				MoveTable(srcTableno,dstTableno);
				Cursor.Current = Cursors.Default;
			}
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			//确定选中了一个桌台
			if (CurrentSelectedTableno!="")
			{
				Cursor.Current = Cursors.WaitCursor;
				string srcTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				string dstTableno = textBox1.Text;
				CombineTable(srcTableno,dstTableno);
				Cursor.Current = Cursors.Default;
			}
		}

		private bool IsCancelBill(string billno)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginIsCancelBill(billno,null,null);
			int p = 1;
			bool rtn=false;
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndIsCancelBill(ar);
			return rtn;
		}

		private bool IsPresentBill(string billno)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginIsPresentBill(billno,null,null);
			int p = 1;
			bool rtn=false;
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndIsPresentBill(ar);
			return rtn;
		}

		private bool IsCustomFood(string foodcode)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginIsCustomFood(foodcode,null,null);
			int p = 1;
			bool rtn=false;
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndIsCustomFood(ar);
			return rtn;
		}

		private bool AllowCancelFood(string userid)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginAllowCancelFood(userid,null,null);
			int p = 1;
			bool rtn=false;
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndAllowCancelFood(ar);
			return rtn;
		}

		//string = presentsign,presentlimit,presenttotal
		private string AllowPresentFood(string userid)
		{
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginAllowPresentFood(userid,null,null);
			int p = 1;
			string rtn="";
			while (!ar.IsCompleted)
			{
				p++;
			}
			rtn=rs.EndAllowPresentFood(ar);
			return rtn;
		}

		private bool TransFood(string foodname, string tableno, string billno,
			                   string quantity, string operandis, string tastes, string opid,
			                   string barcode, int operatecode, string cancelrsn)
		{
			label8.Text = "";
			ROSS.Service rs = new ROSS.Service();
			rs.Url = WebUrl;
			IAsyncResult ar = rs.BeginTransFood(foodname,tableno,billno,quantity,
				                                operandis,tastes,opid,barcode,
				                                operatecode,cancelrsn,null,null);
			int p = 1;
			bool rtn=false;
			while (!ar.IsCompleted)
			{
				p++;
				label8.Text = p.ToString();
				label8.Refresh();
			}
			rtn=rs.EndTransFood(ar);
			return rtn;
		}

		//OperateCode = 0 传菜 OperateCode = 1 催菜 OperateCode = 2 缓菜 OperateCode = 3 退菜
		private bool ExecTransFood(int OperateCode)
		{
			bool rtn = false;
			switch (OperateCode) //检测是哪种远程操作
			{
				case 0: //传菜操作
					//先检测当前单据不是取消单
					if (IsCancelBill(cbAllBillList.Text))
					{
						MessageBox.Show("不能传送取消单中的菜品，操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						return false;
					}
					else //是消费单据
					{
						string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				        int itemCount= lvAllOrderedFoods.Items.Count;
						for (int i = 0; i <= itemCount - 1; i++) //遍历所有选中菜品
						{
							if (lvAllOrderedFoods.Items[i].Checked) //如果选中
							{
								//0 = 未传菜 1 = 已传菜 2 = 已上菜
								//检测当前选中菜品的状态
								int transfered =Convert.ToInt32(lvAllOrderedFoods.Items[i].SubItems[7].Text);
								if (transfered==0) //未传菜，执行传菜操作
								{
									try
									{
										TransFood(lvAllOrderedFoods.Items[i].Text,
											currentTableno,cbAllBillList.Text,
											lvAllOrderedFoods.Items[i].SubItems[1].Text,
											lvAllOrderedFoods.Items[i].SubItems[4].Text,
											lvAllOrderedFoods.Items[i].SubItems[5].Text,
											CurrentLogOnUserID,
											lvAllOrderedFoods.Items[i].SubItems[6].Text,OperateCode,"");
										//更新当前菜品传菜状态
										lvAllOrderedFoods.Items[i].SubItems[7].Text="1"; //transfered
										rtn = true;
									}
									catch
									{
										return false;
									}
								}
								else if (transfered==1) //已传菜
									MessageBox.Show("菜品 "+lvAllOrderedFoods.Items[i].Text+" 已经被远程传菜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
								else if (transfered==2) //已上菜
									MessageBox.Show("菜品 "+lvAllOrderedFoods.Items[i].Text+" 已经上菜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							}				
						}
						if (rtn)
							MessageBox.Show("远程传菜操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					}
					break;
				case 1: //催菜操作
					//先检测当前单据不是取消单
					if (IsCancelBill(cbAllBillList.Text))
					{
						MessageBox.Show("不能传送取消单中的菜品，操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
						return false;
					}
					else //是消费单据
					{
						string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
						int itemCount= lvAllOrderedFoods.Items.Count;
						for (int i = 0; i <= itemCount - 1; i++) //遍历所有选中菜品
						{
							if (lvAllOrderedFoods.Items[i].Checked) //如果选中
							{
								//0 = 未传菜 1 = 已传菜 2 = 已上菜
								//检测当前选中菜品的状态
								int transfered =Convert.ToInt32(lvAllOrderedFoods.Items[i].SubItems[7].Text);
								if (transfered==0) //未传菜，不能执行催菜操作
								{
									MessageBox.Show("菜品 "+lvAllOrderedFoods.Items[i].Text+" 还未传菜菜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
								}
								else if (transfered==1) //已传菜，允许催菜
								{
									try
									{
										TransFood(lvAllOrderedFoods.Items[i].Text,
											currentTableno,cbAllBillList.Text,
											lvAllOrderedFoods.Items[i].SubItems[1].Text,
											lvAllOrderedFoods.Items[i].SubItems[4].Text,
											lvAllOrderedFoods.Items[i].SubItems[5].Text,
											CurrentLogOnUserID,
											lvAllOrderedFoods.Items[i].SubItems[6].Text,OperateCode,"");
										rtn = true;
									}
									catch
									{
										return false;
									}
								}
								else if (transfered==2) //已上菜
									MessageBox.Show("菜品 "+lvAllOrderedFoods.Items[i].Text+" 已经上菜！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);					
							}				
						}
						if (rtn)
							MessageBox.Show("远程催菜操作顺利完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
					}
					break;
				case 2: //缓菜操作

					break;
				case 3: //退菜操作

					break;
			}
			return rtn;
		}

		private void button13_Click(object sender, System.EventArgs e)
		{
			ExecTransFood(0);
		}

		private void button14_Click(object sender, System.EventArgs e)
		{
			ExecTransFood(1);
		}

		private void LoadTableTypeName(Label progress)
		{
			if (progress != null)
			{
				progress.Text = "";
				progress.Refresh();
			}    
			cbTableTypeName.Items.Clear();
            string xmlTableTypeNameList = GetServerData(13, progress,1, null, "", "","");
			TextReader txtReader;
			txtReader = new StringReader(xmlTableTypeNameList);
			XmlTextReader xmlReader = new XmlTextReader(txtReader);
			DataSet objDs = new DataSet("ds");
			objDs.ReadXml(xmlReader);
			int RecCount = objDs.Tables[0].Rows.Count;
			for (int i = 0; i <= RecCount - 1; i++)
			{
				cbTableTypeName.Items.Add(objDs.Tables[0].Rows[i]["name"].ToString());
			}
			cbTableTypeName.Items.Add("全部");
		}

		private bool LoadTableListToPanel(Label progress,int PageNo,string tabletypename)
		{
			if (progress != null)
			{
				progress.Text = "";
				progress.Refresh();
			}    
			if (TableList!=null)
			{
				TableList.RemoveAll();
				TableList= null;
			}
			try
			{
				string xmlBillList = GetServerData(0, progress,PageNo, null, "", "",tabletypename);
				if (xmlBillList=="<ds />")
				{
					//返回的记录为空
					TableList= new TableButtonList(panel1,59,35,4);
					return true;
				}
				else
				{
					TextReader txtReader;
					txtReader = new StringReader(xmlBillList);
					XmlTextReader xmlReader = new XmlTextReader(txtReader);
					DataSet objDs = new DataSet("ds");
					objDs.ReadXml(xmlReader);
					int RecCount = objDs.Tables[0].Rows.Count;
					TableList= new TableButtonList(panel1,59,35,4);
					for (int i = 0; i <= RecCount - 1; i++)
					{
						TableList.AddButton(objDs.Tables[0].Rows[i]["tableno"].ToString(), 
							objDs.Tables[0].Rows[i]["tablename"].ToString(), 
							Convert.ToInt32(objDs.Tables[0].Rows[i]["status"].ToString()));
					}
					return true;
				}				
			}
			catch
			{
				return false;
			}			
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			PriorPanel = pnlLogOn;
			label2.Text= this.Text+" 正式版";
			Setting s = new Setting();
			WebUrl = s.GetWebUrl();            
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch (toolBar1.Buttons.IndexOf(e.Button))
			{
				case 0: //回首页
					if (!pnlLogOn.Visible)
					{
						pnlLogOn.Hide();
						PriorPanel=pnlTableList;
						pnlTableList.Show();
						pnlTableList.BringToFront();
					}					
					break;
				case 1: //上一页
					PriorPanel.Show();
					PriorPanel.BringToFront();
					break;
				case 2: //重新登录
					if (MessageBox.Show("确定要注销并重新登录吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					{
						ROSS.Service rs = new ROSS.Service();
						rs.Url = WebUrl;
						IAsyncResult ar = rs.BeginLogOff(false,null, null);
						int p = 1;
						while (!ar.IsCompleted)
						{
							p++;
						}
						if (!rs.EndLogOff(ar))
						{
							MessageBox.Show("注销登录失败！系统将关闭。","提示", MessageBoxButtons.OK ,MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
							Application.Exit();
						}
						else
						{
							LoggedOff=true;
							pnlLogOn.Show();
							pnlLogOn.BringToFront();
							PriorPanel=pnlLogOn;
						}						
					}
					break;
			}
		}

		private void button16_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (comboBox2.Focused)
			{
				Cursor.Current = Cursors.WaitCursor;
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				LoadNeedSendFoodsToListView(currentTableno, comboBox2.Text);
				//返回单据类别
				ROSS.Service rs = new ROSS.Service();
				rs.Url = WebUrl;
				IAsyncResult ar = rs.BeginGetBillTypeName(comboBox2.Text, null, null);
				int p = 1;
				while (!ar.IsCompleted)
				{
					p++;
				}
				string rtn = rs.EndGetBillTypeName(ar);
				label27.Text =rtn;	
				Cursor.Current = Cursors.Default;
			} 
		}

		private void button15_Click(object sender, System.EventArgs e)
		{
			pnlFoodList.Show();
			pnlFoodList.BringToFront();
		}

		private void button12_Click(object sender, System.EventArgs e)
		{
			if (textBox2.Text!="")
			{
				Cursor.Current = Cursors.WaitCursor;
				try
				{
					ROSS.Service rs = new ROSS.Service();
					rs.Url = WebUrl;
					IAsyncResult ar = rs.BeginLogOn(textBox2.Text, textBox3.Text,false, null, null);
					int p = 1;
					while (!ar.IsCompleted)
					{
						p++;
						label36.Text = p.ToString();
						label36.Refresh();
					}
					int rtn = rs.EndLogOn(ar);
					if (rtn==1)
					{
						//赠送验证成功
						string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
						string operandis="", tastes="";
						Cursor.Current = Cursors.WaitCursor;
						for (int i = 0; i <= lvFoodOperandis.Items.Count - 1; i++)
						{
							if (lvFoodOperandis.Items[i].Checked)
							{
								if (operandis.Trim() != "")
									operandis = operandis + "," + lvFoodOperandis.Items[i].Text;
								else
									operandis = lvFoodOperandis.Items[i].Text;
							}
						}
						for (int i = 0; i <= lvFoodTastes.Items.Count - 1; i++)
						{
							if (lvFoodTastes.Items[i].Checked)
							{
								if (tastes.Trim() != "")
									tastes = tastes + "," + lvFoodTastes.Items[i].Text;
								else
									tastes = lvFoodTastes.Items[i].Text;
							}
						}
						//保存未发送菜品信息
						AddFoodToNeedSend(currentTableno,cbBilllist.Text,dgFoodlist[dgFoodlist.CurrentRowIndex, 1].ToString(),
							dgFoodlist[dgFoodlist.CurrentRowIndex, 2].ToString(),
							dgFoodlist[dgFoodlist.CurrentRowIndex, 5].ToString(),
							dgFoodlist[dgFoodlist.CurrentRowIndex, 4].ToString(),
							Convert.ToDouble(dgFoodlist[dgFoodlist.CurrentRowIndex, 3].ToString()),
							operandis,
							tastes,
							textBox4.Text,
							0,CurrentLogOnUserID,0);
						Cursor.Current = Cursors.Default;
						MessageBox.Show("菜品 "+dgFoodlist[dgFoodlist.CurrentRowIndex, 5].ToString()+" 已经被成功保存到未发送菜品列表中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
						pnlFoodList.Show();
						pnlFoodList.BringToFront();
						PriorPanel=pnlFoodList;
					}
					else if (rtn==0)
					{
						MessageBox.Show("登录失败！请检查用户名和密码输入是否正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					}
					else if (rtn==-1) 
					{
						MessageBox.Show("服务器验证失败！请检查服务器配置是否正确。（"+rtn.ToString()+"）", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
					}
				}
				catch
				{
					Cursor.Current = Cursors.Default;
					MessageBox.Show("连接服务器失败，请检查设备和服务器的网络配置！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}
			}			
		}

		private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (MessageBox.Show("确定要退出餐厅管家吗？","提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.Yes)
			{
				if (LoggedOff) //已经退出登录，直接退出
				{
					//exit
					e.Cancel = false;
				}
				else
				{
					try
					{
						ROSS.Service rs = new ROSS.Service();
						rs.Url = WebUrl;
						IAsyncResult ar = rs.BeginLogOff(false,null, null);
						int p = 1;
						while (!ar.IsCompleted)
						{
							p++;
						}
						if (!rs.EndLogOff(ar))
						{
							Application.Exit();
						}
						else //注销成功
						{
							//exit
							e.Cancel = false;
						}
					}
					catch
					{
						//exit
						e.Cancel = false;
					}					
				}				
			}
			else 
				e.Cancel =true;
		}

		private void button17_Click_1(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			CurrentFoodPageNo=1;
			switch (cbFoodQuery.SelectedIndex)
			{
				case 0: //菜品编码
					GetServerData(8, label6, CurrentFoodPageNo, dgFoodlist, "",cbFoodType.Text , txtFoodQuery.Text);
					break;
				case 1: //菜品名称
					GetServerData(9, label6, CurrentFoodPageNo, dgFoodlist, "",cbFoodType.Text , txtFoodQuery.Text);
					break;
				case 2: //拼音码
					GetServerData(11, label6, CurrentFoodPageNo, dgFoodlist, "",cbFoodType.Text , txtFoodQuery.Text);
					break;
				default:
					GetServerData(1, label6, CurrentFoodPageNo, dgFoodlist, "", cbFoodType.Text , "");
					break;
			}	
			Cursor.Current = Cursors.Default;
		}

		private void cbTableTypeName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			LoadTableListToPanel(label5,1,cbTableTypeName.Text);
		}

		private void button18_Click(object sender, System.EventArgs e)
		{
			if (pnlFoods.Visible)
			{
				pnlSuits.Visible = true;
				pnlSuits.BringToFront();
				pnlFoods.Visible =false;
				button18.Text = "单品";
				//获取套餐数据
				GetServerData(14, label6, CurrentSuitPageNo, dgSuitList, "", "", "");
			}
			else
			{
				pnlFoods.Visible = true;
				pnlFoods.BringToFront();
				pnlSuits.Visible =false;
				button18.Text = "套餐";
			}
		}

		private void cbDept_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			CurrentFoodPageNo=1;
			txtFoodQuery.Text ="";
			LoadFoodTypeNameData(label6,cbDept.Text);
			//获取菜品信息
			if (cbFoodType.Text!="")
			{
				GetServerData(1, label6, CurrentFoodPageNo, dgFoodlist, "",cbFoodType.Text , "");
			}
			else
			{
				dgFoodlist.DataSource =null;
			}			
		}

		private void cbFoodType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			CurrentFoodPageNo=1;
			txtFoodQuery.Text ="";
			//获取菜品信息
			GetServerData(1, label6, CurrentFoodPageNo, dgFoodlist, "",cbFoodType.Text , "");
		}

		private void cbFoodQuery_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtFoodQuery.Text ="";
		}        

		private void button19_Click(object sender, System.EventArgs e)
		{
			//确定选中了一个桌台
			if (CurrentSelectedTableno!="")
			{
				string currentTableno = CurrentSelectedTableno;//dgTablelist[dgTablelist.CurrentRowIndex, 1].ToString();
				if (MessageBox.Show("确定要修改 "+currentTableno+" 号桌台的开台人数吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
				{
					ROSS.Service rs = new ROSS.Service();
					rs.Url = WebUrl;
					IAsyncResult ar = rs.BeginModifyTablePsnCount(currentTableno, Convert.ToInt32(numPsnCount.Value), null, null);
					int p = 1;
					int rtn;
					while (!ar.IsCompleted)
					{
						p++;
						label5.Text = p.ToString();
						label5.Refresh();
					}
					//0 = 空闲 1 = 使用中 2 = 预订中 -1 = 未知桌台
					rtn=rs.EndModifyTablePsnCount(ar); 
					switch (rtn)
					{
						case 0: //空闲
							MessageBox.Show(currentTableno+" 号桌台为空闲，修改桌台开台人数操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 1: //使用中
							MessageBox.Show("修改 "+ currentTableno+" 号桌台开台人数成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case 2: //预订中
							MessageBox.Show(currentTableno+" 号桌台为预订中，修改桌台开台人数操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
						case -1: //-1 = 未知桌台
							MessageBox.Show("修改桌台开台人数操作失败！稍候请重新尝试开台操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
							break;
					}
				}
			} 
		}

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // 向上导航
                // 向上键
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // 向下导航
                // 向下键
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // 向左键
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // 向右键
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }
        }
	}
}
