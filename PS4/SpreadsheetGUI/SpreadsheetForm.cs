using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetForm : Form
    {
        /// <summary>
        /// Represents the guts of the spreadsheet.
        /// </summary>
        Spreadsheet sheet;

        /// <summary>
        /// Represents the location of the currently selected cell (A1->Z99)
        /// </summary>
        Cell currentCell;

        /// <summary>
        /// Regex to match possible cells A1->Z99.
        /// </summary>
        protected static Regex isValid = new Regex(@"^[A-Z][1-9][0-9]{0,1}$");

        /// <summary>
        /// Creates a new blank SpreadsheetForm.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

            // Treat strings as case-insensitive, and use version "ps6"
            sheet = new Spreadsheet(s => isValid.IsMatch(s), s => s.ToUpper(), "ps6");

            spreadsheetPanel.SelectionChanged += SpreadsheetPanel_SelectionChanged;

            currentCell = new Cell('A', 1);
            setSelected('A', 1);

            setTitle("Spreadsheet");
        }


        /// <summary>
        /// A simple cell representation, holding only its row, col, and string name.
        /// </summary>
        private class Cell
        {
            private char _col;
            private int _row;

            /// <summary>
            /// Creates a cell with the given col and row.
            /// Throws ArgumentException if col or row is invalid (not in range A1->Z99).
            /// </summary>
            public Cell(char col, int row)
            {
                if (!validRow(row) || !validCol(col))
                    throw new ArgumentException();
                _col = col;
                _row = row;
            }
        
            public string name {
                get
                {
                    // join col and row (e.g. G33)
                    return new string(col, 1) + row;
                }
                set
                {
                    if (!isValid.IsMatch(value))
                        return;

                    _col = value[0];
                    int.TryParse(value.Split(_col)[1], out _row);
                }

            }
            public char col {
                get
                {
                    return _col;
                }
                set
                {
                    if (validCol(value))
                        _col = col;
                }
            }

            public int row
            {
                get
                {
                    return _row;
                }
                set
                {
                    if (validRow(value))
                        _row = row;
                }
            }

            public static bool validRow(int r)
            {
                return (r >= 1 && r <= 99);
            }

            public static bool validCol(char c)
            {
                return (c >= 'A' && c <= 'Z');
            }
        }


        private void spreadsheetPanel_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sets the title of this spreadsheet.
        /// </summary>
        private void setTitle(string title)
        {
            Text = title;
        }

        /// <summary>
        /// Sets the currently selected cell to the given col, row pair, saving the
        /// previous cell contents.
        /// Col must be a char between A and Z, and Row an int between 1 and 99,
        /// otherwise nothing happens.
        /// </summary>
        private void setSelected(char col, int row)
        {
            if (!Cell.validCol(col) || !Cell.validRow(row))
                return;

            saveCellContents();

            currentCell.col = col;
            currentCell.row = row;
            spreadsheetPanel.SetSelection(col - 'A', row - 1);

            cellNameLabel.Text = currentCell.name;
            contentsTextBox.Text = sheet.GetCellContents(currentCell.name).ToString();
            cellValueTextBox.Text = sheet.GetCellValue(currentCell.name).ToString();
        }

        /// <summary>
        /// Saves the contents of the currently selected cell in the spreadsheet.
        /// </summary>
        private void saveCellContents()
        {
            sheet.SetContentsOfCell(currentCell.name, contentsTextBox.Text);
        }

        /// <summary>
        /// Called when the selected cell in the spreadsheet is changed.
        /// </summary>
        private void SpreadsheetPanel_SelectionChanged(SpreadsheetPanel sender)
        {
            int row, col;
            sender.GetSelection(out col, out row);
            setSelected((char)(col + 'A'), row + 1);
        }

        /// <summary>
        /// Called when the New button is pressed or its shortcut called.
        /// Creates a new empty spreadsheet in its own window.
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            SpreadsheetAppContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        /// <summary>
        /// Called when the Open button is pressed or its shortcut called.
        /// Opens a spreadsheet from a file replacing the current spreadsheet.
        /// If this would result in a loss of unsaved data, prompts the user to save.
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Called when the Close button is pressed.
        /// Closes this spreadsheet window. If this is the last window, the application
        /// should be terminated.
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Called when the Save button is pressed or its shortcut called.
        /// Saves the current spreadsheet to a file.
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Called when the About button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
