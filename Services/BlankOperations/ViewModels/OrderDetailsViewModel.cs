using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    sealed class OrderDetailsViewModel : PageViewModel
    {
        private int selectedPageIndex = -1;
        private string selectedPageTitle;

        public int SelectedPageIndex
        {
            get { return this.selectedPageIndex; }
            set
            {
                if (this.selectedPageIndex != value)
                {
                    this.selectedPageIndex = value;
                    OnPropertyChanged("SelectedPageIndex");
                    OnPropertyChanged("SelectedPageTitle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the displayed title of the selected page
        /// </summary>
        public string SelectedPageTitle
        {
            get { return this.selectedPageTitle; }
            set
            {
                if (this.selectedPageTitle != value)
                {
                    this.selectedPageTitle = value;
                    OnPropertyChanged("SelectedPageTitle");
                }
            }
        }
    }
}
