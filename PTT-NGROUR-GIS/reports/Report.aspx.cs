using Connector;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public partial class reports_Report : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                QueryParameter rptData = new QueryParameter(Request);
                Initialize(rptData);
                //=====================================
                //============ Code Here ============

                string renderType = "PDF";            // Word || PDF || Image
                string fileExtention = ".pdf";         // doc || pdf || .png


                Dictionary<string, object> rptConfig = null;
                if (rptData.Parameter.ContainsKey("rptConfig"))
                {
                    rptConfig = rptData.Parameter["rptConfig"] as Dictionary<string, object>;
                    fileExtention = rptConfig["EXTENSION"] as string;
                    if (fileExtention == ".doc") renderType = "Word";
                    else if (fileExtention == ".pdf") renderType = "PDF";
                    else if (fileExtention == ".png") renderType = "Image";
                }

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                string deviceInf = string.Empty;
                byte[] bytes = ReportViewer1.LocalReport.Render(renderType, deviceInf, out mimeType, out encoding, out extension, out streamIds, out warnings);
                string fileName = String.IsNullOrEmpty(ReportViewer1.LocalReport.DisplayName) ? "untitled" : ReportViewer1.LocalReport.DisplayName;

                // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName.Trim() + fileExtention);
                Response.BinaryWrite(bytes); // create the file
                Response.Flush(); // send it to the client to download


                //=====================================
            }
            catch (Exception ex)
            {

            }
        }
    }

    #region Initialize
    private List<string> DisableExport = new List<string>();
    private void Initialize(QueryParameter rptData)
    {

        Dictionary<string, object> rptParams = null;
        Dictionary<string, object> rptDatasets = null;
        Dictionary<string, object> rptConfig = null;
        string rdlcPath = null;
        string reportName = null;

        if (rptData.Parameter.ContainsKey("rptConfig"))
        {
            rptConfig = rptData.Parameter["rptConfig"] as Dictionary<string, object>;
            if (rptConfig.ContainsKey("RDLC_PATH"))
            {
                rdlcPath = rptConfig["RDLC_PATH"] as string;
                var paperSize = int.Parse(rptConfig["PAPER_SIZE"].ToString());
                var paperType = int.Parse(rptConfig["FILE_TYPE"].ToString());

                if (paperSize == 1 && paperType == 1) rdlcPath = rdlcPath + "A3Portrait.rdlc";
                else if (paperSize == 1 && paperType == 2) rdlcPath = rdlcPath + "A3Landscape.rdlc";
                else if (paperSize == 2 && paperType == 1) rdlcPath = rdlcPath + "A4Portrait.rdlc";
                else if (paperSize == 2 && paperType == 2) rdlcPath = rdlcPath + "A4Landscape.rdlc";

                if (string.IsNullOrEmpty(rdlcPath))
                {
                    throw new Exception("RDLC_PATH is null or empty.");
                }
            }

            if (rptConfig.ContainsKey("REPORT_NAME"))
            {
                reportName = rptConfig["REPORT_NAME"] as string;
            }

            if (rptConfig.ContainsKey("DISABLE_EXPORT"))
            {
                string export = rptConfig["DISABLE_EXPORT"] as string;
                if (!string.IsNullOrEmpty(export))
                {
                    string[] arrExport = export.Split(',');
                    foreach (string str in arrExport)
                    {
                        switch (str.Trim().ToLower())
                        {
                            case "excel":
                                this.DisableExport.Add("EXCELOPENXML");
                                break;
                            case "word":
                                this.DisableExport.Add("WORDOPENXML");
                                break;
                            case "pdf":
                                this.DisableExport.Add("PDF");
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            throw new Exception("Not found a \"rptConfig\" parameter.");
        }
        ReportViewer1.LocalReport.ReportPath = "reports/rdlc/" + rdlcPath;
        ReportViewer1.LocalReport.EnableExternalImages = true;
        ReportViewer1.LocalReport.DisplayName = reportName;
        Header.Title = reportName;

        if (rptData.Parameter.ContainsKey("rptParams"))
        {
            rptParams = rptData.Parameter["rptParams"] as Dictionary<string, object>;
            AddReportParameter(rptParams);
        }
        if (rptData.Parameter.ContainsKey("rptDatasets"))
        {
            if (rptData.Parameter["rptDatasets"] != null)
            {
                rptDatasets = rptData.Parameter["rptDatasets"] as Dictionary<string, object>;
                AddDataset(rptDatasets);
            }
        }
    }
    #region Report Parameter and Dataset
    private void AddReportParameter(Dictionary<string, object> rptParams)
    {
        if (rptParams != null && rptParams.Count > 0)
        {
            foreach (KeyValuePair<string, object> pair in rptParams)
            {
                AddReportParameter(pair.Key, pair.Value);
            }
        }
    }
    private void AddReportParameter(string name, object value)
    {
        ReportParameter param = null;
        if (value == null)
        {
            //param = new ReportParameter(name, null);
        }
        else
        {
            if (value.GetType() == typeof(ArrayList))
            {
                List<string> strList = new List<string>();
                foreach (object o in value as ArrayList)
                {
                    object oVal = o;
                    if (oVal == null)
                    {
                        oVal = string.Empty;
                    }
                    strList.Add(oVal.ToString());
                }
                param = new ReportParameter(name);
                param.Values.AddRange(strList.ToArray());
            }
            else
            {
                param = new ReportParameter(name, value.ToString());
            }

            ReportViewer1.LocalReport.SetParameters(param);
        }
    }
    private void AddDataset(Dictionary<string, object> rptDatasets)
    {
        if (rptDatasets != null && rptDatasets.Count > 0)
        {
            foreach (KeyValuePair<string, object> pair in rptDatasets)
            {
                if (pair.Value == null)
                {
                    //
                }
                else
                {
                    DataTable dt = new DataTable();
                    if (pair.Value.GetType() == typeof(ArrayList))
                    {
                        ArrayList arrList = pair.Value as ArrayList;
                        //Dictionary<string, object>[] dictArr = (Dictionary<string, object>[])arrList.ToArray(typeof(Dictionary<string, object>));
                        //List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>(dictArr);
                        //dt = DictionaryListToDataTable(dictList);
                        dt = ArrayListToDataTable(arrList);
                    }
                    else if (pair.Value.GetType() == typeof(Dictionary<string, object>))
                    {
                        Dictionary<string, object> dict = pair.Value as Dictionary<string, object>;
                        if (dict.ContainsKey("SP"))
                        {
                            QueryParameter queryParam = new QueryParameter(dict);
                            IDatabaseConnector dbConnector = new DatabaseConnectorClass();
                            QueryResult result = dbConnector.ExecuteStoredProcedure(queryParam);
                            if (result.Success && result.DataTable != null)
                            {
                                dt = result.DataTable;
                            }
                        }
                    }
                    AddDataset(pair.Key, dt);
                }
            }
        }
    }
    private void AddDataset(string name, DataTable dt)
    {
        ReportViewer1.LocalReport.DataSources.Add(new ReportDataSource(name, dt));
    }
    #endregion
    private DataTable ArrayListToDataTable(ArrayList arrayList)
    {
        DataTable dt = new DataTable();
        if (arrayList.Count > 0)
        {
            Dictionary<string, object> dict = arrayList[0] as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> oPair in dict)
            {
                dt.Columns.Add(oPair.Key.ToUpper());
            }

            foreach (object o in arrayList)
            {
                dict = o as Dictionary<string, object>;
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                foreach (KeyValuePair<string, object> oPair in dict)
                {
                    if (dt.Columns.Contains(oPair.Key))
                    {
                        dr[oPair.Key.ToUpper()] = oPair.Value;
                    }
                }
            }
        }

        return dt;
    }
    protected void ReportViewer1_PreRender(object sender, EventArgs e)
    {
        System.Reflection.FieldInfo info;
        foreach (Microsoft.Reporting.WebForms.RenderingExtension extension in ReportViewer1.LocalReport.ListRenderingExtensions())
        {
            if (this.DisableExport.Contains(extension.Name))
            {
                info = extension.GetType().GetField("m_isVisible", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                info.SetValue(extension, false);
            }
        }
    }
    #endregion
}