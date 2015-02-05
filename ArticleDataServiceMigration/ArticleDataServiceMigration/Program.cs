using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;
using System.Net;
using NDesk.Options;

namespace ArticleDataServiceMigration
{
    class Program
    {
        static string site_uid = "";
        static string LogFileName = "";
        static DateTime gStartDate;
        static int failedArticles = 0;
        static DbCommon.dbcommon DataDb = new DbCommon.dbcommon();
        static db2DataSet.MyDataSetTableAdapters.ARTICLETableAdapter ArticleTableAdapter = new db2DataSet.MyDataSetTableAdapters.ARTICLETableAdapter();
        static db2DataSet.MyDataSetTableAdapters.RELATED_CONTENTTableAdapter RelatedContentTableAdapter = new db2DataSet.MyDataSetTableAdapters.RELATED_CONTENTTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.SECTION_ITEMTableAdapter sectionItemTableAdapter = new db2DataSet.MyDataSetTableAdapters.SECTION_ITEMTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.CONTENT_GROUPTableAdapter ContentGroupTableAdapter = new db2DataSet.MyDataSetTableAdapters.CONTENT_GROUPTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.CONTENT_ITEM_CONTENT_GROUPTableAdapter ContentItemContentGroupTableAdapter = new db2DataSet.MyDataSetTableAdapters.CONTENT_ITEM_CONTENT_GROUPTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.RelatedPackagesTableAdapter RelatedPackagesTableAdapter = new db2DataSet.MyDataSetTableAdapters.RelatedPackagesTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.RelatedArticlesTableAdapter RelatedArticlesTableAdapter = new db2DataSet.MyDataSetTableAdapters.RelatedArticlesTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter ContentItemTableAdapter = new db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter();
        static db2DataSet.MyDataSetTableAdapters.IMAGETableAdapter ImageTableAdapter = new db2DataSet.MyDataSetTableAdapters.IMAGETableAdapter();


        enum asset
        {
            article = 102,
            image = 108,
            freeform = 111
        }

        static void writeExceptionLog( string errmsg, string stack)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string msg = string.Format(" message:{0} stack: {1}",  errmsg, stack);
            string LogPath = string.Format("{0}\\{1}ExceptionLog.log",path, LogFileName);
            StreamWriter sw = new StreamWriter(LogPath, true);
            sw.WriteLine(msg);
            sw.Close();
            Console.WriteLine(msg);
        } // writeExceptionLog

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


        static string GetSectionItem(string article_uid, string anchor_uid)
        {
            string sectionName = "";
            try
            {
                db2DataSet.MyDataSet.SECTION_ITEMDataTable SectionDataTable = new db2DataSet.MyDataSet.SECTION_ITEMDataTable();
                sectionItemTableAdapter.FillBy2(SectionDataTable, anchor_uid);
                if (SectionDataTable.Count() != 1)
                {
                    writetolog(string.Format("GetSectionItem article {0} anchor_uid {1} returned rows: {2}", article_uid, anchor_uid, SectionDataTable.Count()));
                    return sectionName;
                }
                db2DataSet.MyDataSet.SECTION_ITEMRow SectionRow = (db2DataSet.MyDataSet.SECTION_ITEMRow)SectionDataTable.Rows[0];

                sectionName = Convert.IsDBNull(SectionRow["SECTION_NAME"]) ? "" : SectionRow.SECTION_NAME;
            }
            catch (Exception e)
            {
                writetolog(string.Format("GetSectionItem Article_UID : {0} Error: {1} ", article_uid, e.Message));

            }
            return sectionName;

        }



