using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models.DataModel;
using System.Collections.Generic;
using System.Linq;

namespace PTT_NGROUR.DTO
{
    public class DtoRisk
    {
        private enum enumExcelRiskManagementColumn
        {
            REGION_ID = 2,
            RC = 3,
            RISK_SCORE = 7,
            INTERNAL_CORROSION = 9,
            EXTERNAL_CORROSION = 11,
            THIRD_PARTY_INTERFERENCE = 13,
            LOSS_OF_GROUND_SUPPORT = 15,
        }

        public enum RiskType
        {
            INTERNAL_CORROSION = 1,
            EXTERNAL_CORROSION = 2,
            THIRD_PARTY_INTERFERENCE = 3,
            LOSS_OF_GROUND_SUPPORT = 4,
        }

        public int Worksheet = 5;
        public int StartRow = 3;

        public IEnumerable<ModelRiskManagementImport> ReadExcelRiskManagementImport(
          System.IO.Stream pStreamExcel,
          int pIntMonth,
          int pIntYear,
          string pStrUploadBy)
        {
            if (pStreamExcel == null || pStreamExcel.Length <= 0)
            {
                yield break;
            }
            using (var exel = new OfficeOpenXml.ExcelPackage(pStreamExcel))
            {
                var exWorkSheet = exel.Workbook.Worksheets[Worksheet];

                int intColCount = exWorkSheet.Dimension.End.Column;
                int intRowCount = exWorkSheet.Dimension.End.Row;

                int intEndRow = intRowCount - 6;
                for (int intRow = StartRow; intRow < intEndRow; ++intRow)
                {
                    var pipImport = new ModelRiskManagementImport()
                    {
                        REGION = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.REGION_ID).GetInt(),
                        RC = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.RC).GetString(),
                        RISK_SCORE = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.RISK_SCORE).GetDecimal(),
                        INTERNAL_CORROSION = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.INTERNAL_CORROSION).GetDecimal(),
                        EXTERNAL_CORROSION = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.EXTERNAL_CORROSION).GetDecimal(),
                        THIRD_PARTY_INTERFERENCE = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.THIRD_PARTY_INTERFERENCE).GetDecimal(),
                        LOSS_OF_GROUND_SUPPORT = getExcelRiskManagementValue(exWorkSheet, intRow, enumExcelRiskManagementColumn.LOSS_OF_GROUND_SUPPORT).GetDecimal(),
                        MONTH = pIntMonth,
                        YEAR = pIntYear
                    };
                    yield return pipImport;
                }
            };
        }

        private object getExcelRiskManagementValue(OfficeOpenXml.ExcelWorksheet pWorkSheet, int intRow, enumExcelRiskManagementColumn pCol)
        {
            if (pWorkSheet == null)
            {
                return null;
            }
            return pWorkSheet.GetValue(intRow, (int)pCol);
        }
        public IEnumerable<Models.DataModel.ModelRiskManagementImport> GetListRiskManagementImportDuplicate(IEnumerable<Models.DataModel.ModelRiskManagementImport> pListModel)
        {
            if (pListModel == null && !pListModel.Any(x => x != null))
            {
                return null;
            }

            string strAllRc = pListModel
                .Where(x => x != null && !string.IsNullOrEmpty(x.RC))
                .Select(x => "'" + x.RC.Replace("'", "''") + "'")
                .Aggregate((x, y) => x + "," + y);

            if (string.IsNullOrEmpty(strAllRc))
            {
                return null;
            }

            string strCommand = @"SELECT * FROM RISK_IMPORT
            WHERE YEAR = {0}
            AND MONTH = {1}
            AND RC IN ({2})";
            var model1 = pListModel.Where(x => x != null).First();
            strCommand = string.Format(strCommand, model1.YEAR, model1.MONTH, strAllRc);
            var dal = new DAL.DAL();
            var result = dal.ReadData(
                pStrCommand: strCommand,
                pFuncReadData: r => new ModelRiskManagementImport(r));
            dal = null;
            return result;
        }

        public void InsertRiskManagementImport(ModelRiskManagementImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = @"INSERT INTO RISK_IMPORT
                (
                    REGION,
                    RC,
                    RISK_SCORE,
                    INTERNAL_CORROSION,
                    EXTERNAL_CORROSION,
                    THIRD_PARTY_INTERFERENCE,
                    LOSS_OF_GROUND_SUPPORT,
                    MONTH,
                    YEAR
                ) 
                VALUES ( '{0}' ,{1} ,{2} ,{3} ,{4} ,{5} ,{6} , {7} ,{8} )";
            strCommand = string.Format(
                strCommand,
                pModel.REGION,
                pModel.RC,
                pModel.RISK_SCORE,
                pModel.INTERNAL_CORROSION,
                pModel.EXTERNAL_CORROSION,
                pModel.THIRD_PARTY_INTERFERENCE,
                pModel.LOSS_OF_GROUND_SUPPORT,
                pModel.MONTH,
                pModel.YEAR
            );
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void UpdateRiskManagementImport(ModelRiskManagementImport pModel)
        {
            if (pModel == null)
            {
                return;
            }
            string strCommand = "UPDATE RISK_IMPORT SET " +
                "RISK_SCORE = '{0}'," +
                "INTERNAL_CORROSION = '{1}'," +
                "EXTERNAL_CORROSION = '{2}'," +
                "THIRD_PARTY_INTERFERENCE = '{3}'," +
                "LOSS_OF_GROUND_SUPPORT = '{4}'," +
                "WHERE 1=1 " +
                "AND RC = {5} " +
                "AND MONTH = {6} " +
                "AND YEAR = {7}";
            strCommand = string.Format(
                strCommand,
                pModel.RISK_SCORE,
                pModel.INTERNAL_CORROSION,
                pModel.EXTERNAL_CORROSION,
                pModel.THIRD_PARTY_INTERFERENCE,
                pModel.LOSS_OF_GROUND_SUPPORT,
                pModel.RC,
                pModel.MONTH,
                pModel.YEAR);
            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }

        public void UpdateAcceptanceCriteria(ModelAcceptanceCriteria model)
        {
            if (model == null)
            {
                return;
            }

            string strCommand = "UPDATE RISK_THRESHOLD SET " +
                "RISK_CRITERIA = '{0}'," +
                "UPDATE_DATE = sysdate," +
                "UPDATE_BY = '{1}'";
            strCommand = string.Format(
                strCommand,
                model.RISK_CRITERIA,
                model.UPDATE_BY);

            var dal = new DAL.DAL();
            dal.ExecuteNonQuery(strCommand);
            dal = null;
        }
    }
}