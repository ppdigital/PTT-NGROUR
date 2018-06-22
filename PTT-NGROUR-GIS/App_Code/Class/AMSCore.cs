using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;

public class AMSCore
{
    public static string ApplicationUrl
    {
        get
        {
            return string.Format("{0}://{1}{2}/", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.ApplicationPath);
        }
    }

    public static DateTime StringToDateTime(string date)
    {
        string[] format = new string[] {
                "dd/MM/yyyy HH:mm:ss",
                "d/M/yyyy H:m:s",
                "dd/MM/yyyy HH:mm",
                "d/M/yyyy H:m",
                "dd/MM/yyyy",
                "d/M/yyyy",
                "HH:mm:ss",
                "HH:mm",
            };
        foreach (string f in format)
        {
            try
            {
                return StringToDateTime(date, f);
            }
            catch { }
        }
        throw new Exception(string.Format("ไม่สามารถแปลง {0} เป็น DateTime ได้", date));
    }
    public static DateTime StringToDateTime(string date, string format)
    {
        try
        {
            return DateTime.ParseExact(date, format, new System.Globalization.CultureInfo("th-TH"));
        }
        catch
        {
            throw new Exception(string.Format("ไม่สามารถแปลง {0} เป็น DateTime ได้", date));
        }
    }
    public static string DateTimeToString(DateTime? date, string format)
    {
        if (date != null && date.HasValue)
        {
            return DateTimeToString(date.Value, format);
        }
        return null;
    }
    public static string DateTimeToString(DateTime? date, string format, System.Globalization.CultureInfo cultureInfo)
    {
        if (date != null && date.HasValue)
        {
            return DateTimeToString(date.Value, format, cultureInfo);
        }
        return null;
    }
    public static string DateTimeToString(DateTime? date)
    {
        if (date != null && date.HasValue)
        {
            return DateTimeToString(date.Value);
        }
        return null;
    }
    public static string DateTimeToString(DateTime date)
    {
        return date.ToString("dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("th-TH"));
    }
    public static string DateTimeToString(DateTime date, string format)
    {
        return DateTimeToString(date, format, new System.Globalization.CultureInfo("th-TH"));
    }
    public static string DateTimeToString(DateTime date, string format, System.Globalization.CultureInfo cultureInfo)
    {
        return date.ToString(format, cultureInfo);
    }
    public static long DateTimeToUnixTimeStamp(DateTime date, double timezone = -420)
    {
        return long.Parse((date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(timezone * -1)).TotalMilliseconds.ToString("###0"));
    }
    public static DateTime UnixTimeStampToDateTime(long milliseconds, double timezone = -420)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds).AddMinutes(timezone * -1);
    }
    public static DateTime UnixTimeStampToDateTime(int seconds, double timezone = -420)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds).AddMinutes(timezone * -1);
    }

    public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt)
    {
        try
        {
            return DataTableToDictionary(dt, new System.Globalization.CultureInfo("th-TH"));
        }
        catch (Exception ex)
        {
            throw new Exception("ไม่สามารถแปลงจาก DataTable เป็น Dictionary ได้ เนื่องจาก " + ex.Message);
        }
    }
    public static List<Dictionary<string, object>> DataTableToDictionary(DataTable dt, System.Globalization.CultureInfo cultureInfo)
    {
        List<Dictionary<string, object>> dataDict = null;
        try
        {
            dataDict = dt.AsEnumerable().Select(dr => dt.Columns.Cast<DataColumn>().ToDictionary(
            dc => dc.ColumnName,
            dc => dr[dc] is DateTime
                ? DateTimeToString(dr[dc] as DateTime?, "dd/MM/yyyy HH:mm:ss", cultureInfo)
                : dr[dc]
                )).ToList();
            return dataDict;
        }
        catch (Exception ex)
        {
            throw new Exception("ไม่สามารถแปลงจาก DataTable เป็น Dictionary ได้ เนื่องจาก " + ex.Message);
        }
    }
    public static string CallRestService(string url, Connector.QueryParameter queryParameter)
    {
        System.Net.WebClient client = new System.Net.WebClient();
        client.BaseAddress = url;
        client.Encoding = new UTF8Encoding();
        //client.Headers = new System.Net.WebHeaderCollection();
        //client.Headers.Add("Content-Type", "application/json");
        System.Collections.Specialized.NameValueCollection values = new System.Collections.Specialized.NameValueCollection();
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        foreach (var param in queryParameter.Parameter)
        {
            string dictType = typeof(Dictionary<string, object>).Name;
            string arrayType = typeof(System.Collections.ArrayList).Name;
            string listType = typeof(List<object>).Name;
            string paramValue = string.Empty;
            string paramType = param.Value.GetType().Name;
            if (paramType == dictType || paramType == arrayType || paramType == listType)
            {
                paramValue = serializer.Serialize(param.Value);
            }
            else if (paramType == typeof(bool).Name)
            {
                paramValue = param.Value.Equals(true) ? "true" : "false";
            }
            else
            {
                paramValue = param.Value.ToString();
            }
            values.Add(param.Key, paramValue);
        }
        return Encoding.UTF8.GetString(client.UploadValues(url, "POST", values));

        // Add an Accept header for JSON format.
    }
    public static System.Configuration.ConnectionStringSettings GetConnectionString(string key = "dbConnection")
    {
        if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["PREFIX"]))
        {
            return ConfigurationManager.ConnectionStrings[key];
        }
        else
        {
            if (ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["PREFIX"] + key] != null)
            {
                return ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["PREFIX"] + key];
            }
            else
            {
                return ConfigurationManager.ConnectionStrings[key];
            }
        }
    }
    public static string WebConfigReadKey(string key)
    {
        string prefix = ConfigurationManager.AppSettings["PREFIX"];
        string value = string.Empty;

        value = ConfigurationManager.AppSettings[prefix + key];
        if (value == null)
        {
            value = ConfigurationManager.AppSettings[key];
        }
        if (!string.IsNullOrEmpty(value))
        {
            foreach (string _key in ConfigurationManager.AppSettings.Keys)
            {
                if (_key == "PREFIX") continue;
                string strReplace = string.Empty;
                if (!string.IsNullOrEmpty(prefix))
                {
                    strReplace = "${" + _key.Replace(prefix, "") + "}";
                }
                else
                {
                    strReplace = "${" + _key + "}";
                }
                if (value.IndexOf(strReplace) != -1)
                {
                    value = value.Replace(strReplace, WebConfigReadKey(_key));
                }
            }
        }

        return value;
    }
    public static List<string> WebConfigKeys()
    {
        var configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        var appSettingsSection = (AppSettingsSection)configuration.GetSection("appSettings");
        string prefix = ConfigurationManager.AppSettings["PREFIX"];
        List<string> keyConfigs = new List<string>();
        foreach (string _key in ConfigurationManager.AppSettings.Keys)
        {
            if (_key == "PREFIX") { continue; }
            if (appSettingsSection.Settings[_key].LockItem) continue;
            if (!string.IsNullOrEmpty(prefix))
            {
                if (_key.IndexOf(prefix) != 0) { continue; }
                keyConfigs.Add(_key.Replace(prefix, ""));
            }
            else
            {
                keyConfigs.Add(_key);
            }
        }
        foreach (string _key in ConfigurationManager.AppSettings.Keys)
        {
            if (_key == "PREFIX") { continue; }
            if (appSettingsSection.Settings[_key].LockItem) continue;
            if (keyConfigs.Count(o => _key.IndexOf(o) != -1) > 0) { continue; }
            keyConfigs.Add(_key);
        }
        return keyConfigs;
    }

    public static void WriteLogInfo(string message)
    {
        WriteLog("LogDetail", "info", message);
    }
    public static void WriteLogInfo(string format, params object[] args)
    {
        WriteLog("LogDetail", "info", format, null, args);
    }

    public static void WriteLogDebug(string message)
    {
        WriteLog("LogDetail", "debug", message);
    }
    public static void WriteLogDebug(string format, params object[] args)
    {
        WriteLog("LogDetail", "debug", format, null, args);
    }

    public static void WriteLogError(string message)
    {
        WriteLog("LogDetail", "error", message);
    }
    public static void WriteLogError(string format, params object[] args)
    {
        WriteLog("LogDetail", "error", format, null, args);
    }
    public static void WriteLogError(string message, Exception ex)
    {
        WriteLog("LogDetail", "error", message, ex);
    }

    public static void WriteRequestLogInfo(string message)
    {
        WriteLog("LogRequest", "info", message);
    }
    public static void WriteRequestLogInfo(string format, params object[] args)
    {
        WriteLog("LogRequest", "info", format, null, args);
    }

    public static void WriteRequestLogDebug(string message)
    {
        WriteLog("LogRequest", "debug", message);
    }
    public static void WriteRequestLogDebug(string format, params object[] args)
    {
        WriteLog("LogRequest", "debug", format, null, args);
    }

    public static void WriteRequestLogError(string message)
    {
        WriteLog("LogRequest", "error", message);
    }
    public static void WriteRequestLogError(string format, params object[] args)
    {
        WriteLog("LogRequest", "error", format, null, args);
    }
    public static void WriteRequestLogError(string message, Exception ex)
    {
        WriteLog("LogRequest", "error", message, ex);
    }

    private static void WriteLog(string logger, string logType, string message, Exception ex = null)
    {
        try
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ILog _log = log4net.LogManager.GetLogger(logger);
            if (logType == "info")
            {
                _log.Info(message, ex);
            }
            else if (logType == "error")
            {
                _log.Error(message, ex);
            }
            else if (logType == "debug")
            {
                _log.Debug(message, ex);
            }
        }
        catch { }
    }
    private static void WriteLog(string logger, string logType, string format, Exception ex = null, params object[] args)
    {
        try
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ILog _log = log4net.LogManager.GetLogger(logger);
            if (logType == "info")
            {
                _log.InfoFormat(format, args);
            }
            else if (logType == "error")
            {
                _log.ErrorFormat(format, args);
            }
            else if (logType == "debug")
            {
                _log.DebugFormat(format, args);
            }
        }
        catch { }
    }




    //public static bool SendEmail(Dictionary<string, object> jsonParams)
    //{
    //    try
    //    {
    //        System.Configuration.AppSettingsReader appSettingReader = new System.Configuration.AppSettingsReader();
    //        char _splitMail = ';';
    //        string sysPath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["gallerySysPath"]);

    //        using (MailMessage _mailMessage = new MailMessage())
    //        {

    //            string username = appSettingReader.GetValue("CREDENTIAL_USER", typeof(string)).ToString();
    //            string password = appSettingReader.GetValue("CREDENTIAL_PASS", typeof(string)).ToString();
    //            //string host = appSettingReader.GetValue("smtp_host", typeof(string)).ToString();
    //            //int port = Convert.ToInt32(appSettingReader.GetValue("smtp_port", typeof(int)));
    //            string defaultEmail = appSettingReader.GetValue("EMAILFROM", typeof(string)).ToString();
    //            NetworkCredential _credential = new NetworkCredential(username, password);

    //            if (!string.IsNullOrEmpty(jsonParams["MAILFROM"].ToString()))
    //            {
    //                defaultEmail = jsonParams["MAILFROM"].ToString();
    //            }

    //            _mailMessage.From = new MailAddress(defaultEmail);

    //            if (!string.IsNullOrEmpty(jsonParams["MAILTO"].ToString()))
    //            {
    //                string[] _arrTo = jsonParams["MAILTO"].ToString().Split(_splitMail);

    //                _arrTo.ToList<string>().ForEach(t => _mailMessage.To.Add(new MailAddress(t)));
    //            }

    //            if (!string.IsNullOrEmpty(jsonParams["MAILCC"].ToString()))
    //            {
    //                string[] _arrCC = jsonParams["MAILCC"].ToString().Split(_splitMail);

    //                _arrCC.ToList<string>().ForEach(

    //                    t => _mailMessage.CC.Add(new MailAddress(t))

    //                    );
    //            }

    //            if (!string.IsNullOrEmpty(jsonParams["MAILBCC"].ToString()))
    //            {
    //                string[] _arrBcc = jsonParams["MAILCC"].ToString().Split(_splitMail);

    //                _arrBcc.ToList<string>().ForEach(t => _mailMessage.Bcc.Add(new MailAddress(t)));
    //            }


    //            if (jsonParams["ATTACHMENTS"] != null && !string.IsNullOrEmpty(jsonParams["ATTACHMENTS"].ToString()))
    //            {

    //                string[] _arrBcc = jsonParams["ATTACHMENTS"].ToString().Split('|');

    //                for (int i = 0; i < _arrBcc.Length; i++)
    //                {
    //                    string path = Path.Combine(sysPath, _arrBcc[i].Replace("/", "\\"));
    //                    _mailMessage.Attachments.Add(new System.Net.Mail.Attachment(path));
    //                }

    //            }


    //            _mailMessage.Subject = jsonParams["MAILSUBJECT"].ToString();
    //            _mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
    //            _mailMessage.Body = jsonParams["MAILBODY"].ToString();
    //            _mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
    //            _mailMessage.IsBodyHtml = true;
    //            _mailMessage.Priority = MailPriority.Normal;

    //            //using (SmtpClient _smtpClient = new SmtpClient("CDGEMAIL.cdg.co.th"))
    //            using (SmtpClient _smtpClient = new SmtpClient())
    //            {

    //                _smtpClient.UseDefaultCredentials = false;
    //                //   _smtpClient.EnableSsl = true;
    //                _smtpClient.Credentials = _credential;
    //                //   _smtpClient.Port = port;

    //                _smtpClient.Send(_mailMessage);
    //            }


    //        }
    //        return true;
    //    }
    //    catch (SmtpFailedRecipientsException ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}

    public static bool SendEmail(Connector.QueryParameter queryParameter)
    {
        try
        {
            char splitMail = ';';
            using (MailMessage mailMessage = new MailMessage())
            {
                string server = AMSCore.WebConfigReadKey("EMAIL_SERVER");
                string port = AMSCore.WebConfigReadKey("EMAIL_PORT");

                string username = AMSCore.WebConfigReadKey("EMAIL_CREDENTIAL_USER");
                string password = AMSCore.WebConfigReadKey("EMAIL_CREDENTIAL_PASS");
                string defaultEmail = AMSCore.WebConfigReadKey("EMAIL_FROM");
                NetworkCredential credential = new NetworkCredential(username, password);
                if (queryParameter["MAIL_FROM"] != null && !string.IsNullOrEmpty(queryParameter["MAIL_FROM"].ToString()))
                {
                    defaultEmail = queryParameter["MAIL_FROM"].ToString();
                }
                mailMessage.From = new MailAddress(defaultEmail);

                if (queryParameter["MAIL_TO"] != null && !string.IsNullOrEmpty(queryParameter["MAIL_TO"].ToString()))
                {
                    string[] arrTo = queryParameter["MAIL_TO"].ToString().Split(splitMail);
                    arrTo.ToList<string>().ForEach(t => mailMessage.To.Add(new MailAddress(t)));
                }

                if (queryParameter["MAIL_CC"] != null && !string.IsNullOrEmpty(queryParameter["MAIL_CC"].ToString()))
                {
                    string[] arrCC = queryParameter["MAIL_CC"].ToString().Split(splitMail);
                    arrCC.ToList<string>().ForEach(t => mailMessage.CC.Add(new MailAddress(t)));
                }

                if (queryParameter["MAIL_BCC"] != null && !string.IsNullOrEmpty(queryParameter["MAIL_BCC"].ToString()))
                {
                    string[] arrBcc = queryParameter["MAIL_BCC"].ToString().Split(splitMail);
                    arrBcc.ToList<string>().ForEach(t => mailMessage.Bcc.Add(new MailAddress(t)));
                }

                if (queryParameter.Files != null && queryParameter.Files.Count > 0)
                {
                    foreach (Connector.FileParameter fileParameter in queryParameter.Files)
                    {
                        mailMessage.Attachments.Add(new System.Net.Mail.Attachment(fileParameter.File.FullName));
                    }
                }

                mailMessage.Subject = queryParameter["MAIL_SUBJECT"].ToString();
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Body = queryParameter["MAIL_BODY"].ToString();
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;

                using (SmtpClient smtpClient = new SmtpClient(server, int.Parse(port)))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = credential;
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
            }
            return true;
        }
        catch (SmtpFailedRecipientsException ex)
        {
            throw new Exception(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static Image ResizeImage(Image imageFile, Connector.QueryParameter queryParameter)
    {
        MemoryStream imageMemoryStream = new MemoryStream();
        imageFile.Save(imageMemoryStream, imageFile.RawFormat);

        /* reduce quality image */
        long fileSize = queryParameter["FILE_SIZE"] != null ? Int64.Parse(queryParameter["FILE_SIZE"].ToString()) : 0;
        if (fileSize > 0)
        {
            int quality = 100;
            while ((imageMemoryStream.Length > fileSize) && (quality > 0))
            {
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                quality -= 10;

                imageMemoryStream = new MemoryStream();
                imageFile.Save(imageMemoryStream, GetEncoder(ImageFormat.Jpeg), encoderParams);
            }

            //imageFile = Image.FromStream(imageMemoryStream);
            //imageFile.Save(@"C:\Temp\temp\test_q.jpg", imageFile.RawFormat);
        }

        /* resize image */
        int width = queryParameter["WIDTH"] != null ? Int32.Parse(queryParameter["WIDTH"].ToString()) : 0;
        int height = queryParameter["HEIGHT"] != null ? Int32.Parse(queryParameter["HEIGHT"].ToString()) : 0;
        if (width > 0 && height > 0)
        {
            // default value of autoRatio is true
            bool autoRatio = queryParameter["AUTO_RATIO"] != null ? Convert.ToBoolean(queryParameter["AUTO_RATIO"].ToString()) : true;
            if (autoRatio == true)
            {
                decimal ratioX = Decimal.Divide(width, imageFile.Width);
                decimal ratioY = Decimal.Divide(height, imageFile.Height);
                decimal ratio = Math.Min(ratioX, ratioY);
                width = (int)Decimal.Multiply(imageFile.Width, ratio);
                height = (int)Decimal.Multiply(imageFile.Height, ratio);
            }

            Bitmap newImage = new Bitmap(width, height);
            Graphics thumbGraph = Graphics.FromImage(newImage);

            imageFile = Image.FromStream(imageMemoryStream);
            thumbGraph.DrawImage(imageFile, 0, 0, width, height);

            imageMemoryStream = new MemoryStream();
            newImage.Save(imageMemoryStream, imageFile.RawFormat);

            //imageFile = Image.FromStream(imageMemoryStream);
            //imageFile.Save(@"C:\Temp\temp\test_s.jpg", imageFile.RawFormat);
        }

        return Image.FromStream(imageMemoryStream);
    }

    public static Image ResizeImage(string imageUrl, Connector.QueryParameter queryParameter)
    {
        HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imageUrl);
        using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
        {
            using (Stream stream = httpWebReponse.GetResponseStream())
            {
                return ResizeImage(Image.FromStream(stream), queryParameter);
            }
        }
    }

    public static Image CreateThumbnail(Image imageFile, Connector.QueryParameter queryParameter)
    {
        MemoryStream imageMemoryStream = new MemoryStream();
        imageFile.Save(imageMemoryStream, imageFile.RawFormat);

        int resizeRatio = 10;

        //resize_ratio

        if (queryParameter["RESIZE_RATIO"] != null)
        {
            if (Int32.Parse(queryParameter["RESIZE_RATIO"].ToString()) < 100)
            {
                resizeRatio = 100 - Int32.Parse(queryParameter["RESIZE_RATIO"].ToString());
            }
        }

        long fileSize = (imageMemoryStream.Length * resizeRatio) / 100;

        int defalutImageSize = 256;

        //IMAGESIZE

        if (queryParameter["IMAGESIZE"] != null)
        {
            if (Int32.Parse(queryParameter["IMAGESIZE"].ToString()) <= 100)
            {
                defalutImageSize = Int32.Parse(queryParameter["IMAGESIZE"].ToString());
            }
        }

        if (queryParameter["FILE_SIZE"] != null)
        {
            queryParameter["FILE_SIZE"] = fileSize;
        }
        else
        {
            queryParameter.Add("FILE_SIZE", fileSize);
        }

        queryParameter.Add("AUTO_RATIO", true);

        Image resultImage = ResizeImage(imageFile, queryParameter);
        queryParameter.Add("WIDTH", defalutImageSize);
        queryParameter.Add("HEIGHT", defalutImageSize);

        return FixedSizeThumbnail(resultImage, queryParameter);
    }

    public static Image FixedSizeThumbnail(Image imgPhoto, Connector.QueryParameter queryParameter)
    {
        int Width = Int32.Parse(queryParameter["WIDTH"].ToString());
        int Height = Int32.Parse(queryParameter["HEIGHT"].ToString());

        int sourceWidth = imgPhoto.Width;
        int sourceHeight = imgPhoto.Height;
        int sourceX = 0;
        int sourceY = 0;
        int destX = 0;
        int destY = 0;

        float nPercent = 0;
        float nPercentW = 0;
        float nPercentH = 0;

        nPercentW = ((float)Width / (float)sourceWidth);
        nPercentH = ((float)Height / (float)sourceHeight);
        if (nPercentH < nPercentW)
        {
            nPercent = nPercentH;
            destX = System.Convert.ToInt16((Width -
                          (sourceWidth * nPercent)) / 2);
        }
        else
        {
            nPercent = nPercentW;
            destY = System.Convert.ToInt16((Height -
                          (sourceHeight * nPercent)) / 2);
        }

        int destWidth = (int)(sourceWidth * nPercent);
        int destHeight = (int)(sourceHeight * nPercent);

        Bitmap bmPhoto = new Bitmap(Width, Height,
                          PixelFormat.Format24bppRgb);
        bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);


        bmPhoto.MakeTransparent(Color.Transparent);

        Graphics grPhoto = Graphics.FromImage(bmPhoto);
        grPhoto.Clear(Color.Transparent);



        grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        grPhoto.DrawImage(imgPhoto,
            new Rectangle(destX, destY, destWidth, destHeight),
            new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
            GraphicsUnit.Pixel);

        grPhoto.Dispose();
        string targetPath = AMSCore.WebConfigReadKey("THUMBNAIL_PATH");

        string[] fileName = queryParameter.Files[0].Name.Split('.');

        string fileSave = targetPath + "\\" + fileName[0] + "_Thumbnail" + "." + fileName[fileName.Length - 1].ToString();

        bmPhoto.Save(fileSave, ImageFormat.Png);
        return bmPhoto;
    }

    private static ImageCodecInfo GetEncoder(ImageFormat format)
    {
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

        foreach (ImageCodecInfo codec in codecs)
        {
            if (codec.FormatID == format.Guid)
            {
                return codec;
            }
        }
        return null;
    }

    public static Dictionary<string, object> CoordinateImage(Image image)
    {
        Dictionary<string, object> geotag = null;
        ulong[] Latitude = new ulong[6];
        ulong[] Longitude = new ulong[6];

        try
        {
            PropertyItem LatitudeProp = image.GetPropertyItem(0x0002);
            PropertyItem LongitudeProp = image.GetPropertyItem(0x0004);
            int[] imageCoorProp = new int[6] { 0, 4, 8, 12, 16, 20 };
            for (var i = 0; i < 6; i++)
            {
                Latitude[i] = BitConverter.ToUInt32(LatitudeProp.Value, imageCoorProp[i]);
                Longitude[i] = BitConverter.ToUInt32(LongitudeProp.Value, imageCoorProp[i]);
            }

            geotag = new Dictionary<string, object>(){
                {"x", ConvertCoordinate(Longitude)},
                {"y", ConvertCoordinate(Latitude)},
                {"spatialReference", new Dictionary<string, object>() {{"wkid",4326}}}
            };
        }
        catch
        {
            //ไฟล์ภาพไม่มี Properties ของ GPS
            geotag = new Dictionary<string, object>(){
                {"x", null},
                {"y", null},
                {"spatialReference", null}
            };
        }

        return geotag;
    }

    public static Dictionary<string, object> CoordinateImage(string imageUrl)
    {
        HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imageUrl);
        using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
        {
            using (Stream stream = httpWebReponse.GetResponseStream())
            {
                return CoordinateImage(Image.FromStream(stream));
            }
        }
    }

    private static double ConvertCoordinate(ulong[] coordinates)
    {
        double dd = double.Parse(coordinates[0].ToString()) / double.Parse(coordinates[1].ToString());
        double mm = double.Parse(coordinates[2].ToString()) / double.Parse(coordinates[3].ToString()) / 60;
        double ss = double.Parse(coordinates[4].ToString()) / double.Parse(coordinates[5].ToString()) / 3600;
        return dd + mm + ss;
    }

    #region 3DES
    private static readonly byte[] key = new byte[]
      {
         144,24,138,199,76,214,156,202,
         215,2,80,234,152,204,95,48,
         245,68,36,8,104,231,212,199
      };

    /// <summary>
    /// Encryption initialization vector.
    /// </summary>
    private static readonly byte[] iv = new byte[]
      {
         107,78,8,71,32,44,210,59
      };

    private static TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
    private static UTF8Encoding utf8 = new UTF8Encoding();

    /// <summary>
    /// Decrypt a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Decrypted data as string</returns>
    public static string Decrypt3DES(string text)
    {
        byte[] input = Convert.FromBase64String(text);
        byte[] output = Transform(input, des.CreateDecryptor(key, iv));
        return utf8.GetString(output);
    }

    /// <summary>
    /// Encrypt a string
    /// </summary>
    /// <param name="text"></param>
    /// <returns>Encrypted data as string</returns>
    public static string Encrypt3DES(string text)
    {
        byte[] input = utf8.GetBytes(text);
        byte[] output = Transform(input, des.CreateEncryptor(key, iv));
        return Convert.ToBase64String(output);
    }

    /// <summary>
    /// Encrypt or Decrypt bytes.
    /// </summary>
    /// <remarks>
    /// This is used by the public methods
    /// </remarks>
    /// <param name="input">Data to be encrypted/decrypted</param>
    /// <param name="cryptoTransform">
    /// <example>des.CreateEncryptor(this.keyValue, this.iVValue)</example>
    /// </param>
    /// <returns>Byte data containing result of opperation</returns>
    private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
    {
        // Create the necessary streams
        MemoryStream memory = new MemoryStream();
        CryptoStream stream = new CryptoStream(memory, cryptoTransform, CryptoStreamMode.Write);

        // Transform the bytes as requesed
        stream.Write(input, 0, input.Length);
        stream.FlushFinalBlock();

        // Read the memory stream and convert it back into byte array
        memory.Position = 0;
        byte[] result = new byte[memory.Length];
        memory.Read(result, 0, result.Length);

        // Clean up
        memory.Close();
        stream.Close();

        // Return result
        return result;
    }
    #endregion
}