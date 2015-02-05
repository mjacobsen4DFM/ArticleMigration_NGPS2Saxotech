using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModels;

namespace LoadSectionAnchorMap
{
    class Program
    {

        static void Main(string[] args)
        {
            Int16 mysiteid = Convert.ToInt16(args[0]);
            string smysiteid = mysiteid.ToString();
            livee_11Entities le = new livee_11Entities();
            var res = (from si in le.SECTION_ITEM
                      join ci in le.CONTENT_ITEM
                      on si.SECTION_ITEM_UID equals ci.SECTION_ANCHOR_UID
                       where ci.SITE_UID == smysiteid && ci.CONTENT_TYPE_UID == 102 
                      select new
                          {
                              ci.SECTION_ANCHOR_UID,
                              si.SECTION_NAME
                          }).Distinct().OrderBy(doc => doc.SECTION_NAME);

            ArticleDataDbEntities adb = new ArticleDataDbEntities();

            // Removed , ts = DateTime.Now  field datatime2(7) is new to sqlserver 2008 express and not supported on older sqlserver implementations
            // 	[ts] [datetime2](7) NOT NULL,
            // ALTER TABLE [dbo].[sectionAnchorMap] ADD  CONSTRAINT [DF_sectionAnchorMap_ts]  DEFAULT (sysdatetime()) FOR [ts]

            foreach (var v in res)
            {
                Console.WriteLine("{0}  {1} ", v.SECTION_ANCHOR_UID, v.SECTION_NAME);

                sectionAnchorMap SAM = new sectionAnchorMap { sectionAnchor = v.SECTION_ANCHOR_UID, siteid = mysiteid, sectionName = v.SECTION_NAME };
                adb.AddTosectionAnchorMaps(SAM);
              
               
            } // foreach
            adb.SaveChanges();

        } // End Main
    } // class program
} // End Namespace
