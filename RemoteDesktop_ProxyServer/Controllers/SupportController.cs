﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RemoteDesktop_ProxyServer.Controllers
{       
    //ALWAYS REQUIRE HTTPS for release builds
#if !DEBUG
        [RequireHttps]
#endif
    public class SupportController : Controller
    {
        private string GetIP4Address()
        {
            if (Request.UserHostAddress == "::1") return "127.0.0.1";
            string IP4Address = String.Empty;
            foreach (IPAddress IPA in Dns.GetHostAddresses(Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty) return IP4Address;
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetID(string computername, string username, string mac, int session)
        {
            Debug.WriteLine("Received GetID request " + computername + "  " + username + "  " + mac + "  " + session);
            var c = RemoteDesktop_ProxyServer.Signalr.ProxyWatcher.ReserveID(GetIP4Address(), computername, username, mac, session);
            if (c == null) return Content("");
            var st = c.Src_ID.ToString() + "\n";
            st += c.AES_Session_Key;
            return Content(st);
        }
        public ActionResult GetP2P_File()
        {
            string path = ConfigurationManager.AppSettings["RD_P2P_File"];
            if(string.IsNullOrWhiteSpace(path)) throw new Exception("Missing P2p File");
            var res = new RemoteDesktop_ProxyServer.Code.ResourceLayout();
            res.IDS_STRINGDISCLAIMERMESSAGE = "This is a changed Message";
            return File(RemoteDesktop_ProxyServer.Code.UpdateEXE.Update(path, res), "application/exe", Path.GetFileName(path));
        }

        [HttpPost]
        public ActionResult Authenticate(string Username, string Password)
        {

            Debug.WriteLine("Received auth request " + Username + "  " + Password);
            bool result = false;
            //BELOW IS TESTING OF THE SERVICE.. Code should be replaced with real authentication code
            if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
            {
                result = (Username == "test") && (Password == "abc123");
            }

            if (result)
            {
                FormsAuthentication.SetAuthCookie(Username, false);
            }
            return new EmptyResult();
        }
    }
}