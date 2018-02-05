using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsIKnow.WebHelpers
{
    public class WebApplicationOptions
    {
        public PathString PathBase { get; set; } = null;
        public DataProtectionOptions DataProtection { get; set; } = new DataProtectionOptions();
    }
}
