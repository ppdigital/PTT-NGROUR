using System.Collections.Generic;

public class AMSNotificationPack
{
    public List<string> to { get; set; }
    public AMSNotificationConfig notification { get; set; }
    public Dictionary<string, object> data { get; set; }
    public string priority { get; set; }
    public bool content_available { get; set; }

    public AMSNotificationPack()
    {
        to = new List<string>();
        notification = new AMSNotificationConfig();
        priority = "high";
        content_available = true;
    }
}