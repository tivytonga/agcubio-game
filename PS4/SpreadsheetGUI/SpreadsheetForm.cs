using SpreadsheetUtilities;
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

            /// <summary>
            /// Creates a cell with the name.
            /// Throws ArgumentException if name is invalid (not in range A1->Z99).
            /// </summary>
            public Cell(string name)
            {
                if (!isValid.IsMatch(name))
                    throw new ArgumentException();
                this.name = name;
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
                        _col = value;
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
                        _row = value;
                }
            }

            /// <summary>
            /// The row as 0-based.
            /// </summary>
            public int rowAsPanelIndex
            {
                get { return row - 1; }
            }

            /// <summary>
            /// The col as 0-based.
            /// </summary>
            public int colAsPanelIndex
            {
                get { return col - 'A'; }
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
        /// Returns true if new cell is a valid location and old cell contained valid contents.
        /// </summary>
        private bool setSelected(char col, int row)
        {
            if (!Cell.validCol(col) || !Cell.validRow(row))
            {
                spreadsheetPanel.SetSelection(currentCell.colAsPanelIndex, currentCell.rowAsPanelIndex);
                return false;
            }

            if (!saveCellContents())
            {
                spreadsheetPanel.SetSelection(currentCell.colAsPanelIndex, currentCell.rowAsPanelIndex);
                return false;
            }

            currentCell.col = col;
            currentCell.row = row;
            spreadsheetPanel.SetSelection(currentCell.colAsPanelIndex, currentCell.rowAsPanelIndex);
            return true;
        }

        /// <summary>
        /// Saves the contents of the currently selected cell in the spreadsheet,
        /// also updating the values displayed on the grid. Returns true if this happens
        /// succesfully, or false if an invalid formula had been entered, in which case
        /// a MessageBox explaining this is shown.
        /// </summary>
        private bool saveCellContents()
        {
            HashSet<string> needsUpdate;
            try {
                needsUpdate = new HashSet<string>(sheet.SetContentsOfCell(currentCell.name, cellContentsTextBox.Text));
            }
            catch (CircularException)
            {
                MessageBox.Show("The formula entered causes a circular dependency and is therefore invalid.", "Invalid Formula.");
                return false;
            }
            catch (FormulaFormatException e)
            {
                MessageBox.Show(e.Message, "Invalid Formula.");
                return false;
            }
            /*
            Debug.WriteLine("Dependents:");
            foreach (string name in needsUpdate)
            {
                Cell cell = new Cell(name);
                spreadsheetPanel.SetValue(cell.colAsPanelIndex, cell.rowAsPanelIndex, getCellValue(cell));

                Debug.WriteLine(cell.name);
                string o;
                spreadsheetPanel.GetValue(cell.colAsPanelIndex, cell.rowAsPanelIndex, out o);
                Debug.WriteLine("Cell: "+cell.name);
                Debug.WriteLine("Displayed value: "+o);
                Debug.WriteLine("Internal value: "+getCellValue(cell));
                Debug.WriteLine("");
                
            }
            Debug.WriteLine("");
            */
            // Temporary(?) fix for an error, can achieve as follows after replacing this foreach loop with the above commented:
            /*Click B1, contents "=A1", click B1 to update,
			change A1 at any point, B1 does not update (and loses
			Dependence on A1 somehow, until clicking on B1 again) */
            foreach (string name in sheet.GetNamesOfAllNonemptyCells())
            {
                Cell cell = new Cell(name);
                spreadsheetPanel.SetValue(cell.colAsPanelIndex, cell.rowAsPanelIndex, getCellValue(cell));
            }

            return true;
        }

        /// <summary>
        /// Returns the value of the given cell as a string. If this is
        /// an invalid formula, then returns the reason as a string.
        /// </summary>
        private string getCellValue(Cell cell)
        {
            object val = sheet.GetCellValue(cell.name);
            if (val is FormulaError)
                return ((FormulaError)val).Reason;
            return val.ToString();
        }

        /// <summary>
        /// Returns the contents of the given cell as a string. If this is
        /// a Formula, then prepends "=".
        /// </summary>
        private string getCellContents(Cell cell)
        {
            object content = sheet.GetCellContents(cell.name);
            if (content is Formula)
                return "=" + content.ToString();
            return content.ToString();
        }

        /// <summary>
        /// Called when the selected cell in the spreadsheet is changed.
        /// Saves the previously selected cell, updates the current selection if new cell
        /// is valid and old cell contains valid contents.
        /// </summary>
        private void SpreadsheetPanel_SelectionChanged(SpreadsheetPanel sender)
        {
            int row, col;
            sender.GetSelection(out col, out row);
            if (!setSelected((char)(col + 'A'), row + 1))
                return;

            cellNameLabel.Text = currentCell.name;
            cellContentsTextBox.Text = getCellContents(currentCell);
            cellValueTextBox.Text = getCellValue(currentCell);
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
