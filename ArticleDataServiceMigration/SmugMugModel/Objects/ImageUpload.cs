using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
//using Newtonsoft.Json;

namespace SmugMugModel
{
    public class UploadEventArgs : EventArgs
    {
        public float PercentComplete { get; internal set; }
        /// <summary>
        /// The filename of the image (or video).
        /// This header overrides whatever is set as the filename in the PUT endpoint.
        /// </summary>
        public string FileName { get; set; }
    }

    public  class ImageUpload
    {
        #region Properties
        /// <summary>
        /// A valid session [required]
        /// </summary>
        private string SessionID;
        /// <summary>
        /// The id of the album to upload the photo (or video) to [required]
        /// </summary>
        private long AlbumID;
        /// <summary>
        /// The altitude at which the image (or video) was taken.
        /// </summary>
        public int? Altitude { get; set; }
        /// <summary>
        /// The size of the image (or video) in bytes
        /// </summary>
        private int ByteCount;
        /// <summary>
        /// The caption for the image (or video).
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// The base64 encoded image data.
        /// </summary>
        private Base64FormattingOptions Data;
        /// <summary>
        /// The id of the image to replace.
        /// </summary>
        private int? ImageID;
        /// <summary>
        /// Hide the image (or video). Default: false
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// The keyword string for the image (or video).
        /// </summary>
        public string Keywords { get; set; }
        /// <summary>
        /// The latitude at which the image (or video) was taken.
        /// </summary>
        public float? Latitude { get; set; }
        /// <summary>
        /// The longitude at which the image (or video) was taken.
        /// </summary>
        public float? Longitude { get; set; }
        /// <summary>
        /// The size of the image (or video) in bytes.
        ///This header allows error detection for incomplete uploads.
        /// </summary>
        private int Length;
        /// <summary>
        /// Return a more human friendly response. Default: false
        /// </summary>
        public bool Pretty { get; set; }
        /// <summary>
        /// The response type. Values: JSON, PHP, REST (default), XML-RPC
        /// </summary>
        private string ResponseType = "JSON";
        /// <summary>
        /// The API version [required]
        /// </summary>
        private string Version = "1.2.2";

        private int chunkSize = 1024 * 32; //the default chunksize for reading from the file
        #endregion


        public event EventHandler<UploadEventArgs> UploadCompleted;
        public event EventHandler<UploadEventArgs> UploadStarted;
        public event EventHandler<UploadEventArgs> UploadProgress;



        /// <summary>
        /// Converts the MD5 sum from a byte array to a string
        /// </summary>
        /// <param name="arr">The byte array</param>
        /// <returns></returns>
        private static string GetStringFromHash(byte[] arr)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte item in arr)
            {
                var first = item >> 4;
                var second = (item & 0x0F);

                s.AppendFormat("{0:X}{1:X}", first, second);
            }

