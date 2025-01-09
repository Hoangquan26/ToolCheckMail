using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolMail.Data
{
    public class ProxyData
    {
        public List<string> ApiKeys = [];
        public string ProxyName = "";
        public static int ThreadPerProxy = 10;
        public List<string> ProxyPort = [];
        public static int proxyIndex = 0;
        public static int proxyUsage = 0;
        public List<string> getAllPort()
        {
            return this.ProxyPort;
        }

        public List<string> getAllKeys()
        {
            return this.ApiKeys;
        }
        
        public string getCurrentPort()
        {
            int portIndex = 0;
            if(ProxyData.proxyUsage + 1 >= ProxyData.ThreadPerProxy)
            {
                ProxyData.proxyIndex++;
                ProxyData.proxyUsage = 0;
            }
            portIndex = ProxyData.proxyIndex % ProxyData.ThreadPerProxy;
            ++ProxyData.proxyUsage;
            return this.ProxyPort[portIndex] ?? this.ProxyPort[0];
        }

        public void resetState()
        {
            ProxyData.proxyIndex = 0;
            ProxyData.proxyUsage = 0;
        }
    }
}
