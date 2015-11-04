﻿using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
        /// Handles the data plotting.
        /// </summary>
        Graph graph;

        /// <summary>
        /// Regex to match possible cells A1->Z99.
        /// </summary>
        protected static Regex isValid = new Regex(@"^[A-Z][1-9][0-9]{0,1}$");

        /// <summary>
        /// True if the underlying spreadsheet has been changed.
        /// </summary>
        private bool Changed { get { return sheet.Changed; } }

        /// <summary>
        /// Sets the title of this spreadsheet as "Spreadsheet -- filename" where
        /// filename is the provided string. An empty string will result in "Spreadsheet". 
        /// </summary>
        private void setTitle(string name)
        {
            string title = "Spreadsheet";
            if (name != "" && name != null)
                title += " -- " + name;
            Text = title;
        }

        /// <summary>
        /// If the spreadsheet has changed since being opened or saved, ensures an asterisk * appears
        /// at the end of the title. If a file has not been changed, ensures the opposite.
        /// </summary>
        private void checkTitleChanged()
        {
            if (Changed && Text[Text.Length - 1] != '*')
                Text += "*";
            else if (!Changed)
                setTitle(filename);
        }

        private string filename;

        /// <summary>
        /// Creates a new blank SpreadsheetForm.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

            // Treat strings as case-insensitive, and use version "ps6"
            sheet = new Spreadsheet(s => isValid.IsMatch(s), s => s.ToUpper(), "ps6");

            spreadsheetPanel.SelectionChanged += SpreadsheetPanel_SelectionChanged;
            cellContentsTextBox.PreviewKeyDown += CellContentsTextBox_PreviewKeyDown;

            graph = new Graph(chart);
            chart.Visible = false;

            currentCell = new Cell('A', 1);
            setSelected('A', 1);
            setTitle("");
        }

        public SpreadsheetForm(string filename)
        {
            this.filename = filename;
        }

        /// <summary>
        /// Class for handling graphing data.
        /// </summary>
        private class Graph{
            private Chart chart;
            private DataPointCollection points;
            private SortedSet<double> xVals;
            
            /// <summary>
            /// Starts our graph using the underlying Chart.
            /// </summary>
            public Graph(Chart c)
            {
                chart = c;
                points = c.Series[0].Points;
                xVals = new SortedSet<double>();
                Clear();
            }

            /// <summary>
            /// Clears the graph of any data points.
            /// </summary>
            public void Clear()
            {
                points.Clear();
                xVals.Clear();
            }

            /// <summary>
            /// Sets the graph data to be all direct x,y pairs from the given data that are both doubles (as strings) and not already in the graph.
            /// If one set is bigger than the other, the corresponding elements without matches are not graphed.
            /// </summary>
            public void SetData(List<string> xData, List<string> yData)
            {
                Clear();
                Dictionary<double, double> data = new Dictionary<double, double>();
                
                for (int i = 0; i < xData.Count; i++)
                {
                    double xDub, yDub;
                    if (Double.TryParse(xData[i], out xDub) && Double.TryParse(yData[i], out yDub))
                    {
                        data.Add(xDub, yDub);
                        xVals.Add(xDub);
                    }
                }

                foreach (double x in xVals)
                {
                    points.AddXY(x, data[x]);
                }
            }

            /// <summary>
            /// Toggles the visibility of the graph.
            /// </summary>
            public void ToggleVisible()
            {
                chart.Visible = !chart.Visible;
            }
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

            /// <summary>
            /// The name, e.g. B12
            /// </summary>
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

            /// <summary>
            /// The column, like "C13" --> 'C'
            /// </summary>
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

            /// <summary>
            /// The row, like "C13" --> 13
            /// </summary>
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

            /// <summary>
            /// True if row between 1 and 99 inclusive.
            /// </summary>
            public static bool validRow(int r)
            {
                return (r >= 1 && r <= 99);
            }

            /// <summary>
            /// True is col between 'A' and 'Z' inclusive.
            /// </summary>
            public static bool validCol(char c)
            {
                return (c >= 'A' && c <= 'Z');
            }
        }

        /// <summary>
        /// Loads a spreadsheet from a file
        /// </summary>
        private void spreadsheetPanel_Load(object sender, EventArgs e)
        {
            try {
                // Open chosen Spreadsheet file
                sheet = new Spreadsheet(this.filename, s => true, s => s, "");

                // Clear the current Spreadsheet file
                spreadsheetPanel.Clear();

                // Read through the Spreadsheet file
                foreach (string cell in sheet.GetNamesOfAllNonemptyCells())
                {
                    // Set the name of the cell
                    cellNameLabel.Text = cell;

                    // Set the contents of the cell
                    cellContentsTextBox.Text = sheet.GetCellContents(cell).ToString();

                    // Set the value of the cell
                    cellValueTextBox.Text = sheet.GetCellValue(cell).ToString();

                }
            }
            catch(Exception)
            {

            }
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
                string val = getCellValue(cell);
                spreadsheetPanel.SetValue(cell.colAsPanelIndex, cell.rowAsPanelIndex, val);
            }
            spreadsheetPanel.SetValue(currentCell.colAsPanelIndex, currentCell.rowAsPanelIndex, getCellValue(currentCell));

            updateGraph();
            
            return true;
        }

        /// <summary>
        /// Updates the graph with all data from columns A and B.
        /// </summary>
        private void updateGraph()
        {
            List<string> xData = new List<string>();
            List<string> yData = new List<string>();
            
            for (int row = 1; row < 100; row ++)
            {
                xData.Add(getCellValue(new Cell('A', row)));
                yData.Add(getCellValue(new Cell('B', row)));
            }

            graph.SetData(xData, yData);
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
        /// is valid and old cell contains valid contents. Updates the title if a change has occurred.
        /// </summary>
        private void SpreadsheetPanel_SelectionChanged(SpreadsheetPanel sender)
        {
            int row, col;
            sender.GetSelection(out col, out row);
            ChangeSelection((char)(col + 'A'), row + 1);
        }

        /// <summary>
        /// Call to move current cell to given col, row pair and update and save the previous cell.
        /// </summary>
        private void ChangeSelection(char col, int row)
        {
            if (!setSelected(col, row))
                return;

            checkTitleChanged();
            cellNameLabel.Text = currentCell.name;
            cellContentsTextBox.Text = getCellContents(currentCell);
            cellValueTextBox.Text = getCellValue(currentCell);
            cellContentsTextBox.Focus();
        }

        /// <summary>
        /// Saves the current contents and moves the selection in the given direction
        /// (up, down, left, right) if possible.
        /// </summary>
        private void MoveRelative(Keys dir)
        {
            char oldCol = currentCell.col;
            int oldRow = currentCell.row;
            char newCol = oldCol;
            int newRow = oldRow;
            switch (dir)
            {
                case Keys.Up:
                    newRow = oldRow - 1;
                    if (!Cell.validRow(newRow))
                        newRow = oldRow;
                    break;

                case Keys.Down:
                    newRow = oldRow + 1;
                    if (!Cell.validRow(newRow))
                        newRow = oldRow;
                    break;

                case Keys.Left:
                    newCol = (char)(oldCol - 1);
                    if (!Cell.validCol(newCol))
                        newCol = oldCol;
                    break;

                case Keys.Right:
                    newCol = (char)(oldCol + 1);
                    if (!Cell.validCol(newCol))
                        newCol = oldCol;
                    break;

                default: // Invalid Key
                    return;
            }

            ChangeSelection(newCol, newRow);
        }


        /// <summary>
        /// Called upon a KeyDown event in the cellContentsTextBox.
        /// 
        /// If key code is Enter, move to the next cell downwards.
        /// If key code is Tab, move to the next cell rightwards.
        /// If Shift key is also pressed, then these movements are reversed.
        /// If Shift and an arrow key is pressed, moves in the pressed direction.
        /// If movement would result in an invalid cell, then no movement will occur.
        /// In any of the above cases, attempts to save the contents in the cell.
        /// 
        /// If F3 is pressed, the visibility of the graph is toggled.
        /// </summary>
        private void CellContentsTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
                MoveRelative(Keys.Up);
            else if (e.KeyCode == Keys.Enter)
                MoveRelative(Keys.Down);
            else if (e.Shift && e.KeyCode == Keys.Tab)
                MoveRelative(Keys.Left);
            else if (e.KeyCode == Keys.Tab)
                MoveRelative(Keys.Right);
            else if (e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        MoveRelative(e.KeyCode);
                        break;

                    default:
                        return;
                }
            }
            else if (e.KeyCode == Keys.F3)
                    graph.ToggleVisible();
        }

        /// <summary>
        /// Called when the New button is pressed or its shortcut called.
        /// Creates a new empty spreadsheet in its own window.
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If user hits the keys "CTRL + N"
            // Tell the application context to run the form on the same
            // thread as the other forms.
            KeyEventArgs keyStroke = new KeyEventArgs(Keys.Control);
            if (keyStroke.Control && keyStroke.KeyCode == Keys.N)
                SpreadsheetAppContext.getAppContext().RunForm(new SpreadsheetForm());

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
            // Keeps track of whether user wants to open a chosen Spreadsheet file
            bool openSpreadsheet = false;

            // Create an instance of the open file dialog box
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter for types of files allowed to open,
            //    title of the dialog, and multiselection option
            openFileDialog.Filter = "Spreadsheet Files (.sprd)|*.sprd|All Files(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Title = "Select a Spreadsheet file to open";
            openFileDialog.Multiselect = false;

            // Need to check if current Spreadsheet has been saved before opening a new Spreadsheet

            // If user hits the keys "CTRL + O"
            KeyEventArgs keyStroke = new KeyEventArgs(Keys.Control);
            
            if (keyStroke.Control && keyStroke.KeyCode == Keys.O)
                openFileDialog.ShowDialog();

            // Call ShowDialog method to show the dialog box and keep track of user's actions
            //   (they click 'OK' or 'Cancel'
            DialogResult userActions = openFileDialog.ShowDialog();

            // User chooses a file and clicks "OK" (Actually shows up as "Open" on button)
            if (userActions == DialogResult.OK)
            {

                //TODO only show message if spreadsheet changed
                // Pop up a message to ask user if they want to save current Spreadsheet before opening a new one
                // Set up the look of the message box, message, and buttons
                string prompt = "There are unsaved changes in your Spreadsheet. Would you like to save " +
                                    "current changes before opening a new Spreadsheet?";
                MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                DialogResult userResult = MessageBox.Show(prompt, "", buttons, icon);
                
                // Perform actions based on the button user chooses
                switch (userResult)
                {
                    // User selects Yes: Save current Spreadsheet and then open the chosen file
                    case DialogResult.Yes:
                        {
                            openSpreadsheet = true;
                            saveToolStripMenuItem_Click(sender, e);
                            // TODO Open selected Spreadsheet file
                            break;
                        }

                    // User selects No: Close current Spreadsheet. Open selected Spreadsheet file
                    case DialogResult.No:
                        {
                            openSpreadsheet = true;
                            closeToolStripMenuItem_Click(sender, e);
                            // TODO Open selected Spreadsheet file
                            break;
                        }
                    // User selects Cancel: Close out message box. Do not save or open a new Spreadsheet.
                    case DialogResult.Cancel:
                        { 
                            break;
                        }
                }

                // Name of file user wants to open
                this.filename = openFileDialog.FileName;

                // Open the user's chosen Spreadsheet file
                if (openSpreadsheet)
                {
                    spreadsheetPanel_Load(sender, e);

                    // Open chosen Spreadsheet file
                    //sheet = new Spreadsheet(openFileDialog.FileName, s => true, s => s, "ps6");

                    // Clear the current Spreadsheet file
                    //spreadsheetPanel.Clear();

                    // Read through the Spreadsheet file


                }
            }

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

            // Create an instance of the save file dialog box
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // If user hits the keys "CTRL + S"
            KeyEventArgs keyStroke = new KeyEventArgs(Keys.Control);

            if (keyStroke.Control && keyStroke.KeyCode == Keys.S)
                saveFileDialog.ShowDialog();

            // Set filter for saving a Spreadsheet
            saveFileDialog.Filter = "Spreadsheet Files (.sprd)|*.sprd|All Files(*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.Title = "Save Spreadsheet As...";
            
            // Call ShowDialog method to show the dialog box and keep track of user's actions
            //   (they click 'OK' or 'Cancel'
            DialogResult userActions = saveFileDialog.ShowDialog();

            // TODO Save the file
            // if the filename does not end with .sprd
            if (saveFileDialog.FileName != "")
            {
                string saveFilename = saveFileDialog.FileName;

                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        {
                            if (!saveFilename.EndsWith(".sprd"))
                                saveFilename += ".sprd";
                            sheet.Save(saveFilename);
                            break;
                        }
                    case 2:
                        {
                            sheet.Save(saveFilename);
                            break;
                        }
                }

                setTitle(saveFilename);
                filename = saveFilename;
                checkTitleChanged();
            }
            
        }

        /// <summary>
        /// Called when the About button is pressed.
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
