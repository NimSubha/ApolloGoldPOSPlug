/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.TimeRegistration
{
    using System.ComponentModel.Composition;
    using Microsoft.Dynamics.Retail.Notification.Contracts;
    using Microsoft.Dynamics.Retail.Pos.Contracts;
    using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
    using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
    using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

    [Export(typeof(ITimeRegistration))]
    public sealed class TimeRegistration : ITimeRegistration
    {
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application
        {
            get { return this.application; }
            set { this.application = value; }
        }

        /// <summary>
        /// Time registration, this allows operator to register time clock in\clock out time
        /// </summary>
        /// <param name="posTransaction">Current pos transaction.</param>
        public void TimeRegistrations(IPosTransaction posTransaction)
        {
            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(
                new RegisterTimeNotification(), () => {});
            this.Application.Services.Interaction.InteractionRequest(request);
        }

        /// <summary>
        /// Show view time clock entries.
        /// </summary>
        /// <param name="posTransaction">Reference to an IPosTransaction object.</param>
        public void ViewTimeClockEntries()
        {
            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(
                new ViewTimeClockEntriesNotification(), () => { });
            this.Application.Services.Interaction.InteractionRequest(request);
        }
    }
}
