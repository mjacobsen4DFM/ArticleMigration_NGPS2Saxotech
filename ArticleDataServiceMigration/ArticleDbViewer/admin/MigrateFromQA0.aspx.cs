using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DataModels;


public partial class MigrateFromQA0 : WebFrontEnd //  System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
	        DrpPub.Items.Clear();
    	    DataSet ds = BusinessRule.BusinessRule.distinctPubs();
    	    foreach ( DataRow dr in ds.Tables[0].Rows) {
        		string siteid = dr[0].ToString();
		        string publicationName = dr[1].ToString();
		        DrpPub.Items.Add(new ListItem(publicationName, siteid));
    	    }
        }
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        string siteid = DrpPub.SelectedValue;

        mytarget2.Attributes.Add("src", string.Format("migrateqao.aspx?startDate={0}&stopDate={1}&siteid={2}", TxtStart.Text, txtEnd.Text, siteid));

        
//        Response.BufferOutput = false;
//        for (int i = 1; i <= 10; i++) {
////            AreaOuput.InnerHtml = string.Format("Output {0} <br/>", i);
//            Response.Write(string.Format("Output {0} <br/>", i));
//            System.Threading.Thread.Sleep(1000);
            
//        }
        
    }
}