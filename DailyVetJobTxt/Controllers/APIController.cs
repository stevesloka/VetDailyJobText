using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace DailyVetJobTxt.Controllers
{
    public class APIController : Controller
    {
        //
        // GET: /API/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendSMSAccountRequest(string phoneTo)
        {
            string resultMsg = "";

            //Get new random number
            Random num = new Random();
            int newPIN = num.Next(1000, 99999);

            //Verify it doesn't exist in the DB (keep trying until we get one)
            while (LData.BizPhoneUser.DoesPinExist(newPIN))
            {
                newPIN = num.Next(10000, 99999);
            }

            //Create the db record
            resultMsg = LData.BizPhoneUser.CreatePhoneUser(phoneTo, newPIN);
            if (resultMsg == "")
            {
                string msgString = string.Format("[VetTxt] Your PIN is: {0}", newPIN);

                var twilio = new TwilioRestClient(
                    System.Configuration.ConfigurationManager.AppSettings["Twilio_AccountSID"],
                    System.Configuration.ConfigurationManager.AppSettings["Twilio_AuthToken"]);

                var msg = twilio.SendSmsMessage(System.Configuration.ConfigurationManager.AppSettings["Twilio_SMSFromNumber"], phoneTo, msgString);
            }
            else
            {
                throw new ApplicationException("Could not create Phone User!");
            }

            return View(resultMsg);
        }

        public ActionResult LookForJobsForUser(string phoneNumber)
        {
            int zipCode = 0; string keywords = "";
            LData.BizPhoneUser.GetUserPrefs(phoneNumber, out zipCode, out keywords);

            string searchQuery = string.Format("http://beta.nrd.devis.com/jobSearch/raw/jobSearch?q={1}&location={0}&includeNearbyCities=on&datePosted=1&sort=jobposting-dateposted&order=desc", zipCode, keywords);

            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            WebRequest request = WebRequest.Create(searchQuery);

            try
            {
                WebResponse response = request.GetResponse();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    XmlTextReader reader = new XmlTextReader(sr);

                    try
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element: // The node is an element.
                                    Console.Write("<" + reader.Name);
                                    Console.WriteLine(">");
                                    break;
                                case XmlNodeType.Text: //Display the text in each element.
                                    Console.WriteLine(reader.Value);
                                    break;
                                case XmlNodeType.EndElement: //Display the end of the element.
                                    Console.Write("</" + reader.Name);
                                    Console.WriteLine(">");
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // handle if necessary
                    }
                }
            }
            catch (WebException)
            {
                // handle if necessary    
            }



            return View();
        }

        public object GetObjectFromXML(string s, Type t)
        {
            Stream xmlStream = new MemoryStream(s.Length);
            StreamWriter sw = new StreamWriter(xmlStream);
            sw.Write(s);
            sw.Flush();
            xmlStream.Seek(0, SeekOrigin.Begin);
            XmlSerializer serializer = new XmlSerializer(t);
            return serializer.Deserialize(xmlStream);
        }

    }
}
