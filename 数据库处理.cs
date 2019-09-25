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

        public 数据库处理()
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\config\基本信息.xml");
                _con = "Server=" + util.Read("OtherConfig", "DB", "Server");
                //解密连接数据库字符串
                DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
                _con = _con + helper.Decrypto(util.Read("OtherConfig", "DB", "Database"));
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

        public string 分钟降水量入库(DateTime sDate, DateTime eDate)
        {
            string adminCodes = 获取实况站点范围(0, 0, 0);
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
                }
                catch (Exception)
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
