using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmugMugModel
{
    public class Community : SmugMugObject
    {
        #region Properties
        /// <summary>
        /// The id for this community
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// The name for this community
        /// </summary>
        public string Name { get; set; }
        #endregion


        // They return "invalid method" (probably removed in version 1.2.2 of the API)


        ///// <summary>
        ///// Join a community
        ///// </summary>
        ///// <returns></returns>
        //public bool Join()
        //{
        //    CommunicationHelper ch = new CommunicationHelper();
        //    var resp = ch.ExecuteMethod<CommunityResponse>("smugmug.communities.join", basic, "CommunityID", this.id);
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

        ///// <summary>
        ///// Leave a community
        ///// </summary>
        ///// <returns></returns>
        //public bool Leave()
        //{
        //    CommunicationHelper ch = new CommunicationHelper();
        //    var resp = ch.ExecuteMethod<CommunityResponse>("smugmug.communities.leave", basic, "CommunityID", this.id);
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
