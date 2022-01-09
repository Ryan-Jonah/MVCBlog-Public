using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Enums
{
    public enum ModerationType
    {
        [Description("Political Propoganda")]
        Political,
        [Description("Offensive Language")]
        Language,
        [Description("Drug References")]
        Drugs,
        [Description("Theatening Speech")]
        Theatening,
        [Description("Sexual Content")]
        Sexual,
        [Description("Religous Discussion")]
        Religous,
        [Description("Hate Speech")]
        HateSpeech,
        [Description("Targeting Shaming")]
        Shaming,
        [Description("Other Type of Moderation")]
        Other
    }
}
