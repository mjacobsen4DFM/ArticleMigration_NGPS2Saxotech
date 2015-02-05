using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using DataModels;
using OWC;
using System.Configuration;
using UncAccess;



//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;


public partial class admin_migrateqao : System.Web.UI.Page
{
        static rest rc = new rest();
        static DbCommon.dbcommon Dbcommon = new DbCommon.dbcommon();
        static int failedArticles = 0;


         static bool DoesFileExist(string uncBasePath,string user, string pwd, string dom,  string filenamepath)
        {
            bool status = false;
            using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
            {
                if (unc.NetUseWithCredentials(uncBasePath , user, dom, pwd))
                {
                    if (File.Exists(filenamepath))
                        status = true;
                }
            }// End using 
            return status;
        }


        static int ProcessImages(string article_uid, List<CONTENT_ITEM> ContentItems)
        {
            int NumberOfImages = 0;
            foreach (CONTENT_ITEM ContentItem in ContentItems)
            {
                string uid = ContentItem.CONTENT_ITEM_UID;
                List<IMAGE> ImageItems = BusinessModel.GetImageItems(uid);
                foreach (IMAGE ImageItem in ImageItems)
                {
                    string errmsg = "";
                    string imgurl = ImageItem.IMAGE_URL;
                    bool fileExists = false;
                    string ImageUrl = ConfigurationManager.AppSettings["ImageUrl"];
                    if (imgurl.StartsWith(ImageUrl))
                    {
                        string uncBasePath = ConfigurationManager.AppSettings["uncBasePath"];
                        string image_unc = imgurl.Replace(ImageUrl, string.Format(@"{0}\", uncBasePath)).Replace(@"/",@"\");
                        string dom = ConfigurationManager.AppSettings["uncDomain"];
                        string user = ConfigurationManager.AppSettings["uncUser"];
                        string pwd = ConfigurationManager.AppSettings["uncPwd"];
                        fileExists = DoesFileExist(uncBasePath, user, pwd, dom, image_unc);
                    }
                    else
                    {
                        fileExists = rc.ImageExists(ImageItem.IMAGE_URL);
                    }
                    if (fileExists) {
                        int? filesize = (ImageItem.FILE_SIZE == null) ? 0 : ImageItem.FILE_SIZE;
                        string caption = (ImageItem.CAPTION == null) ? "" : ImageItem.CAPTION;
                        string media_type = (ImageItem.MEDIA_SIZE_TYPE_UCODE == null) ? "" : ImageItem.MEDIA_SIZE_TYPE_UCODE;
                        Dbcommon.spExecute("LoadImage", ref  errmsg, "@article_uid", article_uid, "@asset_uid", uid, "@imagepath", ImageItem.IMAGE_URL, "@position", "0", "@width", ImageItem.WIDTH, "@height", ImageItem.HEIGHT, "@caption", caption, "@filesize", filesize, "@media_type", media_type);
                        if (errmsg.Length.Equals(0))
                            NumberOfImages += 1;
                    }// end if fileExists
                    if (!fileExists)
                        Console.WriteLine("Image does not exist {0}", ImageItem.IMAGE_URL);
                } // foreach IMAGE
            } // foreach contentitem
            return NumberOfImages;
        } // ProcessImages


         static int ProcessFreeForms(string article_uid, List<CONTENT_ITEM> FreeformContentItems)
        {
            int Numberoffreeforms = 0;
            foreach (CONTENT_ITEM FreeformContentItem in FreeformContentItems)
            {
                string uid = FreeformContentItem.CONTENT_ITEM_UID;
                List<FREEFORM> FreeFormItems = BusinessModel.GetFreeFormItems(uid);
                foreach (FREEFORM FreeFormItem in FreeFormItems)
                {
                    string errmsg = "";
                    string FREEFORM_HTML = (FreeFormItem.FREEFORM_HTML == null) ? "" : FreeFormItem.FREEFORM_HTML;
                    Dbcommon.spExecute("LoadFreeForm", ref  errmsg, "@article_uid", article_uid, "@asset_uid", FreeFormItem.CONTENT_ITEM_UID, "@html", FREEFORM_HTML);
                    if (errmsg.Length.Equals( 0))
                        Numberoffreeforms += 1;
                }// foreach
            } // foreach
            return Numberoffreeforms;
        } // ProcessFreeForms


        static void UpdateRelatedContent(string article_uid, List<CONTENT_ITEM_REL> RelatedContentitems, ref int NumberOfImages )
        {
            NumberOfImages = 0;
            int NumberofPdfs = 0, Numberoffreeforms = 0;
            Predicate <CONTENT_ITEM> FindImage = (CONTENT_ITEM p) => { return p.CONTENT_TYPE_UID == (int) BusinessModel.asset.image; };
            Predicate<CONTENT_ITEM> FindPdf = (CONTENT_ITEM p) => { return p.CONTENT_TYPE_UID == (int)BusinessModel.asset.pdf; };
            Predicate<CONTENT_ITEM> FreeForm = (CONTENT_ITEM p) => { return p.CONTENT_TYPE_UID == (int)BusinessModel.asset.freeform; };

            List<CONTENT_ITEM> MyContentItems = new List<CONTENT_ITEM>();  
            foreach (CONTENT_ITEM_REL RelatedContentitem in RelatedContentitems)
            {
                string uid = RelatedContentitem.RELATED_CONTENT_ITEM_UID;
                 List<CONTENT_ITEM> ContentItems=  BusinessModel.GetContentItems(uid);
                MyContentItems.AddRange(ContentItems);
            }
            List<CONTENT_ITEM> ImageContentItems = MyContentItems.FindAll(FindImage);

            if (ImageContentItems.Count > 0)
               NumberOfImages = ProcessImages(article_uid,ImageContentItems);

            List<CONTENT_ITEM> PdfContentItems = MyContentItems.FindAll(FindPdf);
            //if (PdfContentItems.Count > 0)
            //   NumberofPdfs = ProcessPdfs(article_uid, PdfContentItems);
            List<CONTENT_ITEM> FreeformContentItems = MyContentItems.FindAll(FreeForm);
            if (FreeformContentItems.Count > 0)
                Numberoffreeforms = ProcessFreeForms(article_uid, FreeformContentItems);


        } //UpdateRelatedContent


        static string scrub(string source)
        {
            Regex rgx = new Regex(@"[^\u0009\u000a\u000d\u0020-\uD7FF\uE000-\uFFFD]");
            return rgx.Replace(source, "");
        }

            static string GetArticleCategory(string siteid, string article_uid)
        {
            List<CONTENT_GROUP> MyContentGroups = new List<CONTENT_GROUP>();
            Predicate<CONTENT_GROUP> findcategory = (CONTENT_GROUP p) => { return p.CONTENT_GROUP_TYPE_UCODE.Equals("CATEGORY"); };

            List<CONTENT_ITEM_CONTENT_GROUP> ContentItemContentGroups = BusinessModel.GetContentItemContentGroups(article_uid, siteid);
            foreach ( CONTENT_ITEM_CONTENT_GROUP ContentItemContentGroup in ContentItemContentGroups) {
                List<CONTENT_GROUP> ContentGroup = BusinessModel.GetContentGroup(ContentItemContentGroup.CONTENT_GROUP_UID);
                    MyContentGroups.AddRange(ContentGroup);
            }
            List<CONTENT_GROUP> categoryItems = MyContentGroups.FindAll(findcategory);
            string groupname ="";
            if (categoryItems.Count > 0)
                groupname = (categoryItems[0].GROUP_NAME == null) ? "NEWS" : categoryItems[0].GROUP_NAME.Trim();
            if ( string.IsNullOrEmpty(groupname) )
                groupname ="NEWS";
            return groupname;
        }

       static List<BusinessModel.CONTENT> GetRelatedArticles(string uid, string siteid)
        {

            List<BusinessModel.CONTENT> content = new List<BusinessModel.CONTENT>();
            try
            {
                content= BusinessModel.GetRelatedArticle(uid,siteid);
            }
            catch (System.Exception e)
            {
            }
            return content;
        }



        static string GetOriginalcontentSiteId(string uid)
        {
            string siteuid = "";
            List<CONTENT_ITEM> contentItems = BusinessModel.GetContentItemsAll(uid);
            if (contentItems.Count == 1)
                siteuid = contentItems[0].SITE_UID;
            return siteuid;

        }


        static void UpdateArticle(string siteid, string article_uid, CONTENT_ITEM ContentItemRow, int NumberOfImages)
        {
            string origsite = "";
            string errmsg = "";
            DateTime? StartDate = (ContentItemRow.START_DATE == null) ? DateTime.Now : ContentItemRow.START_DATE;
            DateTime? EndDate = (ContentItemRow.END_DATE == null) ? DateTime.Now.AddYears(100) : ContentItemRow.END_DATE;
            string keyword = (ContentItemRow.KEYWORD == null) ? "" : ContentItemRow.KEYWORD;
            string seodescription = scrub(ContentItemRow.SEO_DESCRIPTIVE_TEXT == null ? "" : ContentItemRow.SEO_DESCRIPTIVE_TEXT);
            string heading = scrub(ContentItemRow.CONTENT_TITLE == null ? "" : ContentItemRow.CONTENT_TITLE);
            string category = GetArticleCategory(siteid, article_uid);
            List<BusinessModel.CONTENT> content = GetRelatedArticles(ContentItemRow.CONTENT_ITEM_UID, siteid);
            string sRelatedArticles = "";
            BusinessModel.CONTENT lastitem = null;
            if (content.Count > 0)
                lastitem = content[content.Count - 1];
            string comma = ",";
            foreach (BusinessModel.CONTENT c in content)
            {
                if ( c.Equals(lastitem))
                    comma ="";
                    sRelatedArticles += string.Format("{0}{1}", c.uid,comma);
            }
            string origContentItemUid = (ContentItemRow.ORIG_CONTENT_ITEM_UID == null) ? "" : ContentItemRow.ORIG_CONTENT_ITEM_UID;
            string SectionAnchorUID = (ContentItemRow.SECTION_ANCHOR_UID == null) ? "0" : ContentItemRow.SECTION_ANCHOR_UID;

            if (origContentItemUid.Length > 0)
                origsite = GetOriginalcontentSiteId(origContentItemUid);  // shared content save original Site UID

            List<ARTICLE> Article = BusinessModel.GetArticle(article_uid);
            if (Article.Count != 1)
                return;
            ARTICLE ArticleRow = Article[0];
            string summary = scrub((ArticleRow.ABSTRACT == null ) ? "" : ArticleRow.ABSTRACT);
            string byline = scrub((ArticleRow.BYLINE == null ) ? "" : ArticleRow.BYLINE);
            string body = scrub((ArticleRow.BODY == null ) ? "" : ArticleRow.BODY);
            string subtitle = scrub((ArticleRow.SUBTITLE == null ) ? "" : ArticleRow.SUBTITLE);
            Dbcommon.spExecute("LoadArticle", ref  errmsg, "@siteid", siteid, "@article_uid", article_uid, "@category", category, "@anchor", SectionAnchorUID, "@startdate", StartDate, "@enddate", EndDate, "@heading", heading, "@body", body, "@byline", byline, "@subtitle", subtitle, "@summary", summary, "@RelatedArticles", sRelatedArticles, "@seodescription", seodescription, "@keyword", keyword, "@imagecount", NumberOfImages, "@origsite", origsite);
            if (errmsg.Length > 0)
                failedArticles += 1;
        }



    protected void Page_Load(object sender, EventArgs e)
    {
        int articleCount = 0;
        int skip = 0;
        Response.BufferOutput = false;
        string str_startdate = Request["startDate"];
        string str_stopDate = Request["stopDate"];
        string siteid = Request["siteid"];
        DateTime startdate = DateTime.ParseExact(str_startdate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        DateTime stopDate = DateTime.ParseExact(str_stopDate, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

        List<CONTENT_ITEM> ContentItemRows = BusinessModel.GetContentItems(siteid, (int)BusinessModel.asset.article, startdate, stopDate);
        int total = ContentItemRows.Count;
        Response.Write(string.Format("Total Rows: {0}", total));
        foreach (CONTENT_ITEM ContentItemRow in ContentItemRows)
        {
            string article_uid = ContentItemRow.CONTENT_ITEM_UID;
            int NumberOfImages = 0;
            bool articleExists = BusinessRule.BusinessRule.DoesArticleExist(article_uid);
            if (!articleExists)
            {
                List<CONTENT_ITEM_REL> RelatedContentitems = BusinessModel.GetRelatedContentitems(article_uid);
                if (RelatedContentitems.Count > 0)
                {
                    UpdateRelatedContent(article_uid, RelatedContentitems, ref NumberOfImages);
                } // RelatedContentitems
                UpdateArticle(siteid, article_uid, ContentItemRow, NumberOfImages);
            }
            else
            {
                skip += 1;
            }
            articleCount += 1;
            if ((articleCount % 100) == 0)
                Response.Write(string.Format("{0}/{1} already existed: {2} skipped <br/>", articleCount, total, skip));
        }// for each
        Response.Write(string.Format("Migration siteid: {2}  start: {0} stop: {1} From Qa0 complete <br/>", startdate, stopDate, siteid));
    } // Page Load
}