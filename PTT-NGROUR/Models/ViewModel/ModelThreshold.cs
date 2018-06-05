using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public int ThresholdId { get; set; }
        public string Color { get; set; }
        public EnumThresholdType ThresholdType { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
    }
}