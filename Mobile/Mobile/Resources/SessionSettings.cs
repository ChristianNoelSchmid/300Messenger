using System;
using System.Collections.Generic;
using System.Text;

namespace Mobile.Resources
{
    public class SessionSettings
    {
        public SessionSettings(string jwt)
        {
            Jwt = jwt;
        }
        public string Jwt { get; private set; }
    }
}
