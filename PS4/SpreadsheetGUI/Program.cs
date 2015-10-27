using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// Eric was here first.
// Tivinia was here.
namespace SpreadsheetGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            SpreadsheetAppContext context = SpreadsheetAppContext.getAppContext();
            context.RunForm(new SpreadsheetForm());
            Application.Run(context);

        }
    }


    /// <summary>
    /// Keeps track of the current spreadsheet windows.
    /// </summary>
    class SpreadsheetAppContext : ApplicationContext
    {
        // Number of open windows
        private int windowCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetAppContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetAppContext()
        {
        }

        /// <summary>
        /// Returns the one SpreadsheetApplicationContext.
        /// </summary>
        public static SpreadsheetAppContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new SpreadsheetAppContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            windowCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--windowCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }
}
