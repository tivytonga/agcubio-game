Author:	Eric Longberg 
Last updated:	October 15, 2015

The DLLs are taken from PS2 and PS3.
DependencyGraph was taken from a version post-grading and -fixing on 9/28/15.
Formula was taken from a version post-grading and -fixing on 10/6/15.

Design thoughts: The idea of this spreadsheet representation will be to have
a dictionary mapping names of cells to cells themselves (which will hold the content and values of the cells).
We will use a dependency graph to make sure any cells containing formulas do not cause a circular dependency.
Cells contain their contents, which are used to determine actual value (i.e. a Formula would be content, value would be numerical result).

The XML files used for testing purposes will be stored in the SpreadsheetTests project in the "TestXMLs" folder.