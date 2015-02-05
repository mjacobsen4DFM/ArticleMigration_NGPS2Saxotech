using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Xml.Xsl;
using System.IO;
using System.Text.RegularExpressions;


namespace CreateSaxoArticle
{
    class Program
    {
        static DbCommon.dbcommon DataDb = new DbCommon.dbcommon();
        static string LogFileName = "";

        static string cdata(string src)
        {
            return string.Format("<![CDATA[{0}]]>", src);
        }

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


        static string GetSaxoValue(string key, string category)
        {
            string querystring = string.Format("select {0} from saxo_categoryMap where category = '{1}'", key, category);
            DataSet ds =  BusinessRule.BusinessRule.BusGetDataset(querystring);
            int nrows = ds.Tables[0].Rows.Count;
            if (nrows == 0)
                return ConfigurationManager.AppSettings[key].ToString();
            else
                return ds.Tables[0].Rows[0][0].ToString();

        }

        static string getData(string path)
        {
            string buffer = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                buffer = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
              Console.WriteLine(string.Format("{0} {1}", e.Message, e.StackTrace));
            }
            return buffer;
        } // get Data



        static void Main(string[] args)
        {
            // Dictionary<string, string> articleStrFields = new Dictionary<string, string>();
             string codeBase = AppDomain.CurrentDomain.BaseDirectory;
             string documents = string.Format(@"{0}..\..\..\documents",codeBase);
             string temp =      string.Format(@"{0}..\..\..\temp", codeBase);
            string articleXML = "";
            string articleSaxoXML = "";
            int count = 0;
            string saxohost = ConfigurationManager.AppSettings["saxohost"].ToString();

            string[] keys = { "article_uid", "siteid", "origsite", "anchor", "seodescription", "relatedarticles", "heading", "summary", "body", "byline", "category"};
            string siteid = args[0];
            string destination_siteid = args[1];
            DateTime startdate = DateTime.ParseExact(args[2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopdate = DateTime.ParseExact(args[3], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            LogFileName = args[4];
            string myarticle_uid = "";
            if (args.Length == 6)
                myarticle_uid = args[5];

            string query = string.Format("select * from article where startdate >= '{0}' and startdate <= '{1}' and siteid = {2} order by startdate", startdate.ToShortDateString(),stopdate.ToShortDateString(),siteid);
            if (myarticle_uid.Length > 0)
                query = string.Format("select * from article where article_uid = '{0}'",myarticle_uid);

            
            DataSet ds =  BusinessRule.BusinessRule.BusGetDataset(query);
            Console.WriteLine("Total: {0}", ds.Tables[0].Rows.Count);
            foreach (DataRow articleRow in ds.Tables[0].Rows)
            {
                XmlDocument x = new XmlDocument();
                x.Load(string.Format(@"{0}\xmldocument.xml", documents));
                x.GetElementsByTagName("siteuri").Item(0).InnerText = string.Format("{0}/{1}", saxohost, destination_siteid);
                x.GetElementsByTagName("site").Item(0).InnerText = destination_siteid;

                string article_uid = articleRow["article_uid"].ToString();
                int imagecount = Convert.ToInt32(articleRow["imagecount"]);

                startdate = Convert.IsDBNull(articleRow["startdate"]) ? DateTime.Now : (DateTime)articleRow["startdate"];
                DateTime enddate = Convert.IsDBNull(articleRow["enddate"]) ? DateTime.Now.AddYears(100) : (DateTime)articleRow["enddate"];
                x.GetElementsByTagName("startdate").Item(0).InnerXml = startdate.ToString("yyyyMMdd");
                x.GetElementsByTagName("enddate").Item(0).InnerXml = enddate.ToString("yyyyMMdd");
                if (imagecount > 0)
                {
                    query = string.Format("select si.url, i.caption from image i, saxo_image si where i.asset_uid = si.asset_uid and  si.destination_siteid ='{0}' and  i.asset_uid in (select asset_uid from asset where article_uid = '{1}')", destination_siteid, article_uid);
                    DataSet dsImage = BusinessRule.BusinessRule.BusGetDataset(query);
                    if (dsImage.Tables[0].Rows.Count == 1) {
                        x.GetElementsByTagName("image").Item(0).InnerText = dsImage.Tables[0].Rows[0][0].ToString();
                        x.GetElementsByTagName("imageCaption").Item(0).InnerText = Convert.IsDBNull(dsImage.Tables[0].Rows[0][1]) ? "" : dsImage.Tables[0].Rows[0][1].ToString();
                    } else {
                        query = string.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid);
                        DataSet dsGallery = BusinessRule.BusinessRule.BusGetDataset(query);
                        if (dsGallery.Tables[0].Rows.Count == 1)
                        {
                            string galleryuid = dsImage.Tables[0].Rows[0][0].ToString();
                            x.GetElementsByTagName("gallerypathuid").Item(0).InnerXml = galleryuid;
                            Uri uri = new Uri(galleryuid);
                            x.GetElementsByTagName("galleryuid").Item(0).InnerXml = Path.GetFileName(uri.LocalPath);

                        }
                    } // end else
                } // end imagecount >0

                foreach (string k in keys)
                {
                    string value = Convert.IsDBNull(articleRow[k]) ? "" : articleRow[k].ToString();
                    switch (k)
                    {
                        case "category":
                            x.GetElementsByTagName("category").Item(0).InnerXml = cdata(value);
                            x.GetElementsByTagName("category_saxo").Item(0).InnerXml = cdata(GetSaxoValue("saxo_category", value));
                            x.GetElementsByTagName("taxonomyword").Item(0).InnerXml = cdata(GetSaxoValue("taxonomyword",value));
                            break;
                        case "article_uid":
                            x.GetElementsByTagName("contentid").Item(0).InnerText = string.Format("ci_{0}", article_uid);
                            break;
                        case "keyword":
                             XmlNode xn = x.GetElementsByTagName("keywords").Item(0);
                             RegexOptions options = RegexOptions.None;
                             Regex regex = new Regex(@"[ ]{2,}", options);
                             value = regex.Replace(value, @" ");
                             value = value.Trim();
                             if (value.Length > 0)
                             {
                                 string[] keywords = value.Split(' ');
                                 foreach (string kvalue in keywords)
                                 {
                                     XmlNode xk = x.CreateNode(XmlNodeType.Element, "keyword", "");
                                     xk.InnerText = kvalue;
                                     xn.AppendChild(xk);

                                 }
                             }
                            break;
                        default:
                            x.GetElementsByTagName(k).Item(0).InnerXml = cdata(value);
                            break;
                    }
                } // foreach k

                articleXML = string.Format(@"{0}\{1}.xml", temp, article_uid);
                articleSaxoXML = string.Format(@"{0}\{1}_saxo.xml", temp, article_uid);
                x.Save(articleXML);
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(string.Format(@"{0}\article.xslt", documents));
                xslt.Transform(articleXML, articleSaxoXML);
                string saxoarticlexml = getData(articleSaxoXML); // saxotech xml format this is posted to OWS 
                string message = "";
                try
                {
                    count += 1;
                    XmlDocument sx = new XmlDocument();
                    sx.LoadXml(saxoarticlexml);
                    DataDb.spExecute("LoadSaxoArticle", ref  message, "@article_uid", article_uid, "@siteid", siteid, "@destination_siteid", destination_siteid, "@xmldata", saxoarticlexml);
                    if (message.Length > 0)
                        writetolog(string.Format("Article uid: {0} {1}", article_uid, message));
                    else
                        Console.WriteLine("{0} Article: {1}  Saxoxml  ", count, article_uid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Article Exception {0} ", article_uid, ex.Message);
                }
                finally
                {
                    string[] tempFiles = { articleXML, articleSaxoXML };
                    foreach (string tempFile in tempFiles)
                    {
                        if (tempFile.Length > 0 && File.Exists(tempFile))
                            File.Delete(tempFile);
                    }


                }
            } // for each articleRow

        }
    }
}
