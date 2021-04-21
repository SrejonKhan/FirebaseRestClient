using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;


namespace FirebaseRestClient
{
    public class RedirectionListener
    {
        private const string localhostPrefix = "http://127.0.0.1";
        private const string stringLocalhostPrefix = "http://+";

        private HttpListener listener = null;

        private Action<string> responseCb;

        private string htmlCode = "<!DOCTYPE html> <html lang=\"en\"><head>" +
        "<meta charset=\"UTF-8\" /> <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />" +
        "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />" +
        "<title>Thank You</title>" +
        //CSS Styles
        "<style>" +
        "body {background-color: whitesmoke;}" +
        //Card Body
        ".card-body {position: absolute; left: 50%; top: 50%; -webkit-transform: translate(-50%, -65%); transform: translate(-50%, -65%);}" +
        //Card Child
        ".card-child {font-family: Verdana, sans-serif; background-color: white; text-align: center; " +
        "padding: 25px 50px; border-radius: 10px; box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2), 0 6px 20px 0 rgba(0, 0, 0, 0.19); }" +
        //Footer
        ".footer {margin-top: 40px; font-size: 12px; color: gray;}" +
        "</style>" +
        "</head>" +
        //Body
        "<body><div class=\"card-body\"> <div class=\"card-child\"> " +
        //Thank You
        "<h1 style=\"color: #ff0062d5\">Thank You</h1>" +
        //Message
        "<p style=\"color: rgb(114, 114, 114)\">Please return to your Application/Editor again and proceed with next part.</p> " +
        //Footer
        "<p class=\"footer\">FirebaseRestClient by <a style=\"color: #ff0062d5\" href=\"https://www.github.com/SrejonKhan\" target=\"_blank\">Srejon Khan</a></p>" +
        "</div> </div> </body> </html>";


        public void Init(int port, Action<string> responseCallback, bool isStringLocalhost = false)
        {
            if (!HttpListener.IsSupported)
            {
                UnityEngine.Debug.LogWarning("HttpListener is not supported on this platform.");
                return;
            }
            listener = new HttpListener();

            if (isStringLocalhost)
            {
                //AddAddress($"{stringLocalhostPrefix}:{port}/");
                listener.Prefixes.Add($"{stringLocalhostPrefix}:{port}/");
            }
            else
            {
                listener.Prefixes.Add($"{localhostPrefix}:{port}/");
            }

            responseCb = responseCallback;

            listener.Start();

            // Begin waiting for requests.
            listener.BeginGetContext(GetContextCallback, null);
            UnityEngine.Debug.Log($"Listening to {localhostPrefix}:{port}/.");
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }



        void GetContextCallback(IAsyncResult ar)
        {
            // Get the context
            var context = listener.EndGetContext(ar);

            // listen for the next request
            listener.BeginGetContext(GetContextCallback, null);

            responseCb.Invoke(context.Request.RawUrl);

            var responseString = htmlCode;

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            // and send it
            var response = context.Response;
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 200;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }


        /// <summary>
        /// Use this to reserve specific address (e.g  http://+)
        /// </summary>
        /// <param name="address"></param>
        void AddAddress(string address)
        {
            //string args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);
            string args = string.Format($"http add urlacl url={address} user=EVERYONE");

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            System.Diagnostics.Process.Start(psi).WaitForExit();
        }
    }
}