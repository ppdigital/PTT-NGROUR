using Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;

public class AMSNotificationManager
{
    private static string _serviceName = "NotificationManager";
    private static string _fcmPushURL = AMSCore.WebConfigReadKey("FCM_PUCH_URL");
    private static string _fcmServerKey = AMSCore.WebConfigReadKey("FCM_SERVER_KEY");
    private static JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
    private static int _idsPerRequest = 1000;

    //ของเดิม
    public static void Push(List<AMSNotificationPack> notificationPacks)
    {
        try
        {
            List<AMSNotificationPack> groupedNotificationPacks = new List<AMSNotificationPack>();
            foreach (AMSNotificationPack notificationPack in notificationPacks)
            {
                if (notificationPack.to.Count > 0)
                {
                    int loopsCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(notificationPack.to.Count / Convert.ToDouble(_idsPerRequest))));
                    for (int i = 0; i < loopsCount; i++)
                    {
                        AMSNotificationPack groupedNotificationPack = new AMSNotificationPack();
                        int startIndex = i * _idsPerRequest;
                        int size = notificationPack.to.Count - startIndex;
                        groupedNotificationPack.to.AddRange(notificationPack.to.GetRange(startIndex, size > _idsPerRequest ? _idsPerRequest : size));
                        groupedNotificationPack.notification = notificationPack.notification;
                        groupedNotificationPack.data = notificationPack.data;
                        groupedNotificationPack.priority = notificationPack.priority;
                        groupedNotificationPack.content_available = notificationPack.content_available;
                        groupedNotificationPacks.Add(groupedNotificationPack);
                    }
                }
            }
            foreach (AMSNotificationPack notificationPack in groupedNotificationPacks)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_fcmPushURL);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", string.Format("key={0}", _fcmServerKey));
                byte[] bytes = Encoding.UTF8.GetBytes(_jsSerializer.Serialize(notificationPack));
                using (Stream outputStream = request.GetRequestStream())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                new Task(() =>
                {
                    request.GetResponse();
                }).Start();
            }
        }
        catch (Exception e)
        {
        }
    }
    public static void Push(AMSNotificationPack notificationPack)
    {
        Push(new List<AMSNotificationPack>() { notificationPack });
    }

    //เพิ่มใหม่
    //กรณีมี data ที่อยู่ใน pattern สำหรับส่ง Notification
    public static void Push(QueryResult NTqueryResult)
    {
        List<AMSNotificationPack> notificationPacks;
        try
        {
            if (NTqueryResult.Success)
            {
                notificationPacks = ConvertDataTableToNotificationPacks(NTqueryResult.DataTable);
                Push(notificationPacks);
            }
            else
            {
                throw new Exception(NTqueryResult.Message);
            }
        }
        catch (Exception e)
        {

        }

    }
    //กรณี ส่ง parameter ที่มาจาก client
    public static void Push(QueryParameter queryParam)
    {
        Push(queryParam, null);
    }
    //กรณี ส่ง parameter ที่มาจาก client และจาก SP
    public static void Push(QueryParameter queryParam, Dictionary<string, object> NTParameters)
    {
        string NT_SP = string.Empty;
        QueryParameter NTParam;
        QueryResult NTqueryResult;
        try
        {
            if (queryParam != null)
            {
                NTParam = GetNTParameter(queryParam, NTParameters);
            }
            else
            {
                throw new Exception("ไม่มี parameter สำหรับส่ง notitfication");
            }

            if (queryParam["NT"] != null)
            {
                NT_SP = queryParam["NT"].ToString();
            }
            else
            {
                throw new Exception("parameter 'NT' ไม่มี stored procedure สำหรับส่ง notitfication");
            }

            IDatabaseConnector dbConnector = new DatabaseConnectorClass();

            new Task(() =>
            {
                NTqueryResult = dbConnector.ExecuteStoredProcedure(NT_SP, NTParam);
                Push(NTqueryResult);
            }).Start();
        }
        catch (Exception e)
        {

        }

    }

    //ของเดิม สำหรับแปลง parameter ที่ post มาเป็น json ในรูปแบบสำหรับส่ง notification
    public static AMSNotificationPack ConvertJsonToNotificcationPack(QueryParameter qParams)
    {
        try
        {
            AMSNotificationPack notificationPack = new AMSNotificationPack();
            List<string> registration_ids = GetListFromObject(qParams["registration_ids"]).Cast<string>().ToList();
            if (registration_ids.Count < 1)
            {
                throw (new Exception("registration_ids must has at least 1 token."));
            }
            Dictionary<string, object> notification = qParams["notification"] as Dictionary<string, object>;
            notificationPack.to.AddRange(registration_ids);
            notificationPack.notification.title = notification["title"] as string;
            notificationPack.notification.body = notification["body"] as string;
            notificationPack.notification.icon = notification["icon"] as string;
            notificationPack.notification.badge = notification["badge"] as string;
            notificationPack.notification.sound = notification["sound"] as string;
            notificationPack.data = qParams["data"] as Dictionary<string, object>;
            notificationPack.priority = qParams["priority"] as string;
            notificationPack.content_available = Convert.ToBoolean(qParams["content_available"]);
            return notificationPack;
        }
        catch (Exception e)
        {
            throw (e);
        }
    }

    private static List<object> GetListFromObject(object obj)
    {
        try
        {
            if (obj == null)
            {
                return new List<object>();
            }
            List<object> tmpList;
            if (obj.GetType() == typeof(ArrayList))
            {
                tmpList = (obj as ArrayList).Cast<object>().ToList();
            }
            else if (obj.GetType() == typeof(List<object>))
            {
                tmpList = obj as List<object>;
            }
            else
            {
                throw (new Exception("Cannot convert object to List<object>."));
            }
            return tmpList;
        }
        catch (Exception e)
        {
            throw (new Exception(string.Format("{0} -> GetList: {1}", _serviceName, e.Message)));
        }
    }

    private static QueryParameter GetNTParameter(QueryParameter queryParam, Dictionary<string, object> NTParameters)
    {
        QueryParameter NTParam = new QueryParameter();

        //รวบ NT parameter จาก queryParam
        if (queryParam != null && queryParam.Parameter != null)
        {
            foreach (KeyValuePair<string, object> p in queryParam.Parameter)
            {
                if (p.Key.IndexOf("NT_") == 0) //check ว่ามี NT_ นำหน้าคือ parameter สำหรับส่ง Notification
                {
                    NTParam.Add(p.Key, p.Value);
                }
            }
        }

        //รวบ NT parameter จาก PO ของ SP ==> PO_NT_XXXX
        if (NTParameters != null)
        {
            foreach (KeyValuePair<string, object> p in NTParameters)
            {
                //if (p.Key.IndexOf("NT_") == 0) //check ว่ามี NT_ นำหน้าคือ parameter สำหรับส่ง Notification
                //{
                NTParam.Add(p.Key, p.Value);
                //}
            }
        }

        return NTParam;
    }
    private static List<AMSNotificationPack> ConvertDataTableToNotificationPacks(DataTable table)
    {
        List<AMSNotificationPack> notificationPacks = new List<AMSNotificationPack>();

        if (table.Rows.Count > 0)
        {
            DataColumnCollection columns = table.Columns;
            foreach (DataRow row in table.Rows)
            {
                Dictionary<string, object> dataCollection = new Dictionary<string, object>();
                AMSNotificationPack notificationPack = new AMSNotificationPack();
                AMSNotificationConfig notification = notificationPack.notification;
                notificationPack.data = dataCollection;
                notificationPacks.Add(notificationPack);

                foreach (DataColumn column in columns)
                {
                    string cName = column.ColumnName;
                    string cValue = row[cName].ToString();

                    switch (cName)
                    {
                        case "REGISTRATION_ID":
                            string[] arrValue = cValue.Split('|');
                            foreach (string val in arrValue)
                            {
                                notificationPack.to.Add(val.Trim());
                            }
                            break;
                        case "TITLE":
                            notification.title = cValue;
                            break;
                        case "BODY":
                            notification.body = cValue;
                            break;
                        case "ICON":
                            notification.icon = cValue;
                            break;
                        case "SOUND":
                            notification.sound = cValue;
                            break;
                        case "BADGE":
                            notification.badge = cValue;
                            break;

                        default:
                            dataCollection.Add(cName, cValue);
                            break;
                    }
                }
            }
        }
        else
        {
            throw new Exception("ไม่มี Data สำหรับส่ง Notification");
        }

        return notificationPacks;
    }
}