        static string GetCategory(string uid)
        {
            string category = "";
            try
            {

                db2DataSet.MyDataSet.CONTENT_GROUPDataTable ContentGroupDataTable = new db2DataSet.MyDataSet.CONTENT_GROUPDataTable();
                ContentGroupTableAdapter.FillBy2(ContentGroupDataTable, uid);
                if (ContentGroupDataTable.Rows.Count == 1)
                {
                    db2DataSet.MyDataSet.CONTENT_GROUPRow ContentGroupRow = (db2DataSet.MyDataSet.CONTENT_GROUPRow)ContentGroupDataTable.Rows[0];
                    if (ContentGroupRow.CONTENT_GROUP_TYPE_UCODE.Equals("CATEGORY"))
                        category = ContentGroupRow.GROUP_NAME.ToUpper().Trim();
                }
                else
                {
                    writetolog(string.Format("GetCategory Returned rows {0}  content_group_uid: {1}", ContentGroupDataTable.Rows.Count, uid));

                }
            }
            catch (Exception e)
            {
                writeExceptionLog( e.Message, e.StackTrace);
            }
            return category;
        }


        static string GetArticleCategory(string article_uid)
        {

            db2DataSet.MyDataSet.CONTENT_ITEM_CONTENT_GROUPDataTable ContentItemContentGroupDataTable = new db2DataSet.MyDataSet.CONTENT_ITEM_CONTENT_GROUPDataTable();
            ContentItemContentGroupTableAdapter.FillBy2(ContentItemContentGroupDataTable, article_uid, site_uid);
            string category = "";
            try
            {
                foreach (db2DataSet.MyDataSet.CONTENT_ITEM_CONTENT_GROUPRow ContentItemContentGroupRow in ContentItemContentGroupDataTable)
                {
                    category = GetCategory(ContentItemContentGroupRow.CONTENT_GROUP_UID);
                    if (category.Length > 0)
                        break;
                } // foreach 
            }
            catch (Exception e)
            {
                writeExceptionLog(e.Message, e.StackTrace);
            }

            return category;
        } // GetArticleCategory


        static string scrub(string source)
        {
            Regex rgx = new Regex(@"[^\u0009\u000a\u000d\u0020-\uD7FF\uE000-\uFFFD]");
            return rgx.Replace(source, "");
        }


        static string[] GetRelatedArticles(string uid)
        {
            int MaxRelatedArticles = 10;
            String[] relatedArticlesUID = new string[0];
            try
            {
                db2DataSet.MyDataSet.RelatedPackagesDataTable RelatedPackagesDataTable = new db2DataSet.MyDataSet.RelatedPackagesDataTable();
                RelatedPackagesTableAdapter.Fill(RelatedPackagesDataTable, uid, site_uid);

                if (RelatedPackagesDataTable.Rows.Count == 0)
                    return new string[0];


                db2DataSet.MyDataSet.RelatedPackagesRow RelatedPackagesRow = (db2DataSet.MyDataSet.RelatedPackagesRow)RelatedPackagesDataTable.Rows[0];
                string ContentGroupUid = RelatedPackagesRow.CONTENT_GROUP_UID;
                 DateTime dt = DateTime.Now;


                db2DataSet.MyDataSet.RelatedArticlesDataTable RelatedArticlesDataTable = new db2DataSet.MyDataSet.RelatedArticlesDataTable();
                RelatedArticlesTableAdapter.Fill(RelatedArticlesDataTable, ContentGroupUid, site_uid, dt);

                int no = RelatedArticlesDataTable.Rows.Count;
                int Max = no;
                if (no == 0)
                    return new string[0];
                if (no > MaxRelatedArticles)
                    Max = MaxRelatedArticles;
                relatedArticlesUID = new String[Max];
                int idx = 0;
                foreach (db2DataSet.MyDataSet.RelatedArticlesRow RelatedArticlesRow in RelatedArticlesDataTable)
                {
                    relatedArticlesUID[idx] = RelatedArticlesRow.CONTENT_ITEM_UID;
                    idx += 1;
                    if (idx >= Max) break;
                }

            }
            catch (Exception e)
            {
                writeExceptionLog( e.Message, e.StackTrace);
            }
            return relatedArticlesUID;
        }



