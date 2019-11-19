using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "PriceList")]
    public partial class ItemPriceListForm : Form
    {
        #region Memeber Variables
        private SqlConnection _connection;
        private string _dataReadId;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPriceListForm"/> class.
        /// </summary>
        protected ItemPriceListForm()
        {   // Default ctor for designer support only.
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPriceListForm"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="dataReadId">The data read id.</param>
        public ItemPriceListForm(SqlConnection connection, string dataReadId)
            : this()
        {
            // Save SQL connection information
            _connection = connection;
            _dataReadId = dataReadId;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GetData();
        }

        /// <summary>
        /// Gets the data for the form.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void GetData()
        {
            string queryString =
                @"SELECT INVENTTABLE.ITEMID, INVENTTABLE.ITEMNAME, INVENTTABLEMODULE.UNITID, INVENTTABLEMODULE.PRICE FROM INVENTTABLE 
                INNER JOIN INVENTTABLEMODULE ON INVENTTABLE.ITEMID = INVENTTABLEMODULE.ITEMID
                WHERE INVENTTABLE.DATAAREAID = @DATAREADID
                AND INVENTTABLEMODULE.MODULETYPE = @MODULETYPE";

            txtDataReadId.Text = _dataReadId;
            txtTerminalId.Text = ApplicationSettings.Terminal.TerminalId;
            txtStoreId.Text = ApplicationSettings.Terminal.StoreId;

            try
            {
                using (SqlCommand command = new SqlCommand(queryString.ToString(), _connection))
                {
                    command.Parameters.AddWithValue("@DATAREADID", _dataReadId); // Desired Store (e.g. "ceu")
                    command.Parameters.AddWithValue("@MODULETYPE", 2); // Customer Price = 2

                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (DataTable myTable = new DataTable())
                        {
                            myTable.Locale = CultureInfo.InvariantCulture;
                            myTable.Load(reader);
                            itemDataGridView.DataSource = myTable;
                            itemDataGridView.AutoResizeColumns();
                            itemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.Fail(ex.Message);
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }
    }
}
