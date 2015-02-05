using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessRuleC;
using DataModels;
using System.Text;



public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();
        List<profile> profiles = BusinessRule.GetProfiles(1, "Lang");
        if (profiles.Count == 1)
        {
            sb.Append(string.Format("Name {0} id: {1} </br>", profiles[0].fieldname, profiles[0].id));
           //    profiles = BusinessRule.GetProfiles()
            profiles = BusinessRule.GetProfiles(3,   4000015);
            foreach ( profile p in profiles ){ 
                sb.Append(string.Format("Level: {0} Name: {1} id:{2} real id: {3} </br>", p.treelevel,p.fieldname, p.id, p.realid));
            }
            area.InnerHtml = sb.ToString();
        }
    }
}