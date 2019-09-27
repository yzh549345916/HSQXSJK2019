using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace _2019HSQXSJK
{
    class CIMISS获取数据
    {
        string _userId = "BEHT_BFHT_2131";
        string _pwd = "YZHHGDJM";
        /// <summary>
        /// 根据行政区域，起止时间从CIMISS获取分钟降水量
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">行政区域编码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_Pre_Minute_byTimeRangeAndRegion(DateTime sDate, DateTime eDate, string adminCodes)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                String userId = _userId;// 
                String pwd = _pwd;// 
                /*   2.2 接口ID */
                String interfaceId1 = "getSurfEleInRegionByTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<String, String> paramsqx = new Dictionary<String, String>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_PRE_MIN"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C,PRE");
                paramsqx.Add("adminCodes", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                String dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder();//返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }
                    else
                    {

                        strData = "";
                    }
                }
                catch
                {
                    strData = "";
                }
                return strData;
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 根据行政区域，起止时间从CIMISS通过SURF_CHN_MAIN_MIN获取分钟温度
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">行政区域编码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_Tem_Minute_byTimeRangeAndRegion_SURF_CHN_MAIN_MIN(DateTime sDate, DateTime eDate, string adminCodes)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                String userId = _userId;// 
                String pwd = _pwd;// 
                /*   2.2 接口ID */
                String interfaceId1 = "getSurfEleInRegionByTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<String, String> paramsqx = new Dictionary<String, String>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_MAIN_MIN"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C,TEM");
                paramsqx.Add("adminCodes", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                String dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder();//返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }
                    else
                    {

                        strData = "";
                    }
                }
                catch
                {
                    strData = "";
                }
                return strData;
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 根据行政区域，起止时间从CIMISS通过SURF_CHN_OTHER_MIN获取分钟温度
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">行政区域编码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_Tem_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(DateTime sDate, DateTime eDate, string adminCodes)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                String userId = _userId;// 
                String pwd = _pwd;// 
                /*   2.2 接口ID */
                String interfaceId1 = "getSurfEleInRegionByTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<String, String> paramsqx = new Dictionary<String, String>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_OTHER_MIN"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C,TEM_Max,TEM_Max_OTime,TEM_Min,TEM_Min_OTime");
                paramsqx.Add("adminCodes", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                String dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder();//返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }
                    else
                    {

                        strData = "";
                    }
                }
                catch
                {
                    strData = "";
                }
                return strData;
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 根据行政区域，起止时间从CIMISS通过SURF_CHN_MAIN_MIN获取分钟气压
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">行政区域编码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_PRS_Minute_byTimeRangeAndRegion_SURF_CHN_MAIN_MIN(DateTime sDate, DateTime eDate, string adminCodes)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                String userId = _userId;// 
                String pwd = _pwd;// 
                /*   2.2 接口ID */
                String interfaceId1 = "getSurfEleInRegionByTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<String, String> paramsqx = new Dictionary<String, String>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_MAIN_MIN"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C,PRS");
                paramsqx.Add("adminCodes", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                String dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder();//返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }
                    else
                    {

                        strData = "";
                    }
                }
                catch
                {
                    strData = "";
                }
                return strData;
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 根据行政区域，起止时间从CIMISS通过SURF_CHN_OTHER_MIN获取分钟气压
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">行政区域编码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_PRS_Minute_byTimeRangeAndRegion_SURF_CHN_OTHER_MIN(DateTime sDate, DateTime eDate, string adminCodes)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                String userId = _userId;// 
                String pwd = _pwd;// 
                /*   2.2 接口ID */
                String interfaceId1 = "getSurfEleInRegionByTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<String, String> paramsqx = new Dictionary<String, String>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_OTHER_MIN"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C,PRS_Max,PRS_Max_OTime,PRS_Min,PRS_Min_OTime");
                paramsqx.Add("adminCodes", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                String dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder();//返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }
                    else
                    {

                        strData = "";
                    }
                }
                catch
                {
                    strData = "";
                }
                return strData;
            }
            catch (Exception)
            {
                return "";
            }

        }
    }
}
