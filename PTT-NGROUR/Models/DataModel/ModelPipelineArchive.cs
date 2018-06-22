using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineArchive
    {
        public ModelPipelineArchive()
        {

        }

        public ModelPipelineArchive(System.Data.IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            this.PIPELINE_ID = pReader["PIPELINE_ID"].GetInt();
            this.FLAG_ID = pReader["FLAG_ID"].GetInt();
            this.MONTH = pReader["MONTH"].GetInt();
            this.RC_NAME = pReader["RC_NAME"].GetString();
            this.REGION = pReader["REGION"].GetString();
            this.UPLOAD_BY = pReader["UPLOAD_BY"].GetString();
            this.UPLOAD_DATE = pReader["UPLOAD_DATE"].GetDate();
            this.VELOCITY = pReader["VELOCITY"].GetDecimal();
            this.YEAR = pReader["YEAR"].GetInt();
        }

        public ModelPipelineArchive(ModelPipelineImport pPipelineImport)
        {
            if (pPipelineImport == null)
            {
                return;
            }
            this.PIPELINE_ID = pPipelineImport.PIPELINE_ID;           
            this.MONTH = pPipelineImport.MONTH;
            this.RC_NAME = pPipelineImport.RC_NAME;
            this.REGION = pPipelineImport.REGION;
            this.UPLOAD_BY = pPipelineImport.UPLOAD_BY;
            this.UPLOAD_DATE = pPipelineImport.UPLOAD_DATE;
            this.VELOCITY = pPipelineImport.VELOCITY;
            this.YEAR = pPipelineImport.YEAR;
        }

        public ModelPipelineArchive Clone()
        {
            var result = new ModelPipelineArchive()
            {
                PIPELINE_ID=this.PIPELINE_ID ,
                FLAG_ID=this.FLAG_ID,
                MONTH = this.MONTH,
                RC_NAME = this.RC_NAME,
                REGION = this.REGION,
                UPLOAD_BY = this.UPLOAD_BY,
                UPLOAD_DATE = this.UPLOAD_DATE,
                VELOCITY = this.VELOCITY,
                YEAR = this.YEAR
            };
            return result;
        }


        public int PIPELINE_ID { get; set; }
        public string RC_NAME { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public DateTime? UPLOAD_DATE { get; set; }
        public string UPLOAD_BY { get; set; }
        public string REGION { get; set; }
        public int FLAG_ID { get; set; }
        public decimal VELOCITY { get; set; }

    }
}