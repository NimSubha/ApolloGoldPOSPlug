using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class OrderDetailsPage : UserControl
    {
        public OrderDetailsPage()
        {
            InitializeComponent();
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(701, 565);
            }
        }

        public virtual void SetViewModel<T>(T viewModel)
        {
            bindingSource.Add(viewModel);
            this.GetViewModel<INotifyPropertyChanged>().PropertyChanged += new PropertyChangedEventHandler(OnViewModel_PropertyChanged);
        }

        /// <summary>
        /// Gets the ViewModel
        /// </summary>        
        protected T GetViewModel<T>()
        {
            return (T)bindingSource.Current;
        }

        /// <summary>
        /// Called when a property changes on the view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Called when the page is activated and becomes the front page
        /// </summary>
        public virtual void OnActivate()
        {
            if (this.GetViewModel<PageViewModel>() != null)
            {
                // Refresh any cached values
                this.GetViewModel<PageViewModel>().Refresh();

                // Refresh control bindings. 
                // We wouldn't need this if the model (CustomerOrderTransaction) also implemented INotifyPropertyChanged.
                this.bindingSource.ResetBindings(false);
            }
        }

        // Called when the Clear button is clicked and this is the focus page
        public virtual void OnClear()
        {
        }

        public virtual bool IsClearEnabled()
        {
            return true;
        }


    }
}
