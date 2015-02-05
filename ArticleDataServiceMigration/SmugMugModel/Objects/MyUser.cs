using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmugMugModel
{
    public class MyUser : SmugMugObject
    {
        #region Properties
        /// <summary>
        /// The account status for this user. Values: Active, Expired
        /// </summary>
        public string AccountStatus { get; internal set; }
        /// <summary>
        /// The account type for this user. Values: Pro, Power, Standard
        /// </summary>
        public string AccountType { get; internal set; }
        /// <summary>
        /// The file size limit for this user's account.
        /// </summary>
        public int FileSizeLimit { get; internal set; }
        /// <summary>
        /// The password hash for this user.
        /// </summary>
        public string PasswordHash { get; internal set; }
        /// <summary>
        /// SmugVault is enabled for this user's account.
        /// </summary>
        public bool SmugVault { get; internal set; }
        /// <summary>
        /// The id for this user.
        /// </summary>
        public long UserId { get; internal set; }
        /// <summary>
        /// The DisplayName for this user.
        /// </summary>
        public string DisplayName { get; internal set; }
        /// <summary>
        /// The homepage URL for this user.
        /// </summary>
        public string URL { get; internal set; }
        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", UserId, basic.NickName, DisplayName);
        }

        /// <summary>
        /// This method destroys an existing session.
        /// </summary>
        /// <param name="u"></param>
        /// <returns>Returns an empty successful response, if it completes without error.</returns>
        public bool Logout()
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.logout", basic);
            if (resp.stat == "ok")
                return true;
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.message);
            }
        }

        #region User
        /// <summary>
        /// Updates the information for the current user
        /// </summary>
        public bool GetInfo()
        {
            return GetInfo("");
        }

        /// <summary>
        /// Updates the information for the current user
        /// </summary>
        /// <param name="Nickname">The NickName for a specific user</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        public bool GetInfo(string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Extras, Pretty, Sandboxed, Strict
            var resp = ch.ExecuteMethod<UserResponse>("smugmug.users.getInfo", basic, "NickName", basic.NickName, "Extras", Extras);
            if (resp.stat == "ok")
            {
                var returnedUser = resp.User;
                if (!string.IsNullOrEmpty(returnedUser.DisplayName))
                    this.DisplayName = returnedUser.DisplayName;
                if (!string.IsNullOrEmpty(returnedUser.URL))
                    this.DisplayName = returnedUser.URL;
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves the information for a user
        /// </summary>
        public User GetInfoForAnotherUser(string NickName)
        {
            return GetInfoForAnotherUser(NickName, "");
        }

        /// <summary>
        /// Retrieves the information for a user
        /// </summary>
        /// <param name="Nickname">The NickName for a specific user</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>User with DisplayName, NickName and URL</returns>
        public User GetInfoForAnotherUser(string NickName, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Extras, Pretty, Sandboxed, Strict
            var resp = ch.ExecuteMethod<UserResponse>("smugmug.users.getInfo", basic, "NickName", NickName, "Extras", Extras);
            if (resp.stat == "ok")
            {
                return resp.User;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves a hierarchical album tree for the user (standard response - Albums only have id, key and title)
        /// </summary>
        /// <returns>List of Categories with id, SubCategory array (id, Name), Albums array (id, Key, Title)</returns>
        public List<Category> GetTree()
        {
            return GetTree(false, true, "", false, 0, "", "");
        }

        /// <summary>
        ///  Retrieves a hierarchical album tree for the user (extended response - Albums have all properties)
        /// </summary>
        /// <param name="Heavy">If true, returns the extended response</param>
        /// <returns>List of Categories with id, SubCategory array (id, Name), Albums array (all properties)</returns>
        public List<Category> GetTree(bool Heavy)
        {
            return GetTree(false, true, "", Heavy, 0, "", "");
        }

        /// <summary>
        ///  Retrieves a hierarchical album tree for the user (extended response - Albums have all properties)
        /// </summary>
        /// <param name="Associative">boolean, returns an associative array. Default: false</param>
        /// <param name="Empty">boolean, Return empty albums, categories and subcategories. Default: true</param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response</param>
        /// <param name="Heavy">heavy response</param>
        /// <param name="LastUpdated">Return results where LastUpdated is after the epoch time provided</param>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <param name="ShareGroupTag">The tag (public id) for the sharegroup</param>
        /// <returns>List of Categories with id, SubCategory array (id, Name), Albums array (all properties)</returns>
        public List<Category> GetTree(bool Associative, bool Empty, string Extras, bool Heavy, int LastUpdated, string SitePassword, string ShareGroupTag)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Empty, Extras, Heavy, LastUpdated, NickName, Pretty, SitePassword, Strict, ShareGroupTag
            var resp = ch.ExecuteMethod<CategoryResponse>("smugmug.users.getTree", basic, "NickName", basic.NickName, "Heavy", Heavy, "Associative", Associative, "Empty", Empty, "Extras", Extras, "LastUpdated", LastUpdated, "SitePassword", SitePassword, "ShareGroupTag", ShareGroupTag);
            if (resp.stat == "ok")
            {
                var categoryList = new List<Category>();
                categoryList.AddRange(resp.Categories);
                if (categoryList != null)
                {
                    foreach (var myCategory in categoryList)
                    {
                        myCategory.basic = basic;
                        if (myCategory.Albums != null)
                        {
                            foreach (var myAlbum in myCategory.Albums)
                            {
                                myAlbum.basic = basic;
                            }
                        }
                        if (myCategory.SubCategories != null)
                        {
                            foreach (var mySubCategory in myCategory.SubCategories)
                            {
                                mySubCategory.basic = basic;
                                if (mySubCategory.Albums != null)
                                {
                                    foreach (var myAlbum in mySubCategory.Albums)
                                    {
                                        myAlbum.basic = basic;
                                    }
                                }
                            }
                        }
                    }
                }
                return categoryList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Gets the transfer statistics for the logged-in user during the given Month and Year. SmugMug only keeps the last few months of traffic on file, so requesting farther back then 2 months may not return valid results. A float is provided for Original and video sizes because it's possible to watch only a portion of a video. 
        /// </summary>
        /// <param name="Month">The month to retrieve statistics for</param>
        /// <param name="Year">The year to retrieve statistics for</param>
        /// <returns></returns>
        public bool GetStats(int Month, int Year)
        {
            return GetStats(Month, Year, false);
        }

        /// <summary>
        /// Gets the transfer statistics for the logged-in user during the given Month and Year. If Heavy is set to "1", transfer statistics for each image in each album will be returned as well
        /// </summary>
        /// <param name="Month">The month to retrieve statistics for</param>
        /// <param name="Year">The year to retrieve statistics for</param>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <returns></returns>
        public bool GetStats(int Month, int Year, bool Heavy)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Month [required], Year [required], Callback, Heavy, Pretty, Strict
            var resp = ch.ExecuteMethod<UserResponse>("smugmug.users.getStats", basic, "Month", Month, "Year", Year, "Heavy", Heavy);
            if (resp.stat == "ok")
            {
                PopulateWithResponse(resp.User, this);
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Album

        /// <summary>
        /// Creates a new album with the specified title in the "Other" category
        /// </summary>
        /// <param name="Title"></param>
        /// <returns>Album (id and Key)</returns>
        public Album CreateAlbum(string Title)
        {
            return CreateAlbum(Title, false);
        }

        /// <summary>
        /// Creates a new album with the specified title in the "Other" category
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Unique">Create an album if one of the same name doesn't already exist in the current hierarchy. Default: false  </param>
        /// <returns>Album (id and Key)</returns>
        public Album CreateAlbum(string Title, bool Unique)
        {
            return CreateAlbum(Title, Unique, "");
        }

        /// <summary>
        /// Creates a new album with the specified title in the "Other" category
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Unique">Create an album if one of the same name doesn't already exist in the current hierarchy. Default: false  </param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response.</param>
        /// <returns>Album (id and Key)</returns>
        public Album CreateAlbum(string Title, bool Unique, string Extras)
        {
            Album a = new Album();
            a.Title = Title;
            a.basic = basic;
            return a.Create(Unique, Extras);
        }

        /// <summary>
        /// Retrieves a list of albums for a given user (standard response - list of albums with id, Key, Category - id,Name; SubCategory - id,Name; Title)
        /// </summary>
        /// <returns></returns>
        public List<Album> GetAlbums()
        {
            return GetAlbums(false, true, "", false, 0, "");
        }

        /// <summary>
        /// Retrieves a list of albums for a given user (heavy response - list of albums with id, Key, Backprinting, CanRank, CategoryID and Name, Clean, ColorCorrection, Comments, CommunityID and Name, Description, EXIF, External, FamilyEdit, FriendEdit, Geography, Header, HideOwner, HighlightID and Key, ImageCoutn, Larges, Originals, Password, PasswordHint, Position, Printable, ProofDays, Protected, Public, Share, SmumugSearchable, SortDirection, SortMethod, SquareThumbs, SubCategoryID and Name, TemplateID, ThemeID and Name and Type, Title, Unique, UnsharpAmount, UnsharpRadius, UnsharpSigma, UnsharpThreshold, WatermarkID and Name, Watermarking, WorldSearchable, X2Larges, X3Larges, XLarges)
        /// </summary>
        /// <param name="Heavy">heavy response</param>
        /// <returns></returns>
        public List<Album> GetAlbums(bool Heavy)
        {
            return GetAlbums(false, true, "", Heavy, 0, "");
        }




        public bool DeleteImage(long id)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], ImageId [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<ImageResponse>("smugmug.images.delete", basic, "ImageID", id);
            if (resp.stat == "ok")
                return true;
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }


        /// <summary>
        /// Retrieves a list of images for a given album (Album with id, Key, ImageCount, and Image array with id, Key, Altitude, Caption, Date, FileName, Duration, Format, Heaight, Hidden, Keywords, LargeURL, LastUpdated, Latitude, Longitude, MD5Sum, MediumURL, OriginalURL, Position, Serial, Size, SmallURL, ThumbURL, TinyURL, Video320URL, Video640URL, Video960URL, Video1280URL, Video1920URL, Width, X2LardeURL, X3LargeURL, XLargeURL)
        /// </summary>
        /// <param name="Associative">boolean, returns an associative array. Default: false</param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response.</param>
        /// <param name="Heavy">heavy response. Default : false</param>
        /// <param name="LastUpdated">Return results where LastUpdated is after the epoch time provided</param>
        /// <param name="Sandboxed">Forces URLs to a location with a crossdomain.xml file. Default: false  </param>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns>List of Images</returns>
        /// false, "", Heavy, 0, false, "")
        /// Peter Bruni added this 12.12.2012
        public List<Image> GetImages(bool Associative, string Extras, bool Heavy, int LastUpdated, bool Sandboxed, string SitePassword,string myId, string myKey)
        {
            string Password = "";

            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], AlbumID [required], AlbumKey [required], Callback, Extras, Heavy, LastUpdated, Password, Pretty, Sandboxed (boolean. Forces URLs to a location with a crossdomain.xml file. Default: false), SitePassword, Strict
            ImageResponse resp = new ImageResponse();
            if ((Password != null) && (Password != String.Empty))
                resp = ch.ExecuteMethod<ImageResponse>("smugmug.images.get", basic, "AlbumID", myId, "AlbumKey", myKey, "Associative", Associative, "Extras", Extras, "Heavy", Heavy, "LastUpdated", LastUpdated, "Sandboxed", Sandboxed, "SitePassword", SitePassword, "Password", Password);
            else
                resp = ch.ExecuteMethod<ImageResponse>("smugmug.images.get", basic, "AlbumID", myId, "AlbumKey", myKey, "Associative", Associative, "Extras", Extras, "Heavy", Heavy, "LastUpdated", LastUpdated, "Sandboxed", Sandboxed, "SitePassword", SitePassword);

            if (resp.stat == "ok")
            {
                var imageList = new List<Image>();
                imageList.AddRange(resp.Album.Images);
                //this.ImageCount = resp.Album.ImageCount;
                //if (imageList != null)
                //{
                //    foreach (var item in imageList)
                //    {
                //        item.basic = basic;
                //        item.Album = this;
                //        item.jsonString = ch.GetJSONstring(); // 11.19.2012 Peter Bruni save original json String
                //    }
                //}
                return imageList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }






        /// <summary>
        /// Retrieves a list of albums for a given user (heavy response - list of albums with id, Key, Backprinting, BoutiquePackaging, CanRank, CategoryID and Name, Clean, ColorCorrection, Comments, CommunityID and Name, Description, EXIF, External, FamilyEdit, FriendEdit, Geography, Header, HideOwner, HighlightID and Key, ImageCoutn, Larges, Originals, Password, PasswordHint, Position, Printable, ProofDays, Protected, Public, Share, SmumugSearchable, SortDirection, SortMethod, SquareThumbs, SubCategoryID and Name, TemplateID, ThemeID and Name and Type, Title, Unique, UnsharpAmount, UnsharpRadius, UnsharpSigma, UnsharpThreshold, WatermarkID and Name, Watermarking, WorldSearchable, X2Larges, X3Larges, XLarges)
        /// </summary>
        /// <param name="Associative">boolean, returns an associative array. Default: false</param>
        /// <param name="Empty">boolean, Return empty albums, categories and subcategories. Default: true</param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response</param>
        /// <param name="Heavy">heavy response</param>
        /// <param name="LastUpdated">Return results where LastUpdated is after the epoch time provided</param>
        /// <param name="NickName">The nickname for a specific user</param>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns></returns>
        public List<Album> GetAlbums(bool Associative, bool Empty, string Extras, bool Heavy, int LastUpdated, string SitePassword)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Empty, Extras, Heavy, LastUpdated, NickName, Pretty, SitePassword, Strict
            var resp = ch.ExecuteMethod<AlbumResponse>("smugmug.albums.get", basic, "NickName", basic.NickName, "Heavy", Heavy, "Associative", Associative, "Empty", Empty, "Extras", Extras, "LastUpdated", LastUpdated, "SitePassword", SitePassword);
            if (resp.stat == "ok")
            {
                var albumList = new List<Album>();
                albumList.AddRange(resp.Albums);
                if (albumList != null)
                {
                    foreach (var myAlbum in albumList)
                    {
                        myAlbum.basic = basic;
                    }
                }
                return albumList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        #endregion

        #region AlbumTemplate

        /// <summary>
        /// Creates a new album template with the specified name 
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        public AlbumTemplate CreateAlbumTemplate(string Name)
        {
            // If the album template doesn't exist
            AlbumTemplate myAlbumTemplate = this.FindAlbumTemplate(Name);
            if (myAlbumTemplate == null)
            {
                // Create it
                myAlbumTemplate = new AlbumTemplate();
                myAlbumTemplate.AlbumTemplateName = Name;
                myAlbumTemplate.basic = basic;
                return myAlbumTemplate.Create();
            }
            // Else, return it
            else
                return myAlbumTemplate;
        }

        /// <summary>
        /// Creates an album template on the site from a locally created album template
        /// </summary>
        /// <param name="at"></param>
        public AlbumTemplate CreateAlbumTemplate(AlbumTemplate at)
        {
            if (at != null)
            {
                // If the album template doesn't exist
                AlbumTemplate myAlbumTemplate = this.FindAlbumTemplate(at.AlbumTemplateName);
                if (myAlbumTemplate == null)
                {
                    // Create it
                    at.basic = basic;
                    return at.Create();
                }
                // Else, return it
                else
                    return myAlbumTemplate;
            }
            else
            {
                throw new ArgumentNullException("Album Template parameter is null!");
            }
        }

        /// <summary>
        /// Retrieves a list of album templates with all their properties for the logged in Power or Pro user: AlbumTemplateID, AlbumTemplateName, Backprinting, CanRank, Clean, Comments, Community (Id and Name), DefaultColor, EXIF, External, FamilyEdit, FriendEdit, Geography, Header, HideOwner, InterceptShipping, Larges, Originals, PackagingBranding, Password, PasswordHint, Printable, ProofDays, Protected, Public, Share, SmugSearchable, SortDirection, SortMethod, SquareThumbs, UnsharpAmount, UnsharpRadius, UnsharpSigma, UnsharpThreshold, WatermarkID, Watermarking, WordSearchable, X2Larges, X3Larges, XLarges
        /// </summary>
        /// <returns></returns>
        public List<AlbumTemplate> GetAlbumTemplates()
        {
            return GetAlbumTemplates(false);
        }

        /// <summary>
        /// Retrieves a list of album templates with all their properties for the logged in Power or Pro user: AlbumTemplateID, AlbumTemplateName, Backprinting, CanRank, Clean, Comments, Community (Id and Name), DefaultColor, EXIF, External, FamilyEdit, FriendEdit, Geography, Header, HideOwner, InterceptShipping, Larges, Originals, PackagingBranding, Password, PasswordHint, Printable, ProofDays, Protected, Public, Share, SmugSearchable, SortDirection, SortMethod, SquareThumbs, UnsharpAmount, UnsharpRadius, UnsharpSigma, UnsharpThreshold, WatermarkID, Watermarking, WordSearchable, X2Larges, X3Larges, XLarges
        /// </summary>
        /// <param name="Associative">boolean, returns an associative array. Default: false</param>
        /// <returns></returns>
        public List<AlbumTemplate> GetAlbumTemplates(bool Associative)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<AlbumTemplateResponse>("smugmug.albumtemplates.get", basic);
            if (resp.stat == "ok")
            {
                var albumTemplateList = new List<AlbumTemplate>();
                albumTemplateList.AddRange(resp.AlbumTemplates);
                if (albumTemplateList != null)
                {
                    foreach (var myAlbumTemplate in albumTemplateList)
                    {
                        myAlbumTemplate.basic = basic;
                    }
                }
                return albumTemplateList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Given a certain name, it finds an album template for the user or returns null if it doesn't exist
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public AlbumTemplate FindAlbumTemplate(string Name)
        {
            var albumTemplateList = this.GetAlbumTemplates();
            if (albumTemplateList != null)
            {
                foreach (var myAlbumTemplate in albumTemplateList)
                {
                    if (myAlbumTemplate.AlbumTemplateName == Name)
                        return myAlbumTemplate;
                }
            }
            return null;
        }

        #endregion

        #region Categories
        /// <summary>
        /// Creates a new category with the given name
        /// </summary>
        /// <param name="Name">The name for the category</param>
        /// <returns>Category (id)</returns>
        public Category CreateCategory(string Name)
        {
            return CreateCategory(Name, "", true, "");
        }

        /// <summary>
        /// Creates a new category with the given name
        /// </summary>
        /// <param name="Name">The name for the category</param>
        /// <param name="NiceName">The nicename for the category</param>
        /// <returns>Category (id)</returns>
        public Category CreateCategory(string Name, string NiceName)
        {
            return CreateCategory(Name, NiceName, true, "");
        }

        /// <summary>
        /// Creates a new category with the given name
        /// </summary>
        /// <param name="Name">The name for the category</param>
        /// <param name="NiceName">The nicename for the category</param>
        /// <param name="Unique">Create a category if one of the same name doesn't already exist in the current hierarchy. Default: false</param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response.</param>
        /// <returns>Category (id)</returns>
        public Category CreateCategory(string Name, string NiceName, bool Unique, string Extras)
        {
            var myCategory = this.FindCategory(Name);
            if (myCategory == null)
            {
                CommunicationHelper ch = new CommunicationHelper();
                // SessionID [required], Name [required], Callback, Extras, NiceName, Pretty, Strict, Unique
                var resp = ch.ExecuteMethod<CategoryResponse>("smugmug.categories.create", basic, "Name", Name, "NiceName", NiceName, "Unique", Unique, "Extras", Extras);
                if (resp.stat == "ok")
                {
                    var temp = resp.Category;
                    temp.basic = basic;
                    temp.Name = Name;
                    return temp;
                }
                else
                {
                    Console.WriteLine(resp.message);
                    throw new SmugMugException(resp.code, resp.message, resp.method);
                }
            }
            else
                return myCategory;
        }

        /// <summary>
        /// Retrieves a list of categories for a given user
        /// </summary>
        /// <returns>List of Categories (id and Name)</returns>
        public List<Category> GetCategories()
        {
            return GetCategories(false, "");
        }

        /// <summary>
        /// Retrieves a list of categories for a given user
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false </param>
        /// <param name="SitePassword">The site password for a specific user.</param>
        /// <returns>List of Categories (id, Name, NiceName, Type)</returns>
        public List<Category> GetCategories(bool Associative, string SitePassword)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], , Associative, Callback, NickName, Pretty, SitePassword, Strict
            var resp = ch.ExecuteMethod<CategoryResponse>("smugmug.categories.get", basic, "NickName", basic.NickName, "Associative", Associative, "SitePassword", SitePassword);
            if (resp.stat == "ok")
            {
                var categoryList = new List<Category>();
                categoryList.AddRange(resp.Categories);
                if (categoryList != null)
                {
                    foreach (var myCategory in categoryList)
                    {
                        myCategory.basic = basic;
                    }
                }
                return categoryList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Given a certain name, it finds a category or returns null
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public Category FindCategory(string Name)
        {
            List<Category> myCategoryList = new List<Category>();
            Category myCategory = null;
            try
            {
                myCategoryList = this.GetCategories();
            }
            catch (SmugMugException sme)
            {
                throw new SmugMugException(sme.code, sme.Message, sme.method);
            }
            if (myCategoryList != null)
            {
                foreach (var item in myCategoryList)
                {
                    if (item.Name == Name)
                    {
                        // Categories are unique in name
                        myCategory = item;
                        break;
                    }
                }
            }
            return myCategory;
        }
        #endregion

        #region Communities
        /// <summary>
        /// Retrieves a list of communities for a user (id and Name)
        /// </summary>
        /// <returns></returns>
        public List<Community> GetCommunities()
        {
            return GetCommunities(false);
        }

        /// <summary>
        /// Retrieves a list of communities for a user (id and Name)
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false.</param>
        /// <returns></returns>
        public List<Community> GetCommunities(bool Associative)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<CommunityResponse>("smugmug.communities.get", basic);
            if (resp.stat == "ok")
            {
                var temp = new List<Community>();
                temp.AddRange(resp.Communities);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Coupons
        /// <summary>
        /// Create a coupon (basic requirements)
        /// </summary>
        /// <param name="Code">The code for the coupon</param>
        /// <param name="Title">The title for the coupon</param>
        /// <param name="Type">The comma separated string of type values to filter results.</param>
        /// <returns></returns>
        public Coupon CreateCoupon(string Code, string Title, CouponTypeEnum couponType, float Amount)
        {
            return CreateCoupon(Code, Title, couponType, Amount, "", "");
        }

        /// <summary>
        /// Create a coupon (with extra attributes in the response)
        /// </summary>
        /// <param name="Code">The code for the coupon</param>
        /// <param name="Title">The title for the coupon</param>
        /// <param name="Type">The comma separated string of type values to filter results.</param>
        /// <param name="AlbumIDs">A comma separated string of AlbumIDs to restrict the coupon.</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Coupon</returns>
        public Coupon CreateCoupon(string Code, string Title, CouponTypeEnum couponType, float Amount, string AlbumIDs, string Extras)
        {
            Coupon c = new Coupon();
            c.basic = basic;
            c.Code = Code;
            c.Title = Title;
            c.Amount = Amount;
            c.Type = couponType;
            c.CreateCoupon(AlbumIDs, Extras);
            return c;
        }

        /// <summary>
        /// Retrieve a list of coupons (Standard response)
        /// </summary>
        /// <returns>List of all the coupons</returns>
        public List<Coupon> GetCoupons()
        {
            return GetCoupons("", "");
        }

        /// <summary>
        /// Retrieve a list of coupons (Heavy response)
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response.</param>
        /// <returns>List of all the coupons</returns>
        public List<Coupon> GetCoupons(string Extras, string Heavy)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Heavy, Pretty, Status, Strict, Type
            var resp = ch.ExecuteMethod<CouponResponse>("smugmug.coupons.get", basic, "Extras", Extras, "Heavy", Heavy);
            if (resp.stat == "ok")
            {
                var temp = new List<Coupon>();
                temp.AddRange(resp.Coupons);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieve a list of coupons (by Status and Type)
        /// </summary>
        /// <param name="Status">The comma separated string of status values to filter results.</param>
        /// <param name="Type">The comma separated string of type values to filter results</param>
        /// <returns>List of all the coupons</returns>
        public List<Coupon> GetCoupons(CouponStatusEnum Status, CouponTypeEnum Type)
        {
            return GetCoupons(Status, Type, "", "");
        }

        /// <summary>
        /// Retrieve a list of coupons.
        /// </summary>
        /// <param name="Status">The comma separated string of status values to filter results.</param>
        /// <param name="Type">The comma separated string of type values to filter results</param>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response.</param>
        /// <returns>List of all the coupons</returns>
        public List<Coupon> GetCoupons(CouponStatusEnum Status, CouponTypeEnum Type, string Heavy, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Heavy, Pretty, Status, Strict, Type
            var resp = ch.ExecuteMethod<CouponResponse>("smugmug.coupons.get", basic, "Extras", Extras, "Heavy", Heavy, "Status", Status, "Type", Type);
            if (resp.stat == "ok")
            {
                var temp = new List<Coupon>();
                temp.AddRange(resp.Coupons);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Family
        /// <summary>
        /// Adds a user to your list of family
        /// </summary>
        /// <param name="Name">The nickname of the family member to be added</param>
        /// <returns></returns>
        public bool AddFamily(string Name)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.family.add", basic, "NickName", Name);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves a list of family (DisplayName, NickName, URL)
        /// </summary>
        /// <returns></returns>
        public List<Family> GetFamily()
        {
            return GetFamily(false, false);
        }

        /// <summary>
        /// Retrieves a list of family (DisplayName, NickName, URL)
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Sandboxed">Forces URLs to a location with a crossdomain.xml file. Default: false</param>
        /// <returns></returns>
        public List<Family> GetFamily(bool Associative, bool Sandboxed)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Pretty, Sandboxed, Strict
            var resp = ch.ExecuteMethod<FamilyResponse>("smugmug.family.get", basic, "Associative", Associative, "Sandboxed", Sandboxed);
            if (resp.stat == "ok")
            {
                var temp = new List<Family>();
                temp.AddRange(resp.Family);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Removes a user from your list of family
        /// </summary>
        /// <param name="Name">The nickname of the family member to be removed</param>
        /// <returns></returns>
        public bool RemoveFamily(string Name)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.family.remove", basic, "NickName", Name);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Removes all users from your list of family
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllFamily()
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.family.removeAll", basic);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Fans
        /// <summary>
        /// Retrieve a list of fans for a user
        /// </summary>        
        /// <returns>List of fans</returns>
        public List<Fans> GetFans()
        {
            return GetFans(false, "", false);
        }

        /// <summary>
        /// Retrieve a list of fans for a user
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <param name="Sandboxed">Forces URLs to a location with a crossdomain.xml file. Default: false</param>
        /// <returns>List of fans</returns>
        public List<Fans> GetFans(bool Associative, string Extras, bool Sandboxed)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Pretty, Sandboxed, Strict
            var resp = ch.ExecuteMethod<FansResponse>("smugmug.fans.get", basic, "Associative", Associative, "Extras", Extras, "Sandboxed", Sandboxed);
            if (resp.stat == "ok")
            {
                var temp = new List<Fans>();
                temp.AddRange(resp.Fans);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Featured
        /// <summary>
        /// Retrieve a list of featured albums for a given user. (basic)
        /// </summary>
        /// <returns>Featured obj</returns>
        public Featured GetFeaturedAlbums()
        {
            return GetFeaturedAlbums(false, false, "", "");
        }

        /// <summary>
        /// Retrieve a list of featured albums for a given user. (heavy)
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <returns>Featured obj</returns>
        public Featured GetFeaturedAlbums(bool Heavy)
        {
            return GetFeaturedAlbums(false, Heavy, "", "");
        }

        /// <summary>
        /// Retrieve a list of featured albums for a given user.
        /// </summary>        
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns>Featured obj</returns>
        public Featured GetFeaturedAlbums(string SitePassword)
        {
            return GetFeaturedAlbums(false, false, "", SitePassword);
        }

        /// <summary>
        /// Retrieve a list of featured albums for a given user.
        /// </summary>        
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <returns>Featured obj</returns>
        public Featured GetFeaturedAlbums(string SitePassword, bool Heavy)
        {
            return GetFeaturedAlbums(false, Heavy, "", SitePassword);
        }

        /// <summary>
        /// Retrieve a list of featured albums for a given user.
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response.</param>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns>Featured obj</returns>
        public Featured GetFeaturedAlbums(bool Associative, bool Heavy, string Extras, string SitePassword)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Heavy, NickName, Pretty, SitePassword, Strict
            var resp = ch.ExecuteMethod<FeaturedResponse>("smugmug.featured.albums.get", basic, "NickName", basic.NickName, "Associative", Associative, "Extras", Extras, "Heavy", Heavy, "SitePassword", SitePassword);
            if (resp.stat == "ok")
            {
                var myFeatured = resp.Featured;
                if (myFeatured != null)
                {
                    myFeatured.basic = basic;
                    var featuredAlbums = myFeatured.Albums;
                    if (featuredAlbums != null)
                    {
                        foreach (var myAlbum in featuredAlbums)
                        {
                            myAlbum.basic = basic;
                        }
                    }
                }
                return myFeatured;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Friend

        /// <summary>
        /// Adds a user to your list of friends
        /// </summary>
        /// <param name="Name">The nickname of the friend to be added</param>
        /// <returns></returns>
        public bool AddFriend(string Name)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.friends.add", basic, "NickName", Name);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves a list of friends (DisplayName, NickName, URL)
        /// </summary>
        /// <returns></returns>
        public List<Friend> GetFriends()
        {
            return GetFriends(false, false);
        }

        /// <summary>
        /// Retrieves a list of friends (DisplayName, NickName, URL)
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Sandboxed">Forces URLs to a location with a crossdomain.xml file. Default: false</param>
        /// <returns></returns>
        public List<Friend> GetFriends(bool Associative, bool Sandboxed)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<FriendResponse>("smugmug.friends.get", basic, "Associative", Associative, "Sandboxed", Sandboxed);
            if (resp.stat == "ok")
            {
                var temp = new List<Friend>();
                temp.AddRange(resp.Friends);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Removes a user from your list of friends
        /// </summary>
        /// <param name="Name">The nickname of the friend to be removed</param>
        /// <returns></returns>
        public bool RemoveFriend(string Name)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], NickName [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.friends.remove", basic, "NickName", Name);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Removes all users from your list of friends
        /// </summary>
        /// <returns></returns>
        public bool RemoveAllFriends()
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.friends.removeAll", basic);
            if (resp.stat == "ok")
            {
                return true;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region Printmark
        /// <summary>
        /// Retrieve the user's list of printmarks. (standard response)
        /// </summary>
        /// <returns>List of printmarks</returns>
        public List<Printmark> GetPrintmarks()
        {
            return GetPrintmarks(false);
        }

        /// <summary>
        /// Retrieve the user's list of printmarks. (heavy response)
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false.</param>
        /// <returns>List of printmarks</returns>
        public List<Printmark> GetPrintmarks(bool Heavy)
        {
            return GetPrintmarks(Heavy, "");
        }

        /// <summary>
        /// Retrieve the user's list of printmarks.
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false.</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response.</param>
        /// <returns>List of printmarks</returns>
        public List<Printmark> GetPrintmarks(bool Heavy, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Heavy, Pretty, Strict
            var resp = ch.ExecuteMethod<PrintmarkResponse>("smugmug.printmarks.get", basic, "Heavy", Heavy, "Extras", Extras);
            if (resp.stat == "ok")
            {
                List<Printmark> printmarkList = new List<Printmark>();
                printmarkList.AddRange(resp.Printmarks);
                if (printmarkList != null)
                {
                    foreach (var myPrintmark in printmarkList)
                    {
                        myPrintmark.basic = basic;
                    }
                }
                return printmarkList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region ShareGroup
        /// <summary>
        /// Creates a new sharegroup with name and description
        /// </summary>
        /// <param name="Name">The name for the sharegroup</param>
        /// <returns>ShareGroup object (id and tag)</returns>
        public ShareGroup CreateShareGroup(string Name)
        {
            return CreateShareGroup(Name, "", false, "", "", "");
        }

        /// <summary>
        /// Creates a new sharegroup with name and description
        /// </summary>
        /// <param name="Name">The name for the sharegroup</param>
        /// <param name="Description">The description for the sharegroup</param>
        /// <returns>ShareGroup object (id and tag)</returns>
        public ShareGroup CreateShareGroup(string Name, string Description)
        {
            return CreateShareGroup(Name, Description, false, "", "", "");
        }

        /// <summary>
        /// Creates a new sharegroup with name and description
        /// </summary>
        /// <param name="Name">The name for the sharegroup</param>
        /// <param name="Description">The description for the sharegroup</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>ShareGroup object (id and tag)</returns>
        public ShareGroup CreateShareGroup(string Name, string Description, string Extras)
        {
            return CreateShareGroup(Name, Description, false, "", "", Extras);
        }

        /// <summary>
        /// Creates a new sharegroup with name and description
        /// </summary>
        /// <param name="Name">The name for the sharegroup</param>
        /// <param name="Description">The description for the sharegroup</param>
        /// <param name="AccessPassworded">Allow access to password protected albums from the sharegroup without the password. Default: false</param>
        /// <param name="Password">The password for the sharegroup</param>
        /// <param name="PasswordHint">The password hint for the sharegroup</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>ShareGroup object (id and tag)</returns>
        public ShareGroup CreateShareGroup(string Name, string Description, bool AccessPassworded, string Password, string PasswordHint, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Name [required], AccessPassworded, Callback, Description, Extras, Password, PasswordHint, Pretty, Strict 
            var resp = ch.ExecuteMethod<ShareGroupResponse>("smugmug.sharegroups.create", basic, "Name", Name, "Description", Description, "AccessPassworded", AccessPassworded, "Password", Password, "PasswordHint", PasswordHint, "Extras", Extras);
            if (resp.stat == "ok")
            {
                var temp = resp.ShareGroup;
                temp.basic = basic;
                temp.Name = Name;
                temp.Description = Description;
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Gets a list of existing sharegroups for a user (standard response - id, Tag, AccessPassworded, AlbumCount, Description, Name, Password, PasswordHint, Passworded, URL)
        /// </summary>
        /// <returns></returns>
        public List<ShareGroup> GetShareGroups()
        {
            return GetShareGroups(false, false, "");
        }

        /// <summary>
        /// Gets a list of existing sharegroups for a user (heavy response - id, Tag, AccessPassworded, AlbumCount, Albums array - id, key, Title; Description, Name, Password, PasswordHint, Passworded, URL)
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <returns></returns>
        public List<ShareGroup> GetShareGroups(bool Heavy)
        {
            return GetShareGroups(false, Heavy, "");
        }

        /// <summary>
        /// Gets a list of existing sharegroups for a user (heavy response - id, Tag, AccessPassworded, AlbumCount, Albums array - id, key, Title; Description, Name, Password, PasswordHint, Passworded, URL)
        /// </summary>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns></returns>
        public List<ShareGroup> GetShareGroups(bool Heavy, string Extras)
        {
            return GetShareGroups(false, Heavy, Extras);
        }

        /// <summary>
        /// Gets a list of existing sharegroups for a user (heavy response - id, Tag, AlbumCount, Description, Name, URL, Albums array with id, Key and Title)
        /// </summary>
        /// <param name="Associative">Returns an associative array.Default: false </param>
        /// <param name="Heavy">Returns a heavy response for this method. Default: false</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns></returns>
        public List<ShareGroup> GetShareGroups(bool Associative, bool Heavy, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Extras, Heavy, Pretty, Strict
            var resp = ch.ExecuteMethod<ShareGroupResponse>("smugmug.sharegroups.get", basic, "Associative", Associative, "Heavy", Heavy, "Extras", Extras);
            if (resp.stat == "ok")
            {
                var shareGroupList = new List<ShareGroup>();
                shareGroupList.AddRange(resp.ShareGroups);
                if (shareGroupList != null)
                {
                    foreach (var myShareGroup in shareGroupList)
                    {
                        myShareGroup.basic = basic;
                    }
                }
                return shareGroupList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
        #endregion

        #region SubCategories
        /// <summary>
        /// Retrieves a list of all subcategories for a given user
        /// </summary>
        /// <returns>List of all subcategories (id, Name, NiceName, CategoryId)</returns>
        public List<SubCategory> GetAllSubCategories()
        {
            return GetAllSubCategories(false, "");
        }

        /// <summary>
        /// Retrieves a list of all subcategories for a given user
        /// </summary>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns>List of all subcategories (id, Name, NiceName, CategoryId)</returns>
        public List<SubCategory> GetAllSubCategories(string SitePassword)
        {
            return GetAllSubCategories(false, SitePassword);
        }

        /// <summary>
        /// Retrieves a list of all subcategories for a given user
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false  </param>
        /// <param name="SitePassword">The site password for a specific user</param>
        /// <returns>List of all subcategories (id, Name, NiceName, CategoryId)</returns>
        public List<SubCategory> GetAllSubCategories(bool Associative, string SitePassword)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, NickName, Pretty, SitePassword, Strict
            var resp = ch.ExecuteMethod<SubCategoryResponse>("smugmug.subcategories.getAll", basic, "NickName", basic.NickName, "Associative", Associative, "SitePassword", SitePassword);
            if (resp.stat == "ok")
            {
                var mySubCategoryList = new List<SubCategory>();
                mySubCategoryList.AddRange(resp.SubCategories);
                if (mySubCategoryList != null)
                {
                    foreach (var mySubCategory in mySubCategoryList)
                    {
                        mySubCategory.basic = basic;
                        if (mySubCategory.Category != null)
                        {
                            mySubCategory.Category.basic = basic;
                            if (mySubCategory.Category.SubCategories == null)
                                mySubCategory.Category.SubCategories = new List<SubCategory>();
                            mySubCategory.Category.SubCategories.Add(mySubCategory);
                        }
                    }
                }
                return mySubCategoryList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }

        }
        #endregion

        #region Themes

        /// <summary>
        /// Retrieves a list of themes for a given user
        /// </summary>
        /// <returns>List of themes</returns>
        public List<Theme> GetThemes()
        {
            return GetThemes(false);
        }

        /// <summary>
        /// Retrieves a list of themes for a given user
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false  </param>
        /// <returns>List of themes</returns>
        public List<Theme> GetThemes(bool Associative)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<ThemeResponse>("smugmug.themes.get", basic, "Associative", Associative);
            if (resp.stat == "ok")
            {
                var temp = new List<Theme>();
                temp.AddRange(resp.Themes);
                return temp;

            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        #endregion

        #region Watermarks

        /// <summary>
        /// Retrieves a list of watermarks (only id and name)
        /// </summary>
        /// <returns>List of Watermarks (with id and name)</returns>
        public List<Watermark> GetWatermarks()
        {
            return GetWatermarks(false, false, "");
        }

        /// <summary>
        /// Retrieves a list of watermarks (heavy response)
        /// </summary>
        /// <param name="Heavy">If true, all the properties for the watermark are returned</param>
        /// <returns>List of Watermarks with properties</returns>
        public List<Watermark> GetWatermarks(bool Heavy)
        {
            return GetWatermarks(false, Heavy, "");
        }

        /// <summary>
        /// Retrieves a list of watermarks (heavy response)
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Heavy">If true, all the properties for the watermark are returned</param>
        /// <returns>List of Watermarks with properties</returns>
        public List<Watermark> GetWatermarks(bool Associative, bool Heavy)
        {
            return GetWatermarks(Associative, Heavy, "");
        }

        /// <summary>
        /// Retrieves a list of watermarks (heavy response)
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false</param>
        /// <param name="Heavy">If true, all the properties for the watermark are returned</param>
        /// <param name="Extras">A comma seperated string of additional attributes to return in the response.</param>
        /// <returns>List of Watermarks with properties</returns>
        public List<Watermark> GetWatermarks(bool Associative, bool Heavy, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // SessionID [required], Associative, Callback, Heavy, Pretty, Strict
            var resp = ch.ExecuteMethod<WatermarkResponse>("smugmug.watermarks.get", basic, "Heavy", Heavy, "Extras", Extras);
            if (resp.stat == "ok")
            {
                var watermarkList = new List<Watermark>();
                watermarkList.AddRange(resp.Watermarks);
                if (watermarkList != null)
                {
                    foreach (var myWatermark in watermarkList)
                    {
                        if (myWatermark != null)
                            myWatermark.basic = basic;
                    }
                }
                return watermarkList;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        #endregion

        // TODO They return "invalid method" (probably removed in version 1.2.2 of the API

        ///// <summary>
        ///// Join a community 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool JoinCommunity(int id)
        //{
        //    Community c = new Community();
        //    c.id = id ;
        //    c.SessionID = basic.SessionID;
        //    return c.Join();
        //}

        ///// <summary>
        ///// Leave a community
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public bool LeaveCommunity(int id)
        //{
        //    Community c = new Community();
        //    c.id = id;
        //    c.SessionID = basic.SessionID;
        //    return c.Leave();
        //}

        ///// <summary>
        // /// Get a list of all the available communities
        // /// </summary>
        // /// <returns></returns>
        //public List<Community> GetAvailableCommunities()
        // {
        //     CommunicationHelper ch = new CommunicationHelper();
        //     var resp = ch.ExecuteMethod<CommunityResponse>("smugmug.communities.getAvailable", basic);
        //     if (resp.stat == "ok")
        //     {
        //         var temp = new List<Community>();
        //         temp.AddRange(resp.Communities);
        //         foreach (var item in temp)
        //         {
        //             item.SessionID = basic.SessionID;   
        //         }
        //         return temp;
        //     }
        //     else
        //     {
        //         Console.WriteLine(resp.message);
        //         throw new SmugMugException(resp.code, resp.message, resp.method);
        //     }
        // }

        ///// <summary>
        ///// Leave all communities you are joined to
        ///// </summary>
        ///// <returns></returns>
        //public bool LeaveAllCommunities()
        //{
        //    CommunicationHelper ch = new CommunicationHelper();
        //    var resp = ch.ExecuteMethod<CommunityResponse>("smugmug.communities.leaveAll", basic);
        //    if (resp.stat == "ok")
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        Console.WriteLine(resp.message);
        //        throw new SmugMugException(resp.code, resp.message, resp.method);
        //    }
        //}
    }
}
