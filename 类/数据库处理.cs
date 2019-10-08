using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace _2019HSQXSJK
{
    
    internal class 数据库处理
    {
        private string _con = "";
        private string _error = "";
        string adminCodes = "";
        public 数据库处理()
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
                _con = "Server=" + util.Read("OtherConfig", "DB", "Server");
                //解密连接数据库字符串
                DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
                _con = _con + helper.Decrypto(util.Read("OtherConfig", "DB", "Database"));
                adminCodes = 获取实况站点范围(0, 0, 0);
            }
            catch (Exception ex)
            {
                _error = ex.Message + "\r\n";
            }
        }
        public 数据库处理(string myadminCodes)
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
                _con = "Server=" + util.Read("OtherConfig", "DB", "Server");
                //解密连接数据库字符串
                DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
                _con = _con + helper.Decrypto(util.Read("OtherConfig", "DB", "Database"));
                adminCodes = myadminCodes;
            }
            catch (Exception ex)
            {
                _error = ex.Message + "\r\n";
            }
           

        }

        /// <summary>
        /// 按照要求获取实况站点范围
        /// </summary>
        /// <param name="TimeLevel">要素时效等级0—所有时次要素种类均入库；1—所有要素种类的分钟数据均入库；2—所有要素种类的小时数据均入库；3—所有要素种类的日数据均入库</param>
        /// <param name="DataLevel">要素种类等级0—所有要素种类均入库；100—所有地面实况；101—地面降水；102—地面气温；103——地面气压；104—地面相对湿度；105—10m风uv；200—所有高空实况</param>
        /// <param name="Class">ID种类0——————ID为地区行政编码;1——————ID为区站号行政编码</param>
        /// <returns>查询得到的区站号或者行政编码</returns>
        public string 获取实况站点范围(short TimeLevel, int DataLevel, short Class)
        {
            string fhStr = "";
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = $"select * from 实况站点范围 where TimeLevel={TimeLevel} and DataLevel={DataLevel} and Class={Class}"; //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        fhStr += sqlreader.GetString(sqlreader.GetOrdinal("ID")).Trim() + ',';
                    }

                    if (fhStr.Length > 0)
                    {
                        fhStr = fhStr.Substring(0, fhStr.Length - 1);
                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            return fhStr;
        }

        public List<入库个数统计信息> 获取入库个数统计信息(string 表名, DateTime sDate, DateTime eDate)
        {
            List<入库个数统计信息> rktj = new List<入库个数统计信息>();
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = $"select * from 获取入库个数统计信息 where 表名={表名} and 时间<={sDate} and 时间>={eDate}"; //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        try
                        {
                            rktj.Add(new 入库个数统计信息()
                            {
                                个数 = sqlreader.GetInt32(sqlreader.GetOrdinal("个数")),
                                时间 = sqlreader.GetDateTime(sqlreader.GetOrdinal("时间")),
                            });
                        }
                        catch
                        {
                        }

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            return rktj;
        }
        /// <summary>
        /// 将指定起止时间范围内的数据库统计情况保存至
        /// </summary>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        public void 统计信息入库(string 表名,DateTime sDate, DateTime eDate)
        {
            DataTable dataTable = 获取指定时间范围表名小时入库信息(表名, sDate, eDate);
            SqlBulkCopyByDatatable(_con, "入库个数统计信息_LS", dataTable);
        }
        /// <summary>
        /// 获取指定时间范围、指定表名的分钟级入库信息
        /// </summary>
        /// <param name="表名"></param>
        /// <param name="sDate"></param>
        /// <param name="eDate"></param>
        /// <returns></returns>
        public DataTable 获取指定时间范围表名分钟入库信息(string 表名, DateTime sDate, DateTime eDate)
        {
            DataTable dt = new DataTable("入库个数统计信息_LS");
            dt.Columns.Add("表名", Type.GetType("System.String"));
            dt.Columns.Add("时间", Type.GetType("System.DateTime"));
            dt.Columns.Add("个数", Type.GetType("System.Int32"));
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = $"select count(1) 个数,DateTime 时间 from {表名} where DateTime between '{sDate}' and '{eDate}' group by DateTime";
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        try
                        {
                            DataRow dr = dt.NewRow();
                            dr["表名"] = 表名;
                            dr["时间"] = sqlreader.GetDateTime(sqlreader.GetOrdinal("时间"));
                            dr["个数"] = sqlreader.GetInt32(sqlreader.GetOrdinal("个数"));
                            dt.Rows.Add(dr);
                        }
                        catch(Exception e)
                        {
                        }

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            return dt;
        }

        public DataTable 获取指定时间范围表名小时入库信息(string 表名, DateTime sDate, DateTime eDate)
        {
            DataTable dt = new DataTable("入库个数统计信息_LS");
            dt.Columns.Add("表名", Type.GetType("System.String"));
            dt.Columns.Add("时间", Type.GetType("System.DateTime"));
            dt.Columns.Add("个数", Type.GetType("System.Int32"));
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                try
                {
                    mycon.Open(); //打开
                    string sql = $"select count(1) 个数,(dateadd(hh,datepart(hh,DateTime),CONVERT(varchar(10), DateTime, 23))) 时间 from {表名} where DateTime between '{sDate.ToString("yyyy-MM-dd HH:00:00")}' and '{eDate.ToString("yyyy-MM-dd HH:59:59")}' group by (dateadd(hh,datepart(hh,DateTime),CONVERT(varchar(10), DateTime, 23)))";
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        try
                        {
                            DataRow dr = dt.NewRow();
                            dr["表名"] = 表名;
                            dr["时间"] = sqlreader.GetDateTime(sqlreader.GetOrdinal("时间"));
                            dr["个数"] = sqlreader.GetInt32(sqlreader.GetOrdinal("个数"));
                            dt.Rows.Add(dr);
                        }
                        catch (Exception e)
                        {
                        }

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            return dt;
        }

        public string 分钟降水量入库(DateTime sDate, DateTime eDate)
        {
           
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_Pre_Minute_byTimeRangeAndRegion(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();
            DataTable dt = new DataTable("SK_Pre_Minute_LS");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("Pre_Minute", Type.GetType("System.Single"));

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls > 0 && fls < 999900)
                    {
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{Convert.ToDateTime(szls[0]).ToLocalTime()}'").Length > 0)
                            break;
                        DataRow dr = dt.NewRow();
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = Convert.ToDateTime(szls[0]).ToLocalTime();
                        dr["Pre_Minute"] = fls;
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_Pre_Minute_LS", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条降水量数据。\n";
            统计信息入库("SK_Pre_Minute", sDate, eDate);
            dt = null;
            return fhStr;
        }

        public string 分钟常规温度入库(DateTime sDate, DateTime eDate)
        {
          if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_Tem_Minute_byTimeRangeAndRegion_SURF_CHN_MAIN_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();
            DataTable dt = new DataTable("SK_Tem_Minute_LS1");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("Tem_Minute", Type.GetType("System.Single"));

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls > -999900 && fls < 999900)
                    {
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{Convert.ToDateTime(szls[0]).ToLocalTime()}'").Length > 0)
                            break;
                        DataRow dr = dt.NewRow();
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = Convert.ToDateTime(szls[0]).ToLocalTime();
                        dr["Tem_Minute"] = fls;
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_Tem_Minute_LS1", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条常规温度分钟数据。\n";
            统计信息入库("SK_Tem_Minute",sDate, eDate);
            dt = null;
            return fhStr;
        }
        public string 分钟其他温度入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_Tem_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();
            
            DataTable dt = new DataTable("SK_Tem_Minute_LS2");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("Tem_Max_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("Tem_Max_Time_Minute", Type.GetType("System.DateTime"));
            dt.Columns.Add("Tem_Min_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("Tem_Min_Time_Minute", Type.GetType("System.DateTime"));
            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    float fls2 = Convert.ToSingle(szls[4]);
                    if (fls2 < 999900 && fls < 999900)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);
                        DataRow dr = dt.NewRow();
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{dtls.ToLocalTime()}'").Length>0)
                            break;
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = dtls.ToLocalTime();
                        dr["Tem_Max_Minute"] = fls;
                        dr["Tem_Min_Minute"] = fls2;
                        dr["Tem_Max_Time_Minute"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[3])).ToLocalTime();
                        dr["Tem_Min_Time_Minute"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[5])).ToLocalTime();
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_Tem_Minute_LS2", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条其他温度分钟数据。\n";
            dt = null;
            return fhStr;
        }
        public DateTime CIMISSDateTimeMinuteConvert(DateTime dateTime,string time)
        {
            DateTime myDate = dateTime;
            time = time.PadLeft(4, '0');
            try
            {
                myDate = dateTime.Date.AddHours(Convert.ToInt32(time.Substring(0, 2))).AddMinutes(Convert.ToInt32(time.Substring(2, 2)));
                if ((myDate - dateTime).TotalHours > 1)
                    myDate = myDate.AddDays(-1);
            }
            catch
            {
            }
            return myDate;
        }
        public string 分钟常规气压入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_PRS_Minute_byTimeRangeAndRegion_SURF_CHN_MAIN_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();
            DataTable dt = new DataTable("SK_PRS_Minute_LS1");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("PRS_Minute", Type.GetType("System.Single"));

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls > -999900 && fls < 999900)
                    {
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{Convert.ToDateTime(szls[0]).ToLocalTime()}'").Length > 0)
                            break;
                        DataRow dr = dt.NewRow();
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = Convert.ToDateTime(szls[0]).ToLocalTime();
                        dr["PRS_Minute"] = fls;
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_PRS_Minute_LS1", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条常规气压分钟数据。\n";
            统计信息入库("SK_PRS_Minute",sDate, eDate);
            dt = null;
            return fhStr;
        }
        public string 分钟其他气压入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_PRS_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();

            DataTable dt = new DataTable("SK_PRS_Minute_LS2");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("PRS_Max_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("PRS_Max_Time_Minute", Type.GetType("System.DateTime"));
            dt.Columns.Add("PRS_Min_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("PRS_Min_Time_Minute", Type.GetType("System.DateTime"));
            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    float fls2 = Convert.ToSingle(szls[4]);
                    if (fls2 < 999900 && fls < 999900)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);
                        DataRow dr = dt.NewRow();
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{dtls.ToLocalTime()}'").Length > 0)
                            break;
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = dtls.ToLocalTime();
                        dr["PRS_Max_Minute"] = fls;
                        dr["PRS_Min_Minute"] = fls2;
                        dr["PRS_Max_Time_Minute"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[3])).ToLocalTime();
                        dr["PRS_Min_Time_Minute"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[5])).ToLocalTime();
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_PRS_Minute_LS2", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条其他气压分钟数据。\n";
            dt = null;
            return fhStr;
        }
        public string 分钟常规湿度入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_RHU_Minute_byTimeRangeAndRegion_SURF_CHN_MAIN_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();
            DataTable dt = new DataTable("SK_RHU_Minute_LS1");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("RHU_Minute", Type.GetType("System.Single"));

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls >=0 && fls < 999990)
                    {
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{Convert.ToDateTime(szls[0]).ToLocalTime()}'").Length > 0)
                            break;
                        DataRow dr = dt.NewRow();
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = Convert.ToDateTime(szls[0]).ToLocalTime();
                        dr["RHU_Minute"] = fls;
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_RHU_Minute_LS1", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条常规湿度分钟数据。\n";
            统计信息入库("SK_RHU_Minute", sDate, eDate);
            dt = null;
            return fhStr;
        }
        public string 分钟其他湿度入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_RHU_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();

            DataTable dt = new DataTable("SK_RHU_Minute_LS2");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("DPT_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("RHU_Min_Minute", Type.GetType("System.Single"));
            dt.Columns.Add("RHU_Min_Time_Minute", Type.GetType("System.DateTime"));
            dt.Columns.Add("VAP_Minute", Type.GetType("System.Single"));
            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    float fls2 = Convert.ToSingle(szls[3]);
                    if (fls2 < 999990 && fls < 999990)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);
                        DataRow dr = dt.NewRow();
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{dtls.ToLocalTime()}'").Length > 0)
                            break;
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = dtls.ToLocalTime();
                        dr["DPT_Minute"] = fls;
                        dr["RHU_Min_Minute"] = fls2;
                        dr["RHU_Min_Time_Minute"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[4])).ToLocalTime();
                        dr["VAP_Minute"] = Convert.ToSingle(szls[5]);
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\r\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_RHU_Minute_LS2", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条其他湿度分钟数据。\n";
            dt = null;
            return fhStr;
        }
        public string 分钟其他风入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_WIND_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();

            DataTable dt = new DataTable("SK_Wind_Minute_LS2");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("WIN_D_Avg_2mi", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_Avg_2mi", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_D_Avg_10mi", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_Avg_10mi", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_D_S_Max", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_Max", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_Max_OTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("WIN_D_INST", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_INST", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_D_INST_Max", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_Inst_Max", Type.GetType("System.Single"));
            dt.Columns.Add("WIN_S_INST_Max_OTime", Type.GetType("System.DateTime"));
            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[3]);
                    if (fls >= 0 && fls < 999990)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);
                        DataRow dr = dt.NewRow();
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{dtls.ToLocalTime()}'").Length > 0)
                            break;
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = dtls.ToLocalTime();
                        dr["WIN_D_Avg_2mi"] = Convert.ToSingle(szls[2]);
                        dr["WIN_S_Avg_2mi"] = fls;
                        dr["WIN_D_Avg_10mi"] = Convert.ToSingle(szls[4]);
                        dr["WIN_S_Avg_10mi"] = Convert.ToSingle(szls[5]);
                        dr["WIN_D_S_Max"] = Convert.ToSingle(szls[6]);
                        dr["WIN_S_Max"] = Convert.ToSingle(szls[7]);
                        dr["WIN_S_Max_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[8])).ToLocalTime();
                        dr["WIN_D_INST"] = Convert.ToSingle(szls[9]);
                        dr["WIN_S_INST"] = Convert.ToSingle(szls[10]);
                        dr["WIN_D_INST_Max"] = Convert.ToSingle(szls[11]);
                        dr["WIN_S_Inst_Max"] = Convert.ToSingle(szls[12]);
                        dr["WIN_S_INST_Max_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[13])).ToLocalTime();
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_Wind_Minute_LS2", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条分钟风数据。\n";
            统计信息入库("SK_Wind_Minute", sDate, eDate);
            dt = null;
            return fhStr;
        }
        public string 分钟其他资料入库(DateTime sDate, DateTime eDate)
        {
            if (adminCodes.Trim().Length == 0)
                return "";
            CIMISS获取数据 cIMISS = new CIMISS获取数据();
            string myData = cIMISS.CIMISS_SK_Other_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(sDate, eDate, adminCodes);
            if (myData.Trim().Length == 0)
                return "";
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return "";
            //List<DanYS> danYs = new List<DanYS>();

            DataTable dt = new DataTable("SK_Other_Minute_LS2");
            dt.Columns.Add("StationID", Type.GetType("System.String"));
            dt.Columns.Add("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("EVP_Big", Type.GetType("System.Single"));
            dt.Columns.Add("GST_Max", Type.GetType("System.Single"));
            dt.Columns.Add("GST_Max_Otime", Type.GetType("System.DateTime"));
            dt.Columns.Add("GST_Min", Type.GetType("System.Single"));
            dt.Columns.Add("GST_Min_OTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("GST_80cm", Type.GetType("System.Single"));
            dt.Columns.Add("GST_160cm", Type.GetType("System.Single"));
            dt.Columns.Add("GST_320cm", Type.GetType("System.Single"));
            dt.Columns.Add("LGST_Max", Type.GetType("System.Single"));
            dt.Columns.Add("LGST_Max_OTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("LGST_Min", Type.GetType("System.Single"));
            dt.Columns.Add("LGST_Min_OTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("VIS_HOR_1MI", Type.GetType("System.Single"));
            dt.Columns.Add("VIS_HOR_10MI", Type.GetType("System.Single"));
            dt.Columns.Add("VIS_Min", Type.GetType("System.Single"));
            dt.Columns.Add("VIS_Min_OTime", Type.GetType("System.DateTime"));
            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[3]);
                    if (fls > -999990 && fls < 999990)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);
                        DataRow dr = dt.NewRow();
                        if (dt.Select($"StationID='{szls[1]}' and DateTime='{dtls.ToLocalTime()}'").Length > 0)
                            break;
                        dr["StationID"] = szls[1];
                        dr["DateTime"] = dtls.ToLocalTime();
                        dr["EVP_Big"] = Convert.ToSingle(szls[2]);
                        dr["GST_Max"] = fls;
                        dr["GST_Max_Otime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[4])).ToLocalTime();
                        dr["GST_Min"] = Convert.ToSingle(szls[5]);
                        dr["GST_Min_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[6])).ToLocalTime();
                        dr["GST_80cm"] = Convert.ToSingle(szls[7]);
                        dr["GST_160cm"] = Convert.ToSingle(szls[8]);
                        dr["GST_320cm"] = Convert.ToSingle(szls[9]);
                        dr["LGST_Max"] = Convert.ToSingle(szls[10]);
                        dr["LGST_Max_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[11])).ToLocalTime();
                        dr["LGST_Min"] = Convert.ToSingle(szls[12]);
                        dr["LGST_Min_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[13])).ToLocalTime();
                        dr["VIS_HOR_1MI"] = Convert.ToSingle(szls[14]);
                        dr["VIS_HOR_10MI"] = Convert.ToSingle(szls[15]);
                        dr["VIS_Min"] = Convert.ToSingle(szls[16]);
                        dr["VIS_Min_OTime"] = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[17])).ToLocalTime();
                        dt.Rows.Add(dr);

                    }
                }
                catch (Exception e)
                {
                    _error += e.Message + "\n";
                }
            }

            if (dt.Rows.Count == 0)
                return "";
            SqlBulkCopyByDatatable(_con, "SK_Other_Minute_LS2", dt);
            string fhStr = DateTime.Now + "成功保存" + sDate + "至" + eDate + $"      {dt.Rows.Count}条其他分钟数据。\n";
            统计信息入库("SK_Other_Minute", sDate, eDate);
            dt = null;
            return fhStr;
        }
        #region  SqlBulkCopy批量快速入库
        /// <summary>
        /// SqlBulkCopy批量快速入库
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">数据源</param>
        private void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
        {
            using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.FireTriggers))
            {
                try
                {
                    sqlbulkcopy.DestinationTableName = TableName;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }
                    sqlbulkcopy.WriteToServer(dt);
                    dt = null;
                }
                catch (Exception e)
                {
                }
            }
        }
        #endregion
        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>
        #region LIST转换为Datatable
        private DataTable ToDataTable<T>(List<T> items)

        {
            var tb = new DataTable(typeof(T).Name);


            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);


            foreach (PropertyInfo prop in props)

            {
                Type t = GetCoreType(prop.PropertyType);

                tb.Columns.Add(prop.Name, t);
            }


            foreach (T item in items)

            {
                var values = new object[props.Length];


                for (int i = 0; i < props.Length; i++)

                {
                    values[i] = props[i].GetValue(item, null);
                }


                tb.Rows.Add(values);
            }


            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)

        {
            return !t.IsValueType || t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)

        {
            if (t != null && IsNullable(t))

            {
                if (!t.IsValueType)

                {
                    return t;
                }

                return Nullable.GetUnderlyingType(t);
            }

            return t;
        }
        #endregion
        public class DanYS
        {
            public string StationID { get; set; }
            public DateTime DateTime { get; set; }
            public float Pre_Minute { get; set; }
        }

        public class 入库个数统计信息
        {
            public string 表名 { get; set; }
            public DateTime 时间 { get; set; }
            public int 个数 { get; set; }
        }
    }
}
