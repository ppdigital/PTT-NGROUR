using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineImport
    {

        public ModelPipelineImport()
        {

        }

        public ModelPipelineImport(System.Data.IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            this.DIAMETER = pReader["DIAMETER"].GetDecimal();
            this.EFFICIENCY = pReader["EFFICIENCY"].GetDecimal();
            this.FLOW = pReader["FLOW"].GetDecimal();
            this.LENGTH = pReader["LENGTH"].GetDecimal();
            this.LOAD = pReader["LOAD"].GetDecimal();
            this.MONTH = pReader["MONTH"].GetInt();
            this.OUTSIDE_DIAMETER = pReader["OUTSIDE_DIAMETER"].GetDecimal();
            this.PIPELINE_ID = pReader["PIPELINE_ID"].GetInt();
            this.RC_NAME = pReader["RC_NAME"].GetString();
            this.REGION = pReader["REGION"].GetString();
            this.ROUGHNESS = pReader["ROUGHNESS"].GetDecimal();
            this.SERVICE_STATE = pReader["SERVICE_STATE"].GetString();
            this.UPLOAD_BY = pReader["UPLOAD_BY"].GetString();
            this.UPLOAD_DATE = pReader["UPLOAD_DATE"].GetDate();
            this.VELOCITY = pReader["VELOCITY"].GetDecimal();
            this.WALL_THICKNESS = pReader["WALL_THICKNESS"].GetDecimal();
            this.YEAR = pReader["YEAR"].GetInt();
        }

        public int PIPELINE_ID { get; set; }

        public string RC_NAME { get; set; }

        public decimal FLOW { get; set; }

        public decimal DIAMETER { get; set; }

        public decimal LENGTH { get; set; }

        public decimal EFFICIENCY { get; set; }

        public decimal ROUGHNESS { get; set; }

        public decimal LOAD { get; set; }

        public decimal VELOCITY { get; set; }

        public decimal OUTSIDE_DIAMETER { get; set; }

        public decimal WALL_THICKNESS { get; set; }

        public string SERVICE_STATE { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }

        public DateTime? UPLOAD_DATE { get; set; }

        public string UPLOAD_BY { get; set; }

        public string REGION { get; set; }

        


    }
}