using System;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace Microsoft.Dynamics.Retail.Pos.FiscalCore
{
    /// <summary>
    /// This class persist printer related data.
    /// </summary>
    [Serializable]
    public class PersistentPrinterData : ISerializable
    {
        private static readonly string persistFileName;

        private decimal _grandTotal;
        private string _zReport;
        private string _serialNumber;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PersistentPrinterData"/> is dirty.
        /// The object will be dirty if it was unable to persist changes made or if it could not be restored
        /// from the persistent data store.
        /// </summary>
        /// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
        public bool Dirty { get; private set; }

        /// <summary>
        /// Gets the grand total.
        /// </summary>
        /// <value>The grand total.</value>
        public decimal GrandTotal
        {
            get { return _grandTotal; }
        }

        /// <summary>
        /// Gets the Z report.
        /// </summary>
        /// <value>The Z report.</value>
        public string ZReport
        {
            get { return _zReport; }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public string SerialNumber
        {
            get { return _serialNumber; }
        }


        /// <summary>
        /// Sets the grand total and persist the object.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetGrandTotal(decimal value)
        {
            _grandTotal = value;
            UpdateObject();
        }

        /// <summary>
        /// Sets the Z report.
        /// </summary>
        /// <param name="newZReport">The new Z report.</param>
        public void SetZReport(string newZReport)
        {
            _zReport = newZReport;
            UpdateObject();
        }

        /// <summary>
        /// Sets the serial number.
        /// </summary>
        /// <param name="newSerialNumber">The new serial number.</param>
        public void SetSerialNumber(string newSerialNumber)
        {
            _serialNumber = newSerialNumber;
            UpdateObject();
        }

        /// <summary>
        /// Updates the object persistent file.
        /// </summary>
        private void UpdateObject()
        {
            this.Dirty = true;
            try
            {
                PersistentHelper.PersistObjectToFile(persistFileName, this);
                this.Dirty = false;
            }
            catch (System.IO.IOException)
            {
            }
            catch (System.UnauthorizedAccessException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }
        }

        /// <summary>
        /// Initializes the <see cref="PersistentPrinterData"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static PersistentPrinterData()
        {
            // Allow AppSetting to override the default persistent data store.
            string defaultPersistFileName = "PrinterData.Bin";
            string configPersitFileName = ConfigurationManager.AppSettings["PersistentPrinterData"];

            if (!string.IsNullOrEmpty(configPersitFileName))
            {
                defaultPersistFileName = configPersitFileName;
            }
            else
            {
                string loc = System.Reflection.Assembly.GetExecutingAssembly().Location;

                defaultPersistFileName = Path.Combine(Path.GetDirectoryName(loc), defaultPersistFileName);
            }


            persistFileName = defaultPersistFileName;
        }

        /// <summary>
        /// Get the persisted object.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static PersistentPrinterData PersistedObject()
        {
            object tempResult = null;
            PersistentPrinterData result;

            try
            {
                tempResult = PersistentHelper.RestoreObjectFromFile(persistFileName, new PersistentPrinterDataBinder());
            }
            catch (System.IO.IOException)
            {
            }
            catch (System.UnauthorizedAccessException)
            {
            }
            catch (System.Security.SecurityException)
            {
            }
            catch (System.TypeInitializationException)
            {
            }
            catch (Exception)
            {
            }

            result = tempResult as PersistentPrinterData;


            return (result == null) ? new PersistentPrinterData() : result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentPrinterData"/> class.
        /// </summary>
        private PersistentPrinterData()
        {
            _grandTotal = 0m;
            _zReport = string.Empty;
            _serialNumber = string.Empty;
            this.Dirty = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentPrinterData"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PersistentPrinterData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            this._grandTotal = (decimal)info.GetValue("_grandTotal", typeof(decimal));
            this._serialNumber = (string)info.GetValue("_serialNumber", typeof(string));
            this._zReport = (string)info.GetValue("_zReport", typeof(string));
        }

        #region ISerializable Members

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">
        /// The caller does not have the required permission.
        /// </exception>
        [System.Security.SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("_grandTotal", this._grandTotal);
            info.AddValue("_serialNumber", this._serialNumber);
            info.AddValue("_zReport", this._zReport);
        }

        #endregion
    }

    /// <summary>
    /// Create a Serialization binder so that the classes in this assembly can be de-serialized even
    /// when this is done from the Trigger or Service sub-folder.
    /// </summary>
    internal class PersistentPrinterDataBinder : SerializationBinder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public override Type BindToType(string assemblyName, string typeName)
        {
            // See MSDN for details on creating a binder:
            // http://msdn.microsoft.com/en-us/library/system.runtime.serialization.serializationbinder.aspx

            Type typeToDesialize = null;
            string currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().FullName;            
            assemblyName = currentAssembly;

            typeToDesialize = Type.GetType(string.Format("{0},{1}", typeName, assemblyName));

            return typeToDesialize;
        }
    }

    /// <summary>
    /// Helper class used to persist object to and from files.
    /// </summary>
    internal static class PersistentHelper
    {
        /// <summary>
        /// Persists the object to file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="objectToPersist">The object to persist.</param>
        public static void PersistObjectToFile(string filename, ISerializable objectToPersist)
        {
            using (Stream fileStream = File.Open(filename, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fileStream, objectToPersist);
            }
        }

        /// <summary>
        /// Restores the object from file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static object RestoreObjectFromFile(string filename, SerializationBinder binder)
        {
            using (Stream fileStream = File.Open(filename, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                if (binder != null)
                {
                    formatter.Binder = binder;
                }

                object result = formatter.Deserialize(fileStream);

                return result;
            }
        }
    }
}
