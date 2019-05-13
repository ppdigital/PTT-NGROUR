using PTT_NGROUR.ExtentionAndLib;
using System.Data;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGetRisk
    {
        public ModelGetRisk()
        {

        }

        public ModelGetRisk(IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            REGION = pReader.GetColumnValue("REGION").GetInt();
            RC_NAME = pReader.GetColumnValue("RC_NAME").GetString();
            LICENSE_NO = pReader.GetColumnValue("LICENSE_NO").GetInt();
            RISK_SCORE = pReader.GetColumnValue("RISK_SCORE").GetDecimal();
            RISK_SCORE_COLOR = pReader.GetColumnValue("RISK_SCORE_COLOR").GetString();
            RISK_SCORE_RISK = pReader.GetColumnValue("RISK_SCORE_RISK").GetString();
            INTERNAL_CORROSION = pReader.GetColumnValue("INTERNAL_CORROSION").GetDecimal();
            INTERNAL_CORROSION_COLOR = pReader.GetColumnValue("INTERNAL_CORROSION_COLOR").GetString();
            INTERNAL_CORROSION_RISK = pReader.GetColumnValue("INTERNAL_CORROSION_RISK").GetString();
            EXTERNAL_CORROSION = pReader.GetColumnValue("EXTERNAL_CORROSION").GetDecimal();
            EXTERNAL_CORROSION_COLOR = pReader.GetColumnValue("EXTERNAL_CORROSION_COLOR").GetString();
            EXTERNAL_CORROSION_RISK = pReader.GetColumnValue("EXTERNAL_CORROSION_RISK").GetString();
            THIRD_PARTY_INTERFERENCE = pReader.GetColumnValue("THIRD_PARTY_INTERFERENCE").GetDecimal();
            THIRD_PARTY_INTERFERENCE_COLOR = pReader.GetColumnValue("THIRD_PARTY_INTERFERENCE_COLOR").GetString();
            THIRD_PARTY_INTERFERENCE_RISK = pReader.GetColumnValue("THIRD_PARTY_INTERFERENCE_RISK").GetString();
            LOSS_OF_GROUND_SUPPORT = pReader.GetColumnValue("LOSS_OF_GROUND_SUPPORT").GetDecimal();
            LOSS_OF_GROUND_SUPPORT_COLOR = pReader.GetColumnValue("LOSS_OF_GROUND_SUPPORT_COLOR").GetString();
            LOSS_OF_GROUND_SUPPORT_RISK = pReader.GetColumnValue("LOSS_OF_GROUND_SUPPORT_RISK").GetString();
            YEAR = pReader.GetColumnValue("YEAR").GetInt();
        }

        public int REGION { get; set; }
        public string RC_NAME { get; set; }
        public int LICENSE_NO { get; set; }
        public decimal RISK_SCORE { get; set; }
        public string RISK_SCORE_COLOR { get; set; }
        public string RISK_SCORE_RISK { get; set; }
        public decimal INTERNAL_CORROSION { get; set; }
        public string INTERNAL_CORROSION_COLOR { get; set; }
        public string INTERNAL_CORROSION_RISK { get; set; }
        public decimal EXTERNAL_CORROSION { get; set; }
        public string EXTERNAL_CORROSION_COLOR { get; set; }
        public string EXTERNAL_CORROSION_RISK { get; set; }
        public decimal THIRD_PARTY_INTERFERENCE { get; set; }
        public string THIRD_PARTY_INTERFERENCE_COLOR { get; set; }
        public string THIRD_PARTY_INTERFERENCE_RISK { get; set; }
        public decimal LOSS_OF_GROUND_SUPPORT { get; set; }
        public string LOSS_OF_GROUND_SUPPORT_COLOR { get; set; }
        public string LOSS_OF_GROUND_SUPPORT_RISK { get; set; }
        public int YEAR { get; set; }
        public bool HAS_FILE { get; set; }
        public string[] FILES { get; set; }
    }
}