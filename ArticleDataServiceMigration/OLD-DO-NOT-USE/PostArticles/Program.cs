using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Configuration;
using System.IO;


namespace PostArticles
{
    class Program
    {
        static OWC.rest rc = new OWC.rest();
        static DbCommon.dbcommon DataDb = new DbCommon.dbcommon();

        static string LogFileName = "";

        static void writetolog(string errmsg)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string msg = string.Format(" message:{0} ", errmsg);
            string LogPath = string.Format("{0}\\{1}Error.log", path, LogFileName);
            StreamWriter sw = new StreamWriter(LogPath, true);
            sw.WriteLine(msg);
            sw.Close();
            Console.WriteLine(msg);
        } // writetolog

        static string getViewUri(string story_xml)
        {
            string viewuri = "";
            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(story_xml);
                XmlElement root = xdoc.DocumentElement;
                if (root.HasAttribute("viewuri"))
                    viewuri = root.GetAttribute("viewuri");
            }
            catch (Exception e)
            {
                writetolog(string.Format("getViewUri {0} {1}", e.Message, e.StackTrace));
            }
            return viewuri;
        }


        static void Main(string[] args)
        {
            string siteid = args[0];
            string destination_siteid = args[1];
            string saxohost = ConfigurationManager.AppSettings["saxohost"].ToString();
            string username = ConfigurationManager.AppSettings["username"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            Uri StoryAddress = new Uri(string.Format("{0}/{1}/stories", saxohost, destination_siteid));

            DateTime startdate = DateTime.ParseExact(args[2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopdate = DateTime.ParseExact(args[3], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            LogFileName = args[4];
            string query = string.Format("select article_uid, xmldata from saxo_article where destination_siteid = '{0}' and article_uid in (select article_uid from article where startdate >= '{1}' and startdate <= '{2}' and siteid = {3}) ", destination_siteid, startdate.ToShortDateString(), stopdate.ToShortDateString(), siteid);
            string myarticle_uid = "";
            if (args.Length == 6)
                myarticle_uid = args[5];
            if (myarticle_uid.Length > 0)
                query = string.Format("select article_uid, xmldata from saxo_article where article_uid = '{0}'", myarticle_uid);

            
            DataSet ds = BusinessRule.BusinessRule.BusGetDataset(query);
            Console.WriteLine("Total: {0}", ds.Tables[0].Rows.Count);
            int counter = 0;
            foreach (DataRow articleRow in ds.Tables[0].Rows)
            {
                string article_uid = articleRow["article_uid"].ToString();
                string article_xml = articleRow["xmldata"].ToString();
                //XmlDocument x = new XmlDocument();
                //x.LoadXml(article_xml);
                //XmlElement onlstory =(XmlElement) x.GetElementsByTagName("onl:story").Item(0);   //.GetAttribute("site");
                
                //onlstory.SetAttribute("site", ConfigurationManager.AppSettings["targetsite"].ToString());
                //onlstory.SetAttribute("siteuri", ConfigurationManager.AppSettings["saxohost"].ToString());
                //string xml_buffer = x.OuterXml;
                string message = "";
                counter += 1;
                string storyurl = rc.postStory(StoryAddress, username, pwd, article_xml, ref message);
                if (message.Length > 0)
                    writetolog(string.Format("Article id: {0} postStory {1} ", article_uid, message));
                else
                {
                    Uri storyuri = new Uri(storyurl);
                    byte[] bytestoryxml = rc.getFile(storyuri, username, pwd, ref  message);
                    if (message.Length == 0)
                    {
                        string story_xml = Encoding.UTF8.GetString(bytestoryxml);
                        string viewuri = getViewUri(story_xml);
                        if (viewuri.Length > 0)
                        {
                            Console.WriteLine("{0} Article_uid {1} Updated", counter, article_uid);
                            DataDb.spExecute("updateSaxoArticleViewURI", ref message, "@article_uid", article_uid, "@destination_siteid", destination_siteid, "@viewuri", viewuri, "@storyurl", storyurl);
                            if (message.Length > 0)
                                Console.WriteLine("Article id: {0} {1}", article_uid, message);
                        }
                        }
                    else
                    {
                        writetolog(string.Format("Article id: {0} storyurl: {1}  Error: {2}", article_uid, storyurl, message));
                    }
                }
            } // forech articleRow

        } // Main
    } // PostArticles
} // namespace
