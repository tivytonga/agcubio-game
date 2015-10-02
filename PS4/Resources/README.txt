Author:	Eric Longberg 
Date:	October 1, 2015

The DLLs (Formula and DependencyGraph) are taken from versions of PS2 and PS3 on 9/28/15.
Design thoughts: The idea of this spreadsheet representation will be to have
a dictionary mapping names of cells to cells themselves (which will hold the content of the cells).
We will use a dependency graph to make sure any cells containing formulas do not cause a circular dependency.