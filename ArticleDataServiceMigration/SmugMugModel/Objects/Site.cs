using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace SmugMugModel
{
    public class Site
    {
        static public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Pings SmugMug
        /// </summary>
        /// <returns>Returns an empty successful response, if it completes without error.</returns>
        public bool Ping()
        {
            // APIKey [required], Callback, Pretty
            CommunicationHelper ch = new CommunicationHelper();
            var resp = ch.ExecuteMethod<SmugMugResponse>("smugmug.service.ping", null);
            if (resp.stat == "ok")
                return true;
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public MyUser Login(string EmailAddress, string Password)
        {
            return Login(EmailAddress, Password, "");
        }

        /// <summary>
        /// Authenticates a user based on the specified email address (or nickname) and password
        /// </summary>
        /// <param name="EmailAddress">The email address (or nickname) for the user.</param>
        /// <param name="Password">The password for the user.</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, Type, FileSizeLimit, PasswordHash, SessionID, SmugVault, User (id, DisplayName, NickName))</returns>
        public MyUser Login(string EmailAddress, string Password, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            string su;
            // APIKey [required], EmailAddress [required], Password [required], Callback (JSON & PHP responses only), Extras, Pretty, Sandboxed, Strict
            var resp = ch.ExecuteMethod<LoginResponse>("smugmug.login.withPassword", null, out su,"EmailAddress", EmailAddress, "Password", Password, "Extras", Extras);
            if (resp.stat == "ok")
            {
                MyUser u = new MyUser();
                resp.Login.PopulateUser(u);
                u.basic._su = su;
                return u;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public MyUser Login(int UserId, string PasswordHash)
        {
            return Login(UserId, PasswordHash, "");
        }

        /// <summary>
        /// Authenticates a user based on the specified user id and password hash.
        /// </summary>
        /// <param name="UserId">The id for a specific user</param>
        /// <param name="PasswordHash">The password hash for the user</param>
        /// <param name="Extras">A comma separated string of additional attributes to return in the response</param>
        /// <returns>Login (AccountStatus, AccountType, FileSizeLimit, SessionID, SmugVault, User (id, DisplayName, NickName, URL))</returns>
        public MyUser Login(int UserId, string PasswordHash, string Extras)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // APIKey [required], Callback, Extras, PasswordHash [required], Pretty, Sandboxed, Strict, UserID [required]
            string su;
            var resp = ch.ExecuteMethod<LoginResponse>("smugmug.login.withHash", null, out su, "UserID", UserId, "PasswordHash", PasswordHash, "Extras", Extras);
            if (resp.stat == "ok")
            {
                MyUser u = new MyUser();
                resp.Login.PopulateUser(u);
                u.basic._su = su;
                return u;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Establishes an anonymous session.
        /// </summary>
        /// <returns>Login (Session (id))</returns>
        public MyUser Login()
        {
            // APIKey [required], Callback, Pretty, Strict
            CommunicationHelper ch = new CommunicationHelper();
            var resp = ch.ExecuteMethod<LoginResponse>("smugmug.login.anonymously", null);
            if (resp.stat == "ok")
            {
                MyUser currentUser = new MyUser();
                if (currentUser.basic == null)
                    currentUser.basic = new SmugMugBase();
                currentUser.basic.SessionID = resp.Login.Session.id;
                currentUser.basic.NickName = ""; 
                currentUser.DisplayName = "";
                return currentUser;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <returns>List of Templates (id and Name)</returns>
        public List<Template> StylesGetTemplates()
        {
            return StylesGetTemplates(false);
        }

        /// <summary>
        /// Retrieves a list of style templates
        /// </summary>
        /// <param name="Associative">Returns an associative array. Default: false </param>
        /// <returns>List of Templates (id and Name)</returns>
        public List<Template> StylesGetTemplates(bool Associative)
        {
            CommunicationHelper ch = new CommunicationHelper();
            // Associative, Callback, Pretty, Strict
            var resp = ch.ExecuteMethod<TemplateResponse>("smugmug.styles.getTemplates", null, "Associative", Associative);
            if (resp.stat == "ok")
            {
                var temp = new List<Template>();
                temp.AddRange(resp.Templates);
                return temp;
            }
            else
            {
                Console.WriteLine(resp.message);
                throw new SmugMugException(resp.code, resp.message, resp.method);
            }
        }
    }
}
