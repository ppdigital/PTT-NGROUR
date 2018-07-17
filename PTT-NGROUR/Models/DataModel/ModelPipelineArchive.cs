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
            FLAG_ID             = pReader.GetColumnValue("FLAG_ID").GetInt();
            PIPELINE_ID         = pReader.GetColumnValue("PIPELINE_ID").GetInt();
            RC_NAME             = pReader.GetColumnValue("RC_NAME").GetString();
            FLOW                = pReader.GetColumnValue("FLOW").GetDecimal();
            DIAMETER            = pReader.GetColumnValue("DIAMETER").GetDecimal();
            LENGTH              = pReader.GetColumnValue("LENGTH").GetDecimal();
            EFFICIENCY          = pReader.GetColumnValue("EFFICIENCY").GetDecimal();
            ROUGHNESS           = pReader.GetColumnValue("ROUGHNESS").GetDecimal();
            LOAD                = pReader.GetColumnValue("LOAD").GetDecimal();
            VELOCITY            = pReader.GetColumnValue("VELOCITY").GetDecimal();
            OUTSIDE_DIAMETER    = pReader.GetColumnValue("OUTSIDE_DIAMETER").GetDecimal();
            WALL_THICKNESS      = pReader.GetColumnValue("WALL_THICKNESS").GetDecimal();
            SERVICE_STATE       = pReader.GetColumnValue("SERVICE_STATE").GetString();
            MONTH               = pReader.GetColumnValue("MONTH").GetInt();
            YEAR                = pReader.GetColumnValue("YEAR").GetInt();
            UPLOAD_DATE         = pReader.GetColumnValue("UPLOAD_DATE").GetDate();
            UPLOAD_BY           = pReader.GetColumnValue("UPLOAD_BY").GetString();
            REGION              = pReader.GetColumnValue("REGION").GetString();

        }

        public ModelPipelineArchive(ModelPipelineImport pPipelineImport)
        {
            if (pPipelineImport == null)
            {
                return;
            }            
            PIPELINE_ID = pPipelineImport.PIPELINE_ID;
            RC_NAME = pPipelineImport.RC_NAME;
            FLOW = pPipelineImport.FLOW;
            DIAMETER = pPipelineImport.DIAMETER;
            LENGTH = pPipelineImport.LENGTH;
            EFFICIENCY = pPipelineImport.EFFICIENCY;
            ROUGHNESS = pPipelineImport.ROUGHNESS;
            LOAD = pPipelineImport.LOAD;
            VELOCITY = pPipelineImport.VELOCITY;
            OUTSIDE_DIAMETER = pPipelineImport.OUTSIDE_DIAMETER;
            WALL_THICKNESS = pPipelineImport.WALL_THICKNESS;
            SERVICE_STATE = pPipelineImport.SERVICE_STATE;
            MONTH = pPipelineImport.MONTH;
            YEAR = pPipelineImport.YEAR;
            UPLOAD_DATE = pPipelineImport.UPLOAD_DATE;
            UPLOAD_BY = pPipelineImport.UPLOAD_BY;
            REGION = pPipelineImport.REGION;
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


        public int FLAG_ID { get; set; }
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