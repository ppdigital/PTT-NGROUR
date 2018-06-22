using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AMSNotificationConfig
{
    public string title { get; set; }
    public string body { get; set; }
    public string icon { get; set; }
    public string badge { get; set; }
    public string sound { get; set; }

    public AMSNotificationConfig()
    {
        badge = "1";
        sound = "default";
    }
}