using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelThreshold
    {
        public ModelThresholdItem[] ThresholdItems { get; set; }
    }

    public enum EnumThresholdType
    {
        None , GateStation, PipeLine
    }

    public class ModelThresholdItem
    {

        public ModelThresholdItem()
        {

        }

        public ModelThresholdItem(System.Data.IDataReader pReader)
        {
            Color = pReader["COLOR"].GetString();
            MaxValue = pReader["MAXVAL"].GetDecimal();
            MinValue = pReader["MINVAL"].GetDecimal();
            ThresholdId = pReader["THRESHOLD_ID"].GetInt();
            ThresholdType = pReader["ThresholdType"].GetEnum<Models.ViewModel.EnumThresholdType>(Models.ViewModel.EnumThresholdType.None);
        }

        public int ThresholdId { get; set; }
        public string Color { get; set; }
        public EnumThresholdType ThresholdType { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }
}