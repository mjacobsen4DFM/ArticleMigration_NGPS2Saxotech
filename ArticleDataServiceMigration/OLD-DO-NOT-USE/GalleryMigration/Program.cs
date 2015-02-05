using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;
using System.Xml.Xsl;
using System.Xml;


namespace GalleryMigration
{
    class Program
    {
        static OWC.rest rc = new OWC.rest();
        static DbCommon.dbcommon DataDb = new DbCommon.dbcommon();
        static string LogFileName = "";
        static string documents = "";
        static string temp = "";

        #region Levenstein
        public static float CalculateSimilarity(String s1, String s2)
        {
            if ((s1 == null) || (s2 == null)) return 0.0f;

            float dis = LevenshteinDistance.Compute(s1, s2);
            float maxLen = s1.Length;
            if (maxLen < s2.Length)
                maxLen = s2.Length;
            if (maxLen == 0.0F)
                return 1.0F;
            else return 1.0F - dis / maxLen;
        }

        static class LevenshteinDistance
        {
            //    /// <summary>
            //    /// Compute the distance between two strings.
            //    /// </summary>
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                    return m;
                if (m == 0)
                    return n;
                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {   //Step 4
                    for (int j = 1; j <= m; j++)
                    {  // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                        // Step 6
                        d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            } // Compute
        } // end class  LevenshteinDistance
        #endregion




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
            DataSet ds = BusinessRule.BusinessRule.BusGetDataset(querystring);
            int nrows = ds.Tables[0].Rows.Count;
            if (nrows == 0)
                return ConfigurationManager.AppSettings[key].ToString();
            else
                return ds.Tables[0].Rows[0][0].ToString();

        }


        static string BuildGallery(string saxohost, string destination_siteid, string article_uid, string category, string startdate, string query, ref float similarity)
        {
            string xmldata = "";
            similarity = 0;
            string galleryXML = "";
            string gallerySaxo = "";
            try
            {

                string mappedcat = GetSaxoValue("saxo_category", category);
                DataSet ds = BusinessRule.BusinessRule.BusGetDataset(query);
                int count = ds.Tables[0].Rows.Count;
                int itemnumber = 1;
                XmlDocument x = new XmlDocument();
                x.Load(string.Format(@"{0}\gallery.xml", documents));
                x.GetElementsByTagName("siteuri").Item(0).InnerText = string.Format("{0}/{1}", saxohost, destination_siteid);
                x.GetElementsByTagName("site").Item(0).InnerText = destination_siteid;
                x.GetElementsByTagName("cat").Item(0).InnerText = mappedcat;
                x.GetElementsByTagName("startdate").Item(0).InnerText = startdate;
                XmlNode xn = x.GetElementsByTagName("images").Item(0);
                int NumImages = ds.Tables[0].Rows.Count;
                int rownumber = 0;
                string[] ExtrasImageNames;
                ExtrasImageNames = new string[NumImages];
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string location = dr[1].ToString();
                    string caption = dr[2].ToString();
                    string extrasImagepath = dr[3].ToString();
                    Uri uri = new Uri(extrasImagepath);
                    ExtrasImageNames[rownumber] = Path.GetFileName(uri.LocalPath);
                    if (NumImages == 2 && rownumber == 1) // Article with two Images calculates similarity 
                        similarity = CalculateSimilarity(ExtrasImageNames[0], ExtrasImageNames[1]);
                    XmlNode xk = x.CreateNode(XmlNodeType.Element, "image", "");
                    xk.InnerXml = string.Format("<location>{0}</location><itemnumber>{1}</itemnumber><caption>{2}</caption>", cdata(location), itemnumber, cdata(caption));
                    itemnumber += 1;
                    xn.AppendChild(xk);
                    rownumber += 1;
                }
                string xsltemplate = string.Format(@"{0}\gallery.xslt", documents);
                galleryXML = string.Format(@"{0}\{1}_gal.xml", temp, article_uid);
                gallerySaxo = string.Format(@"{0}\{1}_galsaxo.xml", temp, article_uid);
                x.Save(galleryXML);
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xsltemplate);
                xslt.Transform(galleryXML, gallerySaxo);
                StreamReader sr = new StreamReader(gallerySaxo);
                xmldata = sr.ReadToEnd();
                sr.Close();
            }

            catch (Exception e)
            {
                writetolog(string.Format("{0} {1}", e.Message, e.StackTrace));
            }
            finally
            {
                string[] tempFiles = { galleryXML, gallerySaxo };
                foreach (string tempFile in tempFiles)
                {
                    if (tempFile.Length > 0 && File.Exists(tempFile))
                        File.Delete(tempFile);
                }

            }
            return xmldata;
        } // BuildGalleries



        static void Main(string[] args)
        {
            string message = "", query = "";
            string saxohost = ConfigurationManager.AppSettings["saxohost"].ToString();
            string username = ConfigurationManager.AppSettings["username"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            string siteid = args[0];
            string destination_siteid = args[1];
            DateTime startdate = DateTime.ParseExact(args[2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopdate = DateTime.ParseExact(args[3], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            LogFileName = args[4];

            DataSet ds;
            string codeBase = AppDomain.CurrentDomain.BaseDirectory;
            documents = string.Format(@"{0}..\..\..\documents", codeBase);
            temp = string.Format(@"{0}..\..\..\temp", codeBase);
            query = string.Format("select article_uid,category,startdate,imagecount from article where startdate >= '{0}' and startdate <= '{1}' and imagecount > 1 and siteid= '{2}'",startdate, stopdate, siteid);
            ds = BusinessRule.BusinessRule.BusGetDataset(query);
            int count = ds.Tables[0].Rows.Count;
            Console.WriteLine(string.Format("Potential Galleries: {0}", count));
            int counter = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                message = "";
                string gallery_uid = "";
                string article_uid = dr["article_uid"].ToString();
                string category = dr["category"].ToString();
                string strStartdate = ((DateTime)dr["startdate"]).ToString("yyyy-MM-dd");
                string imagecount = dr["imagecount"].ToString();
                query = string.Format("Select sx.asset_uid, sx.url,im.caption, im.imagepath from saxo_image sx , image im where (im.asset_uid = sx.asset_uid) and sx.destination_siteid = '{0}' and im.asset_uid in ( select asset_uid from asset where asset_type = 108 and article_uid = {1} )", destination_siteid, article_uid);
                float similarity = 0;
                string xmldata = BuildGallery(saxohost, destination_siteid, article_uid, category, strStartdate, query, ref similarity);
                Uri GalleryAddress = new Uri(string.Format("{0}/{1}/galleries", saxohost, destination_siteid));

                if (xmldata.Length > 0)
                    gallery_uid = rc.postStory(GalleryAddress, username, pwd, xmldata, ref  message);
                if (gallery_uid.Length > 0 && message.Length == 0)
                {
                    DataDb.spExecute("loadSaxoGallery", ref message, "@article_uid", article_uid, "@destination_siteid", destination_siteid, "@gallery_uid", gallery_uid, "@similarity", similarity);
                    if (message.Length > 0)
                        writetolog(string.Format("{0} Article uid: {1} {2}", counter, article_uid, message));
                    else
                    {
                        Uri uri = new Uri(gallery_uid);
                        string galleryname = Path.GetFileName(uri.LocalPath);
                        Console.WriteLine(string.Format("{0} uid: {1} images: {2} sim: {3} guid: {4}", counter, article_uid, imagecount ,similarity , galleryname));
                    }
                    }
                else
                {
                    writetolog(string.Format("Article uid: {0} {1}", article_uid, message));

                }
                counter += 1; 
            }

        }
    }
}
