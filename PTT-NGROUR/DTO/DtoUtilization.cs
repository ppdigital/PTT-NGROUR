﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;

namespace PTT_NGROUR.DTO
{
    public class DtoUtilization
    {
        private enum enumExcelPipelineColumn
        {
            RC_NAME = 1,
            FLOW = 2,
            DIAMETER = 3,
            LENGTH = 4,
            EFFICIENCY = 5,
            ROUGHNESS = 6,
            LOAD = 7,
            VELOCITY = 8,
            OUTSIDE_DIAMETER = 9,
            WALL_THICKNESS = 10,
            SERVICE_STATE = 11,
            MONTH = 12,
            YEAR = 13,
            UPLOAD_DATE = 14,
            UPLOAD_BY = 15,
            REGION_ID = 16,
        }

        private enum enumExcelGateColumn
        {
            GateName = 1,
            GatePressure = 2,
            GateFlow = 3,
            GateDescription = 4
        }
        public IEnumerable<Models.DataModel.ModelGateStationImport> ReadExcelGateStationImport(
        System.IO.Stream pStreamExcel,
        int pIntMonth,
        string pStrRegionId,
        string pStrUploadBy,
        int pIntYear)
        {
            if (pStreamExcel == null || pStreamExcel.Length <= 0)
            {
                yield break;
            }
            using (var exel = new OfficeOpenXml.ExcelPackage(pStreamExcel))
            {
                var exWorkSheet = exel.Workbook.Worksheets[1];

                int intColCount = exWorkSheet.Dimension.End.Column;
                int intRowCount = exWorkSheet.Dimension.End.Row;

                int intStartRow = 4;
                int intEndRow = intRowCount - 6;
                for (int intRow = intStartRow; intRow < intEndRow; ++intRow)
                {
                    var gateImport = new Models.DataModel.ModelGateStationImport()
                    {
                        FLOW = getExcelGateValue(exWorkSheet, intRow, enumExcelGateColumn.GateFlow).GetDecimal(),
                        GATE_NAME = getExcelGateValue(exWorkSheet, intRow, enumExcelGateColumn.GateName).GetString(),
                        MONTH = pIntMonth,
                        PRESSURE = getExcelGateValue(exWorkSheet, intRow, enumExcelGateColumn.GatePressure).GetDecimal(),
                        REGION = pStrRegionId,
                        UPLOAD_BY = pStrUploadBy,
                        UPLOAD_DATE = DateTime.Now,
                        YEAR = pIntYear
                    };
                    yield return gateImport;
                }
            }
        }

