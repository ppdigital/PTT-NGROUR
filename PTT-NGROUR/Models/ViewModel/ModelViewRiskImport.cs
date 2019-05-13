using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelViewRiskImport
    {
        public string RC_NAME { get; set; }
        public int YEAR { get; set; }
        public HttpPostedFileBase[] FILES { get; set; }
    }
}