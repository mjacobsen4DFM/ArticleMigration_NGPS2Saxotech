using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.IO;
using System.Xml.Xsl;
using System.Xml;


namespace ImageMigration
{
    class Program
    {
        static OWC.rest rc = new OWC.rest();
        static DbCommon.dbcommon DataDb = new DbCommon.dbcommon();

        static string LogFileName = "";
        static string documents = "";
        static string temp = "";



        /// <summary>
        /// Contains approximate string matching
        /// </summary>

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

        static void Main(string[] args)
        {
            string message = "", location = "", query = "";
            string saxohost = ConfigurationManager.AppSettings["saxohost"].ToString();
            string username = ConfigurationManager.AppSettings["username"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            string siteid = args[0];
            string destination_siteid = args[1];
            DateTime startdate = DateTime.ParseExact(args[2], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopdate = DateTime.ParseExact(args[3], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            LogFileName = args[4];
            string myarticle_uid ="";
            if (args.Length == 6)
                myarticle_uid = args[5];

            DataSet ds;
            string codeBase = AppDomain.CurrentDomain.BaseDirectory;
            documents = string.Format(@"{0}..\..\..\documents", codeBase);
            temp = string.Format(@"{0}..\..\..\temp", codeBase);

            query = string.Format("select asset_uid, imagepath from image where asset_uid in (select asset_uid from asset where asset_type = 108 and  article_uid in (select article_uid from article where startdate >= '{0}' and startdate <= '{1}' and siteid ='{2}' and imagecount > 0 ))", startdate, stopdate, siteid);
            if (myarticle_uid.Length > 0 )
                query = string.Format("select asset_uid, imagepath from image where asset_uid in (select asset_uid from asset where asset_type = 108 and  article_uid = '{0}' )", myarticle_uid);
            ds = BusinessRule.BusinessRule.BusGetDataset(query);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string asset_uid = dr["asset_uid"].ToString();
                if (!BusinessRule.BusinessRule.DoesSaxoAssetExist(destination_siteid, asset_uid))
                {
                    string ImageUrl = dr["imagepath"].ToString();
                    byte[] data = rc.getFile(new Uri(ImageUrl), ref message);
                    if (message.Length > 0)
                        writetolog(string.Format("Asset uid: {0} ImageUrl: {1} getfile {2}", asset_uid, ImageUrl, message));
                    else
                    {
                        Uri img_address = new Uri(string.Format("{0}/{1}/myaccount/tempstore/images", saxohost, destination_siteid));
                        location = rc.postImage(img_address, username, pwd, ref message, data);
                        if (message.Length > 0)
                            writetolog(string.Format("Asset uid: {0} {1}", asset_uid, message));
                        else
                        {
                            DataDb.spExecute("LoadSaxoImage", ref message, "@asset_uid", asset_uid, "@destination_siteid", destination_siteid, "@url", location);
                            if (message.Length > 0)
                                writetolog(string.Format("Asset uid: {0} {1}", asset_uid, message));
                            else
                                Console.WriteLine("Asset uid: {0} Destination Site: {1} Location: {2}", asset_uid, destination_siteid, location);
                        }
                        } // end else
                } // if !DoesSaxoExist
            } // foreach
        } // Main
    } // Program
} // End Namespace ImageMigration