        public IEnumerable<Models.DataModel.ModelPipelineImport> ReadExcelPipelineImport(
        System.IO.Stream pStreamExcel,
        int pIntMonth,
        string pStrRegionId,
        string pStrUploadBy,
        int pIntYear)
        {
            if (pStreamExcel == null || pStreamExcel.Length <= 0)
            {
                yield break;
            }
            using (var exel = new OfficeOpenXml.ExcelPackage(pStreamExcel))
            {
                var exWorkSheet = exel.Workbook.Worksheets[1];

                int intColCount = exWorkSheet.Dimension.End.Column;
                int intRowCount = exWorkSheet.Dimension.End.Row;

                int intStartRow = 4;
                int intEndRow = intRowCount - 6;
                for (int intRow = intStartRow; intRow < intEndRow; ++intRow)
                {
                    var pipImport = new Models.DataModel.ModelPipelineImport()
                    {
                        DIAMETER = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.DIAMETER).GetDecimal(),
                        EFFICIENCY = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.EFFICIENCY).GetDecimal(),
                        FLOW = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.FLOW).GetDecimal(),
                        LENGTH = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.LENGTH).GetDecimal(),
                        LOAD = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.LOAD).GetDecimal(),
                        MONTH = pIntMonth,
                        OUTSIDE_DIAMETER = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.OUTSIDE_DIAMETER).GetDecimal(),
                        PIPELINE_ID = 0,
                        RC_NAME = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.RC_NAME).GetString(),
                        REGION = pStrRegionId,
                        ROUGHNESS = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.ROUGHNESS).GetDecimal(),
                        SERVICE_STATE = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.SERVICE_STATE).GetString(),
                        UPLOAD_BY = pStrUploadBy,
                        UPLOAD_DATE = DateTime.Now,
                        VELOCITY = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.VELOCITY).GetDecimal(),
                        WALL_THICKNESS = getExcelPipelineValue(exWorkSheet, intRow, enumExcelPipelineColumn.WALL_THICKNESS).GetDecimal(),
                        YEAR = pIntYear
                    };
                    yield return pipImport;
                }
            };

        }

        private object getExcelPipelineValue(OfficeOpenXml.ExcelWorksheet pWorkSheet, int intRow, enumExcelPipelineColumn pCol)
        {
            if (pWorkSheet == null)
            {
                return null;
            }
            return pWorkSheet.GetValue(intRow, (int)pCol);
        }

        private object getExcelGateValue(OfficeOpenXml.ExcelWorksheet pWorkSheet, int intRow, enumExcelGateColumn pCol)
        {
            if (pWorkSheet == null)
            {
                return null;
            }
            return pWorkSheet.GetValue(intRow, (int)pCol);
        }

        public IEnumerable<string> GetListRcProject()
        {
            var dal = new DAL.DAL();
            string strCommand = "select distinct RC_Project from GIS_NGR_PL";
            var result = dal.ReadData<string>(strCommand, x =>
            {
                return x["RC_Project"].GetString();
            });
            return result;
        }

        public IEnumerable<string> GetListGisGateStationName()
        {
            var dal = new DAL.DAL();
            string strCommand = "select distinct Name from GIS_Gate_Station";
            var result = dal.ReadData<string>(strCommand, x =>
            {
                return x["Name"].GetString();
            });
            return result;
        }

        public void InsertPipelineImport(Models.DataModel.ModelPipelineImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"INSERT INTO PIPELINE_IMPORT ( RC_NAME, FLOW, DIAMETER, LENGTH, EFFICIENCY, 
                ROUGHNESS, LOAD, VELOCITY, OUTSIDE_DIAMETER, WALL_THICKNESS, SERVICE_STATE,
                MONTH, YEAR, UPLOAD_DATE, UPLOAD_BY, REGION) 
                VALUES ( '{0}' ,{1} ,{2} ,{3} ,{4} ,{5} ,{6} , {7} ,{8} ,{9} ,'{10}' ,{11} ,{12} , sysdate , '{13}' , {14} )";
            strCommand = string.Format(strCommand,
                pModel.RC_NAME.Trim().Replace("'", "''"), pModel.FLOW, pModel.DIAMETER, pModel.LENGTH, pModel.EFFICIENCY,
                pModel.ROUGHNESS, pModel.LOAD, pModel.VELOCITY, pModel.OUTSIDE_DIAMETER, pModel.WALL_THICKNESS, pModel.SERVICE_STATE.Replace("'", "''"),
                pModel.MONTH, pModel.YEAR, pModel.UPLOAD_BY.Trim().Replace("'", "''"), pModel.REGION.Trim().Replace("'", "''"));
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void InsertGateImport(Models.DataModel.ModelGateStationImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"INSERT INTO GATESTATION_IMPORT ( GATE_NAME,PRESSURE,FLOW, MONTH, YEAR,UPLOAD_DATE, UPLOAD_BY,REGION) 
                VALUES ( '{0}',{1},{2},{3},{4},SYSDATE,'{5}','{6}' )";

            strCommand = string.Format(strCommand,
                pModel.GATE_NAME.Trim().Replace("'", "''"),
                pModel.PRESSURE,
                pModel.FLOW,
                pModel.MONTH,
                pModel.YEAR,
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION.Trim().Replace("'", "''"));
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void UpdateGateImport(Models.DataModel.ModelGateStationImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"UPDATE GATESTATION_IMPORT SET    	
GATE_NAME   = '{0}',
    PRESSURE    = {1},
    FLOW        = {2},
    UPLOAD_DATE = sysdate,
    UPLOAD_BY   = '{3}',
    REGION      = '{4}'
WHERE  	1=1
	AND GATE_ID     = {5}
	AND MONTH       = {6}
	AND YEAR        = {7}";
            strCommand = string.Format(strCommand,
                pModel.GATE_NAME.Trim().Replace("'", "''"),
                pModel.PRESSURE, pModel.FLOW,
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION.Trim().Replace("'", "''"), pModel.GATE_ID,
                pModel.MONTH,
                pModel.YEAR);
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void UpdatePipelineImport(Models.DataModel.ModelPipelineImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"UPDATE PIPELINE_IMPORT SET
       RC_NAME          = '{0}',
       FLOW             = {1},
       DIAMETER         = {2},
       LENGTH           = {3},
       EFFICIENCY       = {4},
       ROUGHNESS        = {5},
       LOAD             = {6},
       VELOCITY         = {7},
       OUTSIDE_DIAMETER = {8},
       WALL_THICKNESS   = {9},
       SERVICE_STATE    = '{10}',       
       UPLOAD_DATE      = sysdate,
       UPLOAD_BY        = '{11}',
       REGION           = '{12}'
WHERE 1=1 
	AND PIPELINE_ID	= {13}
	AND MONTH	= {14}
	AND YEAR	= {15}";
            strCommand = string.Format(strCommand, 
                pModel.RC_NAME.Trim().Replace("'","''") , 
                pModel.FLOW ,
                pModel.DIAMETER ,
                pModel.LENGTH ,
                pModel.EFFICIENCY ,
                pModel.ROUGHNESS ,
                pModel.LOAD,
                pModel.VELOCITY,
                pModel.OUTSIDE_DIAMETER,
                pModel.WALL_THICKNESS,
                pModel.SERVICE_STATE.Trim().Replace("'","''"),
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION.Trim().Replace("'", "''"),
                pModel.PIPELINE_ID ,
                pModel.MONTH ,
                pModel.YEAR
            );
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void UpdateGateArchive(Models.DataModel.ModelGateStationArchive pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"UPDATE GATESTATION_ARCHIVE SET    
       	GATE_NAME   = '{0}',
       	FLOW        = {1}
       	UPLOAD_DATE = sysdate,
       	UPLOAD_BY   = '{2}',
       	REGION      = '{3}',
       	FLAG_ID     = {4}
WHERE 1=1 
	AND GATE_ID     = {5}
	AND MONTH       = {6}
	AND YEAR        = {7}";
            strCommand = string.Format(strCommand,
                pModel.GATE_NAME.Trim().Replace("'", "''"),
                pModel.FLOW, 
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION, pModel.GATE_ID);
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void InsertPipelineArchive(Models.DataModel.ModelPipelineArchive pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"INSERT INTO PTTOUR.PIPELINE_ARCHIVE (RC_NAME, MONTH, YEAR, UPLOAD_DATE, UPLOAD_BY, REGION,FLAG_ID, VELOCITY) 
            VALUES ( '{0}',{1} , {2} , sysdate , '{3}' , {4} , {5} , {6})";

            strCommand = string.Format(strCommand,
                pModel.RC_NAME.Trim().Replace("'", "''"),
                pModel.MONTH,
                pModel.YEAR,
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION.Trim().Replace("'", "''"),
                pModel.FLAG_ID,
                pModel.VELOCITY);
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;

        }


        public void UpdatePipelineArchive(Models.DataModel.ModelPipelineArchive pModel)
        {
            if (pModel == null) return;
            string strCommand = @"UPDATE PIPELINE_ARCHIVE SET    
       RC_NAME     = '{0}',       
       UPLOAD_DATE = sysdate,
       UPLOAD_BY   = '{1}',
       REGION      = '{2}',
       FLAG_ID     = {3},
       VELOCITY    = {4}
WHERE  1=1
	AND PIPELINE_ID = {5}	
	AND    MONTH       = {6}
	AND    YEAR        = {7}";

            strCommand = string.Format(strCommand,
                pModel.RC_NAME.Trim().Replace("'", "''"), 
                 
                pModel.UPLOAD_BY.Trim().Replace("'" , "''") , 
                pModel.REGION.Trim().Replace("'" , "''"), 
                pModel.FLAG_ID , pModel.VELOCITY , pModel.PIPELINE_ID , pModel.MONTH, pModel.YEAR );
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }
        public void InsertGateArchive(Models.DataModel.ModelGateStationArchive pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"INSERT INTO PTTOUR.GATESTATION_ARCHIVE (GATE_NAME, FLOW,MONTH,YEAR, UPLOAD_DATE,UPLOAD_BY, REGION, FLAG_ID) 
                VALUES ('{0}',{1},{2},{3},sysdate,'{4}','{5}',{6} )";

            strCommand = string.Format(strCommand,
                pModel.GATE_NAME.Trim().Replace("'", "''"),
                pModel.FLOW,
                pModel.MONTH,
                pModel.YEAR,
                pModel.UPLOAD_BY.Trim().Replace("'", "''"),
                pModel.REGION.Trim().Replace("'", "''"),
                pModel.FLAG_ID);
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;

        }

        public bool IsGateStationImportDuplicate(Models.DataModel.ModelGateStationImport pModel)
        {
            if (pModel == null)
            {
                return false;
            }
            string strCommand = @"select case when exists(
                select 1 from GateStation_Import where gate_Name = '{0}' and month ={1} and year = {2}
            ) then 1 else 0 end xx from dual";
            strCommand = string.Format(strCommand, pModel.GATE_NAME.Replace("'", "''"), pModel.MONTH, pModel.YEAR);
            var dal = new DAL.DAL();
            var objExecute = dal.ExecuteScalar(strCommand);
            return 1.Equals(objExecute);
        }


        public IEnumerable<Models.DataModel.ModelGateStationImport> GetListGateImportDuplicate(IEnumerable<Models.DataModel.ModelGateStationImport> pListModel)
        {
            if (pListModel == null && !pListModel.Any())
            {
                return null;
            }
            string strAllGateName =
                pListModel.Select(x => "'" + x.GATE_NAME.Replace("'", "''") + "'")
                .Aggregate((x, y) => x + "," + y);
            if (string.IsNullOrEmpty(strAllGateName))
            {
                return null;
            }
            string strCommand = @"select * from gateStation_Import
            where year = {0} and month = {1}
            and Gate_Name in ({2})";
            var model1 = pListModel.First();
            strCommand = string.Format(strCommand, model1.YEAR, model1.MONTH, strAllGateName);
            var dal = new DAL.DAL();
            var result = dal.ReadData<Models.DataModel.ModelGateStationImport>(
                pStrCommand: strCommand,
                pFuncReadData: x => new Models.DataModel.ModelGateStationImport(x));
            dal = null;
            return result;
        }

        public IEnumerable<Models.DataModel.ModelPipelineImport> GetListPipeline()
        {
            string strCommand = @"select * from PIPELINE_IMPORT";
            var dal = new DAL.DAL();
            var result = dal.ReadData<Models.DataModel.ModelPipelineImport>(
                pStrCommand: strCommand,
                pFuncReadData: r => new Models.DataModel.ModelPipelineImport(r));
            dal = null;
            return result;
        }


        public IEnumerable<Models.DataModel.ModelPipelineImport> GetListPipelineImportDuplicate(IEnumerable<Models.DataModel.ModelPipelineImport> pListModel)
        {
            if (pListModel == null && !pListModel.Any(x=> x != null))
            {
                return null;
            }
            string strAllRcName = pListModel
                .Where(x => x != null && !string.IsNullOrEmpty( x.RC_NAME))
                .Select(x => "'" + x.RC_NAME.Replace("'", "''") + "'")
                .Aggregate((x, y) => x + "," + y);

            if(string.IsNullOrEmpty(strAllRcName))
            {
                return null;
            }
            string strCommand = @"select * from PIPELINE_IMPORT
            where year = {0} and month = {1}
            and RC_NAME in ({2})";
            var model1 = pListModel.Where(x=> x!= null).First();
            strCommand = string.Format(strCommand, model1.YEAR, model1.MONTH, strAllRcName);
            var dal = new DAL.DAL();
            var result = dal.ReadData<Models.DataModel.ModelPipelineImport>(
                pStrCommand: strCommand, 
                pFuncReadData: r => new Models.DataModel.ModelPipelineImport(r));
            dal = null;
            return result;
        }

        public IEnumerable<Models.DataModel.ModelPipelineArchive> GetListPipelineArchiveDuplicate(IEnumerable<Models.DataModel.ModelPipelineArchive> pListModel)
        {
            if (pListModel == null && !pListModel.Any())
            {
                return null;
            }
            string strAllRcName = pListModel.Select(x => "'" + x.RC_NAME.Replace("'", "''") + "'")
                .Aggregate((x, y) => x + "," + y); ;
            if (string.IsNullOrEmpty(strAllRcName))
            {
                return null;
            }
            string strCommand = @"select * from PIPELINE_ARCHIVE
            where year = {0} and month = {1}
            and RC_NAME in ({2})";
            var model1 = pListModel.First();

            strCommand = string.Format(strCommand, model1.YEAR, model1.MONTH, strAllRcName);
            var dal = new DAL.DAL();
            var result = dal.ReadData<Models.DataModel.ModelPipelineArchive>(
                pStrCommand: strCommand,
                pFuncReadData: x => new Models.DataModel.ModelPipelineArchive(x)
            );

            dal = null;
            return result;
            
        }

        public IEnumerable<Models.DataModel.ModelGateStationArchive> GetListGateArchiveDuplicate(IEnumerable<Models.DataModel.ModelGateStationImport> pListModel)
        {
            if (pListModel == null && !pListModel.Any())
            {
                return null;
            }
            string strAllGateName =
                pListModel.Select(x => "'" + x.GATE_NAME.Replace("'", "''") + "'")
                .Aggregate((x, y) => x + "," + y);
            if (string.IsNullOrEmpty(strAllGateName))
            {
                return null;
            }
            string strCommand = @"select * from gateStation_Archive
            where year = {0} and month = {1}
            and Gate_Name in ({2})";
            var model1 = pListModel.First();
            strCommand = string.Format(strCommand, model1.YEAR, model1.MONTH, strAllGateName);
            var dal = new DAL.DAL();
            var result = dal.ReadData<Models.DataModel.ModelGateStationArchive>(
                pStrCommand: strCommand,
                pFuncReadData: x =>
                {
                    var gi = new Models.DataModel.ModelGateStationImport(x);
                    return new Models.DataModel.ModelGateStationArchive(gi);
                });
            dal = null;
            return result;
        }

        public int GetGateImportID(Models.DataModel.ModelGateStationImport pModel)
        {
            if (pModel == null)
            {
                return 0;
            }
            string strSql = @"select Gate_Id from gateStation_Import
            where rownum = 1 and Gate_Name = '{0}' and Month={1} and year = {2} ";
            string.Format(strSql, pModel.GATE_NAME, pModel.MONTH, pModel.YEAR);
            var dal = new DAL.DAL();
            var objExecute = dal.ExecuteScalar(strSql);
            return 0;
        }

        public bool IsPipelineImportDuplicate(Models.DataModel.ModelPipelineImport pModel)
        {
            if (pModel == null)
            {
                return false;
            }
            string strCommand = @"select case when exists(
                select 1 from pipeline_Import where rc_Name = '{0}' and month ={1} and year = {2}
            ) then 1 else 0 end xx from dual";
            strCommand = string.Format(strCommand, pModel.RC_NAME.Replace("'", "''"), pModel.MONTH, pModel.YEAR);
            var dal = new DAL.DAL();
            var objExecute = dal.ExecuteScalar(strCommand);
            return 1.Equals(objExecute);
        }

        public IEnumerable<Models.ViewModel.ModelThresholdItem> GetListThreshold()
        {
            string strQuery = @"select 
                PIPELINE_THRESHOLD_ID THRESHOLD_ID , 
                'PipeLine' ThresholdType ,
                COLOR , 
                MINVAL , 
                MAXVAL ,
                UPDATED_BY ,
                COLOR_HEX
            from PIPELINE_THRESHOLD
            union all
            select 
                GATE_THRESHOLD_ID THRESHOLD_ID , 
                'GateStation' ThresholdType ,
                COLOR , 
                MINVAL , 
                MAXVAL ,
                UPDATED_BY ,
                COLOR_HEX
            from GATESTATION_THRESHOLD";
            //order by threshold_id desc , thresholdtype desc";
            var dal = new DAL.DAL();
            var listThreshold = dal.ReadData<Models.ViewModel.ModelThresholdItem>(strQuery, x => new Models.ViewModel.ModelThresholdItem(x));
            return listThreshold;
        }

        public void UpdateThreshold(Models.ViewModel.ModelThresholdItem pModel)
        {
            if(pModel == null)
            {
                return;
            }
            string strCommand = string.Empty;            
            if (pModel.ThresholdType == Models.ViewModel.EnumThresholdType.GateStation)
            {                
                strCommand = @"UPDATE GATESTATION_THRESHOLD
SET 
       MINVAL                = {0},
       MAXVAL                = {1},
       UPDATED_DATE          = sysdate,
       UPDATED_BY            = '{2}'
WHERE  GATE_THRESHOLD_ID = {3}";
            }
            else if( pModel.ThresholdType == Models.ViewModel.EnumThresholdType.PipeLine )
            {
                strCommand = @"UPDATE PIPELINE_THRESHOLD
SET
       MINVAL                = {0},
       MAXVAL                = {1},
       UPDATED_DATE          = sysdate,
       UPDATED_BY            = '{2}'
WHERE  PIPELINE_THRESHOLD_ID = {3}";
            }
            if (string.IsNullOrEmpty(strCommand))
            {
                return;
            }
            strCommand = string.Format(strCommand,                 
                pModel.MinValue, 
                pModel.MaxValue, 
                pModel.UPDATED_BY.Trim().Replace("'","''"), 
                pModel.ThresholdId);

            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }
    }
}