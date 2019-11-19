using System;
using System.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.FiscalLog
{
    /// <summary>
    /// Full Lazy singleton for a Fiscal Log
    /// </summary>
    public sealed class FiscalLogSingleton
    {
        private FiscalLogSingleton()
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void ApplicationStart()
        {
            Debug.WriteLine("Application Start");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void ApplicationEnd()
        {
            Debug.WriteLine("Application End");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void PostPriceOverride()
        {
            Debug.WriteLine("PostPriceOverride");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void BeginTransaction()
        {
            Debug.WriteLine("BeginTransaction");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void DisplayingSplashScreen()
        {
            Debug.WriteLine("DisplayingSplashScreen");
        }

        public static FiscalLogSingleton Instance
        {
            get { return Nested.instance; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        class Nested
        {
            /// <summary>
            /// Initializes the <see cref="Nested"/> class.
            /// Explict static ctor required to ensure compiler does not marek type as "beforefieldinit"
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }

            internal static readonly FiscalLogSingleton instance = new FiscalLogSingleton();
        }
    }
}
