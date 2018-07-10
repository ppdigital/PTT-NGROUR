using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelUtilizationReportPdfOutput
    {
        public class ThresholdStatus
        {
            public int OK { get; set; }
            public int Warning { get; set; }
            public int Alert { get; set; }
            public int Flag { get; set; }
            public void ResetValue()
            {
                this.OK = _intZero;
                this.Alert = _intZero;
                this.Flag = _intZero;
                this.Warning = _intZero;
            }
        }

        private const string _strOK = "OK";
        private const string _strWarning = "Warning";
        private const string _strAlert = "Alert";
        private const string _strFlag = "Flag";
        private const string _strPipeline = "PIPELINE";
        private const string _strGateStation = "GATESTATION";
        private const int _intZero = 0;


        public ModelUtilizationReportPdfOutput()
        {
            this.CurrentGate = new ThresholdStatus();
            this.CurrentPipeline = new ThresholdStatus();
            this.Gate = new ThresholdStatus();
            this.Pipeline = new ThresholdStatus();
        }

        public void SetListGatePipeCurrent(IEnumerable<DataModel.ModelViewGatePipeReport> pListData)
        {
            this.CurrentGate.ResetValue();
            this.CurrentPipeline.ResetValue();
            if(pListData == null || !pListData.Any())
            {
                return;
            }
            foreach(var itemData in pListData)
            {
                if (itemData == null) continue;
                switch (itemData.TYPE)
                {
                    case _strPipeline:
                        switch (itemData.THRESHOLD){
                            case _strOK:
                                this.CurrentPipeline.OK++;
                                break;
                            case _strWarning:
                                this.CurrentPipeline.Warning++;
                                break;
                            case _strAlert:
                                this.CurrentPipeline.Alert++;
                                break;
                            case _strFlag:
                                this.CurrentPipeline.Flag++;
                                break;
                        }
                        break;
                    case _strGateStation:
                        switch (itemData.THRESHOLD)
                        {
                            case _strOK:
                                this.CurrentGate.OK++;
                                break;
                            case _strWarning:
                                this.CurrentGate.Warning++;
                                break;
                            case _strAlert:
                                this.CurrentGate.Alert++;
                                break;
                            case _strFlag:
                                this.CurrentGate.Flag++;
                                break;
                        }
                        break;
                }
            }            
        }
        public void SetListGatePipe(IEnumerable<DataModel.ModelViewGatePipeReport> pListData)
        {
            this.Gate.ResetValue();
            this.Pipeline.ResetValue();
            this.ListSearchData = pListData;
            if (pListData == null || !pListData.Any())
            {
                return;
            }
            foreach (var itemData in pListData)
            {
                if (itemData == null) continue;
                switch (itemData.TYPE)
                {
                    case _strPipeline:
                        switch (itemData.THRESHOLD)
                        {
                            case _strOK:
                                this.Pipeline.OK++;
                                break;
                            case _strWarning:
                                this.Pipeline.Warning++;
                                break;
                            case _strAlert:
                                this.Pipeline.Alert++;
                                break;
                            case _strFlag:
                                this.Pipeline.Flag++;
                                break;
                        }
                        break;
                    case _strGateStation:
                        switch (itemData.THRESHOLD)
                        {
                            case _strOK:
                                this.Gate.OK++;
                                break;
                            case _strWarning:
                                this.Gate.Warning++;
                                break;
                            case _strAlert:
                                this.Gate.Alert++;
                                break;
                            case _strFlag:
                                this.Gate.Flag++;
                                break;
                        }
                        break;
                }
            }
        }
        public ThresholdStatus CurrentGate { get; set; }
        public ThresholdStatus CurrentPipeline { get; set; }
        public ThresholdStatus Gate { get; set; }
        public ThresholdStatus Pipeline { get; set; }
        public IEnumerable<DataModel.ModelViewGatePipeReport> ListSearchData { get; set; }
    }
}