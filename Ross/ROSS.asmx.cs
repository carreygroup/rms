using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Service : System.Web.Services.WebService
{
    public class FoodInfo : Object
    {
        public string FoodCode;
        public string TableType;
        public string Tableno;
        public string Billno;
        public string FoodName;
        public string TypeName;
        public string DeptName;
        public string unit;
        public string quantity;
        public string operandi;
        public string taste;
        public string operatorid;
        public int OperateCode;  //OperateCode = 0 传菜 OperateCode = 1 催菜 OperateCode = 2 缓菜 OperateCode = 3 退菜
        public string CancelRsn; //退菜原因
        public string BarCode;

    }

    //const string addstring = "<?xml version=" + "\"" + "1.0" + "\"" + " encoding=" + "\"" + "gb2312" + "\"" + "?><tbl>" + "</tbl>";
    string connstring = @"Data Source=localhost;Initial Catalog=RMS;user id=sa;password=!q2w3e4r5t";
    
    public Service()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
        System.Configuration.Configuration rootWebConfig1 =
    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(null);
        if (0 < rootWebConfig1.AppSettings.Settings.Count)
        {
            System.Configuration.KeyValueConfigurationElement SqlServerName =
                rootWebConfig1.AppSettings.Settings["SQLServerName"];
            //connstring = @"Data Source=" + SqlServerName.Value + ";Initial Catalog=RMS;Integrated Security=True";
        }
    }

    //返回指定页数的相关数据
    private string GetDataOriginalXmlString(int target, int ItemCountPerPage,
                                            int PageNo, string connstring,
                                            string queryfield1, string queryfield2,
                                            string queryfield3)
    {
        using (SqlConnection connection = new SqlConnection(
                   connstring))
        {
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = connection;
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.CommandText = "GetData";

            SqlParameter param0 = new SqlParameter();
            param0.ParameterName = "@target";
            param0.Size = 10;
            param0.SqlDbType = SqlDbType.Int;
            param0.Value = target;
            sqlcmd.Parameters.Add(param0);

            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@itemcount";
            param1.Size = 10;
            param1.SqlDbType = SqlDbType.Int;
            param1.Value = ItemCountPerPage;
            sqlcmd.Parameters.Add(param1);

            SqlParameter param2 = new SqlParameter();
            param2.ParameterName = "@pageno";
            param2.Size = 10;
            param2.SqlDbType = SqlDbType.Int;
            param2.Value = PageNo;
            sqlcmd.Parameters.Add(param2);

            SqlParameter param3 = new SqlParameter();
            param3.ParameterName = "@relatedField1";
            param3.Size = 50;
            param3.SqlDbType = SqlDbType.VarChar;
            if (queryfield1.Trim() == "")
                param3.Value = "null";
            else
                param3.Value = queryfield1;
            sqlcmd.Parameters.Add(param3);

            SqlParameter param4 = new SqlParameter();
            param4.ParameterName = "@relatedField2";
            param4.Size = 50;
            param4.SqlDbType = SqlDbType.VarChar;
            if (queryfield2.Trim() == "")
                param4.Value = "null";
            else
                param4.Value = queryfield2;
            sqlcmd.Parameters.Add(param4);

            SqlParameter param5 = new SqlParameter();
            param5.ParameterName = "@relatedField3";
            param5.Size = 50;
            param5.SqlDbType = SqlDbType.VarChar;
            if (queryfield3.Trim() == "")
                param5.Value = "null";
            else
                param5.Value = queryfield3;
            sqlcmd.Parameters.Add(param5);

            DataSet objDs = new DataSet("ds");
            SqlDataAdapter dbDa = new SqlDataAdapter();
            dbDa.SelectCommand = sqlcmd;
            connection.Open();
            dbDa.Fill(objDs, "t");

            TextWriter stringWriter = new StringWriter();
            objDs.WriteXml(stringWriter);
            string rtnxml = stringWriter.ToString();
            //rtnxml="<?xml version=" + "\"" + "1.0" + "\"" + " encoding=" + "\"" + "gb2312" + "\"" + "?><tbl>" + rtnxml.Trim() + "</tbl>";
            connection.Close();
            return rtnxml;
        }
    }

    //返回全局唯一标识符
    private string GenUniqueCode()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
        /*
        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand sqlProc = new SqlCommand();
        sqlProc.Connection = conn;
        sqlProc.CommandType = CommandType.StoredProcedure;
        sqlProc.CommandText = "genUNIQUE_ID";

        SqlParameter rtnparam = new SqlParameter();
        rtnparam.ParameterName = "@rtn_id";
        rtnparam.Size = 50;
        rtnparam.SqlDbType = SqlDbType.VarChar;
        rtnparam.Direction = ParameterDirection.Output;
        sqlProc.Parameters.Add(rtnparam);

        conn.Open();

        sqlProc.ExecuteNonQuery();
        return rtnparam.Value.ToString();
        */
    }

    //返回服务器的最后一条前台营业单据号
    private string GenLastBillno()
    {
        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand sqlProc = new SqlCommand();
        sqlProc.Connection = conn;
        sqlProc.CommandType = CommandType.StoredProcedure;
        sqlProc.CommandText = "NewBalanceBillNo";
        SqlParameter rtnparam = new SqlParameter();
        rtnparam.ParameterName = "@BillNo";
        rtnparam.Size = 50;
        rtnparam.SqlDbType = SqlDbType.VarChar;
        rtnparam.Direction = ParameterDirection.Output;
        sqlProc.Parameters.Add(rtnparam);
        conn.Open();
        sqlProc.ExecuteNonQuery();
        return rtnparam.Value.ToString();
    }

    //返回单据类型编码
    private string GetBillTypeCode(string BillTypeName)
    {
        SqlConnection conn = new SqlConnection(connstring);
        conn.ConnectionString = connstring;
        string sqlstr = "SELECT BillTypeCode FROM BillType where name='" + BillTypeName + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回单据类型名称
    private string GetBillTypeNameFromCode(string BillTypeCode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        conn.ConnectionString = connstring;
        string sqlstr = "SELECT name FROM BillType where BillTypeCode='" + BillTypeCode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回单据类型编码
    private string GetBillTypeCode2(string billno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        conn.ConnectionString = connstring;
        string sqlstr = "SELECT billtypecode FROM salebill_t where billno='" + billno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回消费单据类型编码
    private string GetConsumeBillTypeCode()
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT billtypecode FROM billtype where consumebill='1'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //检测当前单据号和单据类型是否已经存在于当前营业表中
    private bool SameBillinSale(string billno, string billtypename)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT serialno FROM salebill_all where billno='" + billno + "' and billtypecode='" + GetBillTypeCode(billtypename) + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows.Count > 0) //存在该单据号
            return true;
        else
            return false;
    }

    //加单操作 0 = 成功 1 = 已存在该单据号 2 = 加单失败
    private int AddBillToTable(string tableno, string billno, string billtypename, string userid)
    {
        int rtn = 2;
        if (!SameBillinSale(billno, billtypename))
        {
            try
            {
                AddSaleBill(GetCurrentTableSerialNo(tableno), tableno, billno, GetBillTypeCode(billtypename), userid);
                rtn = 0;
            }
            catch
            {
                rtn = 2;
            }
        }
        else
            rtn = 1;
        return rtn;
    }

    //执行开台后加单操作
    private bool AddSaleBill(string SerialNo, string Tableno, string Billno, string BillTypeCode, string EmpID)
    {
        try
        {
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = conn;
            sqlcmd.CommandText = "insert into salebill_t (serialno,tableno,Billno,billtypecode,empid) " +
                                 "values (" +
                                 "'" + SerialNo + "'" + "," +
                                 "'" + Tableno + "'" + "," +
                                 "'" + Billno + "'" + "," +
                                 "'" + BillTypeCode + "'" + "," +
                                 "'" + EmpID + "'" + ")";
            conn.Open();
            sqlcmd.ExecuteNonQuery();
            conn.Close();
            return true;
        }
        catch
        {
            connstring = "";
            return false;
        }

    }

    //记录开台信息
    private void AddBalanceTable(string SerialNo, string Tableno, string OpenPsn, string EmpID)
    {
        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = "insert into BalanceTable_t (serialno,tableno,OpenPsn,empid,begintime) " +
                             "values (" +
                             "'" + SerialNo + "'" + "," +
                             "'" + Tableno + "'" + "," +
                             OpenPsn + "," +
                             "'" + EmpID + "'" + "," +
                             "'" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "'" + ")";

        conn.Open();
        sqlcmd.ExecuteNonQuery();
        conn.Close();
    }

    //开台 0 = 成功 1 = 使用中 2 = 预订中 3 = 加单失败 存在首张单据 4 = 加单失败 未知
    //      5 = 未知错误
    private int ExecOpenTable(string tableno, int openpsncount, string billno, string userid)
    {
        //检测桌台属性
        //0=空闲，1=使用，2=预定中，-1=不存在该桌台
        //检测原桌台状态
        int rtn = 3;
        int scrTableStatus = GetCurrentTableStatus(tableno);
        switch (scrTableStatus)
        {
            case 0: //空闲 执行开台
                string lastbillno;

                if (billno == "auto") //若是自动生成单据号则返回系统最后一条营业单据号
                {
                    lastbillno = GenLastBillno();
                }
                else lastbillno = billno;
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connstring;
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = conn;
                sqlcmd.CommandText = "update tablestatus set status =1,PeopleNumber=" + openpsncount + ",begintime='" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "'" + " where tableno='" + tableno + "'";
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                conn.Close();
                //准备开台、加单
                string serialno;
                serialno = GenUniqueCode();
                AddBalanceTable(serialno, tableno, openpsncount.ToString(), userid);
                if (!SameBillinSale(lastbillno, GetBillTypeNameFromCode(GetConsumeBillTypeCode())))
                {
                    if (AddSaleBill(serialno, tableno, lastbillno, GetConsumeBillTypeCode(), userid))
                    {
                        rtn = 0;
                    }
                    else
                        rtn = 4; //加单失败 未知
                }
                else
                    rtn = 3; //加单失败 存在同号单据
                break;
            case 1: //使用中
                rtn = 1;
                break;
            case 2: //预订中
                rtn = 2;
                break;
            default:
                rtn = 5;
                break;
        }
        return rtn;
    }

    //返回菜品是否跟踪及菜品剩余数量
    private double GetFoodRemainQuantity(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT remain FROM foodlist where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;

        conn.Open();

        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
    }

    //返回当前时间段名
    private string GetCurrentTimeBlock()
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "select value from syssetup where parametercode='CurrentTimeblockName'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;

        conn.Open();

        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回当前桌台营业全局唯一流水线号
    private string GetCurrentTableSerialNo(string tableno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT serialno FROM BalanceTable_t where tableno='" + tableno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回当前桌台状态
    private int GetCurrentTableStatus(string tableno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT status FROM tablestatus where tableno='" + tableno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
    }

    //返回当前桌台类别
    private string GetTableTypeName(string tableno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT name FROM view_tablelist where tableno='" + tableno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回菜品编码
    private string GetFoodCode(string foodname)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT foodcode FROM foodlist where foodname='" + foodname + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回菜品名称
    private string GetFoodName(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT foodname FROM foodlist where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回菜品类别名称
    private string GetFoodTypeName(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT typename FROM view_FoodList where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回菜品所属部门类别名称
    private string GetFoodDeptTypeName(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT deptname FROM view_FoodList where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //返回菜品单位名称
    private string GetFoodUnit(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT unit FROM FoodList where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return ds.Tables[0].Rows[0][0].ToString();
    }

    //当前桌台是否存在
    private bool TableExist(string tableno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT count(*) FROM tablestatus where tableno='" + tableno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0)
            return true;
        else
            return false;
    }

    //检测当前单据类别是否为取消单
    private bool ExecIsCancelBill(string billtypecode, string billtypename)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        if (billtypename == "")
            sqlstr = "SELECT CancelBill FROM billtype where billtypecode='" + billtypecode + "'";
        else
            sqlstr = "SELECT CancelBill FROM billtype where name='" + billtypename + "'";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;

        conn.Open();

        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows[0][0].ToString() == "1")
            return true;
        else
            return false;
    }

    //检测当前单据类别是否为赠送单
    private bool ExecIsPresentBill(string billtypecode, string billtypename)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        if (billtypename == "")
            sqlstr = "SELECT PresentBill FROM billtype where billtypecode='" + billtypecode + "'";
        else
            sqlstr = "SELECT PresentBill FROM billtype where name='" + billtypename + "'";

        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;

        conn.Open();

        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows[0][0].ToString() == "1")
            return true;
        else
            return false;
    }

    //检测当前菜品是否为定制菜
    private bool ExecIsCustomFood(string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        sqlstr = "SELECT isunicode FROM foodlist where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;

        conn.Open();

        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows[0][0].ToString() == "1")
            return true;
        else
            return false;
    }

    //返回独立菜品售价
    private string GetFoodPrice(string foodcode, bool suit)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        if (!suit) //不是获取套餐中的菜品价格
            sqlstr = "SELECT specialfood,specialprice,currentfood,currentprice,price FROM foodlist where foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows.Count > 0)
        {
            //检测是否是特价菜或者时价菜
            if (ds.Tables[0].Rows[0]["specialfood"].ToString() == "1") //是特价菜
                return ds.Tables[0].Rows[0]["specialprice"].ToString();
            else if (ds.Tables[0].Rows[0]["currentfood"].ToString() == "1") //是时价菜
                return ds.Tables[0].Rows[0]["currentprice"].ToString();
            else
                return ds.Tables[0].Rows[0]["price"].ToString();
        }
        else
            return "0";
    }

    //更新菜品剩余数量
    private void UpdateFoodRemainQuantity(string foodcode, double quantity, int opt)
    {
        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        if (opt == 1) //加操作
            sqlcmd.CommandText = "update foodlist set remain=remain+" + quantity.ToString() + " where foodcode='" + foodcode + "'";
        else if (opt == 0) //减操作
            sqlcmd.CommandText = "update foodlist set remain=remain-" + quantity.ToString() + " where foodcode='" + foodcode + "'";
        conn.Open();
        sqlcmd.ExecuteNonQuery();
        conn.Close();
    }

    //向单据加菜
    private string AddFoodtoBill(string serialno, string foodguid, string billno, string billtypecode, string foodcode,
                          string foodprice, double addquantity, double decquantity,
                          string operandis, string tastes, string OperatorID,
                          string suitcode, double suitquantity)
    {
        string barcode = "";
        //检测菜品剩余数量
        double foodremain; //若跟踪菜品数量，该值应该 > 0，若不跟踪 该值为 -1

        foodremain = GetFoodRemainQuantity(foodcode);

        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand MysqlProc = new SqlCommand();
        MysqlProc.Connection = conn;
        MysqlProc.CommandType = CommandType.StoredProcedure;
        MysqlProc.CommandText = "proc_AddFoodtoBill";

        SqlParameter sparam1 = new SqlParameter();
        sparam1.ParameterName = "@SerialNo";
        sparam1.Size = 50;
        sparam1.SqlDbType = SqlDbType.VarChar;
        sparam1.Value = serialno;
        MysqlProc.Parameters.Add(sparam1);

        SqlParameter sparam16 = new SqlParameter();
        sparam16.ParameterName = "@FoodGuid";
        sparam16.Size = 50;
        sparam16.SqlDbType = SqlDbType.VarChar;
        sparam16.Value = foodguid;
        MysqlProc.Parameters.Add(sparam16);

        SqlParameter sparam2 = new SqlParameter();
        sparam2.ParameterName = "@Billno";
        sparam2.Size = 50;
        sparam2.SqlDbType = SqlDbType.VarChar;
        sparam2.Value = billno;
        MysqlProc.Parameters.Add(sparam2);

        SqlParameter sparam3 = new SqlParameter();
        sparam3.ParameterName = "@BillTypeCode";
        sparam3.Size = 10;
        sparam3.SqlDbType = SqlDbType.VarChar;
        sparam3.Value = billtypecode;
        MysqlProc.Parameters.Add(sparam3);

        SqlParameter sparam4 = new SqlParameter();
        sparam4.ParameterName = "@FoodCode";
        sparam4.Size = 50;
        sparam4.SqlDbType = SqlDbType.VarChar;
        sparam4.Value = foodcode;
        MysqlProc.Parameters.Add(sparam4);

        SqlParameter sparam5 = new SqlParameter();
        sparam5.ParameterName = "@FoodPrice";
        sparam5.Size = 10;
        sparam5.SqlDbType = SqlDbType.VarChar;
        sparam5.Value = foodprice;
        MysqlProc.Parameters.Add(sparam5);

        SqlParameter sparam6 = new SqlParameter();
        sparam6.ParameterName = "@Operandi";
        sparam6.Size = 50;
        sparam6.SqlDbType = SqlDbType.VarChar;
        sparam6.Value = operandis;
        MysqlProc.Parameters.Add(sparam6);

        SqlParameter sparam7 = new SqlParameter();
        sparam7.ParameterName = "@Taste";
        sparam7.Size = 50;
        sparam7.SqlDbType = SqlDbType.VarChar;
        sparam7.Value = tastes;
        MysqlProc.Parameters.Add(sparam7);

        SqlParameter sparam8 = new SqlParameter();
        sparam8.ParameterName = "@AddQuantity";
        //sparam8.Size = 50;
        sparam8.SqlDbType = SqlDbType.Float;
        sparam8.Value = addquantity;
        MysqlProc.Parameters.Add(sparam8);

        SqlParameter sparam9 = new SqlParameter();
        sparam9.ParameterName = "@DecQuantity";
        //sparam8.Size = 50;
        sparam9.SqlDbType = SqlDbType.Float;
        sparam9.Value = decquantity;
        MysqlProc.Parameters.Add(sparam9);

        SqlParameter sparam10 = new SqlParameter();
        sparam10.ParameterName = "@OperatorID";
        sparam10.Size = 50;
        sparam10.SqlDbType = SqlDbType.VarChar;
        sparam10.Value = OperatorID;
        MysqlProc.Parameters.Add(sparam10);

        SqlParameter sparam11 = new SqlParameter();
        sparam11.ParameterName = "@TimeBlock";
        sparam11.Size = 50;
        sparam11.SqlDbType = SqlDbType.VarChar;
        sparam11.Value = GetCurrentTimeBlock();
        MysqlProc.Parameters.Add(sparam11);

        SqlParameter sparam14 = new SqlParameter();
        sparam14.ParameterName = "@suitcode";
        sparam14.Size = 50;
        sparam14.SqlDbType = SqlDbType.VarChar;
        sparam14.Value = suitcode;
        MysqlProc.Parameters.Add(sparam14);

        SqlParameter sparam15 = new SqlParameter();
        sparam15.ParameterName = "@suitquantity";
        sparam15.SqlDbType = SqlDbType.Float;
        sparam15.Value = suitquantity;
        MysqlProc.Parameters.Add(sparam15);

        SqlParameter sparam13 = new SqlParameter();
        sparam13.ParameterName = "@barcode";
        sparam13.Size = 50;
        sparam13.SqlDbType = SqlDbType.VarChar;
        sparam13.Direction = ParameterDirection.Output;
        //sparam13.Value = barcode;
        //barcode = sparam13.Value.ToString();
        MysqlProc.Parameters.Add(sparam13);

        SqlParameter sparam12 = new SqlParameter();
        sparam12.ParameterName = "@OperationCode";
        sparam12.Size = 2; //
        sparam12.SqlDbType = SqlDbType.Char;

        if (ExecIsCancelBill(GetBillTypeCode2(billno), ""))
        {
            sparam12.Value = -1; //减菜
            MysqlProc.Parameters.Add(sparam12);
            conn.Open();
            MysqlProc.ExecuteNonQuery();

            if (foodremain > -1) //跟踪菜品，将剩余菜品数量加一
                UpdateFoodRemainQuantity(foodcode, decquantity, 1);
        }
        else
        {
            sparam12.Value = 1; //加菜
            if (foodremain > -1) //检测是否跟踪菜品 跟踪
            {
                if (foodremain >= addquantity) //检测剩余数量是否允许加菜，允许
                {
                    MysqlProc.Parameters.Add(sparam12);
                    conn.Open();
                    MysqlProc.ExecuteNonQuery();
                    //更新数量
                    UpdateFoodRemainQuantity(foodcode, addquantity, 0);
                }
                else //菜品剩余数量不够，显示错误
                {

                }
            }
            else //不跟踪，直接加菜
            {
                MysqlProc.Parameters.Add(sparam12);
                conn.Open();
                MysqlProc.ExecuteNonQuery();
            }
        }
        barcode = sparam13.Value.ToString();
        return barcode;
    }

    //0 = 成功 
    //1 = 原桌台空闲 
    //2 = 原桌台预订中 
    //3 = 目标桌台使用中 
    //4 = 目标桌台预订中 
    //5 = 原桌台不存在 
    //6 = 目标桌台不存在
    private int ExecMoveTable(string srcTableno, string dstTableno)
    {
        //检测桌台属性
        //0=空闲，1=使用，2=预定中，-1=不存在该桌台
        //检测原桌台状态
        if (!TableExist(srcTableno))
        {
            return 5;
        }
        if (!TableExist(dstTableno))
        {
            return 6;
        }
        int rtn = 5;
        int scrTableStatus = GetCurrentTableStatus(srcTableno);
        int dstTableStatus = GetCurrentTableStatus(dstTableno);
        switch (scrTableStatus)
        {
            case 0: //原桌台空闲
                rtn = 1;
                break;
            case 1: //原桌台使用中
                //检测目标桌台状态
                switch (dstTableStatus)
                {
                    case 0: //目标桌台空闲，执行转台
                        string srcTableTableSerialNo = GetCurrentTableSerialNo(srcTableno);
                        SqlConnection conn = new SqlConnection(connstring);
                        SqlCommand sqlcmd = new SqlCommand();
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "update salebill_t set " +
                                             "tableno='" + dstTableno + "'" +
                                             " where serialno='" + srcTableTableSerialNo + "' and tableno='" + srcTableno + "'";
                        conn.Open();
                        sqlcmd.ExecuteNonQuery();

                        sqlcmd.CommandText = "update BalanceTable_t set " +
                                             "tableno='" + dstTableno + "'" +
                                             " where serialno='" + srcTableTableSerialNo + "' and tableno='" + srcTableno + "'";
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.CommandText = "update tablestatus set " +
                                             "status='0'" +
                                             " where tableno='" + srcTableno + "'";
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.CommandText = "update tablestatus set " +
                                             "status='1'" +
                                             " where tableno='" + dstTableno + "'";
                        sqlcmd.ExecuteNonQuery();
                        conn.Close();
                        rtn = 0;
                        break;

                    case 1: //目标桌台使用中
                        rtn = 3;
                        break;
                    case 2: //目标桌台预订中
                        rtn = 4;
                        break;
                }
                break;

            case 2: //原桌台预订中
                rtn = 2;
                break;
        }
        return rtn;
    }

    //0 = 成功 
    //1 = 原桌台空闲 
    //2 = 原桌台预订中 
    //3 = 目标桌台空闲 
    //4 = 目标桌台预订中 
    //5 = 原桌台不存在
    //6 = 目标桌台不存在
    private int ExecCompineTable(string srcTableno, string dstTableno)
    {
        //检测桌台属性
        //0=空闲，1=使用，2=预定中，-1=不存在该桌台
        //检测原桌台状态
        if (!TableExist(srcTableno))
        {
            return 5;
        }
        if (!TableExist(dstTableno))
        {
            return 6;
        }
        int rtn = 5;
        int scrTableStatus = GetCurrentTableStatus(srcTableno);
        int dstTableStatus = GetCurrentTableStatus(dstTableno);
        switch (scrTableStatus)
        {
            case 0: //原桌台空闲
                rtn = 1;
                break;
            case 1: //原桌台使用中
                //检测目标桌台状态
                switch (dstTableStatus)
                {
                    case 0: //目标桌台空闲，执行转台                        
                        rtn = 3;
                        break;
                    case 1: //目标桌台使用中
                        string srcTableTableSerialNo = GetCurrentTableSerialNo(srcTableno);
                        string dstTableTableSerialNo = GetCurrentTableSerialNo(dstTableno);
                        SqlConnection conn = new SqlConnection(connstring);
                        SqlCommand sqlcmd = new SqlCommand();
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "update balancetable_t set " +
                                             "serialno='" + dstTableTableSerialNo + "' " +
                                             "where serialno='" + srcTableTableSerialNo + "'";
                        conn.Open();
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.CommandText = "update salebill_t set " +
                                             "serialno='" + dstTableTableSerialNo + "' " +
                                             "where serialno='" + srcTableTableSerialNo + "'";
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd.CommandText = "update salelist_t set " +
                                             "serialno='" + dstTableTableSerialNo + "' " +
                                             "where serialno='" + srcTableTableSerialNo + "'";
                        sqlcmd.ExecuteNonQuery();
                        conn.Close();
                        rtn = 0;
                        break;
                    case 2: //目标桌台预订中
                        rtn = 4;
                        break;
                }
                break;

            case 2: //原桌台预订中
                rtn = 2;
                break;
        }
        return rtn;
    }

    private void EnsureQueueExists(string queuePath)
    {
        if (!System.Messaging.MessageQueue.Exists(queuePath))
        {
            System.Messaging.MessageQueue.Create(queuePath);
        }
    }

    //更新已传菜标志
    private void UpdateFoodTransSign(string barcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = "update salelist_t set transfered='1' where barcode='" + barcode + "'";
        conn.Open();
        sqlcmd.ExecuteNonQuery();
        conn.Close();
    }

    //自动分单打印
    private bool ExecTransFood(string foodcode, string tableno, string tabletype, string billno,
                           string foodname, string foodtypename, string deptname, string unit,
                           string quantity, string operandis, string tastes, string opid,
                           string barcode, int operatecode, string cancelrsn)
    {
        bool rtn = false;
        string queuePath = ".\\Private$\\RMS";
        EnsureQueueExists(queuePath);
        System.Messaging.MessageQueue queue = new System.Messaging.MessageQueue(queuePath);
        queue.Formatter = new System.Messaging.XmlMessageFormatter(new Type[] { typeof(FoodInfo) });
        FoodInfo orderRequest = new FoodInfo();
        orderRequest.FoodCode = foodcode;
        orderRequest.TableType = tabletype;
        orderRequest.Tableno = tableno;
        orderRequest.Billno = billno;
        orderRequest.FoodName = foodname;
        orderRequest.TypeName = foodtypename;
        orderRequest.DeptName = deptname;
        orderRequest.unit = unit;
        orderRequest.quantity = quantity;
        orderRequest.operandi = operandis;
        orderRequest.taste = tastes;
        orderRequest.operatorid = opid;
        orderRequest.BarCode = barcode;
        orderRequest.OperateCode = operatecode; //OperateCode = 0 传菜 OperateCode = 1 催菜 OperateCode = 2 缓菜 OperateCode = 3 退菜
        orderRequest.CancelRsn = cancelrsn;
        queue.Send(orderRequest);
        //更新已传菜标志
        UpdateFoodTransSign(barcode);
        rtn = true;
        return rtn;
    }

    private bool ExecCurrentTableExistFood(string tableno, string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT count(*) FROM saledfoods where tableno='" + tableno + "' and foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()) > 0) //已经存在
            return true;
        else
            return false;
    }

    private string GetEmpID(string userid)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT empid FROM operator where operatorid='" + userid + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
            return "";
    }

    private string GetSysSetup(string parametercode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT value FROM syssetup where parametercode='" + parametercode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows.Count > 0)
        {
            return ds.Tables[0].Rows[0][0].ToString();
        }
        else
            return "";
    }

    private double ExecGetCurrentTableAllowCancelFoodQty(string tableno, string foodcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "SELECT sum(addquantity) as t1,sum(decquantity) as t2 FROM saledfoods where tableno='" + tableno + "' and foodcode='" + foodcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        return Convert.ToDouble(ds.Tables[0].Rows[0]["t1"].ToString()) - Convert.ToDouble(ds.Tables[0].Rows[0]["t2"].ToString());
    }

    //返回单据类型
    private string GetBillNoTypeCode(string billno)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        sqlstr = "SELECT billtypecode FROM salebill_t where billno='" + billno + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds, "t");
        return ds.Tables[0].Rows[0][0].ToString();
    }

    private double SuitInBillCount(string serialno, string suitcode)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        sqlstr = "select count(*),suitquantity from salelist_t where serialno='" + serialno + "' and suitcode='" + suitcode + "' group by suitcode,suitquantity";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds, "t");
        if (ds.Tables[0].Rows.Count > 0)
        {
            return Convert.ToDouble(ds.Tables[0].Rows[0]["suitquantity"].ToString());
        }
        else
            return 0;
    }

    private bool ExecAddSuitToBill(string tableno, string suitcode, string billno, double suitquantity, string userid)
    {
        //获取套餐中菜品
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        sqlstr = "SELECT foodcode,foodname,unit,foodqty,foodprice FROM view_suitfoods where suitcode='" + suitcode + "'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds, "t");
        //检测当前serialno下有无相同suitcode的套餐
        string CurrentTableSerialNo = GetCurrentTableSerialNo(tableno);
        double suitcount = SuitInBillCount(CurrentTableSerialNo, suitcode);
        //若有则 suitcount会 > 0，添加菜品时更新 suitquantity = suitquantity + suitcount
        double totalsuitquantity = suitquantity + suitcount;
        string SuitFoodGuid = GenUniqueCode();
        for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
        {
            //添加套餐中的菜品
            AddFoodtoBill(CurrentTableSerialNo,
                          SuitFoodGuid,
                          billno,
                          GetBillNoTypeCode(billno),
                          ds.Tables[0].Rows[i]["foodcode"].ToString(),
                          ds.Tables[0].Rows[i]["foodprice"].ToString(),
                          suitquantity * Convert.ToDouble(ds.Tables[0].Rows[i]["foodqty"].ToString()),
                          0,
                          "",
                          "",
                          userid, suitcode, suitquantity);
        }
        conn.Close();
        return true;
    }

    private int ExecModifyTablePsnCount(string tableno, int PsnCount)
    {
        //检测桌台属性
        //0=空闲，1=使用，2=预定中，-1=不存在该桌台
        //检测原桌台状态
        int rtn = -1;
        int scrTableStatus = GetCurrentTableStatus(tableno);
        switch (scrTableStatus)
        {
            case 0: //空闲 返回
                rtn = 0;
                break;
            case 1: //使用中
                SqlConnection conn = new SqlConnection(connstring);
                string sqlstr = "";
                sqlstr = "update TableStatus set peoplenumber=" + PsnCount.ToString() + " where tableno='" + tableno + "'";
                SqlCommand sqlcmd = new SqlCommand();
                sqlcmd.Connection = conn;
                sqlcmd.CommandText = sqlstr;
                conn.Open();
                sqlcmd.ExecuteNonQuery();
                rtn = 1;
                break;
            case 2: //预订中
                rtn = 2;
                break;
            default:
                rtn = 5;
                break;
        }
        return rtn;
    }

    [WebMethod]
    public bool AllowCancelFood(string userid)
    {
        SqlConnection conn = new SqlConnection(connstring);
        string sqlstr = "";
        sqlstr = "SELECT count(*) FROM op_acs where operatorid='" + userid + "' and access='tc'";
        SqlCommand sqlcmd = new SqlCommand();
        sqlcmd.Connection = conn;
        sqlcmd.CommandText = sqlstr;

        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = sqlcmd;
        conn.Open();
        DataSet ds = new DataSet();
        da.Fill(ds);
        conn.Close();
        if (ds.Tables[0].Rows.Count > 0)
        {
            if (ds.Tables[0].Rows[0][0].ToString() == "1")
                return true;
            else
                return false;
        }
        else
            return false;
    }

    [WebMethod]
    public string AllowPresentFood(string userid)
    {
        string empid = GetEmpID(userid);
        if (empid != "")
        {
            SqlConnection conn = new SqlConnection(connstring);
            string sqlstr = "";
            sqlstr = "SELECT presentsign,presentlimit,presenttotal FROM employee where empid='" + empid + "' and disabled=0 and presentsign=1";
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = conn;
            sqlcmd.CommandText = sqlstr;

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlcmd;
            conn.Open();
            DataSet ds = new DataSet();
            da.Fill(ds);
            conn.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0][0].ToString() + "," + ds.Tables[0].Rows[0][1].ToString() + "," + ds.Tables[0].Rows[0][2].ToString();
            }
            else
                return "";
        }
        else
            return "";
    }

    [WebMethod]
    public bool CurrentTableExistFood(string tableno, string foodname)
    {
        return ExecCurrentTableExistFood(tableno, GetFoodCode(foodname));
    }

    [WebMethod]
    public double GetCurrentTableAllowCancelFoodQty(string tableno, string foodname)
    {
        return ExecGetCurrentTableAllowCancelFoodQty(tableno, GetFoodCode(foodname));
    }

    //Hash加密
    private string MD5Crypt(string s)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedBytes;
        UTF8Encoding encoder = new UTF8Encoding();
        hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(s));
        return BitConverter.ToString(hashedBytes);
    }

    //-1 -2 -3 -4 验证失败 0 = 登录失败 1 = 成功
    [WebMethod]
    public int LogOn(string userid, string password, bool WriteDog)
    {
        try
        {
            if (userid == "")
            {
                return 0;
            }
            SqlConnection conn = new SqlConnection(connstring);
            SqlCommand sqlcmd = new SqlCommand();
            sqlcmd.Connection = conn;
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandText = "select count(*) from operator where operatorid='" + userid + "'" +
                                 " and " +
                                 "ppc_psw='" + password + "'" +
                                 " and " +
                                 "disabled=0 and PPC=1";
            SqlDataAdapter dbDa = new SqlDataAdapter();
            dbDa.SelectCommand = sqlcmd;
            DataSet objDs = new DataSet("ds");
            conn.Open();
            dbDa.Fill(objDs, "t");
            conn.Close();
            if (Convert.ToInt32(objDs.Tables["t"].Rows[0][0].ToString()) > 0)
            {
                return 1;
            }
            else
                return 0;
        }
        catch
        {
            return 0;
        }
    }

    [WebMethod]
    public bool LogOff(bool WriteDog)
    {
        //可以记录退出登录的信息.....
        return true;
    }

    [WebMethod]
    public string GetData(int target, int itemcount, int pageno,
                          string tableno, string billno,
                          string foodquery)
    {
        return GetDataOriginalXmlString(target, itemcount, pageno, connstring, tableno, billno, foodquery);
    }

    [WebMethod]
    //开台
    //    0 = 成功 1 = 使用中 2 = 预订中 3 = 加单失败 存在首张单据 4 = 加单失败 未知
    //    5 = 未知错误
    public int OpenTable(string tableno, int openpsncount, string userid)
    {
        try
        {
            return ExecOpenTable(tableno, openpsncount, "auto", userid);
        }
        catch
        {
            connstring = "";
            return 3;
        }
    }

    [WebMethod]
    public bool AddFoodToConsumeBill(string tableno, string billno, string foodcode,
                                     double addquantity, string operandis, string tastes,
                                     string userid)
    {
        string barcode = "";
        barcode = AddFoodtoBill(GetCurrentTableSerialNo(tableno),
                      GenUniqueCode(),
                      billno,
                      GetBillTypeCode2(billno),
                      foodcode,
                      Convert.ToString((addquantity * Convert.ToDouble(GetFoodPrice(foodcode, false)))),
                      addquantity,
                      0,
                      operandis,
                      tastes,
                      userid, "", 0);
        //检测是否自动传菜
        if (GetSysSetup("autoPrintFood") == "1") //点菜后自动传菜
        {
            ExecTransFood(foodcode, tableno, GetTableTypeName(tableno), billno,
                                 GetFoodName(foodcode), GetFoodTypeName(foodcode),
                                 GetFoodDeptTypeName(foodcode), GetFoodUnit(foodcode),
                                 addquantity.ToString(), operandis, tastes, userid, barcode, 0, "");
        }
        return true;
    }

    [WebMethod]
    public bool AddFoodToCancelBill(string tableno, string billno, string foodcode,
                                    double decquantity, string userid)
    {
        AddFoodtoBill(GetCurrentTableSerialNo(tableno),
                              GenUniqueCode(),
                              billno,
                              GetBillTypeCode2(billno),
                              foodcode,
                              Convert.ToString((decquantity * Convert.ToDouble(GetFoodPrice(foodcode, false)))),
                              0,
                              decquantity,
                              "",
                              "",
                              userid, "", 0);
        return true;
    }

    [WebMethod]
    public bool AddSuitToBill(string tableno, string suitcode, string billno, double suitquantity, string userid)
    {
        return ExecAddSuitToBill(tableno, suitcode, billno, suitquantity, userid);
    }

    [WebMethod]
    public bool TransFood(string foodname, string tableno, string billno,
                          string quantity, string operandis, string tastes, string opid,
                          string barcode, int operatecode, string cancelrsn)
    {
        //OperateCode = 0 传菜 
        //OperateCode = 1 催菜 
        //OperateCode = 2 缓菜 
        //OperateCode = 3 退菜
        string foodcode = GetFoodCode(foodname);
        return ExecTransFood(foodcode, tableno, GetTableTypeName(tableno), billno,
                             GetFoodName(foodcode), GetFoodTypeName(foodcode),
                             GetFoodDeptTypeName(foodcode), GetFoodUnit(foodcode),
                             quantity, operandis, tastes, opid, barcode, operatecode, cancelrsn);
    }

    [WebMethod]
    public string GetNewGuid()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
    }

    [WebMethod]
    public bool IsCancelBill(string billno)
    {
        string billtypecode = GetBillTypeCode2(billno);
        return ExecIsCancelBill(billtypecode, "");
    }

    [WebMethod]
    public bool IsPresentBill(string billno)
    {
        string billtypecode = GetBillTypeCode2(billno);
        return ExecIsPresentBill(billtypecode, "");
    }

    [WebMethod]
    public bool IsCustomFood(string foodcode)
    {
        return ExecIsCustomFood(foodcode);
    }

    [WebMethod]
    public int AddBill(string tableno, string billtypename, string userid)
    {
        int rtn = AddBillToTable(tableno, GenLastBillno(), billtypename, userid);
        return rtn;
    }

    [WebMethod]
    public string GetBillTypeName(string billno)
    {
        return GetBillTypeNameFromCode(GetBillTypeCode2(billno));
    }

    [WebMethod]
    public int MoveTable(string srcTableno, string dstTableno)
    {
        return ExecMoveTable(srcTableno, dstTableno);
    }

    [WebMethod]
    public int GetTableStatus(string Tableno)
    {
        return GetCurrentTableStatus(Tableno);
    }

    [WebMethod]
    public int CombineTable(string srcTableno, string dstTableno)
    {
        return ExecCompineTable(srcTableno, dstTableno);
    }

    [WebMethod]
    public int ModifyTablePsnCount(string Tableno, int PsnCount)
    {
        return ExecModifyTablePsnCount(Tableno, PsnCount);
    }
}
