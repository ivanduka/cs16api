using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace NEB.RDI.Shared.Helpers
{
    public class Cs10HttpRequest : ICs10HttpRequest
    {

        public CookieContainer LoginPost()
        {
            //Create cookie container to store the llcookie.
            var cc = new CookieContainer();

            //Create the HttpWebRequest object to log the user in.  Set the correct hostname and path for your environment in the URI object.
            var loginRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(ConfigurationManager.AppSettings["CS10ServerSearch"] + ConfigurationManager.AppSettings["CS10ServerExecutable"]));

            //Set up the POST data.  User credentials can be adjusted as needed.
            var postData = "func=ll.login";
            postData += "&Username=" + ConfigurationManager.AppSettings["CS10ServerUser"];
            postData += "&Password=" + ConfigurationManager.AppSettings["CS10ServerPassword"];
            var loginPost = Encoding.UTF8.GetBytes(postData);

            //Set up the required request parameters for the login POST.
            loginRequest.ContentLength = loginPost.Length;
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Method = "POST";
            loginRequest.Referer = ConfigurationManager.AppSettings["CS10ServerSearch"];
            loginRequest.CookieContainer = cc;
            using (var stream = loginRequest.GetRequestStream())
            {
                stream.Write(loginPost, 0, loginPost.Length);
            }

            //Send the login POST.  This will deposit the required cookies in the cookie container.
            using (var loginResponse = (HttpWebResponse) loginRequest.GetResponse())
            {
                //Stream search results to string, ready to parse.
                using (Stream dataStream = loginResponse.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            var responseFromServer = reader.ReadToEnd();

                            if (responseFromServer.Contains("[Invalid username/password specified.]") ||
                                responseFromServer.Contains("Content Server - Error"))
                                cc = null;

                            //Close off remaining objects.
                            reader.Close();
                        }
                        dataStream.Close();
                    }
                } loginResponse.Close();
            }
            return cc;
        }

        public string GetData(CookieContainer cc, string postData)
        {
            string responseFromServer = null;
            var searchPost = Encoding.UTF8.GetBytes(postData);
            //Create a new HttpWebRequest object to run the search.  Set the correct hostname and path for your environment in the URI object.
            var searchRequest = (HttpWebRequest)WebRequest.Create(new Uri(ConfigurationManager.AppSettings["CS10ServerSearch"] + ConfigurationManager.AppSettings["CS10ServerExecutable"]));
            //Set up the required request parameters for the search POST.
            searchRequest.ContentLength = searchPost.Length;
            searchRequest.ContentType = "application/x-www-form-urlencoded";
            searchRequest.Method = "POST";
            searchRequest.CookieContainer = cc;
            //Always set the referer to match the FQDN of the server or the request will be rejected.
            searchRequest.Referer = ConfigurationManager.AppSettings["CS10ServerSearch"];
            using (var stream = searchRequest.GetRequestStream())
            {
                stream.Write(searchPost, 0, searchPost.Length);
            }
            using (var searchResponse = (HttpWebResponse)searchRequest.GetResponse())
            {
                //Stream search results to string, ready to parse.
                using (Stream dataStream = searchResponse.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            responseFromServer = reader.ReadToEnd();

                            //Close off remaining objects.
                            reader.Close();
                        }
                        dataStream.Close();
                    }
                }
                searchResponse.Close();
            }
            return responseFromServer;
        }

        public Stream GetFile(int nodeId)
        {
            //Create a new HttpWebRequest object to run the search.  Set the correct hostname and path for your environment in the URI object.
            var searchRequest = (HttpWebRequest)WebRequest.Create(new Uri(ConfigurationManager.AppSettings["CS10ServerDownload"] + ConfigurationManager.AppSettings["CS10ServerExecutable"] + string.Format("?func=ll&objId={0}&objaction=download&viewType=1", nodeId)));
            //Set up the required request parameters for the search POST.
            searchRequest.ContentType = "application/x-www-form-urlencoded";
            searchRequest.Method = "GET";
            //Always set the referer to match the FQDN of the server or the request will be rejected. 
            searchRequest.Referer = ConfigurationManager.AppSettings["CS10ServerDownload"];
            //set the proxy property to null to skip the proxy autodetect step
            searchRequest.Proxy = null;
            var searchResponse = (HttpWebResponse)searchRequest.GetResponse();
             //Stream search results to string, ready to parse.
             return searchResponse.GetResponseStream();  
         } 

        public byte[] GetFileBuffer(int nodeId)
        {
            byte[] buffer = null;
            //Create a new HttpWebRequest object to run the search.  Set the correct hostname and path for your environment in the URI object.
            var searchRequest = (HttpWebRequest)WebRequest.Create(new Uri(ConfigurationManager.AppSettings["CS10ServerDownload"] + ConfigurationManager.AppSettings["CS10ServerExecutable"] + string.Format("?func=ll&objId={0}&objaction=download&viewType=1", nodeId)));
            //Set up the required request parameters for the search POST.
            searchRequest.ContentType = "application/x-www-form-urlencoded";
            searchRequest.Method = "GET";
            //Always set the referer to match the FQDN of the server or the request will be rejected.
            searchRequest.Referer = ConfigurationManager.AppSettings["CS10ServerDownload"];
            //set the proxy property to null to skip the proxy autodetect step
            searchRequest.Proxy = null; 
            using (var searchResponse = (HttpWebResponse) searchRequest.GetResponse())
            {
                //Stream search results to string, ready to parse.
                using (var stream = searchResponse.GetResponseStream())
                {
                    buffer = ReadFully(stream);
                    stream.Close();
                }
                searchResponse.Close();
            }
            return buffer;
        }

        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}