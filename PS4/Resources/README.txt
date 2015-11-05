Authors:	Eric Longberg and Tivinia Pohahau
Last updated:	November 3, 2015


DLLs:
DependencyGraph was taken from a version post-grading and -fixing on 9/28/15, from PS3.
Formula was taken from a version post-grading and -fixing on 10/6/15, from PS2.
SpreadsheetPanel comes from course repository PS6 skeleton, revision from 10/20/15.


Design thoughts:
To interact between the panel and underlying spreadsheet, we have an inner Cell class and several associated methods.
Any such interactions are expected to go through these methods. For example: saving to the underlying spreadsheet is to happen
only when a movement to another cell is attempted. This includes hitting Enter or Tab or an arrow key or clicking on another cell.
Saving to file is to be prompted whenever data would be lost and the user attempts to close the spreadsheet/application or open a previously-saved file.
An asterisk * by the title will denote when a spreadsheet has unsaved changes.

Additionally, we have a basic graphing feature (F3 to toggle). This takes the values of cells in column A as X-variables and pairs them by row
to the values of cells in column B as Y-variables, connecting the dots to form a "line." The basis for all of this comes from the built-in Chart class.

Focus is to remain on the Contents Textbox except in the following circumstances:
--The menu bar is selected.
--The Value Textbox is selected (e.g. for the purpose of copying the value of a cell).

Keyboard shortcuts:
Ctrl + S					-- Open a menu for saving the spreadsheet to file.
Ctrl + O					-- Prompt saving (if needed) and open a menu for opening a saved spreadsheet in the current file.
Ctrl + N					-- Open a new spreadsheet in a new window.
Shift + Down or Enter		-- Move down.
Shift + Up or Shift + Enter -- Move up.
Shift + Right or Tab		-- Move right.
Shift + Left or Shift + Tab -- Move left.
F3							-- Toggle the visibility of the graph.