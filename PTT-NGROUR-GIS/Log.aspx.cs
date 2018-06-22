using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class Log : AMSBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string requestType = string.IsNullOrEmpty(Request["REQUESTTYPE"]) ? "" : Request["REQUESTTYPE"];

        // string logFilter = string.IsNullOrEmpty(Request["LOGFILTER"]) ? "" : Request["LOGFILTER"];
        //query
        string age = string.IsNullOrEmpty(Request["AGE"]) ? "" : Request["AGE"];
        string dateAge = string.IsNullOrEmpty(Request["DATEAGE"]) ? "" : Request["DATEAGE"];
        string monthAge = string.IsNullOrEmpty(Request["MONTHAGE"]) ? "" : Request["MONTHAGE"];
        string source = string.IsNullOrEmpty(Request["SOURCE"]) ? "" : Request["SOURCE"];
        string search = string.IsNullOrEmpty(Request["SEARCH"]) ? "" : Request["SEARCH"];
        string machine = string.IsNullOrEmpty(Request["MACHINE"]) ? "" : Request["MACHINE"];

        string startTime = string.IsNullOrEmpty(Request["STARTTIME"]) ? "" : Request["STARTTIME"];
        string endTime = string.IsNullOrEmpty(Request["ENDTIME"]) ? "" : Request["ENDTIME"];

        string type = string.IsNullOrEmpty(Request["TYPE"]) ? "" : Request["TYPE"];
        string retrospect = string.IsNullOrEmpty(Request["RETROSPECT"]) ? "" : Request["RETROSPECT"];

        string strFilter = string.IsNullOrEmpty(Request["FILTER"]) ? "" : Request["FILTER"];

        //param Setting
        string keepLog = string.IsNullOrEmpty(Request["KEEPLOG"]) ? "" : Request["KEEPLOG"];
        string logPath = string.IsNullOrEmpty(Request["LOGPATH"]) ? "" : Request["LOGPATH"];


        string[] logFilter = new string[] { };

        if (age == "month")
        {
            dateAge = monthAge;
        }

        if (strFilter == "")
        {
            logFilter = new string[] { 
               // "ERROR", "WARN", "INFO", "DEBUG" 
            };
        }
        else
        {
            logFilter = strFilter.Split(',');
        }


        //   searchFile();
        if (requestType != "")
        {
            //  ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "Func()", true);\
            //ClientScript.RegisterStartupScript
            //    (GetType(), "Javascript", "javascript: Func(); ", true);

            //   dateAge = "26/08/2558";
            if (requestType == "query")
            {
                searchFile(logFilter, age, dateAge, startTime, endTime, source, machine, search);
            }
            else if (requestType == "delete")
            {
                deleteLog(type, retrospect);
            }
            else if (requestType == "setting")
            {
                WriteXmlFile(keepLog, logPath);
            }
            else if (requestType == "login")
            {
                //  string sysPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["logsPath"]);
                Login();
            }
            else if (requestType == "readXml")
            {
                ReadXmlFileLogin();
            }

        }
        else
        {

            if (string.IsNullOrEmpty((string)Session["DVS_LOGIN"]))
            {

            }
            else
            {
                ReadXmlFile();
                InsertScriptTag("var login =true;");
            }

        }

    }

    void Page_LoadComplete(object sender, EventArgs e)
    {
        // call your download function
        // ReadXmlFile();
    }

    private void Login()
    {

        Session["DVS_LOGIN"] = "1";



        Connector.QueryResult qResult = new Connector.QueryResult();
        Response.ContentType = "application/json";
        Response.Write(qResult.ToJson());
        Response.End();

        //ReadXmlFile();
    }

    private void deleteLog(string type, string retrospectDay)
    {
        Connector.QueryResult qResult = new Connector.QueryResult();
        try
        {
            DateTime endDate = DateTime.Now;
            int minus = 0;
            if (type == "number")
            {
                minus = Int32.Parse(retrospectDay) * (-1);
            }
            DateTime startDate = endDate.AddDays(minus);
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo("D:\\ProjectCode\\NHA\\NHAE145027\\Log3");

            foreach (DirectoryInfo direct in downloadedMessageInfo.GetDirectories())
            {
                foreach (FileInfo file in direct.GetFiles())
                {
                    if (type != "all")
                    {
                        if (startDate.Date <= file.CreationTime.Date && endDate.Date >= file.CreationTime.Date)
                        {
                            file.Delete();
                        }
                    }
                    else
                    {
                        file.Delete();
                    }
                }
                if (direct.GetFiles().Length <= 0)
                {
                    direct.Delete();
                }
            }
            qResult.Success = true;
        }
        catch (Exception ex)
        {
            qResult.Success = false;
            qResult.Message = ex.Message;
        }

        Response.ContentType = "application/json";
        Response.Write(qResult.ToJson());
        Response.End();

    }

    private void searchFile(string[] logFilter, string age, string strDateAge, string startTime, string endTime, string source, string machine, string search)
    {


        Connector.QueryResult connect = new Connector.QueryResult();
        Dictionary<string, object> objList = new Dictionary<string, object>();

        try
        {

            string[] filterType = new string[4] { "ERROR", "WARN", "INFO", "DEBUG" };

            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(System.IO.Path.Combine(machine));

            // System.IO.Path.Combine(machine);

            // System.IO.StreamReader file = new System.IO.StreamReader();

            DateTime dateAge = new DateTime();
            DateTime startDateObj = startDate(age, strDateAge);
            DateTime endDateObj = endDate(age, strDateAge);




            int startT = int.Parse(startTime);
            int endT = int.Parse(endTime);
            string strHr = "";
            int hr = 0;

            foreach (DirectoryInfo direct in downloadedMessageInfo.GetDirectories())
            {

                Dictionary<string, List<string>> resultDic = new Dictionary<string, List<string>>();
                if ((startDateObj.Month <= direct.CreationTime.Month && startDateObj.Year <= direct.CreationTime.Year)
                    && (endDateObj.Month >= direct.CreationTime.Month && endDateObj.Year >= direct.CreationTime.Year))
                {

                    foreach (FileInfo file in direct.GetFiles())
                    {

                        List<string> resultList = new List<string>();

                        try
                        {
                            if (startDateObj.Date <= file.CreationTime.Date && endDateObj.Date >= file.CreationTime.Date)
                            {
                                if ((file.Name.ToUpper().IndexOf(source.ToUpper()) >= 0) || source == "all")
                                {

                                    //   System.IO.StreamReader StrRead = new System.IO.StreamReader(file.FullName, Encoding.Default);


                                    using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    {
                                        using (StreamReader StrRead = new StreamReader(fileStream, Encoding.Default))
                                        {

                                            string txtRead = "";
                                            int i = 0;

                                            //  string json = StrRead.ReadToEnd();

                                            while (!(StrRead.EndOfStream))
                                            {
                                                string txt = StrRead.ReadLine().ToString();

                                                if (Array.IndexOf(filterType, txt.Split(' ')[0].Trim()) < 0 || i == 0)
                                                {
                                                    txtRead += txt + "<br/>";
                                                }
                                                else
                                                {
                                                    if (Array.IndexOf(logFilter, txtRead.Split(' ')[0].Trim()) >= 0)
                                                    {
                                                        strHr = txtRead.Split(':')[0].Trim();
                                                        hr = int.Parse(strHr.Substring(strHr.Length - 2, 2));
                                                        if (hr >= startT && hr <= endT)
                                                        {
                                                            resultList.Add(txtRead);
                                                        }
                                                    }

                                                    txtRead = txt + "<br/>"; ;
                                                }

                                                i++;
                                            }

                                            StrRead.Close();
                                            StrRead.Dispose();

                                            if (Array.IndexOf(logFilter, txtRead.Split(' ')[0].Trim()) >= 0)
                                            {
                                                strHr = txtRead.Split(':')[0].Trim();
                                                hr = int.Parse(strHr.Substring(strHr.Length - 2, 2));
                                                if (hr >= startT && hr <= endT)
                                                {
                                                    resultList.Add(txtRead);
                                                }
                                            }


                                            if (resultList.Count > 0)
                                            {

                                                resultDic.Add(file.Name, resultList);
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }

                objList.Add(direct.Name.ToString(), resultDic);
            }

            connect.AddOutputParam("data", objList);
            connect.AddOutputParam("search", search);
        }
        catch (Exception ex)
        {
            connect.Message = ex.Message;
            connect.Success = false;
        }



        Response.ContentType = "application/json";
        Response.Write(connect.ToJson());
        Response.End();

    }


    public void ReadXmlFile()
    {

        Setting overview = new Setting();
        try
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Setting));

            System.IO.StreamReader file = new System.IO.StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configLog.xml"));
            overview = (Setting)reader.Deserialize(file);
            file.Close();
            file.Dispose();

            InsertScriptTag("var keeplog =" + overview.keeplog + ";" + "var strLogpath ='" + string.Join("|", overview.logpath) + "';");
        }
        catch (Exception ex)
        {
            string defaultLogPath = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "logs");
            InsertScriptTag("var keeplog =" + "90" + ";" + "var strLogpath ='" + defaultLogPath.Replace("\\", "/") + "';");

        }

    }

    public void ReadXmlFileLogin()
    {

        Setting overview = new Setting();
        string data;
        Connector.QueryResult qResult = new Connector.QueryResult();
        Dictionary<string, string> dataDic = new Dictionary<string, string>();


        try
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Setting));
            //System.IO.StreamReader file = new System.IO.StreamReader("D:\\ProjectCode\\NHA\\NHAE145027\\xml\\configLog.xml");
            System.IO.StreamReader file = new System.IO.StreamReader(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configLog.xml"));
            overview = (Setting)reader.Deserialize(file);
            file.Close();
            file.Dispose();

            dataDic.Add("keeplog", overview.keeplog);
            dataDic.Add("strLogpath", string.Join("|", overview.logpath));
        }
        catch (Exception ex)
        {
            //System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName



            string startupPath = System.IO.Path.GetFullPath(".\\");

            dataDic.Add("keeplog", "90");
            string defaultLogPath = System.IO.Path.Combine(System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "logs");
            dataDic.Add("strLogpath", string.Join("|", defaultLogPath));

            qResult.Success = true;
            qResult.Message = "{}";
        }


        qResult.AddOutputParam("data", dataDic);


        Response.ContentType = "application/json";
        Response.Write(qResult.ToJson());
        Response.End();
    }



    public void WriteXmlFile(string keepLog, string logPath)
    {
        Connector.QueryResult qResult = new Connector.QueryResult();

        System.Xml.Serialization.XmlSerializer writer;
        System.IO.FileStream file;
        Setting overview = new Setting();
        List<string> listlogPath = new List<string>();
        List<string> arr = new List<string>();

        try
        {

            for (int i = 0; i <= logPath.Split(',').Length - 1; i++)
            {
                if (logPath.Split(',')[i] != "")
                {
                    arr.Add(logPath.Split(',')[i]);
                }
            }


            overview.keeplog = keepLog;
            overview.logpath = arr.ToArray();


            writer = new System.Xml.Serialization.XmlSerializer(typeof(Setting));

            file = new System.IO.FileStream(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configLog.xml"), FileMode.Truncate, FileAccess.Write);

            writer.Serialize(file, overview);
            file.Close();
            file.Dispose();

            Dictionary<string, string> dic = new Dictionary<string, string>();


            dic.Add("keeplog", keepLog);
            dic.Add("strLogpath", string.Join("|", arr));

            qResult.AddOutputParam("data", dic);
        }
        catch (Exception ex)
        {
            writer = new System.Xml.Serialization.XmlSerializer(typeof(Setting));
            file = new System.IO.FileStream(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configLog.xml"), FileMode.CreateNew, FileAccess.Write);

            writer.Serialize(file, overview);
            file.Close();
            file.Dispose();

            Dictionary<string, string> dic = new Dictionary<string, string>();


            dic.Add("keeplog", keepLog);
            dic.Add("strLogpath", string.Join("|", arr));

            qResult.AddOutputParam("data", dic);


            qResult.Success = false;
            qResult.Message = ex.Message;
        }

        Response.ContentType = "application/json";
        Response.Write(qResult.ToJson());
        Response.End();
    }



    private DateTime startDate(string age, string strDateAge)
    {
        DateTime resultDate = new DateTime();

        DateTime dateStart = DateTime.Now;

        if (age == "day")
        {
            string[] str = strDateAge.Split('/');
            resultDate = new DateTime(Int32.Parse(str[2]) - 543, Int32.Parse(str[1]), Int32.Parse(str[0]));
        }
        else if (age == "month")
        {
            string[] str = strDateAge.Split('/');
            resultDate = new DateTime(Int32.Parse(str[1]) - 543, Int32.Parse(str[0]), 1);
        }
        else if (age == "thismonth")
        {
            resultDate = new DateTime(dateStart.Year, dateStart.Month, 01);
        }
        else if (age == "lastmonth")
        {
            resultDate = new DateTime(dateStart.Year, dateStart.Month - 1, 01);
        }
        else if (age == "all")
        {
            resultDate = dateStart.AddMonths(-3);
        }
        else if (age == "yesterday")
        {
            resultDate = dateStart.AddDays(-1);
        }
        else
        {
            int miuns = numberDate(age);
            resultDate = dateStart.AddDays(miuns);
        }


        return resultDate;
    }


    private DateTime endDate(string age, string strDateAge)
    {

        DateTime resultDate = new DateTime();

        DateTime dateStart = DateTime.Now;

        if (age == "day")
        {
            string[] str = strDateAge.Split('/');
            resultDate = new DateTime(Int32.Parse(str[2]) - 543, Int32.Parse(str[1]), Int32.Parse(str[0]));
        }
        else if (age == "month")
        {
            string[] str = strDateAge.Split('/');

            DateTime monthDate = new DateTime(Int32.Parse(str[1]) - 543, Int32.Parse(str[0]), 1);

            resultDate = new DateTime(monthDate.Year, monthDate.Month, DateTime.DaysInMonth(monthDate.Year, monthDate.Month));
        }
        else if (age == "thismonth")
        {
            resultDate = new DateTime(dateStart.Year, dateStart.Month, DateTime.DaysInMonth(dateStart.Year, dateStart.Month));
        }
        else if (age == "lastmonth")
        {
            resultDate = new DateTime(dateStart.Year, dateStart.Month - 1, DateTime.DaysInMonth(dateStart.Year, dateStart.Month - 1));
        }
        else if (age == "all")
        {
            resultDate = dateStart;
        }
        else if (age == "yesterday")
        {
            resultDate = dateStart.AddDays(-1);
        }
        else
        {
            int miuns = numberDate(age);
            resultDate = dateStart;
        }

        return resultDate;
    }

    private int numberDate(string age)
    {

        int result = 0;

        switch (age)
        {
            case "today":
                result = 0;
                break;
            //case "yesterday":
            //    result = -1;
            //    break;
            case "lastthreedays":
                result = -3;
                break;
            case "lastweek":
                result = -7;
                break;
            default:
                break;
        }

        return result;
    }

    public class Setting
    {
        public String keeplog;
        public String[] logpath;
        // public List<string> machine;
    }

}