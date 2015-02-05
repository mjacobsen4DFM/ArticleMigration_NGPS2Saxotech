using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Collections;
using NDesk.Options;

namespace Migration
{
    class Program
    {


        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: migration [OPTIONS]+ ");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        } // Show Help

        static void wait()
        {
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }


        static bool CheckArgument(List<string> arg, string name)
        {
            if (arg.Count == 1)
                return true;
            string _msg = String.Format("You must specify argument --{0}", name);
            if (arg.Count > 1)
                _msg =  String.Format("You must specify only one argument with  --{0}", name);
            Console.WriteLine("{0}",_msg);              
            return false;
        }

        static void MigrateImagesOnly(string siteid ,string saxotechHost,string documents, string temp, string username, string pwd,string destination_siteid, DataSet ds)
        {
            int row = 0;
            foreach (DataRow articleRow in ds.Tables[0].Rows)
            {
                int found = 0, sent = 0;
                row += 1;
                string article_uid = articleRow["article_uid"].ToString();
                bool status = BusinessRule.BusinessRule.MigrateImage(saxotechHost, username, pwd, destination_siteid, article_uid, "N", ref found, ref sent);
                Console.WriteLine("Row: {0} Migrated Images  Article: {1} status: {2}", row, article_uid, status);
                if (found > 1)
                { // This is a Gallery 
                    status = BusinessRule.BusinessRule.MigrateGallery(siteid, documents, temp, saxotechHost, username, pwd, destination_siteid, article_uid, "N");
                    Console.WriteLine("Row: {0} Created Gallery  Article: {1}  Number images: {2} status: {3}", row, article_uid,found, status);
                }
            } // foreach

        }


        