        static string GetOrigSite(string article_uid,string contentitemUid)
        {
            string origSiteUid = "";
            db2DataSet.MyDataSet.CONTENT_ITEMDataTable ContentItemDataTable = new db2DataSet.MyDataSet.CONTENT_ITEMDataTable();
            ContentItemTableAdapter.FillByUID(ContentItemDataTable, contentitemUid);
            if (ContentItemDataTable.Rows.Count == 1)
            {
                db2DataSet.MyDataSet.CONTENT_ITEMRow ContentItemRow = (db2DataSet.MyDataSet.CONTENT_ITEMRow)ContentItemDataTable.Rows[0];
                origSiteUid = ContentItemRow.SITE_UID;
            }
            else
            {
                string msg = string.Format("Error GetOrigSite article: {0} contentid: {1} contentitem table returned {2} rows ",article_uid, contentitemUid, ContentItemDataTable.Rows.Count);
                writetolog(msg);

            }
            return origSiteUid;
        }


        static  byte[] getFile(Uri address, ref string message)
        {
            byte[] data = null;
            try
            {
                WebClient wc = new WebClient();
                data = wc.DownloadData(address);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return data;
        }

        static void ProcessFreeForm(string article_uid)
        {
            List<DataModels.FREEFORM> forms = BusModel.GetFreeforms(article_uid);
            foreach (DataModels.FREEFORM form in forms)
            {
                string errmsg = "";
                DataDb.spExecute("LoadFreeForm", ref  errmsg, "@article_uid", article_uid, "@asset_uid", form.CONTENT_ITEM_UID, "@html", form.FREEFORM_HTML);
                if (errmsg.Length > 0)
                    writetolog(string.Format("ProcessFreeForm Article id: {0}  freeform uid: {1}  {2} ", article_uid, form.CONTENT_ITEM_UID, errmsg));

            }// foreach
        }


        static void ProcessPDF(string article_uid)
        {


            List<DataModels.PDF_VIEW> pdfs = BusModel.GetPdf(article_uid);
            foreach (DataModels.PDF_VIEW pdf in pdfs)
            {
                string errmsg = "";
                DataDb.spExecute("Loadpdf", ref  errmsg, "@article_uid", article_uid, "@asset_uid", pdf.CONTENT_ITEM_UID, "@binaryurl", pdf.BINARY_URL, "@caption", pdf.CAPTION);
                if (errmsg.Length > 0)
                    writetolog(string.Format("ProcessPDF Article id: {0}  freeform uid: {1}  {2} ", article_uid, pdf.CONTENT_ITEM_UID, errmsg));

            }// foreach
        }



        static int ProcessImageRow(db2DataSet.MyDataSet.CONTENT_ITEMRow ContentItemRow)
        {
            int imagecount = 0;
            string errmsg = "";
            string article_uid = ContentItemRow.CONTENT_ITEM_UID;
            db2DataSet.MyDataSet.RELATED_CONTENTDataTable RelatedContentItemDataTable = new db2DataSet.MyDataSet.RELATED_CONTENTDataTable();
            RelatedContentTableAdapter.FillBy(RelatedContentItemDataTable, article_uid);  // Get Related Content table for this article
            if (RelatedContentItemDataTable.Rows.Count > 0)
            {
                foreach (db2DataSet.MyDataSet.RELATED_CONTENTRow RelatedContentRow in RelatedContentItemDataTable)
                {
                    try
                    {
                        db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter Local_ContentItemTableAdapter = new db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter();
                        db2DataSet.MyDataSet.CONTENT_ITEMDataTable ContentItemDataTable = new db2DataSet.MyDataSet.CONTENT_ITEMDataTable();
                        Local_ContentItemTableAdapter.FillByUID(ContentItemDataTable, RelatedContentRow.RELATED_CONTENT_ITEM_UID); // Content_item of related content
                        if (ContentItemDataTable.Rows.Count == 1 && ((db2DataSet.MyDataSet.CONTENT_ITEMRow)(ContentItemDataTable.Rows[0])).CONTENT_TYPE_UID == (int)asset.image)
                        {
                            string position = Convert.IsDBNull(RelatedContentRow["POSITION_UCODE"]) ? "" : RelatedContentRow.POSITION_UCODE;
                            string asset_uid = ((db2DataSet.MyDataSet.CONTENT_ITEMRow)(ContentItemDataTable.Rows[0])).CONTENT_ITEM_UID;
                            db2DataSet.MyDataSet.IMAGEDataTable ImageDataTable = new db2DataSet.MyDataSet.IMAGEDataTable();
                            ImageTableAdapter.FillBy(ImageDataTable, asset_uid);
                            if (ImageDataTable.Count() == 1)
                            {
                                db2DataSet.MyDataSet.IMAGERow ImageRow = (db2DataSet.MyDataSet.IMAGERow)ImageDataTable.Rows[0];
                                string caption = Convert.IsDBNull(ImageRow["CAPTION"]) ? "" : ImageRow.CAPTION;
                                string media_type = Convert.IsDBNull(ImageRow["MEDIA_SIZE_TYPE_UCODE"]) ? "" : ImageRow.MEDIA_SIZE_TYPE_UCODE;
                                int filesize = 0;
                                byte[] data = getFile(new Uri(ImageRow.IMAGE_URL), ref errmsg);
                                if (errmsg.Length == 0)
                                    filesize = data.Length;
                                DataDb.spExecute("LoadImage", ref  errmsg, "@article_uid", article_uid, "@asset_uid", asset_uid, "@imagepath", ImageRow.IMAGE_URL, "@position", position, "@width", ImageRow.WIDTH, "@height", ImageRow.HEIGHT, "@caption", caption, "@filesize", filesize, "@media_type", media_type);
                                if (errmsg.Length > 0)
                                    writetolog(string.Format("Article id: {0} LoadImageTable {1} ", article_uid, errmsg));
                                else
                                    imagecount += 1;
                            } // ImageDataTable == 1
                        }
                    }
                    catch (Exception e)
                    {
                        writeExceptionLog(e.Message, e.StackTrace);
                    }
                } // foreach                    
            }
            return imagecount;
        } // ProcessImageRow



        static void ProcessArticleRow(db2DataSet.MyDataSet.CONTENT_ITEMRow ContentItemRow, int imagecount,int articleCount)
        {
            string origsite = "", errmsg ="";

            string article_uid = ContentItemRow.CONTENT_ITEM_UID;
            DateTime StartDate = Convert.IsDBNull(ContentItemRow["START_DATE"]) ? DateTime.Now : ContentItemRow.START_DATE;
            gStartDate = StartDate;
            DateTime EndDate = Convert.IsDBNull(ContentItemRow["END_DATE"]) ? DateTime.Now.AddYears(100) : ContentItemRow.END_DATE;
            string keyword = Convert.IsDBNull(ContentItemRow["KEYWORD"]) ? "" : ContentItemRow.KEYWORD;
            string seodescription = scrub(Convert.IsDBNull(ContentItemRow["SEO_DESCRIPTIVE_TEXT"]) ? "" : ContentItemRow.SEO_DESCRIPTIVE_TEXT);
            string heading = scrub(Convert.IsDBNull(ContentItemRow["CONTENT_TITLE"]) ? "" : ContentItemRow.CONTENT_TITLE);
            string category = GetArticleCategory(article_uid);
            String[] RelatedArticlesUID = GetRelatedArticles(ContentItemRow.CONTENT_ITEM_UID);
            
            StringBuilder relatedArticles = new StringBuilder();
            if (RelatedArticlesUID.Count() > 0)
                foreach (string relatedArticleUID in RelatedArticlesUID)
                    relatedArticles.Append(string.Format("{0},", relatedArticleUID));
            string sRelatedArticles = relatedArticles.ToString();
            if (sRelatedArticles.EndsWith(","))
                sRelatedArticles = sRelatedArticles.Substring(0, sRelatedArticles.Length - 1);

            string SectionAnchorUID = Convert.IsDBNull(ContentItemRow["SECTION_ANCHOR_UID"]) ? "0" : ContentItemRow.SECTION_ANCHOR_UID;
            // if (SectionAnchorUID.Length > 0)
            //    SectionAnchor = GetSectionItem(article_uid, SectionAnchorUID);
            string origContentItemUid = Convert.IsDBNull(ContentItemRow["ORIG_CONTENT_ITEM_UID"]) ? "" : ContentItemRow.ORIG_CONTENT_ITEM_UID;
            if (origContentItemUid.Length > 0)
                origsite = GetOrigSite(article_uid,origContentItemUid);  // shared content save original Site UID
            db2DataSet.MyDataSet.ARTICLEDataTable ArticleDataTable = new db2DataSet.MyDataSet.ARTICLEDataTable();
            ArticleTableAdapter.FillBy2(ArticleDataTable, article_uid); // article table
            if (ArticleDataTable.Rows.Count != 1)
            {
                writetolog(string.Format("Content id: {0} rows returned: {1}", ContentItemRow.CONTENT_ITEM_UID, ArticleDataTable.Rows.Count));
                return;
            }
            db2DataSet.MyDataSet.ARTICLERow ArticleRow = (db2DataSet.MyDataSet.ARTICLERow)ArticleDataTable.Rows[0];
            string summary = scrub(Convert.IsDBNull(ArticleRow["ABSTRACT"]) ? "" : ArticleRow.ABSTRACT);
            // Note saxotech can not handle abstartion field greater then 100
            string byline = scrub(Convert.IsDBNull(ArticleRow["BYLINE"]) ? "" : ArticleRow.BYLINE);
            string body = scrub(Convert.IsDBNull(ArticleRow["BODY"]) ? "" : ArticleRow.BODY);
            string subtitle = scrub(Convert.IsDBNull(ArticleRow["SUBTITLE"]) ? "" : ArticleRow.SUBTITLE);
            // Save the following to the Database ...
            //  @siteid @siteid @article_uid  @category  @anchor @startdate @enddate @heading @body @byline @subtitle @summary @RelatedArticles @seodescription @keyword
            DataDb.spExecute("LoadArticle", ref  errmsg, "@siteid", site_uid, "@article_uid", article_uid, "@category", category, "@anchor", SectionAnchorUID, "@startdate", StartDate, "@enddate", EndDate, "@heading", heading, "@body", body, "@byline", byline, "@subtitle", subtitle, "@summary", summary, "@RelatedArticles", sRelatedArticles, "@seodescription", seodescription, "@keyword", keyword, "@imagecount", imagecount, "@origsite", origsite);
            if (errmsg.Length > 0)
            {
                writetolog(string.Format("Failed Article id: {0} LoadArticle {1} ", article_uid, errmsg));
                failedArticles += 1;
            }
            // else
            //    Console.WriteLine(string.Format("Processed: {0} Article {1} Images: {2}", articleCount, article_uid, imagecount));
        }


        static void LoadArticles(db2DataSet.MyDataSet.CONTENT_ITEMDataTable ContentItemDataTable)
        {
            int articleCount = 0;
            int resetcount = 0;
            DateTime startwatch = DateTime.Now;
            DateTime startwatch2 = DateTime.Now;
            int cnt = 0;
            int total =  ContentItemDataTable.Rows.Count;
            float throttle = 0;
            foreach (db2DataSet.MyDataSet.CONTENT_ITEMRow ContentItemRow in ContentItemDataTable)  // content_item 
            {
                try {
                    string article_uid = ContentItemRow.CONTENT_ITEM_UID;
                    ProcessFreeForm(article_uid);
                    ProcessPDF(article_uid);
                    bool articleExists = BusinessRule.BusinessRule.DoesArticleExist(article_uid);
                        
                    if (!articleExists)
                    {
                        int imagecount = ProcessImageRow(ContentItemRow); // Update Image table
                        articleCount += 1;
                        resetcount = articleCount;

                        ProcessArticleRow(ContentItemRow, imagecount, articleCount);
                        if ((articleCount % 100) == 0)
                        {
                            TimeSpan t = DateTime.Now.Subtract(startwatch);
                            TimeSpan t2 = DateTime.Now.Subtract(startwatch2);

                            int remaining = total - articleCount;
//                            float average = (float) articleCount /(float) ((t.Hours * 60) + (t.Minutes * 60) + t.Seconds);
                            float avg = (float)t2.Seconds /100;
                            throttle += avg;
                            cnt += 1;
                            avg = throttle / (float)cnt;
                            float totaverage =  (float)remaining * avg; // Number of seconds remaining
                            TimeSpan t3 = TimeSpan.FromSeconds(totaverage);


                            Console.WriteLine(string.Format("{0}/{1}  Date: {2} Remaining Time Est: {3}", articleCount, total, gStartDate.ToShortDateString(),  t3));
                            startwatch2 = DateTime.Now;
                        }

                    } // articleExists
                    else
                    {
                        Console.WriteLine(string.Format("Article: {0} exists already skip...", article_uid));
                        articleCount += 1;
                    }
                }
                catch (Exception e)
                {
                    writeExceptionLog( e.Message, e.StackTrace);
                }
            } // foreach
        } // LoadArticles


        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: ArticleDataServiceMigration [OPTIONS]+ ");
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
                _msg = String.Format("You must specify only one argument with  --{0}", name);
            Console.WriteLine("{0}", _msg);
            return false;
        }



