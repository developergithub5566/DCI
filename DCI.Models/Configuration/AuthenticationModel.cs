using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.Configuration
{
    public class AuthenticationModel
    {
        public string Google_ClientId { get; set; }
        public string Google_ClientSecret { get; set; }
        public string Jwt_Issuer { get; set; }
        public string Jwt_Audience { get; set; }
        public string Jwt_Key{ get; set; }
    
    }   
}
