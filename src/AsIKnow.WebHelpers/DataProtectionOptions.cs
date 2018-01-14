using System;
using System.Collections.Generic;
using System.Text;

namespace AsIKnow.WebHelpers
{
    public class DataProtectionOptions
    {
        public string ApplicationName { get; set; } = "application";
        public string KeyRingPath { get; set; } = "./dataprotection/keys";
        public bool DisableAutomaticKeyGeneration { get; set; } = false;
    }
}
