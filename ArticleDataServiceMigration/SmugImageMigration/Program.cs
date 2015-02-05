using System;
using System.Collections.Generic;
using System.Text;
using SmugMugModel;
using System.Configuration;
using System.Net;
using System.Data;
using System.IO;

namespace SmugImageMigration
{
    class Program
    {
        static MyUser user = null;
        static OWC.rest rc = new OWC.rest();
        static List<Album> mAlbums;

        static DbCommon.DataMineDb DataMine = new DbCommon.DataMineDb();
        static string LogFileName = "";

        //static void img_UploadProgress(object sender, UploadEventArgs e)
        //{
        //    Console.WriteLine("{1} - {0,5:N}", e.PercentComplete * 100, e.FileName);
        //}

        //static void img_UploadCompleted(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Upload complete");
        //}

        static public bool ThumbnailCallback()
        {
            return true;
        }

        static  float DeterminePercentageForResize(int thumbSize ,int height, int width)
        {
            int highestValue;
            if (height > width)
                highestValue = height;
            else
                highestValue = width;
            float percent = (float)thumbSize / (float)highestValue;
            if (percent > 1)
                percent = 1;
            return percent;
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



        static void Login()
        {
            Site mySite = new Site();
            Site.Proxy = WebRequest.DefaultWebProxy;
            String userName = ConfigurationManager.AppSettings["username"];
            String password = ConfigurationManager.AppSettings["password"];
            user = mySite.Login(userName, password);
        }

        static void DeleteAll()
        {
            List<Album> myAlbums = user.GetAlbums();
            foreach (Album a in myAlbums)
            {
                a.Delete();
            }
        }

        static int FindAlbum(string article_uid)
        {
            string query = string.Format("select * from smug_gallery where article_uid = '{0}'", article_uid);
            DataSet ds = DataMine.GetDs(query);
            return ds.Tables[0].Rows.Count;
        }

        static Album FindSmugMugAlbum(string myTitle)
        {
            Album mAlbum = mAlbums.Find(Album => Album.Title == myTitle);
            return mAlbum;
        }

        static void  SmgMugAlbums()
        {
            Console.WriteLine("Start Load Album Array {0}", DateTime.Now);
            mAlbums = user.GetAlbums();
            Console.WriteLine("Complete Load Album Array {0}", DateTime.Now);
            //foreach (Album mAlbum in mAlbums)
            //{
            //    Console.WriteLine("NiceName: {0} key: {1} id: {2} ", mAlbum.Title, mAlbum.Key, mAlbum.id);
            //}

        }

        static string RemoveControlCharactersAndExtended(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];

                if (!char.IsControl(ch) && ch < 128 )
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();
        } // RemoveControlCharacters


