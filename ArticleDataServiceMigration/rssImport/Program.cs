using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.XPath;

namespace rssImport
{
    class Program
    {


        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: migration [OPTIONS]+ ");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        } // ShowHelp


        static void AddToLog(string msg, bool waitflag)
        {
            Console.WriteLine(msg);
            #if (DEBUG)
                if (waitflag)
                {
                    Console.WriteLine("Press any key to exit");
                    Console.Read();
                }
            #endif
        }

        public static Boolean IsNumeric(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }

        static string getItemValue(  XmlNode xmlitem, string myType, string xpath, XmlNamespaceManager nsmgr)
        {
            string value ="";
            try
            {
            switch (myType)
            {
                case "stringCollection":
                    foreach ( XmlNode x in xmlitem.SelectNodes(xpath,nsmgr) ){
                        value = string.Format("{0}; {1} ", value, x.InnerText);
                    }
                    break;

                case "string":
                     value = xmlitem.SelectSingleNode(xpath,nsmgr).InnerText;
                     break;
                case "outerxml":
                     if (string.IsNullOrEmpty(xpath))
                         value = xmlitem.OuterXml;
                     else
                         value = xmlitem.SelectSingleNode(xpath, nsmgr).OuterXml;
                     break;
                case "shortdatetime":
                     try
                     {
                         value = Convert.ToDateTime(xmlitem.SelectSingleNode(xpath, nsmgr).InnerText).ToShortDateString();
                     }
                     catch  
                     {
                         value = DateTime.Now.ToString("MM/dd/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                     }
                    break;
                case "datetime":
                     try
                     {
                        value = Convert.ToDateTime(xmlitem.SelectSingleNode(xpath, nsmgr).InnerText).ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                     }
                     catch 
                     {
                         value = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                     }
                     break;
                case "int":
                    value = xmlitem.SelectSingleNode(xpath, nsmgr).InnerText;
                     if ( ! IsNumeric(value))
                         value ="0";
                     break;
                case "profilexml":
                     StringBuilder sb = new StringBuilder();
                     string stmp = xmlitem.SelectSingleNode(xpath, nsmgr).InnerText;
                     string[] profiles = stmp.Split(',');

                     foreach (string p  in profiles) {
                        if (IsNumeric(p))
                            sb.Append(string.Format("<profile>{0}</profile>{1}", p, Environment.NewLine ));   
                     }
                     value =string.Format("<profiles>{0}{1}</profiles>", Environment.NewLine ,sb.ToString());
                     break;

            }
            }
            catch  { }
                return value;

        } // processItem


        static bool getsiteuri(string[] args,out string siteuri)
        {
            bool show_help = false;
            List<string> extra;
            List<string> lsiteuri = new List<string>();
            siteuri = "";
            var p = new OptionSet()
            {
                { "u|siteuri=", " (required) the source Site Code.",  v=> lsiteuri.Add(v) },
                { "h|help",  " Show this message and exit", v => show_help = v != null },


            }; // End OptionSet

            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                AddToLog(string.Format("Exception: {0} Try rssImport --help", e.Message), true);
                return false;
            }
            if (show_help)
            {
                ShowHelp(p);
                AddToLog("", true);
                return false;
            }
            siteuri = lsiteuri[0];
            return true;
        } // 






        static void Main(string[] args)
        {
            OWC.rest rc = new OWC.rest();
            DbCommon.dbcommon db = new DbCommon.dbcommon();
            Dictionary<string, string> fd = new Dictionary<string, string>();
            string message = "", address = "";
            if (!getsiteuri(args, out address))
                return;

            byte[] b = rc.getFile(new Uri(address),  ref message);
            string mrssDocument = Encoding.UTF8.GetString(b);
            XmlDocument x = new XmlDocument();
            x.LoadXml(mrssDocument);
             
            var nsmgr = new XmlNamespaceManager(x.NameTable);
            nsmgr.AddNamespace("dcterms", "http://purl.org/dc/terms/");
            nsmgr.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            nsmgr.AddNamespace("media","http://search.yahoo.com/mrss/");
            XmlNode node = x.SelectSingleNode("/rss/channel/link");
            if (node == null)
            {
                AddToLog("Mrss Node: /rss/channel/link not  Found",true);
                return;
            }
            string webAddress = node.InnerText.Replace("http://","");
            DataSet ds = db.getDs(string.Format("select pubname, pubcode from publication where webaddress = '{0}'", webAddress));
            DataRowCollection drc = ds.Tables[0].Rows;
            int found = drc.Count;

            if (found != 1)
            {
                AddToLog(string.Format("webAddress: {0} not found in table: publication returned rows: {1} ", webAddress, found), true);
                return;
            }

            XmlNodeList xmlitems = x.GetElementsByTagName("item");
            AddToLog(string.Format("Number of Items Found: {0}", xmlitems.Count),false);
            // Note: Below layout must be added to stored procedure rssitem if you are adding fields
            string[,] fields = new string[,] { 
                                     {"@guid","string", "extended/saxo-guid" },
                                     {"@pubdate","shortdatetime","pubDate"},
                                     {"@created","datetime","dcterms:created"},
                                     {"@modified","datetime","dcterms:modified"},
                                     {"@adstructure","stringCollection","category"},
                                     {"@categoryid","int","extended/category-id"},
                                     {"@xmlprofiles","profilexml","extended/profiles"},
                                     {"@xmlitem","outerxml",""},
            };

            int nfields = fields.GetLength(0);
            string pubname = drc[0]["pubname"].ToString();
            string pubcode = drc[0]["pubcode"].ToString();

            try
            {
            foreach (XmlNode xmlitem in xmlitems)
            {
                fd.Clear(); 
                for (int i = 0; i < nfields; i++)
                {
                    string mkey = fields[i,0];
                    string mytype = fields[i, 1];
                    string xpath = fields[i, 2];
                    string value = getItemValue(xmlitem, mytype, xpath,nsmgr);
                    fd.Add(mkey, value);
                }

                int rowsaffected = db.spExecute2("rssitem", ref message, "@pubcode", pubcode, "@pubname", pubname, "@siteaddress", webAddress, "@guid", fd["@guid"], "@pubdate",   fd["@pubdate"], "@created", fd["@created"], "@modified", fd["@modified"], "@adstructure", fd["@adstructure"], "@categoryid", fd["@categoryid"], "@xmlprofiles", fd["@xmlprofiles"], "@xmlitem", fd["@xmlitem"]);
                AddToLog(string.Format("guid: {0} rows affected: {1}", fd["@guid"], rowsaffected), false);
            }
            }
            catch (Exception e)
            {
                AddToLog(e.Message, true);
            }

        }
    }
}