        static void Main(string[] args)
        {
            // Guid g1 = Guid.NewGuid();
            // string key = g1.ToString("N");
            bool show_help = false;

            List<string> extra;
            List<string> siteCodes = new List<string>();
            List<string> Logfiles = new List<string>();
            List<string> startDates = new List<string>();
            List<string> endDates = new List<string>();
            var p = new OptionSet()
            {
                { "c|sitecode=", " (required) the source Site Code.",  v=> siteCodes.Add(v) },
                { "s|start=", " (required) the start Date.",  v=> startDates.Add(v) },
                { "e|end=", " (required) the End Date.",  v=> endDates.Add(v) },
                { "l|log=", " (required) log filename.",  v=> Logfiles.Add(v) },
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
            if (!CheckArgument(Logfiles, "Log"))
                valid = false;
            if (!CheckArgument(startDates, "start"))
                valid = false;
            if (!CheckArgument(endDates, "end"))
                valid = false;
            if (!valid)
            {
                wait();
                return;
            }



            site_uid = siteCodes[0];
            DateTime startdate = DateTime.ParseExact(startDates[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime stopDate = DateTime.ParseExact(endDates[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            LogFileName = Logfiles[0];
            db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter ContentItemTableAdapter = new db2DataSet.MyDataSetTableAdapters.CONTENT_ITEMTableAdapter();
            db2DataSet.MyDataSet.CONTENT_ITEMDataTable ContentItemDataTable = new db2DataSet.MyDataSet.CONTENT_ITEMDataTable();
            Console.WriteLine("site_uid: {0} startdate: {1} stopdate: {2}", site_uid, startdate, stopDate);
            ContentItemTableAdapter.FillBy(ContentItemDataTable, site_uid, (int)asset.article, startdate, stopDate); // Get Articles
            Console.WriteLine("Total Articles: {0} ", ContentItemDataTable.Rows.Count);
            LoadArticles(ContentItemDataTable);
            Console.WriteLine(string.Format("Finished Failed: {0} ", failedArticles));
            Console.Read();
        }
    }
}
