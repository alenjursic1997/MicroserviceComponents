using dotnet_etcd;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ConfigCoreTests.MockingObjects
{
    public class DummyEtcdClient : EtcdClient
    {
        public DummyEtcdClient(string connectionString, int port = 2379, string username = "", string password = "", string caCert = "", string clientCert = "", string clientKey = "", bool publicRootCa = false) 
            : base(connectionString, port, username, password, caCert, clientCert, clientKey, publicRootCa)
        {
        }
    }
}
