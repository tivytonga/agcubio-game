﻿using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

// Eric Longberg
// CS 3500, PS5
// October 15, 2015
namespace SS
{
    /// <summary>
    /// A Spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> nameToCell;
        private DependencyGraph dependencies;
        private bool changed;

        /// <summary>
        /// Creates a new spreadsheet with no extra validity conditions, a normalizer
        /// from every string to itself, and "default" version.
        /// </summary>
        public Spreadsheet()
            : this(s => true, s => s, "default")
        {
        }

        // ADDED FOR PS5
        /// <summary>
        /// Constructs a spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            nameToCell = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            changed = false;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Reads a spreadsheet from the given file, and uses the given validity, normalization, and version.
        /// 
        /// Throws SpreadsheetReadWriteException on any of the following:
        /// The version of the saved spreadsheet does not match the given version parameter.
        /// Any names in the spreadsheet are invalid.
        /// Invalid formulas or circular dependencies are encountered.
        /// Problems opening/reading/closing the file.
        /// anything else
        /// </summary>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            nameToCell = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();

            try
            {
                // Setup XML settings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                // Read from the given file
                using (XmlReader reader = XmlReader.Create(filePath, settings))
                {

                    // ignore the <?xml version="1.0" encoding="utf-8"?> part
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        reader.Read();

                    // Go through opening <spreadsheet version="version info">

                    if (reader.Name != "spreadsheet")
                        throw new SpreadsheetReadWriteException("Invalid format (first element of xml file was not \"spreadsheet\").");

                    reader.MoveToNextAttribute();
                    if (reader.Name != "version")
                        throw new SpreadsheetReadWriteException("Spreadsheet did not have \"version\" as first and only attribute.");
                    if (reader.Value != version)
                        throw new SpreadsheetReadWriteException("Saved spreadsheet version does not match the given version parameter.");

                    string name;
                    string contents;

                    // Read through all the cells. Must take the form:
                    // <cell> <name>...</name> <contents>...</contents> </cell>
                    while (reader.Read())
                    {
                        // <cell>
                        if (reader.Name == "cell")
                        {
                            reader.Read();

                            // <name>
                            if (reader.Name == "name")
                            {
                                reader.Read();
                                name = reader.Value;
                                reader.Read();

                                // </name>
                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "name")
                                {
                                    reader.Read();

                                    // <contents>
                                    if (reader.Name == "contents")
                                    {
                                        reader.Read();
                                        contents = reader.Value;
                                        reader.Read();

                                        // </contents>
                                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "contents")
                                        {
                                            reader.Read();

                                            // </cell>
                                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "cell")
                                            {
                                                SetContentsOfCell(name, contents);
                                                continue;
                                            }
                                            else
                                                throw new SpreadsheetReadWriteException("Expected close of cell element but detected: " + reader.Name);
                                        }
                                        else
                                            throw new SpreadsheetReadWriteException("Expected close of contents element but detected: " + reader.Name);
                                    }
                                    else
                                        throw new SpreadsheetReadWriteException("Expected open of contents element but detected: " + reader.Name);
                                }
                                else
                                    throw new SpreadsheetReadWriteException("Expected close of name element but detected: " + reader.Name);
                            }
                            else
                                throw new SpreadsheetReadWriteException("Expected open of name element detected: " + reader.Name);
                        }

                        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "spreadsheet")
                        {
                            continue;
                        }

                        else
                            throw new SpreadsheetReadWriteException("Expected beginning of cell element or close of spreadsheet but detected: " + reader.Name);
                    }
                }
            }
            
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("File could not be found.");
            }
            catch (DirectoryNotFoundException)
            {
                throw new SpreadsheetReadWriteException("File could not be found.");
            }
            catch (InvalidOperationException)
            {
                // File could not be read, line missing
                throw new SpreadsheetReadWriteException("Invalid file format.");
            }
            catch (XmlException)
            {
                // File could not be read, line missing
                throw new SpreadsheetReadWriteException("Invalid file format.");
            }
            catch (CircularException)
            {
                throw new SpreadsheetReadWriteException("Circular dependency detected.");
            }

            // Start off as unchanged
            changed = false;
        }

        /// <summary>
        /// Represents a single Cell in the spreadsheet. It contains content (string, double, or Formula)
        /// as well as value (string, double, or FormulaError).
        /// If the content is an empty string, the cell is considered empty.
        /// 
        /// Assumes e.g. the user will not ask for the content as a double if said content is not a double.
        /// </summary>
        private class Cell
        {
            public Object content { get; private set;}

            /// <summary>
            /// Creates a new Cell with the given content.
            /// </summary>
            public Cell(Object content)
            {
                this.content = content;
            }

            /// <summary>
            /// Returns true if content is an empty string.
            /// </summary>
            public bool isEmpty()
            {
                if (content is String)
                    return (string)content == "";
                return false;
            }

            /// <summary>
            /// Returns the value (as opposed to the contents) of the cell.  The return
            /// value is either a string, a double, or a SpreadsheetUtilities.FormulaError.
            /// </summary>
            public Object getValue(Func<string, double> lookup)
            {
                if (content is Formula)
                {
                    return ((Formula)content).Evaluate(lookup);
                }
                return content;
            }

            public override string ToString()
            {
                return content.ToString();
            }

        }

        /// <summary>
        /// Intended for use in Formula.Evaluate(lookup).
        /// Returns the double value of a given (normalized) variable (if it has one), otherwise
        /// throws an ArgumentException.
        /// </summary>
        private double lookup(string var)
        {
            Object val = GetCellValue(var);

            if (!(val is Double))
                throw new ArgumentException();

            return (double)val;
        }

        /// <summary>
        /// Throws an InvalidNameException if name is null or does not follow naming requirements:
        /// One or more letters followed by one or more digits.
        /// </summary>
        private void checkValidName(string name)
        {
            if (name == null || !Regex.IsMatch(name, @"^[a-zA-Z]+\d+$"))
                throw new InvalidNameException();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return nameToCell.Keys;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            checkValidName(name);
            name = Normalize(name);
            if (!IsValid(name))
                throw new InvalidNameException();
            Cell cell;
            if (nameToCell.TryGetValue(name, out cell))
                return cell.content;
            return ""; // All cells start as empty

        }

        /// <summary>
        /// Removes the cell with the given name from the dictionary, and if it is a formula
        /// then removes any dependencies necessary.
        /// </summary>
        private void Remove(string name)
        {
            if (nameToCell.ContainsKey(name))
            {
                object contents = GetCellContents(name);
                if (contents is Formula)
                {
                    foreach (string dependee in ((Formula)contents).GetVariables())
                    {
                        dependencies.RemoveDependency(dependee, name);
                    }
                }

                nameToCell.Remove(name);
            }
        }

        /// <summary>
        /// A convenience method for the SetCellContents.
        /// Checks a name is valid and not null, sets the cell's contents, and returns
        /// the set of dependents.
        /// </summary>
        private ISet<string> SetCell(string name, object content)
        {
            checkValidName(name);
            object oldContent = GetCellContents(name);
            if (content.GetType() == oldContent.GetType() && oldContent.ToString() == content.ToString())
                return new HashSet<string>(GetCellsToRecalculate(name));

            Remove(name);
            // If non-empty, we will add it
            if (!(content is string && (string)content == ""))
            {
                nameToCell.Add(name, new Cell(content));
                Changed = true;
            }

            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            return SetCell(name, number);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            return SetCell(name, text);
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException();
            checkValidName(name);
            object oldContent = GetCellContents(name);
            if (oldContent is Formula && formula == (Formula)oldContent)
                return new HashSet<string>(GetCellsToRecalculate(name));

            // Test if putting this Formula into the given cell will cause a CircularException.
            foreach (string dependent in GetCellsToRecalculate(name))
            {
                foreach (string dependee in formula.GetVariables())
                {
                    if (dependent == dependee)
                        throw new CircularException();
                }
            }

            // We are now fine to add this, just update all the dependencies
            foreach (string dependee in formula.GetVariables())
            {
                dependencies.AddDependency(dependee, name);
            }
            Remove(name);
            nameToCell.Add(name, new Cell(formula));
            Changed = true;
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
                throw new ArgumentNullException();
            checkValidName(name);
            return dependencies.GetDependents(name);
        }

        // ADDED FOR PS5
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return changed;
            }
            protected set
            {
                changed = value;
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {

                // Setup XML settings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;

                // Read from the given file
                using (XmlReader reader = XmlReader.Create(filename, settings))
                {
                    // ignore the <?xml version="1.0" encoding="utf-8"?> part
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                        reader.Read();

                    // Go through opening <spreadsheet version="version info">
                    if (reader.Name != "spreadsheet")
                        throw new SpreadsheetReadWriteException("Invalid file format (first element of xml file was not \"spreadsheet\").");

                    reader.MoveToNextAttribute();
                    if (reader.Name != "version")
                        throw new SpreadsheetReadWriteException("Spreadsheet did not have \"version\" as first and only attribute.");

                    return reader.Value;
                }
            }
            catch
            {
                throw new SpreadsheetReadWriteException("Invalid file format.");
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);

                    foreach (string name in GetNamesOfAllNonemptyCells())
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteStartElement("name");
                        writer.WriteValue(name);
                        writer.WriteEndElement();

                        writer.WriteStartElement("contents");
                        Object content = GetCellContents(name);
                        if (content is Formula)
                        {
                            content = "=" + content.ToString();
                        }
                        writer.WriteValue(content.ToString());
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                Changed = false;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Unknown error: " + e);
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            checkValidName(name);
            name = Normalize(name);
            if (!IsValid(name))
                throw new InvalidNameException();
            Cell cell;
            if (nameToCell.TryGetValue(name, out cell))
                return cell.getValue(lookup);
            return ""; // default value is empty string
        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
                throw new ArgumentNullException();
            checkValidName(name);
            name = Normalize(name);
            if (!IsValid(name))
                throw new InvalidNameException();
            
            double d;
            if (Double.TryParse(content, out d))
                return SetCellContents(name, d);

            if (content.Length > 0 && content[0] == '=')
            {
                content = content.Split('=')[1];
                Formula f = new Formula(content, Normalize, IsValid);
                return SetCellContents(name, f);
            }

            return SetCellContents(name, content);
        }
    }
}
