using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModels;


// must add reference to system.data.entity
namespace ArticleDataServiceMigration
{
    class BusModel
    {
         enum  asset
        {
            article = 102,
            image = 108,
            freeform = 111,
            pdf = 104
        }

        static private List<CONTENT_ITEM_REL> ListRelatedContentItems(string uid)
        {
            using (livee_11Entities database = new livee_11Entities())
            {
                var RelatedContentItems = from CONTENT_ITEM_REL in database.CONTENT_ITEM_REL
                                     where CONTENT_ITEM_REL.CONTENT_ITEM_UID.Equals(uid)
                                     select CONTENT_ITEM_REL;

                return RelatedContentItems.ToList();
            }
        }// ListRelatedContent

        static private List<CONTENT_ITEM> ListContentItems(string uid)
        {
            using (livee_11Entities database = new livee_11Entities())
            {
                var ContentItems = from CONTENT_ITEM in database.CONTENT_ITEM
                                   where CONTENT_ITEM.CONTENT_ITEM_UID.Equals(uid) && CONTENT_ITEM.STATE_UCODE.Equals("PRODDEPLOY")
                                   select CONTENT_ITEM;

                return ContentItems.ToList();
            }
        }// ListRelatedContent


        static private List<FREEFORM> ListFreeForms(string uid)
        {
            using (livee_11Entities database = new livee_11Entities())
            {
                var FreeformItems = from FREEFORM in database.FREEFORMs
                                   where FREEFORM.CONTENT_ITEM_UID.Equals(uid)
                                   select FREEFORM;

                return FreeformItems.ToList();
            }
        }// ListRelatedContent

        static private List<PDF_VIEW> ListPdfs(string uid)
        {
            using (livee_11Entities database = new livee_11Entities())
            {
                var pdfItems = from PDF_VIEW in database.PDF_VIEW
                               where PDF_VIEW.CONTENT_ITEM_UID.Equals(uid)
                               select PDF_VIEW;

                return pdfItems.ToList();
            }
        }// ListPdfs


        static private List<CONTENT_ITEM> GetRelatedContentitems(string article_uid, int type_uid)
        {
            List<CONTENT_ITEM> ciList = new List<CONTENT_ITEM>();
            List<CONTENT_ITEM_REL> RelatedContentItems = ListRelatedContentItems(article_uid);
            foreach (CONTENT_ITEM_REL rci in RelatedContentItems)
            {
                string uid = rci.RELATED_CONTENT_ITEM_UID;
                List<CONTENT_ITEM> ContentItems = ListContentItems(uid);
                foreach (CONTENT_ITEM ci in ContentItems)
                {
                    if (ci.CONTENT_TYPE_UID.Equals(type_uid))
                    {
                        ciList.Add(ci);
                    } // end if
                }

            }
            return ciList.ToList();

        } //  GetRelatedContentitems



        static public List<FREEFORM> GetFreeforms(string article_uid)
        {
            List<FREEFORM> ffList = new List<FREEFORM>();
            List<CONTENT_ITEM> ContentItems = GetRelatedContentitems(article_uid, (int)asset.freeform);
            foreach (CONTENT_ITEM ci in ContentItems)
            {
                string uid = ci.CONTENT_ITEM_UID;
                List<FREEFORM> forms = ListFreeForms(uid);
                foreach (FREEFORM form in forms)
                {
                    form.FREEFORM_HTML = (form.FREEFORM_HTML == null) ? "" : form.FREEFORM_HTML;
                    ffList.Add(form);
                }
            }// end for each
            return ffList.ToList();
        } // GetFreeforms


        static public List<PDF_VIEW> GetPdf(string article_uid)
        {
            List<PDF_VIEW> pdfList = new List<PDF_VIEW>();
            List<CONTENT_ITEM> ContentItems = GetRelatedContentitems(article_uid, (int)asset.pdf);
            foreach (CONTENT_ITEM ci in ContentItems)
            {
                string uid = ci.CONTENT_ITEM_UID;
                List<PDF_VIEW> pdfs = ListPdfs(uid);
                foreach (PDF_VIEW pdf in pdfs)
                {
                    pdf.CAPTION =(pdf.CAPTION == null) ? "" : pdf.CAPTION;
                    pdf.BINARY_URL = (pdf.BINARY_URL== null) ? "" : pdf.BINARY_URL;
                    pdfList.Add(pdf);
                }
            }// end for each
            return pdfList.ToList();
        } // GetFreeforms




        static public List<CONTENT_ITEM> GetContentItems(string siteid, int content_type, DateTime startdate, DateTime enddate)
        {

//          SELECT CONTENT_ITEM_UID, VIEWABLE_TYPE_UCODE, SCHEDULE_UID, SITE_UID, SHARE_GLOBAL_FLAG, SHAREABLE_FLAG, EXPORTABLE_FLAG, COPYABLE_FLAG, STATE_UCODE, STATE_SET_DATE, KEYWORD, SOURCE_TYPE_UCODE, ORIG_CONTENT_ITEM_UID, CONTENT_TYPE_UID, START_DATE, END_DATE, UPDATE_DATE, UPDATE_PERSON_UID, CREATE_DATE, CREATE_PERSON_UID, CONTENT_TITLE, CONTENT_DESC, VIGNETTE_CONTENT_GUID, CPS_UID, VERSION, LAUNCHED_DATE, DISPLAY_DATE, SHARE_ASSOC_MEDIA_FLAG, DEPLOYED_FLAG, EXPORTED_DATE, EXPORTED_STATUS, NO_THUMBNAIL, KEEP_INDEFINITE_FLAG, THIRDPARTY_ID, THIRDPARTY_VERSION, THIRDPARTY_PUBLISHING_STATUS, FIRST_PUBLICATION_TIMESTAMP, ORIGINATING_SOURCE, ICON_TYPE_UCODE, SHOW_MAP_FLAG, THIRDPARTY_PROVIDER_ID, PREMIUM_START_DATE, CONTENT_LICENSE_UID, SECTION_ANCHOR_UID, SEO_DESCRIPTIVE_TEXT, MANUAL_SEO_DESCRIPTIVE_TEXT_FLAG FROM NGPS.CONTENT_ITEM where site_uid = ? and content_type_uid = ? and start_date >= ?  and start_date <= ? and ( state_ucode = 'PRODDEPLOY') order by start_date

            using (livee_11Entities database = new livee_11Entities())
            {
                var ContentItems = from CONTENT_ITEM in database.CONTENT_ITEM
                                   where CONTENT_ITEM.SITE_UID.Equals(siteid) && CONTENT_ITEM.CONTENT_TYPE_UID.Equals(content_type) && CONTENT_ITEM.START_DATE >= startdate && CONTENT_ITEM.START_DATE <= enddate && CONTENT_ITEM.STATE_UCODE.Equals("PRODDEPLOY")
                                   orderby CONTENT_ITEM.START_DATE ascending
                                   select CONTENT_ITEM;

                return ContentItems.ToList();
            }
        }

    }
}