        static void Main(string[] args)
        {
            bool show_help = false;
            bool images_only = false;
            bool articles_only = false;
            bool imageUpdate = false;
            bool forcearticlesExcludeImages = false;
            bool filter = false;
            List<string> extra;
            List<string> siteCodes = new List<string>();
            List<string> destCodes = new List<string>();
            List<string> startDates = new List<string>();
            List<string> endDates = new List<string>();
            var p = new OptionSet()
            {
                { "c|sitecode=", " (required) the source Site Code.",  v=> siteCodes.Add(v) },
                { "d|destcode=", " (required) the Destination Pub Code.",  v=> destCodes.Add(v) },
                { "s|start=", " (required) the start Date.",  v=> startDates.Add(v) },
                { "e|end=", " (required) the End Date.",  v=> endDates.Add(v) },
                { "i|images", " (Optional) migrate Images Only",  v=> images_only = v != null },
                { "f|forceArticlesOnly", " (Optional) migrate Articles do not include Images",  v=> forcearticlesExcludeImages = v != null },
                { "w| where filter", " (Optional) migrate Articles based on filter key app.config",  v=> filter = v != null },
                { "a|articles", " (Optional) migrate articles Only ( MUST MIGRATE IMAGES FIRST )",  v=> articles_only = v != null },
                { "u|update Articles with Images", " (Optional) updates existing articles with images",  v=> imageUpdate = v != null },

    			{ "h|help",  " Show this message and exit", v => show_help = v != null },
            };
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("migration: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try migration --help for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                Console.ReadLine();
                return;
            }
            bool valid = true;
            if (!CheckArgument(siteCodes, "sitecode"))
                valid = false;
            if (!CheckArgument(destCodes, "destcode"))
                valid = false;
            if (!CheckArgument(startDates, "start"))
                valid = false;
            if (!CheckArgument(endDates, "end"))
                valid = false;
            if (! valid ) {
                wait();
                return;
            }

            string siteid = siteCodes[0];
            string taxpubid = BusinessRule.BusinessRule.GetTaxPubid(siteid);
            if (taxpubid.Length == 0)
            {
                Console.WriteLine("Please assign a [taxonomyPubId] in table: [saxo_pubMap] for siteid: {0}", siteid);
                wait();
                return;

            }
            string destination_siteid = destCodes[0];
            DateTime startdate = DateTime.ParseExact(startDates[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopdate = DateTime.ParseExact(endDates[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string saxotechHost = BusinessRule.BusinessRule.GetSaxotechHost(Convert.ToInt32(siteid));
            string sqlstatement = string.Format("select article_uid from article where startdate >= '{0}' and startdate <= '{1}' and siteid = {2} ", startdate.ToShortDateString(), stopdate.ToShortDateString(), siteid);
            if (filter)
            {
                sqlstatement = string.Format("{0} {1} ", sqlstatement,ConfigurationManager.AppSettings["filter"].ToString());

            }


            if (images_only || imageUpdate)
                sqlstatement = string.Format("{0} and imagecount > 0 order by startdate", sqlstatement);
            else
                sqlstatement = string.Format("{0} order by startdate", sqlstatement);

            Console.WriteLine(sqlstatement);
            DataSet ds = BusinessRule.BusinessRule.BusGetDataset(sqlstatement);
            Console.WriteLine("Total: {0}", ds.Tables[0].Rows.Count);
            
            string username = ConfigurationManager.AppSettings["saxo_username"].ToString();
            string pwd = ConfigurationManager.AppSettings["saxo_pwd"].ToString();
            string documents = ConfigurationManager.AppSettings["documents"].ToString();
            string temp = ConfigurationManager.AppSettings["temp"].ToString();
            string SkipExists = ConfigurationManager.AppSettings["SkipExists"].ToString();
            if (images_only)
            {
                MigrateImagesOnly(siteid, saxotechHost,documents,temp,  username, pwd, destination_siteid, ds);
                wait();
                return;
            }
            ArrayList holdrules = BusinessRule.BusinessRule.htmlholdrules();
            int row = 0;
            if (articles_only || forcearticlesExcludeImages)
            {
                bool status;
                string ans = (forcearticlesExcludeImages) ? "Y" : "N";
                foreach (DataRow articleRow in ds.Tables[0].Rows)
                {
                    row += 1;
                    string article_uid = articleRow["article_uid"].ToString();
                    if (SkipExists.Equals("Y"))
                    {
                        bool bMigrated = BusinessRule.BusinessRule.ArticleMigrated(article_uid);
                        if (bMigrated)
                            Console.WriteLine("Row: {0} Already Migrated Article: {1} skipping", row, article_uid);
                        else
                        {
                            status = BusinessRule.BusinessRule.MigrateArticle(taxpubid, siteid, destination_siteid, documents, temp, saxotechHost, username, pwd, article_uid, "N", holdrules, ans);
                            Console.WriteLine("Row: {0} Migrated Article: {1} status: {2}", row, article_uid, status);
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Start Row: {0}   Article_UID: {1} Migrate", row, article_uid));
                        status = BusinessRule.BusinessRule.MigrateArticle(taxpubid, siteid, destination_siteid, documents, temp, saxotechHost, username, pwd, article_uid, "N", holdrules, ans);
                        Console.WriteLine("Row: {0} Migrated Article: {1} status: {2}", row, article_uid, status);
                    }
                } // foreach
                wait();
                return;
            } // articles_only



            // if -i -a not used then migrate image , galleries , and articles  ( -u switch will update images for articles that were sent )
            foreach (DataRow articleRow in ds.Tables[0].Rows)
            {
                int found = 0, sent = 0;
                row += 1;
                string article_uid = articleRow["article_uid"].ToString();
                bool status =  BusinessRule.BusinessRule.MigrateImage(saxotechHost, username, pwd, destination_siteid, article_uid, "N", ref found,ref sent);
                if (status)
                {
                    if (found > 1)
                        status = BusinessRule.BusinessRule.MigrateGallery(siteid,documents, temp, saxotechHost, username, pwd, destination_siteid, article_uid, "N");
                    if (status)
                    {
                        // BusinessRule.BusinessRule.MigratePDF(saxotechHost, username, pwd, destination_siteid, article_uid, "N", ref found,ref sent);
                        if (SkipExists.Equals("Y"))
                        {
                            if (!BusinessRule.BusinessRule.ArticleMigrated(article_uid))
                                status = BusinessRule.BusinessRule.MigrateArticle(taxpubid, siteid, destination_siteid, documents, temp, saxotechHost, username, pwd, article_uid, "N", holdrules);
                        }
                        else
                            status = BusinessRule.BusinessRule.MigrateArticle(taxpubid, siteid, destination_siteid, documents, temp, saxotechHost, username, pwd, article_uid, "N", holdrules);
                    } 
                }// end if
                Console.WriteLine("Row: {0} Migrated Article: {1} status: {2} ", row, article_uid, status);   
            } // foreach
        } // Main
    } // Program
} // End Namespace Migration
