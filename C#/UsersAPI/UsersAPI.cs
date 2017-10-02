using System;
using System.IO;
using System.Net;

namespace UsersAPIExample
{
    class UsersAPI
    {
        static void Main()
        {
            Console.WriteLine("LivePerson Users API Example - LiveEngage");
            UsersAPI test = new UsersAPI();
            test.ListClients();
            Console.ReadKey();
        }
		/*
         A web call is needed to discover the baseURI needed to call the Users API for individual LiveEngage accounts for Accounts:
         https://api.liveperson.net/api/account/{YOUR ACCOUNT NUMBER}/service/accountConfigReadOnly/baseURI.json?version=1.0
        {
         "service":"accountConfigReadOnly",
         "account":"5477507",
         "baseURI":"va-a.acr.liveperson.net"
        }
        */
		WebProxy myProxy;
        string appUrl = "https://{YOUR BASE URI}/api/account/{YOUR ACCOUNT NUMBER}/configuration/le-users/users?v=1";

        public UsersAPI()
        {
            if (WebRequest.DefaultWebProxy.GetProxy(new Uri(appUrl)).ToString() != appUrl)
            {
                IWebProxy iProxy = WebRequest.DefaultWebProxy;
                myProxy = new WebProxy(iProxy.GetProxy(new Uri(appUrl)));
                myProxy.UseDefaultCredentials = true;
            }
        }

        public bool ListClients()
        {
            try
            {
                var oauth = new UsersAPIExample.Manager();
				oauth["consumer_key"] = "Your App Key From LiveEngage";
				oauth["consumer_secret"] = "Your Secret From LiveEngage";
				oauth["token"] = "Your Access Token From LiveEngage";
				oauth["token_secret"] = "Your Access Token Secret From LiveEngage";
                
                var authzHeader = oauth.GenerateAuthzHeader(appUrl, "GET");
                var request = (HttpWebRequest)WebRequest.Create(appUrl);
                request.Method = "GET";
                request.Headers.Add("Authorization", authzHeader);

                request.ContentType = "application/json";

                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader streamReader = new StreamReader(response.GetResponseStream(), true);
                    string target = streamReader.ReadToEnd();
                    streamReader.Close();
                    Console.WriteLine(target);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
