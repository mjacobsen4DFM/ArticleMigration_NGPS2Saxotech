using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessRuleC;
using DataModels;
using System.Text;

public partial class Navigation : System.Web.UI.Page
{
     public void PopulateNode(Object sender, TreeNodeEventArgs e)
    {

        // Call the appropriate method to populate a node at a particular level.
        switch (e.Node.Depth)
        {
            case 0:
                // Populate the first-level nodes.
                List<profile> profiles = BusinessRule.GetProfiles(0);

                PopulateLevel(e.Node, profiles);
                break;
            case 1:
                // Populate the second-level nodes.
      //          PopulateProducts(e.Node);
                int result = 0;
                string value = e.Node.Value;
                string name = e.Node.Text;
                int.TryParse(value,out result);
                profiles = BusinessRule.GetProfileParentid( result,name);
                PopulateLevel(e.Node, profiles);
                break;
            default:
                // Do nothing.
                break;
        }

    }




     void PopulateLevel(TreeNode node, List<profile> profiles)
     {
         foreach (profile p in profiles)
         {

             TreeNode newNode = new TreeNode();
             newNode.Text = p.fieldname;
             newNode.Value = p.id.ToString();
             newNode.PopulateOnDemand = true;
             newNode.SelectAction = TreeNodeSelectAction.Expand;
             node.ChildNodes.Add(newNode);
//             tv.Nodes.Add(newNode);
         }

     }


    //public TreeView loadTreeMenu(TreeView tv, List<profile> profiles)
    //{

    //    foreach (profile p in profiles)
    //    {
    //        TreeNode ParentNode = new TreeNode();
    //        ParentNode.Text = p.fieldname;
    //        ParentNode.PopulateOnDemand = true;
    //        ParentNode.SelectAction = TreeNodeSelectAction.Expand;
    //        tv.Nodes.Add(ParentNode);
    //    }

    //    return tv;
    //}



    protected void Page_Load(object sender, EventArgs e)
    {
        //if (! IsPostBack) {
        //loadTreeMenu(LinksTreeView, profiles);
        //}
    }
}