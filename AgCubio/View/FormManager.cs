using System;
using System.Windows.Forms;
using AgCubio;

namespace View
{
    /// <summary>
    /// Class for managing the forms, i.e. putting up the title form and switching to the game form when appropriate.
    /// </summary>
    public class FormManager : ApplicationContext
    {
        protected bool exitAppOnClose;

        public Form CurrentForm
        {
            get { return MainForm; }
            set
            {
                if (MainForm != null)
                {
                    // close the current form, but don't exit the application
                    exitAppOnClose = false;
                    MainForm.Close();
                    exitAppOnClose = true;
                }
                //MainForm.Hide();
                // switch to the new form
                MainForm = value;
                MainForm.Show();
            }
        }

        public FormManager()
        {
            exitAppOnClose = true;
            CurrentForm = new TitleForm(this);
        }

        /// <summary>
        /// When called form TitleForm, switches to a running GameForm.
        /// </summary>
        public void startGameForm(PreservedState state, string playerName)
        {
            CurrentForm = new GameForm(state, playerName);
        }

        // when a form is closed, don't exit the application if this is a swap
        protected override void OnMainFormClosed(object sender, EventArgs e)
        {
            if (exitAppOnClose)
            {
                base.OnMainFormClosed(sender, e);
            }
        }
    }
}
