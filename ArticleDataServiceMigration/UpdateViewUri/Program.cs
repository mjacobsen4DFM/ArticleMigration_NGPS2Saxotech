using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;


namespace UpdateViewUri
{
    class Program
    {
        static void Main(string[] args)
        {
            string _message = "";
            DateTime startwatch;
            TimeSpan t2;
            string username = ConfigurationManager.AppSettings["username"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            string sqlstatement = string.Format("select article_uid, storyurl from saxo_article where viewuri is null or viewuri = ''");
            DataSet ds = BusinessRule.BusinessRule.BusGetDataset(sqlstatement);
            int total = ds.Tables[0].Rows.Count;
            Console.WriteLine("Total: {0}", total);
            int idx = 0;
            foreach (DataRow articleRow in ds.Tables[0].Rows)
            {
                startwatch = DateTime.Now;
                string article_uid = articleRow["article_uid"].ToString();
                string storyurl = articleRow["storyurl"].ToString();
                string viewuri = BusinessRule.BusinessRule.storyurltoViewuri(username, pwd, storyurl);
                DataAuthentication.DataAuthentication.spExecute( "UpdateSaxoViewuri",ref  _message, "@article_uid", article_uid, "@viewuri", viewuri);
                idx += 1;
                t2 = DateTime.Now.Subtract(startwatch);

                Console.WriteLine(" {0}/{1} article_uid: {2} elapsed time: {3} updated", idx,total, article_uid, t2);
            } // foreach
        }
    }
}
