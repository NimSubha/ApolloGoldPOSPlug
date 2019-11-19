using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Windows.Forms;
using System.ComponentModel.Composition;

namespace ApplicationTriggers
{
    [Export(typeof(IApplicationTrigger))]
    public class ApplicationTrigger : IApplicationTrigger
    {

        #region IApplicationTriggerV1 Members

        public void ApplicationStart()
        {
            MessageBox.Show("Application Start");
        }

        public void ApplicationStop()
        {
            MessageBox.Show("Application Stop");
        }

        public void LoginWindowVisible()
        {
            MessageBox.Show("Login Window Visible");
        }

        public void PreLogon(IPreTriggerResult preTriggerResult, string operatorId, string name)
        {
            MessageBox.Show("Pre Logon");
        }

        public void PostLogon(bool loginSuccessful, string operatorId, string name)
        {
            MessageBox.Show("Post Logon");
        }

        public void Logoff(string operatorId, string name)
        {
            MessageBox.Show("Logoff");
        }

        #endregion

    }
}
