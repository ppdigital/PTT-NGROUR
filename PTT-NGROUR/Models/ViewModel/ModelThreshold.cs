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
        None, GateStation, PipeLine
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
            UPDATED_BY = pReader["UPDATED_BY"].GetString();
            COLOR_HEX = pReader["COLOR_HEX"].GetString();
        }

        private string _updateBy = string.Empty;
        private string _colorHex = string.Empty;
        private string _color = string.Empty;


        public int ThresholdId { get; set; }
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (value == null)
                {
                    _color = string.Empty;
                }
                else if (value.Length > 20)
                {
                    _color = value.Substring(0, 20);
                }
                else
                {
                    _color = value;
                }
            }
        }
        public EnumThresholdType ThresholdType { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string UPDATED_BY
        {
            get
            {
                return _updateBy;
            }
            set
            {
                if (value == null)
                {
                    _updateBy = string.Empty;
                }
                else if (value.Length > 20)
                {
                    _updateBy = value.Substring(0, 20);
                }
                else
                {
                    _updateBy = value;
                }
            }
        }
        public string COLOR_HEX
        {
            get
            {
                return _colorHex;
            }
            set
            {
                if (value == null)
                {
                    _colorHex = string.Empty;
                }
                else if (value.Length > 7)
                {
                    _colorHex = value.Substring(0, 7);
                }
                else
                {
                    _colorHex = value;
                }
            }
        }
    }
}