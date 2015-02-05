using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using OWC;
using BusinessRuleC;
using System.Configuration;
using DataModels;
using System.Globalization;

namespace ProfileTree
{
    class Program
    {

        private static Boolean GNAVIGATION = false;

        private static string getrealprofile(int id)
        {
            string message = "";
            rest rc = new rest();
            string user = ConfigurationManager.AppSettings["user"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            string address = ConfigurationManager.AppSettings["nav2real"].ToString();
            address = string.Format("{0}{1}", address, id);
            byte[] b = rc.getFile(new Uri(address), user, pwd, ref message);
            if (message.Length > 0)
                throw new System.Exception(message);
            return Encoding.UTF8.GetString(b);

        }


        static private void PlaceNodes(XmlDocument dom, int treelevel, int rootid, bool updaterealid)
        {
            Dictionary<string,string> attributesDictionary = new Dictionary<string,string>();

            XmlNodeList nl = dom.GetElementsByTagName("onl:profile");
            foreach (XmlNode node in nl)
            {
                attributesDictionary.Clear();
                string[] attributenames = { "id","childrenuri","uri", "childcount","parentid" }; // or var attributenames = new Lst<string> {"uri", "childcount"};
                foreach (string attributename in attributenames ){
                    if (node.Attributes[attributename] != null)
                        attributesDictionary.Add(attributename, node.Attributes[attributename].Value); 
                }
                string fieldname = node.FirstChild.InnerText; 
                string uri = (attributesDictionary.ContainsKey("uri"))?attributesDictionary["uri"]: "";
                string childrenuri = (attributesDictionary.ContainsKey("childrenuri")) ? attributesDictionary["childrenuri"] : "";

                int id = Convert.ToInt32((attributesDictionary.ContainsKey("id")) ? attributesDictionary["id"] : "0" );

                int parentid = Convert.ToInt32( (attributesDictionary.ContainsKey("parentid")) ? Convert.ToInt32( attributesDictionary["parentid"]) : id);
                int childcount = Convert.ToInt32((attributesDictionary.ContainsKey("childcount")) ? attributesDictionary["childcount"] : "0");
                if (treelevel.Equals(0))
                    rootid = parentid;
                int ireal = 0;
                if (GNAVIGATION)
                {
                   string realid = getrealprofile(id);
                   bool res = int.TryParse(realid, out ireal);
                   if (res == false)
                       ireal = 0;

                }
                BusinessRule.AddEntry(treelevel, id, rootid, parentid, fieldname, uri, childcount, childrenuri, ireal, updaterealid);

                // Console.WriteLine(string.Format("id: {0} name: {1}  \n \t  childcount: {2} \n \t \t childrenuri:{3} ", id, fieldname,  childcount, childrenuri)); 
            }


        }


        static int GetRootProfile( string fieldname, ref string childrenuri, ref int parentid)
        {
            List<profile> profiles = BusinessRule.GetProfiles(0,fieldname);
            if (profiles.Count != 1 ) throw new System.Exception(string.Format("Treelevel: 0  fieldname: {0} profile count: {1} should return only one profile ", fieldname, profiles.Count));
            childrenuri = profiles[0].childrenuri;
            parentid = profiles[0].parentid;
            return profiles[0].childcount;

        }

        static List<profile> GetProfiles(int treelevel, int rootid)
        {
            return BusinessRule.GetProfiles(treelevel, rootid);
        }


        static string GetRemoteDocument(string address)
        {
            string message = "";
            rest rc = new rest();
            string user = ConfigurationManager.AppSettings["user"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();

            byte[] b = rc.getFile(new Uri(address), user, pwd, ref message);
            if (message.Length > 0)
                   throw new System.Exception(message);
            return  Encoding.UTF8.GetString(b);
        }

        static void Main(string[] args)
        {
            string childrenuri = "", address ="", XmlData ="";
            bool  updaterealid = ConfigurationManager.AppSettings["updaterealid"].Equals("true", StringComparison.InvariantCultureIgnoreCase)?true:false;
            XmlDocument dom = new XmlDocument();
            int sitecode = 207;
            Console.WriteLine("Reload Profile table YES/NO ?");
            string answer = Console.ReadLine();
            if (answer.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                BusinessRule.truncateProfileTable();
            }  
                                                        
            List<saxo_pubMap> mypubs = BusinessRule.ListPub(sitecode);
            if (mypubs.Count > 1)
            {
                Console.WriteLine(string.Format("Invalid number of pubs returned for site: {0}", sitecode));
                Console.Read();
                return;
            }

            if (answer.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                address = string.Format("{0}{1}/profiles", mypubs[0].OWStarget, mypubs[0].destination_siteid);
                XmlData = GetRemoteDocument(address);
                dom.LoadXml(XmlData);
                PlaceNodes(dom, 0, 0, updaterealid);
            }
            
            Console.WriteLine("Profile Name: (Subject, Geography,Placement,Navigation?");
            string fieldname = Console.ReadLine();
            CultureInfo ci;
            ci = new CultureInfo("en-US");
            if (fieldname.StartsWith("nav",true,ci)) {
                GNAVIGATION = true;
            }

            // string fieldname = "subject"; 
            int rootid = 0;
            int childcount = GetRootProfile(fieldname, ref childrenuri,ref rootid);
            Console.WriteLine(string.Format(" {0} childrenuri: {1} number of children: {2} ", fieldname, childrenuri, childcount));

            XmlData = GetRemoteDocument(childrenuri);
            dom.LoadXml(XmlData);
            int treelevel = 1;

            PlaceNodes(dom, treelevel, rootid,updaterealid);
            List<profile> profiles = GetProfiles(treelevel, rootid);
            while (profiles.Count > 0)
            {
                Console.WriteLine(string.Format("Treelevel {0} profiles: {1}", treelevel, profiles.Count));
                treelevel += 1;
                int idx = 0;
                foreach (profile p in profiles)
                {
                    XmlData = GetRemoteDocument(p.childrenuri);
                    dom.LoadXml(XmlData);
                    PlaceNodes(dom, treelevel, rootid,updaterealid);
                    idx +=1;
                    Console.WriteLine("Processed {0}/{1}", idx, profiles.Count); 
                }
                profiles = GetProfiles(treelevel, rootid);

            } // end while
            Console.WriteLine("Profile load completed. Hit Return to exit");
            Console.Read();
        }
    }
}
