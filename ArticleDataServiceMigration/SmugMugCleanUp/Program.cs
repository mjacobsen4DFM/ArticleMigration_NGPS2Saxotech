using System;
using System.Collections.Generic;
using System.Text;
using SmugMugModel;
using System.Configuration;
using System.Net;
using System.Data;
using System.IO;

namespace SmugMugCleanUp
{
    class Program
    {
        static MyUser user = null;
        static DbCommon.DataMineDb DataMine = new DbCommon.DataMineDb();


        static void Login()
        {
            Site mySite = new Site();
            Site.Proxy = WebRequest.DefaultWebProxy;
            String userName = ConfigurationManager.AppSettings["username"];
            String password = ConfigurationManager.AppSettings["password"];
            user = mySite.Login(userName, password);
        }

        static void deleteSmugImage(string article_uid, string albumid, string albumkey, string image_uid, string imagename)
        {
            string errmsg = "";
            
            List<SmugMugModel.Image> Imagelist = user.GetImages(false, "", true, 0, false, "", albumid, albumkey);
            foreach (Image im in Imagelist)
            {
                if (Convert.ToInt64(image_uid) == im.id)
                {
                    user.DeleteImage(im.id);
                    DataMine.spExecute("UpdateSmugGalleryTrackerTS", ref errmsg, "@article_uid", article_uid, "@image_uid",image_uid);
                }
            } // foreach 
        } // deleteSmugImage

        
        static void Main(string[] args)
        {
            Login();
            string query = string.Format("select g.albumid, g.albumkey, t.article_uid, t.image_uid , t.imagename from smug_gallery_tracker t , smug_gallery g where t.tsDeleted is null and  t.article_uid = g.article_uid " );
            DataSet ds = DataMine.GetDs(query);
            
            int count = ds.Tables[0].Rows.Count;
            Console.WriteLine(string.Format("Total: {0}", count));
            int idx = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                idx += 1;
                Console.WriteLine(string.Format("row: {0}  articleuid: {1} imageid: {2}", idx, dr["article_uid"].ToString(), dr["image_uid"].ToString()));

                deleteSmugImage(dr["article_uid"].ToString(), dr["albumid"].ToString(), dr["albumkey"].ToString(), dr["image_uid"].ToString(), dr["imagename"].ToString());
     

            } // foreach
        }
    }
}
