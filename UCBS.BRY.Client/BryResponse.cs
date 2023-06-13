using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCBS.BRY.Client
{
    public class BryResponse
    {
        public string Status { get; set; } 
        public string Message { get; set; }
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string JsonResponse { get; set; }
        public ReturnType ReturnType { get; set; }  
    }
}
