using PTT_NGROUR.ExtentionAndLib;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelAcceptanceCriteria
    {

        public ModelAcceptanceCriteria()
        {
        }

        public ModelAcceptanceCriteria(System.Data.IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            this.RISK_CRITERIA = pReader["RISK_CRITERIA"].GetInt();
            this.UPDATE_DATE = pReader["UPDATE_DATE"].GetDate();
            this.UPDATE_BY = pReader["UPDATE_BY"].GetString();
        }

        [Required]
        [DataType(DataType.PhoneNumber)]
        //[Range(0, 100)]
        [Display(Name = "Acceptance Criteria")]
        public int RISK_CRITERIA { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public string PreviousUrl { get; set; }
    }
}