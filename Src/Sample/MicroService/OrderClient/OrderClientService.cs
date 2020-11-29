using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrderClient
{


    public class OrderClientService : swaggerClient
    {
        public OrderClientService(HttpClient httpClient)
            : base(httpClient.BaseAddress.ToString(), httpClient)
        {

        }
    }
}
