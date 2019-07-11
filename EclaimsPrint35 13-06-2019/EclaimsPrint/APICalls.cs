using RestSharp;
using System.IO;

namespace EclaimsPrint
{
    class APICalls
    {
        
       string urlPoint = Properties.Settings.Default.URLString;
        /// <summary>
        /// Gets RSA Key From API Endpoint
        /// </summary>
        /// <param name="SubNum"></param>
        /// <returns>PublicKey</returns>
        protected internal static string GetKey(string SubNum)
        {
            var client =
                new RestClient(urlPoint + "/public"); /// call API Public To Get PublicKey
                                                                      


            var request = new RestSharp.RestRequest(Method.GET);


          
            request.AddParameter("sub", SubNum);
            //Create response object to hold response strings(not using headers)
            IRestResponse response = client.Execute(request);

            string content = response.Content;



            return content;
        }
        // Send XPS File (will be sealed class) 
        /// <summary>
        /// SendXPSFile(string subnumber):
        /// 
        /// Authenticates users using hashed and encrypted Username and Password inside of the body as a JSON object
        /// 
        /// Sends XPS file via BYTE to API and Awaits Response
        /// 
        /// Sends response to "whatHappened" string
        /// 
        /// </summary>
        /// <param name="SubNumber"></param>
        /// <returns>Response From API (content)</returns>
        protected internal static string SendXPSFile(string SubNumber, byte[] xpsBytes)
        {
            string savedUserName = Properties.Settings.Default.UserName;
            string savedEncryptedPassword = Properties.Settings.Default.Password;
            


            string usernameEncrypted = Crypto.Encrypt(savedUserName);
         
            var client =
                new RestClient( urlPoint + "/XPS"); /// call API XPS To Send XPS File

            //Encrypt(message);

            var request = new RestRequest(Method.POST);
            request.AddQueryParameter("sub", SubNumber);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { user = usernameEncrypted, pass = savedEncryptedPassword });
            request.AddFile("xps", xpsBytes, "application/octet-stream");
            //Create response object
            IRestResponse response = client.Execute(request);
            //iniate response to a string object for storing (the .netframe would not allow for dynamic value) 
            string content = response.Content;


            return content;
        }
        
    }
}
