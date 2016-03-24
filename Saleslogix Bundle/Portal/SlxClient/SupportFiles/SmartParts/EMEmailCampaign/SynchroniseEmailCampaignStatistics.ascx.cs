using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sage.Platform.WebPortal.SmartParts;
using System.Web.UI;
using Sage.Entity.Interfaces;
using Sage.Platform;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for EmailCampaignDetails
/// </summary>
public partial class EmailCampaignDetails : SmartPartInfoProvider
{
    private Sage.Platform.Security.IRoleSecurityService _roleSecurityService;
    /// <summary>
    /// Gets or sets the role security service.
    /// </summary>
    /// <value>The role security service.</value>
    [Sage.Platform.Application.ServiceDependency]
    public Sage.Platform.Security.IRoleSecurityService RoleSecurityService
    {
        set
        {
            _roleSecurityService = Sage.Platform.Application.ApplicationContext.Current.Services.Get<Sage.Platform.Security.IRoleSecurityService>(true);
        }
        get
        {
            return _roleSecurityService;
        }
    }

    protected override void OnWireEventHandlers()
    {
        base.OnWireEventHandlers();

        // Add a sync task if a pending one doesn't already exist
        RepositoryHelper<IEMSyncTask> rep = EntityFactory.GetRepositoryHelper<IEMSyncTask>();
        Sage.Platform.Repository.ICriteria criteria = rep.CreateCriteria();
        criteria.Add(rep.EF.Eq("TaskType", "SynchroniseAllEmailCampaignHeaders"));
        criteria.Add(rep.EF.Eq("Status", "Pending"));
        var result = criteria.List<IEMSyncTask>();

        if (result == null || result.Count == 0)
        {
            IEMSyncTask syncTask = EntityFactory.Create<IEMSyncTask>();
            syncTask.TaskType = "SynchroniseAllEmailCampaignHeaders";
            syncTask.ScheduledStartTime = DateTime.UtcNow;
            syncTask.Status = "Pending";
            syncTask.TaskData = string.Empty;
            syncTask.Save();
        }
		
		string js = @"
// SLX8 fix: Hide the Main Footer because, for some reason, it is appearing in the wrong location
try {
	dojo.style('MainFooter', 'display', 'none');
}
catch (e) {
  // do nothing if there was an exception (e.g. v7.5.4)
}
";
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "hideFooter", js, true);
    }

    public override Sage.Platform.Application.UI.ISmartPartInfo GetSmartPartInfo(Type smartPartInfoType)
    {
        ToolsSmartPartInfo tinfo = new ToolsSmartPartInfo();

        foreach (Control c in Controls)
        {
            SmartPartToolsContainer cont = c as SmartPartToolsContainer;
            if (cont != null)
            {
                switch (cont.ToolbarLocation)
                {
                    case SmartPartToolsLocation.Right:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.RightTools.Add(tool);
                        }
                        break;
                    case SmartPartToolsLocation.Center:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.CenterTools.Add(tool);
                        }
                        break;
                    case SmartPartToolsLocation.Left:
                        foreach (Control tool in cont.Controls)
                        {
                            tinfo.LeftTools.Add(tool);
                        }
                        break;
                }
            }
        }

        return tinfo;
    }
}