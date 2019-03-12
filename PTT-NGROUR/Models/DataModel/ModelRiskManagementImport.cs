using PTT_NGROUR.ExtentionAndLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelRiskManagementImport
    {

        public ModelRiskManagementImport()
        {

        }

        public ModelRiskManagementImport(System.Data.IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            this.REGION = pReader["REGION"].GetInt();
            this.RC = pReader["RC"].GetString();
            this.RISK_SCORE = pReader["RISK_SCORE"].GetDecimal();
            this.INTERNAL_CORROSION = pReader["INTERNAL_CORROSION"].GetDecimal();
            this.EXTERNAL_CORROSION = pReader["EXTERNAL_CORROSION"].GetDecimal();
            this.THIRD_PARTY_INTERFERENCE = pReader["THIRD_PARTY_INTERFERENCE"].GetDecimal();
            this.LOSS_OF_GROUND_SUPPORT = pReader["LOSS_OF_GROUND_SUPPORT"].GetDecimal();
            this.MONTH = pReader["MONTH"].GetInt();
            this.YEAR = pReader["YEAR"].GetInt();
        }

        public ModelRiskManagementImport Clone()
        {
            var result = new ModelRiskManagementImport()
            {
                REGION = this.REGION,
                RC = this.RC,
                RISK_SCORE = this.RISK_SCORE,
                INTERNAL_CORROSION = this.INTERNAL_CORROSION,
                EXTERNAL_CORROSION = this.EXTERNAL_CORROSION,
                THIRD_PARTY_INTERFERENCE = this.THIRD_PARTY_INTERFERENCE,
                LOSS_OF_GROUND_SUPPORT = this.LOSS_OF_GROUND_SUPPORT,
                MONTH = this.MONTH,
                YEAR = this.YEAR
            };
            return result;
        }

        public int REGION { get; set; }

        public string RC { get; set; }

        public decimal RISK_SCORE { get; set; }

        public decimal INTERNAL_CORROSION { get; set; }

        public decimal EXTERNAL_CORROSION { get; set; }

        public decimal THIRD_PARTY_INTERFERENCE { get; set; }

        public decimal LOSS_OF_GROUND_SUPPORT { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }
    }
}