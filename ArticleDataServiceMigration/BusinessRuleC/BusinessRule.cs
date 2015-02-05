using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModels;
using System.Data.Linq;

namespace BusinessRuleC
{
    public static class BusinessRule
    {



        static public List<saxo_pubMap> ListPub(int mysiteid)
        {

            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {
               
                var matchingPub = from saxo_pubMap in database.saxo_pubMap
                                  where saxo_pubMap.siteid.Equals(mysiteid)
                                  select saxo_pubMap;
                return matchingPub.ToList();
            }
        }// ListPub



        static public void  AddEntry(  int treelevel, int id, int rootid, int parentid, string fieldname, string uri, int childcount, string childrenuri, int realid, bool updaterealid) {

            List<profile> myprofile = GetProfile(id);
            if ((myprofile.Count() > 0) )
            {
                if (updaterealid == true)
                {
                    using (ArticleDataDbEntities database = new ArticleDataDbEntities())
                    {
                        profile pro = database.profiles.SingleOrDefault(p => p.id == id);
                        pro.realid = realid;
                        database.SaveChanges();
                    }
                }
                Console.WriteLine("treelevel: {0} fieldname: {1} parentid {2} rootid {3} already exits" , treelevel, fieldname, parentid , rootid );
                return;
            }

            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {
                profile p = new profile();
                p.id = id;
                p.fieldname = fieldname;
                p.uri = uri;
                p.childcount = childcount;
                p.childrenuri = childrenuri;
                p.treelevel = treelevel;
                p.parentid = parentid;
                p.rootid = rootid;
                p.realid = realid;
                database.profiles.AddObject(p);
                database.SaveChanges();
            }
        }

        static public void truncateProfileTable()
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {
                database.ExecuteStoreCommand("delete from profile");
                database.SaveChanges();
            }
        }


        static public List<profile> GetProfileParentid(int id, string name )
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where (profile.parentid.Equals(id)) && (! profile.fieldname.Equals(name))
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfiles




        static public List<profile> GetProfiles(int treelevel )
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where profile.treelevel.Equals(treelevel)
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfiles


        static public List<profile> GetProfiles(int treelevel, int rootid) {
                using (ArticleDataDbEntities database = new ArticleDataDbEntities())
                {

                    var matchingProfiles = from profile  in database.profiles
                                           where profile.treelevel.Equals(treelevel) && profile.rootid.Equals(rootid)
                                           select profile;
                    return matchingProfiles.ToList();
                }

        } //  GetProfiles

        static public List<profile> GetProfiles(int treelevel, string fieldname )
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where profile.treelevel.Equals(treelevel) && profile.fieldname.ToLower().Equals(fieldname.ToLower())
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfiles

        static public List<profile> GetProfiles(int treelevel, string fieldname, int parentid, int rootid)
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where profile.treelevel.Equals(treelevel) && profile.fieldname.ToLower().Equals(fieldname.ToLower() ) && profile.parentid.Equals(parentid) && profile.rootid.Equals(rootid)
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfiles



        static public List<profile> GetProfilesForId(int treelevel, int id)
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where profile.treelevel.Equals(treelevel) && profile.parentid.Equals(id)
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfiles

        static public List<profile> GetProfile(int id)
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var matchingProfiles = from profile in database.profiles
                                       where profile.id.Equals(id) 
                                       select profile;
                return matchingProfiles.ToList();
            }

        } //  GetProfile


        static public List<sectionAnchorMap> GetSectionAnchorMap(int siteid)
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var s = from sectionAnchorMap in database.sectionAnchorMaps
                        where sectionAnchorMap.siteid.Equals(siteid)
                        select sectionAnchorMap;

                return s.ToList();
            }

        } //  GetSectionAnchorMap


        static public List<sectionAnchorMap> GetSectionAnchorMap(int siteid,string sectionanchor_uid )
        {
            using (ArticleDataDbEntities database = new ArticleDataDbEntities())
            {

                var s = from sectionAnchorMap in database.sectionAnchorMaps
                        where sectionAnchorMap.siteid.Equals(siteid) && sectionAnchorMap.sectionAnchor.Equals(sectionanchor_uid)
                        select sectionAnchorMap;

                return s.ToList();
            }

        } //  GetSectionAnchorMap
        

    }
}