            return s.ToString().ToLower();
        }

        /// <summary>
        /// This is the constructor for the ImageUpload object. It is private so you have to use the CreateUploader method
        /// </summary>
        /// <param name="SessionID">The session ID</param>
        /// <param name="AlbumID">The album ID</param>
        internal ImageUpload(string SessionID, long AlbumID)
        {
            this.SessionID = SessionID;
            this.AlbumID = AlbumID;
        }

        /// <summary>
        /// Uploads a file
        /// </summary>
        /// <param name="fileName">The filename we want to upload</param>
        /// <returns>An Image object that describes the image we uploaded</returns>
        public Image UploadImage(string fileName)
        {
            var byteArr = File.ReadAllBytes(fileName);
            var MD5Sum = GetStringFromHash(System.Security.Cryptography.MD5.Create().ComputeHash(byteArr));

            return UploadImage(fileName, MD5Sum);
        }




        // Peter Bruni special patch for byteArray 
        public Image UploadImage(string filename,string mycaption, byte[] byteArray)
        {
            // var myFileInfo = new FileInfo(filename);
//            var MD5Sum = GetStringFromHash(System.Security.Cryptography.MD5.Create().ComputeHash(byteArr));

            string MD5Checksum = GetStringFromHash(System.Security.Cryptography.MD5.Create().ComputeHash(byteArray));

            var myWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://upload.smugmug.com/{0}", filename));
            myWebRequest.UserAgent = "YASMAPI v1.0";
            myWebRequest.ContentType = "binary/octet-stream";
            myWebRequest.ContentLength = byteArray.Length;
            myWebRequest.Method = WebRequestMethods.Http.Put;

            myWebRequest.Headers.Add("X-Smug-SessionID", SessionID);
            myWebRequest.Headers.Add("X-Smug-Version", Version);
            myWebRequest.Headers.Add("X-Smug-ResponseType", ResponseType);
            myWebRequest.Headers.Add("X-Smug-AlbumID", AlbumID.ToString());
            myWebRequest.Headers.Add("Content-MD5", MD5Checksum);
            myWebRequest.Headers.Add("X-Smug-FileName", filename);

            if (Altitude != null)
                myWebRequest.Headers.Add("X-Smug-Altitude", Altitude.ToString());
            if (mycaption != null)
                myWebRequest.Headers.Add("X-Smug-Caption", mycaption);
            if (Hidden == true)
                myWebRequest.Headers.Add("X-Smug-Hidden", "true");
            if (ImageID != null)
                myWebRequest.Headers.Add("X-Smug-ImageID", ImageID.ToString());
            if (Keywords != null)
                myWebRequest.Headers.Add("X-Smug-Keywords", Keywords);
            if (Latitude != null)
                myWebRequest.Headers.Add("X-Smug-Latitude", Latitude.ToString());
            if (Longitude != null)
                myWebRequest.Headers.Add("X-Smug-Longitude", Longitude.ToString());
            if (Pretty == true)
                myWebRequest.Headers.Add("X-Smug-Pretty", "true");


            int timeOut = ((int)byteArray.Length / 1024) * 1000;
            myWebRequest.Timeout = timeOut;
            var stream = myWebRequest.GetRequestStream();
            stream.Write(byteArray, 0, byteArray.Length);
            stream.Close();

            var resp = myWebRequest.GetResponse();
            string rez = string.Empty;
            using (StreamReader ns = new StreamReader(resp.GetResponseStream()))
            {
                rez = ns.ReadToEnd();
            }

            ////we deserialize the image
            //JsonSerializer js = new JsonSerializer();
            //var response = (ImageResponse)js.Deserialize(new JsonTextReader(new StringReader(rez)), typeof(ImageResponse));

            var response = JSONDotNET.Deserializer.Deserialize<ImageResponse>(rez);

            if (response.stat == "ok")
                return response.Image;
            else
                throw new SmugMugException(response.code, response.message, response.method);
        }



        /// <summary>
        /// Uplads a file
        /// </summary>
        /// <param name="filename">The filename we want to upload</param>
        /// <param name="MD5Checksum">The MD5 checksum of a file</param>
        /// <returns>An Image object that describes the image we uploaded</returns>
        public Image UploadImage(string filename, string MD5Checksum)
        {
            var myFileInfo = new FileInfo(filename);
            var myWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://upload.smugmug.com/{0}", filename));
            myWebRequest.UserAgent = "YASMAPI v1.0";
            myWebRequest.ContentType = "binary/octet-stream";
            myWebRequest.ContentLength = myFileInfo.Length;
            myWebRequest.Method = WebRequestMethods.Http.Put;

            myWebRequest.Headers.Add("X-Smug-SessionID", SessionID);
            myWebRequest.Headers.Add("X-Smug-Version", Version);
            myWebRequest.Headers.Add("X-Smug-ResponseType", ResponseType);
            myWebRequest.Headers.Add("X-Smug-AlbumID", AlbumID.ToString());
            myWebRequest.Headers.Add("Content-MD5", MD5Checksum);
            myWebRequest.Headers.Add("X-Smug-FileName", myFileInfo.Name);

            if (Altitude != null)
                myWebRequest.Headers.Add("X-Smug-Altitude", Altitude.ToString());
            if (Caption != null)
                myWebRequest.Headers.Add("X-Smug-Caption", Caption);
            if (Hidden == true)
                myWebRequest.Headers.Add("X-Smug-Hidden", "true");
            if (ImageID != null)
                myWebRequest.Headers.Add("X-Smug-ImageID", ImageID.ToString());
            if (Keywords != null)
                myWebRequest.Headers.Add("X-Smug-Keywords", Keywords);
            if (Latitude != null)
                myWebRequest.Headers.Add("X-Smug-Latitude", Latitude.ToString());
            if (Longitude != null)
                myWebRequest.Headers.Add("X-Smug-Longitude", Longitude.ToString());
            if (Pretty == true)
                myWebRequest.Headers.Add("X-Smug-Pretty", "true");            

            //we start reading from the file...

            //we have some elements to set
            //- request time out (compute this for 10 kb/sec speed)
            //- the chunk size to use when uploading (how much data to report after)
            if (UploadStarted != null)
                UploadStarted(this, new UploadEventArgs() { FileName = myFileInfo.FullName, PercentComplete = 0 });

            int timeOut = ((int)myFileInfo.Length / 1024) * 1000;
            myWebRequest.Timeout = timeOut;


            long howMuchRead = 0;
            byte[] buffer = new byte[chunkSize];
            int readSoFar = 0;

            try
            {
                using (FileStream sr = new FileStream(myFileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var stream = myWebRequest.GetRequestStream())
                    {
                        while (howMuchRead < myFileInfo.Length)
                        {
                            //we try to read a chunk from the file
                            readSoFar = sr.Read(buffer, 0, chunkSize);
                            howMuchRead += readSoFar;


                            //we now write those files to the web.
                            stream.Write(buffer, 0, readSoFar);
                            //System.Convert.ToBase64String(buffer);

                            if (UploadProgress != null)
                                UploadProgress(this, new UploadEventArgs() { FileName = myFileInfo.FullName, PercentComplete = (float)howMuchRead / (float)myFileInfo.Length });
                        }
                    }
                }
            }
            catch (Exception e)
            {

                //an error has occured...
                throw e;
            }

            var resp = myWebRequest.GetResponse();
            string rez = string.Empty;
            using (StreamReader ns = new StreamReader(resp.GetResponseStream()))
            {
                rez = ns.ReadToEnd();
            }

            if (UploadCompleted != null)
                UploadCompleted(this, new UploadEventArgs() { FileName = myFileInfo.FullName, PercentComplete = (float)howMuchRead / (float)myFileInfo.Length });

            ////we deserialize the image
            //JsonSerializer js = new JsonSerializer();
            //var response = (ImageResponse)js.Deserialize(new JsonTextReader(new StringReader(rez)), typeof(ImageResponse));

            var response = JSONDotNET.Deserializer.Deserialize<ImageResponse>(rez);

            if (response.stat == "ok")
                return response.Image;
            else
                throw new SmugMugException(response.code, response.message, response.method);
        }

    }
}
