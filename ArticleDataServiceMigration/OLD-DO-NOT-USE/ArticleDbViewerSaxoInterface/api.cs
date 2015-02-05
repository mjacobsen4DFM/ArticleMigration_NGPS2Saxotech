using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Xml.Xsl;
using System.IO;
using System.Text.RegularExpressions;


namespace ArticleDbViewerSaxoInterface
{
    public class api
    {
        OWC.rest rc = new OWC.rest();
        DbCommon.DataMineDb DataMine = new DbCommon.DataMineDb();

        static string getData(string path)
        {
            string buffer = "";
            try
            {
                StreamReader sr = new StreamReader(path);
                buffer = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception )
            {

            }
            return buffer;
        } // get Data

        public byte[] GetFile(string ImageUrl, string username, string pwd, ref string message)
        {
            byte[] data = rc.getFile(new Uri(ImageUrl),username, pwd, ref message);
            return data;
        }


        private void sendImage(string saxohost, string username, string pwd, string destination_siteid, DataRow dr, ref StringBuilder sb)
        {
            string message = "";
            string asset_uid = dr["asset_uid"].ToString();
            if (DataMine.DoesSaxoAssetExist(destination_siteid, asset_uid))
            {
                sb.Append(string.Format("asset_uid: {0} exists {1}", asset_uid, Environment.NewLine));
                return;
            }
            string ImageUrl = dr["imagepath"].ToString();
            byte[] data = rc.getFile(new Uri(ImageUrl), ref message);
            if (message.Length > 0)
            {
                sb.Append(string.Format("rc.getfile asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine));
                return;
            }
            Uri img_address = new Uri(string.Format("{0}/{1}/myaccount/tempstore/images", saxohost, destination_siteid));
            string location = rc.postImage(img_address, username, pwd, ref message, data);
            if (message.Length > 0)
            {
                sb.Append(string.Format("rc.postImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine));
                return;
            }
            DataMine.spExecute("LoadSaxoImage", ref message, "@asset_uid", asset_uid, "@destination_siteid", destination_siteid, "@url", location);
            if (message.Length > 0)
                sb.Append(string.Format("sp:LoadSaxoImage asset_uid: {0} message: {1} {2} ", asset_uid, message, Environment.NewLine));
            else
                sb.Append(string.Format("assset_uid: {0} success location: {1} {2} ", asset_uid, location, Environment.NewLine));
        } // sendImage


        
        public void MigrateImage(string saxohost, string username, string pwd, string destination_siteid, string article_uid, ref string message )
        {
            StringBuilder sb = new StringBuilder();
            string query = string.Format("select asset_uid, imagepath from image where asset_uid in (select asset_uid from asset where asset_type = 108 and  article_uid = '{0}' )", article_uid);
            DataSet ds = DataMine.GetDs(query);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                sendImage(saxohost, username, pwd, destination_siteid, dr, ref sb);
            }// foreach
            message = sb.ToString();
        }// MigrateImage




        private string cdata(string src)
        {
            return string.Format("<![CDATA[{0}]]>", src);
        }

        private  string GetSaxoValue(string key, string category)
        {

            string querystring = string.Format("select {0} from saxo_categoryMap where category = '{1}'", key, category);
            DataSet ds = DataMine.GetDs(querystring);
            int nrows = ds.Tables[0].Rows.Count;
            if (nrows == 0)
                return System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            else
                return ds.Tables[0].Rows[0][0].ToString();
        }



        private string BuildGallery(string documents, string temp, string saxohost, string destination_siteid, string article_uid, string category, string startdate, string query, ref string message)
        {
            string xmldata = "";
            string galleryXML = "";
            string gallerySaxo = "";
            try
            {

                string mappedcat = GetSaxoValue("saxo_category", category);
                DataSet ds = DataMine.GetDs(query);
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
                if (NumImages == 0)
                {
                    return "";
                }
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
                //    if (NumImages == 2 && rownumber == 1) // Article with two Images calculates similarity 
                //        similarity = CalculateSimilarity(ExtrasImageNames[0], ExtrasImageNames[1]);
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
              message = string.Format("{0} {1}", e.Message, e.StackTrace);
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




        public void  MigrateGallery(string documents, string temp, string saxohost, string username, string pwd, string destination_siteid, string article_uid, ref string message )
        {
           string gallery_uid ="";;
           float similarity = 0;
           string query = string.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid);
           DataSet ds = DataMine.GetDs(query);
           int idx = ds.Tables[0].Rows.Count;
           if (idx == 1)
           {
               message = string.Format("Gallery_UID exists: {0}", ds.Tables[0].Rows[0][0].ToString());
               return;
           }
           ds.Dispose();
           query = string.Format("select startdate,category from article where article_uid = '{0}'", article_uid);
           ds = DataMine.GetDs(query);
           idx = ds.Tables[0].Rows.Count;
           if (idx != 1)
           {
               message = string.Format("Table article article_uid {0} returned {1} rows", article_uid, idx);
               return;
           }
           DataRow dr = ds.Tables[0].Rows[0];
           string category = dr["category"].ToString();
           string strStartdate = ((DateTime)dr["startdate"]).ToString("yyyy-MM-dd");
           query = string.Format("Select sx.asset_uid, sx.url,im.caption, im.imagepath from saxo_image sx , image im where (im.asset_uid = sx.asset_uid) and sx.destination_siteid = '{0}' and im.asset_uid in ( select asset_uid from asset where asset_type = 108 and article_uid = {1} )", destination_siteid, article_uid);
           string xmldata = BuildGallery(documents, temp, saxohost, destination_siteid, article_uid, category, strStartdate, query, ref message);
           if (message.Length > 0)
           {
               message = string.Format("BuildGallery {0}", message);
               return;
           }
           if ( xmldata.Length > 0 ) {
               Uri GalleryAddress = new Uri(string.Format("{0}/{1}/galleries", saxohost, destination_siteid));
               gallery_uid = rc.postStory(GalleryAddress, username, pwd, xmldata, ref  message);
               if (message.Length > 0)
               {
                    message = string.Format("rc.postStory {0} ", message);
                    return;
               }
               DataMine.spExecute("loadSaxoGallery", ref message, "@article_uid", article_uid, "@destination_siteid", destination_siteid, "@gallery_uid", gallery_uid, "@similarity", similarity);
               if (message.Length > 0)
               {
                    message = string.Format("sp:loadSaxoGallery Article uid: {1} {2}", article_uid, message);
                    return;
               } 
            }// xmldata  
            message = string.Format("GalleryUID: {0}", gallery_uid);
        }// MigrateGallery


        private void loadImageXML(string article_uid, string destination_siteid, ref XmlDocument x)
    {
        string query = string.Format("select si.url, i.caption from image i, saxo_image si where i.asset_uid = si.asset_uid and  si.destination_siteid ='{0}' and  i.asset_uid in (select asset_uid from asset where article_uid = '{1}')", destination_siteid, article_uid);
        DataSet dsImage = DataMine.GetDs(query);
        if (dsImage.Tables[0].Rows.Count == 1)
        {
            x.GetElementsByTagName("image").Item(0).InnerText = dsImage.Tables[0].Rows[0][0].ToString();
            x.GetElementsByTagName("imageCaption").Item(0).InnerText = Convert.IsDBNull(dsImage.Tables[0].Rows[0][1]) ? "" : dsImage.Tables[0].Rows[0][1].ToString();
            return;
        }
        query = string.Format("select gallery_uid from saxo_gallery where article_uid = '{0}' and destination_siteid ='{1}' ", article_uid, destination_siteid);
        DataSet dsGallery = DataMine.GetDs(query);
        if (dsGallery.Tables[0].Rows.Count == 1)
        {
            string galleryuid = dsImage.Tables[0].Rows[0][0].ToString();
            x.GetElementsByTagName("gallerypathuid").Item(0).InnerXml = galleryuid;
            Uri uri = new Uri(galleryuid);
            x.GetElementsByTagName("galleryuid").Item(0).InnerXml = Path.GetFileName(uri.LocalPath);
        }
    }// loadImageXML



        private string getViewUri(string story_xml)
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
            catch (Exception )
            {
            }
            return viewuri;
        }


        public bool MigrateArticle(string siteid, string destination_siteid, string documents, string temp, string saxohost, string username, string pwd, string article_uid, ref string message)
    {
       bool status = false;
       string query = string.Format("select * from article where article_uid = '{0}'", article_uid);
       DataSet ds = DataMine.GetDs(query);
       int idx = ds.Tables[0].Rows.Count;
       if (idx != 1)
       {
           message = string.Format("Table article article_uid: {0} returned: {1} rows", article_uid, idx);
           return status;
       }

       DataRow articleRow = ds.Tables[0].Rows[0];
       XmlDocument x = new XmlDocument();
       x.Load(string.Format(@"{0}\xmldocument.xml", documents));
       x.GetElementsByTagName("siteuri").Item(0).InnerText = string.Format("{0}/{1}", saxohost, destination_siteid);
       x.GetElementsByTagName("site").Item(0).InnerText = destination_siteid;
       int imagecount = Convert.ToInt32(articleRow["imagecount"]);
       DateTime startdate = Convert.IsDBNull(articleRow["startdate"]) ? DateTime.Now : (DateTime)articleRow["startdate"];
       DateTime enddate = Convert.IsDBNull(articleRow["enddate"]) ? DateTime.Now.AddYears(100) : (DateTime)articleRow["enddate"];
       x.GetElementsByTagName("startdate").Item(0).InnerXml = startdate.ToString("yyyyMMdd");
       x.GetElementsByTagName("enddate").Item(0).InnerXml = enddate.ToString("yyyyMMdd");
       if (imagecount > 0)
           loadImageXML(article_uid, destination_siteid, ref x);
       string[] keys = { "article_uid", "siteid", "origsite", "anchor", "seodescription", "relatedarticles", "heading", "summary", "body", "byline", "category" };
       foreach (string k in keys)
       {
           string value = Convert.IsDBNull(articleRow[k]) ? "" : articleRow[k].ToString();
           switch (k)
           {
               case "category":
                   x.GetElementsByTagName("category").Item(0).InnerXml = cdata(value);
                   x.GetElementsByTagName("category_saxo").Item(0).InnerXml = cdata(GetSaxoValue("saxo_category", value));
                   x.GetElementsByTagName("taxonomyword").Item(0).InnerXml = cdata(GetSaxoValue("taxonomyword", value));
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
           string articleXML = string.Format(@"{0}\{1}.xml", temp, article_uid);
           string articleSaxoXML = string.Format(@"{0}\{1}_saxo.xml", temp, article_uid);
           x.Save(articleXML);
           XslCompiledTransform xslt = new XslCompiledTransform();
           xslt.Load(string.Format(@"{0}\article.xslt", documents));
           xslt.Transform(articleXML, articleSaxoXML);
           string article_xml = getData(articleSaxoXML); // saxotech xml format this is posted to OWS 
           try
           {
               XmlDocument sx = new XmlDocument();
               sx.LoadXml(article_xml);
               // @siteid is the original cms numeric site id
               // @destination_siteid -  Saxotech 
               DataMine.spExecute("LoadSaxoArticle", ref  message, "@article_uid", article_uid, "@siteid", siteid, "@destination_siteid", destination_siteid, "@xmldata", article_xml);
               if (message.Length > 0) 
                   return false;
               Uri StoryAddress = new Uri(string.Format("{0}/{1}/stories", saxohost, destination_siteid));
               string storyurl = rc.postStory(StoryAddress, username, pwd, article_xml, ref message);
               if (message.Length > 0) 
                   return false;
               Uri storyuri = new Uri(storyurl);
               byte[] bytestoryxml = rc.getFile(storyuri, username, pwd, ref  message);
               if (message.Length > 0) 
                   return false;
               string story_xml = Encoding.UTF8.GetString(bytestoryxml);
               string viewuri = getViewUri(story_xml);
               DataMine.spExecute("updateSaxoArticleViewURI", ref message, "@article_uid", article_uid, "@destination_siteid", destination_siteid, "@viewuri", viewuri, "@storyurl", storyurl);
               if (message.Length == 0) 
                   status = true;
           }
           catch (Exception ex)
           {
               message =string.Format("Article {0} Exception {1} ", article_uid, ex.Message);
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
           return status;
    } //MigrateArticle

 }
}