        static bool CreateAlbum(int idx, string article_uid, string category, DataSet dsImage)
        {
            string message = "";
            bool sent = true;
            Album myAlbum = null;
            try
            {
                Category myCategory = null;
                myCategory = user.FindCategory(category);
                if (myCategory == null)
                    myCategory = user.CreateCategory(category);
                myAlbum = myCategory.CreateAlbum(article_uid);
                var uploader = myAlbum.CreateUploader();

                foreach (DataRow dr in dsImage.Tables[0].Rows)
                {
                    string sourceurl = dr[0].ToString();
                    string caption = dr[1].ToString();
                    int bytecount = Convert.ToInt32(dr[2]);
                    caption = RemoveControlCharactersAndExtended(caption);
                    Uri uri = new Uri(sourceurl);
                    byte[] byteArray = rc.getFile(uri, ref  message);
                    if (byteArray != null)
                    {
                        //if (byteArray.Length > 1024 * 50)
                        //{

                        //    Stream stream = new MemoryStream(byteArray);
                        //    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                        //    int thumbSize = 300;
                        //    float percent = DeterminePercentageForResize(thumbSize, image.Width, image.Height);
                        //    int iw = Convert.ToInt32((float)(image.Width * percent));
                        //    int ih = Convert.ToInt32((float)(image.Height * percent));

                        //    System.Drawing.Image thumbnailImage = image.GetThumbnailImage(iw, ih, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                        //    MemoryStream imageStream = new MemoryStream();
                        //    thumbnailImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //    byte[] imageContent = new Byte[imageStream.Length];
                        //    imageStream.Position = 0;
                        //    imageStream.Read(imageContent, 0, (int)imageStream.Length);
                        //    stream.Close();
                        //    stream.Dispose();
                        //    Console.WriteLine("Start Smug {0} {1} {2} ", DateTime.Now, Path.GetFileName(uri.LocalPath), imageStream.Length);
                        //    imageStream.Close();
                        //    imageStream.Dispose();
                        //    uploader.UploadImage(Path.GetFileName(uri.LocalPath), caption, imageContent);
                        //    Console.WriteLine("Complete Smug {0} {1}  ", DateTime.Now, Path.GetFileName(uri.LocalPath));
                        //}
                        //else
                        //{
                            Console.WriteLine("Start Smug {0} {1} {2} ", DateTime.Now, Path.GetFileName(uri.LocalPath), byteArray.Length);
                            uploader.UploadImage(Path.GetFileName(uri.LocalPath), caption, byteArray);
                            Console.WriteLine("Complete Smug {0} {1}  ", DateTime.Now, Path.GetFileName(uri.LocalPath));

                       // }
                    }




                    // myAlbum.UploadImageFromURL(sourceurl, 0, bytecount, caption, "", "", 0, 0, "");
                } // foreach


                Album myAlbumInfo = myAlbum.GetInfo();
                int albumId = (int)myAlbumInfo.id;
                string albumKey = myAlbumInfo.Key;
                DataMine.spExecute("LoadSmugGallery", ref message, "@article_uid", article_uid, "@AlbumId", albumId, "@AlbumKey", albumKey);
                if (message.Length > 0)
                {
                    writetolog(string.Format("article uid: {0} {1}", article_uid, message));
                }
                else
                    Console.WriteLine("{0} row: {1} article: {2} id: {3} key: {4}", DateTime.Now, idx, article_uid, albumId, albumKey);
            }
            catch (SmugMugException sme)
            {
                sent = false;
                Console.WriteLine("Smug mug exception {0} {1} ", sme.Message, article_uid);
            }
            catch (WebException ex)
            {
                sent = false;
                Console.WriteLine("smug mug webexception {0}", ex.Message, article_uid);
            }
            catch (Exception ex2) {
                sent = false;
                Console.WriteLine("exception {0}", ex2.Message );
            
            }
            finally
            {
                if (sent == false && myAlbum != null)
                {
                    myAlbum.Delete();
                }
            }
            return sent;
            }


        static void Main(string[] args)
        {
            string message = "";
            Login();
           // DataMine.spExecute("DeleteSmugGallery", ref message);
           // DeleteAll();
            SmgMugAlbums();
            string siteid = args[0];
            string destination_siteid = args[1];
            LogFileName = args[2];

            string query = string.Format("select a.article_uid, a.category , s.similarity from article a, saxo_gallery s where s.destination_siteid = '{0}' and  a.article_uid = s.article_uid and  a.imagecount > 1   and a.siteid= '{1}' order by a.startdate", destination_siteid, siteid);
            DataSet ds = DataMine.GetDs(query);
            int count = ds.Tables[0].Rows.Count;
            int idx = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string article_uid = dr[0].ToString();
                string category = dr[1].ToString();
                string similarity = dr[2].ToString();
                float fsimilarity = 0;
                try
                {
                    fsimilarity = float.Parse(similarity);
                }
                catch (Exception e) { }

                if (FindAlbum(article_uid) == 0 ) // && fsimilarity < .75) // Album not found 
                {

                    Album smugAlbum = FindSmugMugAlbum(article_uid);
                    if (smugAlbum != null)
                        smugAlbum.Delete();
                    query = string.Format("select imagepath, caption, filesize from image where asset_uid in ( select asset_uid from asset where article_uid = '{0}' and asset_type = 108)", article_uid);
                    DataSet dsImage = DataMine.GetDs(query);
                    idx += 1;
                    bool sent = false;
                    int retry = 0;
                    while (sent == false && retry < 3)
                    {
                        retry += 1;
                        sent = CreateAlbum(idx, article_uid, category, dsImage);
                    }// end while
                }
            }


        }
    }
}
