using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmugMugModel
{
    /// <summary>
    /// Basic Information that must be send with each object
    /// </summary>
    public class SmugMugBase
    {
        /// <summary>
        /// The id for this user.
        /// </summary>
        public string SessionID { set; get; }
        /// <summary>
        /// The NickName for this user.
        /// </summary>
        public string NickName { set; get; }
        /// <summary>
        /// As part of overall site hardening against sidejacking, a new cookie _su is returned when logging in. 
        /// This cookie is required to be present when making subsequent calls over https.
        /// </summary>
        public string _su { set; get; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(SessionID))
                sb.AppendFormat("SessionID={0}&", SessionID);
            //if (!string.IsNullOrEmpty(_su))
              //  sb.AppendFormat("_su={0}&", _su);

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}