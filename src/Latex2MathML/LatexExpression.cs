/*  
    This file is part of Latex2MathML.

    Latex2MathML is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Latex2MathML is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Latex2MathML.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Latex2MathML
{
    /// <summary>
    /// Serves as a node in a document object tree. Parses the tree from a TextReader instance.
    /// </summary>
    internal class LatexExpression
    {        
        /// <summary>
        /// Gets or sets the name or data of the expression
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the options of the expression.
        /// </summary>
        public ExpressionOptions Options { get; set; }

        /// <summary>
        /// Gets the child subtrees of the expression.
        /// </summary>
        public List<List<LatexExpression>> Expressions { get; set; }

        /// <summary>
        /// Gets or sets the parent of this expression.
        /// </summary>
        public LatexExpression Parent { get; set; }

        /// <summary>
        /// Gets or sets some object associated with this LatexExpression instance.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the child subtree number in the parent.
        /// </summary>
        public int ParentChildNumber { get; set; }

        /// <summary>
        /// Gets or sets the index in the child subtree in the parent.
        /// </summary>
        public int IndexInParentChild { get; set; }

        /// <summary>
        /// Gets or sets the expression type.
        /// </summary>
        public ExpressionType ExprType { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this expression is in the math mode.
        /// </summary>
        public bool MathMode { get; set; }

        /// <summary>
        /// Gets or sets the LatexToMathMLConverter class instance to customize the conversion result.
        /// </summary>
        public LatexToMathMLConverter Customization { get; set; }

        /// <summary>
        /// Initializes a new instance of the LatexExpression class.
        /// </summary>
        /// <param name="parent">The parent of the builded expression.</param>
        /// <param name="parentChildNumber">Index of the parent child outline.</param>
        /// <param name="indexInParentChild">Index in the parent child outline.</param>
        /// <param name="verbatimMode">True if verbatim mode is on; otherwise, false.</param>
        /// <param name="type">The expression type.</param>
        /// <param name="mathMode">The math mode switch.</param>
        /// <param name="name">The expression name.</param>
        /// <param name="options">The options of the expression.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        /// <param name="values">The child outlines of the expression.</param>
        private LatexExpression(LatexExpression parent, int parentChildNumber, ref int indexInParentChild,
            ref bool verbatimMode, ExpressionType type, bool mathMode, LatexToMathMLConverter customization,
            string name, string options, params string[] values)
        {
            Parent = parent;
            ParentChildNumber = parentChildNumber;
            IndexInParentChild = indexInParentChild++;
            Customization = customization;            
            Name = name;
            ExprType = type;
            MathMode = mathMode;
            #region Switch verbatim mode on/off
            if (type == ExpressionType.Verbatim)
            {
                verbatimMode = false;
            }
            if (Expressions != null && Name == "begin" &&
                Expressions[0][0].Name == "verbatim" && Expressions.Count == 1)
            {
                verbatimMode = true;
            }
            #endregion
            ParseOptions(options);            
            if (values != null)
            {
                ParseExpressions(values);
            }
            #region If a math block, add the alternative text
            if (type == ExpressionType.InlineMath || type == ExpressionType.BlockMath)
            {
                // ReSharper disable PossibleNullReferenceException
                Expressions.Add(new List<LatexExpression>());
                int index = 0;
                Expressions[1].Add(new LatexExpression(this, 1, ref index, ref verbatimMode,
                    ExpressionType.PlainText, false, Customization, values[0], null, null));
                // ReSharper restore PossibleNullReferenceException
            }
            #endregion
            #region Post-parse plain text if in math mode
            if (ExprType == ExpressionType.PlainText && mathMode && Name.Length > 1 &&
                (Parent.ExprType != ExpressionType.Block || Parent.Name != "PlainText") &&
                (Parent.ExprType != ExpressionType.Command || Parent.Name != "begin" && Parent.Name != "end"))
            {
                ExprType = ExpressionType.Block;
                Name = "PlainText";
                #region Parse
                var list = new List<string>();
                var buf = "" + name[0];
                for (int pos = 1; pos < name.Length; pos++)
                {
                    var chrPre = name[pos - 1];
                    var chr = name[pos];
                    if (char.IsWhiteSpace(chr))
                    {
                        list.Add(buf);
                        buf = "";
                        continue;
                    }
                    if (char.IsDigit(chr))
                    {
                        #region Digit
                        if (char.IsDigit(chrPre))
                        {
                            buf += chr;
                            continue;
                        }
                        if (char.IsLetter(chr) || chr == '(')
                        {
                            list.Add("InvisibleTimes");
                        }
                        list.Add(buf);
                        buf = "" + chr;
                        continue;

                        #endregion
                    }
                    if (char.IsLetter(chr))
                    {
                        #region Letter
                        if (char.IsLetter(chrPre))
                        {
                            buf += chr;
                            continue;
                        }
                        if (char.IsDigit(chr) || chr == '(')
                        {
                            list.Add("InvisibleTimes");
                        }
                        list.Add(buf);
                        buf = "" + chr;
                        continue;

                        #endregion
                    }
                    #region >=, <=
                    if (chr == '=')
                    {
                        if (chrPre == '>' || chrPre == '<')
                        {
                            buf += chr;
                        }
                    }
                    #endregion
                    list.Add(buf);
                    buf = "" + chr;
                }                
                list.Add(buf);
                #endregion
                Expressions = new List<List<LatexExpression>>(list.Count) {new List<LatexExpression>()};
                int i;
                for (i = 0; i < list.Count;)
                {
                    if (list[i].Trim() != "")
                    {
                        Expressions[0].Add(new LatexExpression(this, 0, ref i, ref verbatimMode,
                            ExpressionType.PlainText, true, customization, list[i].Trim(), null, null));                        
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Initializes a new instance of the LatexExpression class and copies the specified expression data to it.
        /// </summary>
        /// <param name="expression">The expression to clone.</param>
        public LatexExpression(LatexExpression expression)
        {
            Parent = expression.Parent;
            Name = expression.Name;
            ExprType = expression.ExprType;
            MathMode = expression.MathMode;
            Customization = expression.Customization;
            if (expression.Options != null)
            {
                Options = new ExpressionOptions();
            }
            if (expression.Expressions != null)
            {
                Expressions = new List<List<LatexExpression>>(expression.Expressions);
            }
        }

        /// <summary>
        /// Initializes a new instance of the LatexExpression class of type ExpressionType.Block.
        /// </summary>
        /// <param name="name">The name of the block.</param>
        /// <param name="parent">The parent of the builded expression.</param>
        /// <param name="parentChildNumber">Index of the parent child outline.</param>
        /// <param name="indexInParentChild">Index in the parent child outline.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        public LatexExpression(string name, LatexExpression parent, int parentChildNumber, int indexInParentChild, LatexToMathMLConverter customization)
        {
            Name = name;
            Customization = customization;
            ExprType = ExpressionType.Block;
            MathMode = parent.MathMode |
                parent.ExprType == ExpressionType.BlockMath | parent.ExprType == ExpressionType.InlineMath;
            Parent = parent;
            ParentChildNumber = parentChildNumber;
            IndexInParentChild = indexInParentChild;
            Expressions = new List<List<LatexExpression>> {new List<LatexExpression>()};
        }

        /// <summary>
        /// Recursively continues to build the document object tree.
        /// </summary>
        /// <param name="values">The child outlines to parse.</param>
        private void ParseExpressions(params string[] values)
        {
            if (values.Length > 0)
            {
                Expressions = new List<List<LatexExpression>>(values.Length);
            }
            for (int i = 0; i < values.Length; i++)
            {
                var list = new List<LatexExpression>();
                string beginning = values[i];
                string end;
                LatexExpression expr;
                int index = 0;
                bool verbatimMode = false;
                bool mathMode = MathMode;
                bool whitespaceBefore = false;
                while ((expr = ReadFromTextReader(this, i, ref index, ref verbatimMode,
                    mathMode | ExprType == ExpressionType.InlineMath | ExprType == ExpressionType.BlockMath,
                    Customization, beginning, null, out end, ref whitespaceBefore)) != null)
                {
                    CheckMathMode(expr, ref mathMode);          
                    beginning = end;
                    list.Add(expr);
                }               
                Expressions.Add(list);
            }
        }

        /// <summary>
        /// Preforms the post-parse procedure of the options.
        /// </summary>
        /// <param name="options">The raw options string to parse.</param>
        private void ParseOptions(string options)
        {
            if (options == null) { return; }
            var splittedOptions = options.Split(',');
            Options = new ExpressionOptions();
            Options.AsKeyValue = new Dictionary<string,string>(splittedOptions.Length);
            foreach (var option in splittedOptions)
            {
                var keyValue = option.Split('=');
                Options.AsKeyValue.Add(keyValue[0].Trim(), keyValue.Length > 1? keyValue[1].Trim() : null);
            }
            Options.AsExpressions = new List<LatexExpression>();
            string beginning = options;
            string end;
            LatexExpression expr;
            int index = 0;
            bool verbatimMode = false;
            bool whitespaceBefore = false;
            while ((expr = ReadFromTextReader(this, 0, ref index, ref verbatimMode,
                MathMode | ExprType == ExpressionType.InlineMath | ExprType == ExpressionType.BlockMath,
                Customization, beginning, null, out end, ref whitespaceBefore)) != null)
            {
                beginning = end;
                Options.AsExpressions.Add(expr);
            }
        }

        /// <summary>
        /// Checks for the beginning or the ending of a future block that must be parsed in math mode.
        /// </summary>
        /// <param name="expr">The suspected expression.</param>
        /// <param name="mathMode">The math mode flag.</param>
        private static void CheckMathMode(LatexExpression expr, ref bool mathMode)
        {
            if (!mathMode && expr.ExprType == ExpressionType.Command &&
                        expr.Expressions != null && expr.Name == "begin")
            {
                mathMode = BlockConverter.ImpliesMathMode(expr.Expressions[0][0].Name);
            }
            if (mathMode && expr.ExprType == ExpressionType.Command &&
                expr.Expressions != null && expr.Name == "end")
            {
                if (BlockConverter.ImpliesMathMode(expr.Expressions[0][0].Name))
                {
                    mathMode = false;
                }
            }     
        }

        /// <summary>
        /// Removes this expression from the parent's outline.
        /// </summary>
        public void EraseFromParent()
        {
            for (int i = IndexInParentChild + 1; i < Parent.Expressions[ParentChildNumber].Count; i++)
            {
                Parent.Expressions[ParentChildNumber][i].IndexInParentChild--;
            }
            Parent.Expressions[ParentChildNumber].Remove(this);
        }

        /// <summary>
        /// Parses the document and builds the document object tree.
        /// </summary>
        /// <param name="rdr">The reader to read the document from.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        /// <returns>The root of the document object tree.</returns>
        public static LatexExpression CreateRoot(TextReader rdr, LatexToMathMLConverter customization)
        {
            bool verbatimMode = false;
            int rootIndex = 0;
            var root = new LatexExpression(null, 0, ref rootIndex, ref verbatimMode,
                ExpressionType.Root, false, customization, null, null, null) 
            {
                Expressions = new List<List<LatexExpression>>(1)
            };
            var children = new List<LatexExpression>();            
            string beginning = null;
            string end;
            LatexExpression cmd;
            rootIndex = 0;
            bool mathMode = false;
            bool whitespaceBefore = false;
            while ((cmd = ReadFromTextReader(root, 0, ref rootIndex, ref verbatimMode,
                mathMode, customization, beginning, rdr, out end, ref whitespaceBefore)) != null)
            {
                CheckMathMode(cmd, ref mathMode);
                beginning = end;
                children.Add(cmd);               
            }
            root.Expressions.Add(children);
            return root;
        }

        /// <summary>
        /// Builds a new LatexExpression instance (front).
        /// </summary>
        /// <param name="parent">The parent of the builded expression.</param>
        /// <param name="parentChildNumber">Index of the parent child outline.</param>
        /// <param name="indexInParentChild">Index in the parent child outline.</param>
        /// <param name="verbatimMode">True if verbatim mode is on; otherwise, false.</param>
        /// <param name="beginning">The beginning string.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="end">The stub of the unparsed part.</param>
        /// <param name="mathMode">The math mode switch.</param>
        /// <param name="whitespaceBefore">Indicates whether there was at least one whitespace char before the returned result.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        /// <returns></returns>
        private static LatexExpression ReadFromTextReader(LatexExpression parent, int parentChildNumber, 
            ref int indexInParentChild, ref bool verbatimMode, bool mathMode, LatexToMathMLConverter customization,
            string beginning, TextReader rdr, out string end, ref bool whitespaceBefore)
        {
            if (beginning == null && rdr == null)
            {
                end = null;
                return null;
            }
            return ReadFromTextReaderInner(parent, parentChildNumber, ref indexInParentChild, ref verbatimMode,
                mathMode, customization, beginning ?? rdr.ReadLine(), rdr, out end, ref whitespaceBefore);
        }

        /// <summary>
        /// Builds a new LatexExpression instance (main).
        /// </summary>
        /// <param name="parent">The parent of the builded expression.</param>
        /// <param name="parentChildNumber">Index of the parent child outline.</param>
        /// <param name="indexInParentChild">Index in the parent child outline.</param>
        /// <param name="verbatimMode">True if verbatim mode is on; otherwise, false.</param>
        /// <param name="beginning">The beginning string.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="mathMode">The math mode switch.</param>
        /// <param name="end">The stub of the unparsed part.</param>
        /// <param name="whitespaceBefore">Indicates whether there was at least one whitespace char before the returned result.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        /// <returns>The parsed LatexExpression instance.</returns>
        private static LatexExpression ReadFromTextReaderInner(LatexExpression parent, int parentChildNumber,
            ref int indexInParentChild, ref bool verbatimMode, bool mathMode, LatexToMathMLConverter customization,
            string beginning, TextReader rdr, out string end, ref bool whitespaceBefore)
        {
            string name;

            #region Verbatim
            if (verbatimMode)
            {
                name = ReadToVerbatimEnd(beginning, rdr, out end);
                return new LatexExpression(parent, parentChildNumber, ref indexInParentChild,
                    ref verbatimMode, ExpressionType.Verbatim, false, customization, name, null, null);
            }
            #endregion

            bool whitespaceBeforeGuard;
            string str = ReadToNotEmptyString(beginning, rdr, out whitespaceBeforeGuard);
            whitespaceBefore |= whitespaceBeforeGuard;
            if (str == null)
            {
                end = null;
                return null;
            }
            string value;
            switch (str[0])
            {
                case '\\':
                    #region Command
                    string options;
                    string[] values;
                    name = Regex.Match(str.Substring(1), @"^[a-zA-Z]+\*?").Value;
                    if (String.IsNullOrEmpty(name))
                    {
                        name = "" + str[1];
                        options = null;
                        if (name != "[")
                        {
                            values = null;
                            end = str.Substring(2);
                        }
                        else // Math
                        {
                            value = ParseBraces(ref str, rdr, "\\[", "\\]", out whitespaceBefore);
                            end = str;
                            return new LatexExpression(parent, parentChildNumber, ref indexInParentChild,
                                ref verbatimMode, ExpressionType.BlockMath, mathMode, customization,
                                name, null, value);
                        }
                    }
                    else
                    {
                        ParseCommandOptionsAndValues(
                            str.Substring(name.Length + 1), rdr, out end,
                            out options, out values, out whitespaceBefore);
                    }                    
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild, ref verbatimMode,
                        ExpressionType.Command, mathMode, customization, name, options, values);
                    #endregion
                case '$':
                    #region Math
                    ExpressionType type;
                    if (str[1] == '$')
                    {
                        name = "$$";
                        value = ParseBraces(ref str, rdr, "$$", "$$", out whitespaceBefore);
                        type = ExpressionType.BlockMath;
                    }
                    else
                    {
                        name = "$";
                        value = ParseBraces(ref str, rdr, "$", "$", out whitespaceBefore);
                        type = ExpressionType.InlineMath;
                    }
                    end = str;
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild,
                        ref verbatimMode, type, false, customization, name, null, value);
                    #endregion
                case '%':
                    #region Comment
                    end = null;
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild, ref verbatimMode,
                        ExpressionType.Comment, false, customization, str.Substring(1), null, null);
                    #endregion
                case '{':
                    #region Block
                    value = ParseBraces(ref str, rdr, "{", "}", out whitespaceBefore);
                    end = str;
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild,
                        ref verbatimMode, ExpressionType.Block, mathMode, customization, "{}", null, value);
                    #endregion
                case '&':
                    #region Table cell
                    end = str.Substring(1);
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild, ref verbatimMode,
                        ExpressionType.PlainText, mathMode, customization, "&", null, null);
                    #endregion
                case '^':
                case '_':
                    #region Block for sub and sup
                    if (mathMode)
                    {
                        name = "" + str[0];
                        str = ReadToNotEmptyString(str.Substring(1), rdr, out whitespaceBefore);
                        switch (str[0])
                        {
                            case '{':
                                value = ParseBraces(ref str, rdr, "{", "}", out whitespaceBefore);
                                end = str;
                                break;
                            case '\\':
                                var cmdName = Regex.Match(str.Substring(1), @"^[a-zA-Z]+\*?").Value;
                                ParseCommandOptionsAndValues(
                                    str.Substring(cmdName.Length + 1), rdr, out end,
                                    out options, out values, out whitespaceBefore);
                                value = "\\" + cmdName + options;
                                if (values != null)
                                {
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        value += "{" + values[i] + "}";
                                    }
                                }
                                break;
                            default:
                                value = "" + str[0];
                                if (char.IsDigit(str[0]))
                                {
                                    value = Regex.Match(str, @"\d+").Value;
                                }
                                if (char.IsLetter(str[0]))
                                {
                                    value = Regex.Match(str, @"[a-zA-Z]+").Value;
                                }
                                end = str.Substring(value.Length);
                                break;
                        }
                        return new LatexExpression(parent, parentChildNumber, ref indexInParentChild,
                            ref verbatimMode, ExpressionType.Block, true, customization, name, null, value);
                    }
                    goto default;
                    #endregion
                default:
                    #region Plain text block
                    name = "";
                    var stopChars = mathMode ? new[] { '\\', '$', '{', '%', '&', '^', '_' } :
                        new[] { '\\', '$', '{', '%', '&' };                    
                    int stopPos;
                    while ((stopPos = GetStopPos(str, stopChars)) == -1)
                    {
                        if (rdr != null)
                        {
                            name += str + " ";
                            str = rdr.ReadLine();                            
                            if (str == null) // End of the document
                            {
                                name = name.Substring(0, name.Length - 1);
                                break;
                            }
                            if (str.Trim() == "")
                            {
                                str = "\\paragraph";
                                stopPos = 0;
                                break;
                            }
                        }
                        else
                        {
                            stopPos = str.Length;
                            break;
                        }
                    }                    
                    if (str == null) // End of the document?
                    {
                        end = null;
                    }
                    else
                    {
                        name += str.Substring(0, stopPos);
                        if (stopPos < str.Length && (str[stopPos] == '^' || str[stopPos] == '_'))
                        {
                            var match = Regex.Match(name, @"[a-zA-Z]+\s*\Z");
                            var chunk = match.Value;
                            if (chunk.Trim() == "")
                            {
                                match = Regex.Match(name, @"\d+\s*\Z");
                                chunk = match.Value;
                                if (chunk.Trim() == "")
                                {
                                    match = Regex.Match(name, @"\S\s*\Z");
                                    chunk = match.Value.Trim();
                                }
                            }
                            if (name != chunk)
                            {
                                end = name.Substring(match.Index) + str.Substring(stopPos);
                                name = name.Substring(0, match.Index);                                
                            }
                            else
                            {
                                end = str.Substring(stopPos);
                            }
                        }
                        else
                        {                            
                            end = str.Substring(stopPos);
                        }                        
                    }                    
                    return new LatexExpression(parent, parentChildNumber, ref indexInParentChild, ref verbatimMode,
                        ExpressionType.PlainText, mathMode, customization, (whitespaceBefore? " " : "") + SmartTextTrim(name), null, null);                    
                    #endregion
            }
        }
        
        /// <summary>
        /// Preserves the last whitespace character while removes all others in the end of the string.
        /// </summary>
        /// <param name="text">The string to trim.</param>
        /// <returns>The trimmed text.</returns>
        private static string SmartTextTrim(string text)
        {
            var trimmed = text.TrimEnd();
            if (trimmed.Length != text.Length)
            {
                return trimmed + " ";
            }
            return text;
        }

        /// <summary>
        /// Scans for any occurences of stop chars and returns the lowest position.
        /// </summary>
        /// <param name="str">The string to scan.</param>
        /// <param name="stopChars">The array of stop chars.</param>
        /// <returns></returns>
        private static int GetStopPos(string str, IEnumerable<char> stopChars)
        {
            int stopPos = -1;
            foreach (var chr in stopChars)
            {
                int pos = str.IndexOf(chr);
                if (pos > -1)
                {
                    stopPos = stopPos == -1 ? pos : Math.Min(stopPos, pos);
                }
            }
            return stopPos;
        }

        /// <summary>
        /// Reads to the first occurence of "\end{verbatim}".
        /// </summary>
        /// <param name="beginning">The beginning string.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="end">The stub of the unparsed part.</param>
        /// <returns>The extracted verbatim data.</returns>
        private static string ReadToVerbatimEnd(string beginning, TextReader rdr, out string end)
        {
            int pos;
            var str = beginning;
            string res = "";
            while ((pos = str.IndexOf(@"\end{verbatim}")) == -1)
            {
                res += str + "\n";
                str = rdr.ReadLine();
            }
            if (pos > 0)
            {
                res += str.Substring(0, pos);
            }
            end = str.Substring(pos);
            return res;
        }

        /// <summary>
        /// Reads to the first non-whitespace character.
        /// </summary>
        /// <param name="str">The beginning string.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="whitespaceBefore">Indicates whether there was at least one whitespace char before the returned result.</param>
        /// <returns>The string which starts with a non-whitespace character.</returns>
        public static string ReadToNotEmptyString(string str, TextReader rdr, out bool whitespaceBefore)
        {            
            whitespaceBefore = false;
            while (str == "")
            {
                if (rdr == null)
                {
                    return null;
                }
                while (str == "")
                {
                    str = rdr.ReadLine();
                    if (str == null)
                    {
                        return null;
                    }
                }
                str = str.Trim();
            }
            if (str == null) return null;
            var trimmed = str.Trim();
            if (trimmed.Length != str.Length)
            {
                whitespaceBefore = true;
            }
            return trimmed;            
        }

        /// <summary>
        /// Parses the options and values of a command.
        /// </summary>
        /// <param name="beginning">The beginning string.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="end">The stub of the unparsed part.</param>
        /// <param name="options">The extracted options.</param>
        /// <param name="whitespaceBefore">Indicates whether there was at least one whitespace char before the returned result.</param>
        /// <param name="values">The extracted values.</param>
        private static void ParseCommandOptionsAndValues(string beginning, TextReader rdr,
            out string end, out string options, out string[] values, out bool whitespaceBefore)
        {            
            options = null;
            values = null;            
            var str = ReadToNotEmptyString(beginning, rdr, out whitespaceBefore);
            if (str == null)
            {
                end = null;
                return;
            }            
            bool exit = false;
            var valueList = new List<string>(2);
            while (str != null && !exit)
            {
                var backup = str;
                switch (str[0])
                {
                    case '[':
                        options = "";
                        str = ReadToNotEmptyString(str.Substring(1), rdr, out whitespaceBefore);
                        int pos;
                        while ((pos = str.IndexOf(']')) == -1)
                        {
                            options += str;
                            str = rdr.ReadLine();
                        }
                        options += str.Substring(0, pos);
                        str = ReadToNotEmptyString(str.Substring(pos + 1), rdr, out whitespaceBefore);
                        break;
                    case '{':
                        string value;
                        while ((value = ParseBraces(ref str, rdr, "{", "}", out whitespaceBefore)) != null)
                        {
                            valueList.Add(value);
                        }
                        str = ReadToNotEmptyString(str, rdr, out whitespaceBefore);
                        break;
                    default:
                        exit = true;
                        str = backup;
                        break;
                }
            }
            if (valueList.Count > 0)
            {
                values = new string[valueList.Count];
                valueList.CopyTo(values);
            }
            end = str;            
        }

        /// <summary>
        /// Parses the value enclosed in braces.
        /// </summary>
        /// <param name="str">The beginning string to parse.</param>
        /// <param name="rdr">The reader to read ahead.</param>
        /// <param name="openingBrace">The opening brace form.</param>
        /// <param name="closingBrace">The closing brace form.</param>
        /// <param name="whitespaceBefore">Indicates whether there was at least one whitespace char before the returned result.</param>
        /// <returns>The extracted value.</returns>
        public static string ParseBraces(ref string str, TextReader rdr,
            string openingBrace, string closingBrace, out bool whitespaceBefore)
        {
            whitespaceBefore = false;
            if (str == null)
            {
                return null;
            }
            if (!str.StartsWith(openingBrace))
            {
                return null;
            }
            int brStack = 1;
            var value = "";
            int pos = -1;
            str = str.Substring(openingBrace.Length);
            while (pos == -1)
            {                
                for (int i = 0; i < str.Length; i++)
                {
                    if (str.Substring(i).StartsWith(closingBrace) && (i == 0 || str[i - 1] != '\\'))
                    {
                        brStack--;
                    }
                    if (brStack == 0)
                    {
                        pos = i;
                        break;
                    }
                    if (str.Substring(i).StartsWith(openingBrace) && (i == 0 || str[i - 1] != '\\'))
                    {
                        brStack++;
                        continue;
                    }                                        
                }
                if (pos == -1)
                {
                    value += str + " ";
                    str = rdr.ReadLine();
                }
            }
            value += str.Substring(0, pos);
            str = ReadToNotEmptyString(str.Substring(pos + closingBrace.Length), rdr, out whitespaceBefore);            
            return value;
        }

        /// <summary>
        /// Recursively replaces all occurences of src in all subtrees with the specified sequence of expressions.
        /// </summary>
        /// <param name="src">The string to search and replace.</param>
        /// <param name="repl">The replacesment sequence of expressions.</param>
        public void RecursiveReplace(string src, List<LatexExpression> repl)
        {
            if (Expressions != null)
            {
                foreach (var childOutline in Expressions)
                {
                    for (int i = 0; i < childOutline.Count; i++ )
                    {
                        childOutline[i].RecursiveReplace(src, repl);
                    }
                }
            }
            if (ExprType == ExpressionType.PlainText)
            {
                int pos = Name.IndexOf(src);
                if (pos > -1)
                {
                    if (pos > 0)
                    {
                        var beg = Name.Substring(0, pos);
                        bool verbatimMode = false;
                        int index = IndexInParentChild;                                                
                        Parent.Expressions[ParentChildNumber].Insert(IndexInParentChild,
                            new LatexExpression(Parent, ParentChildNumber, ref index,
                                ref verbatimMode, ExpressionType.PlainText, MathMode,
                                Customization, beg, null, null));
                        IndexInParentChild++;
                    }
                    Parent.Expressions[ParentChildNumber].RemoveAt(IndexInParentChild);
                    for (int i = IndexInParentChild; i < Parent.Expressions[ParentChildNumber].Count; i++)
                    {
                        Parent.Expressions[ParentChildNumber][i].IndexInParentChild = i + repl.Count;
                    }
                    var localRepl = new LatexExpression[repl.Count];
                    repl.CopyTo(localRepl);
                    for (int i = 0; i < repl.Count; i++)
                    {
                        var expr = localRepl[i];
                        expr.Parent = Parent;
                        expr.ParentChildNumber = ParentChildNumber;
                        expr.IndexInParentChild = i + IndexInParentChild;                        
                    }
                    Parent.Expressions[ParentChildNumber].InsertRange(IndexInParentChild, localRepl);
                    var end = Name.Substring(src.Length);
                    if (end != "")
                    {
                        bool verbatimMode2 = false;
                        int index2 = IndexInParentChild + repl.Count;
                        Parent.Expressions[ParentChildNumber].Insert(index2,
                            new LatexExpression(Parent, ParentChildNumber, ref index2,
                                ref verbatimMode2, ExpressionType.PlainText, MathMode,
                                Customization, end, null, null));
                    }
                }
            }
        }

        /// <summary>
        /// The children enumeration delegate.
        /// </summary>
        /// <param name="expr">The expression to operate on.</param>
        /// <returns>True if enumeration should be stopped; otherwise, false.</returns>
        public delegate bool ExpressionVisitDelegate(LatexExpression expr);

        /// <summary>
        /// Recursively enumerate all the children, executing the specified delegate on each child.
        /// </summary>
        /// <param name="visitor">The delegate to execute.</param>
        public bool EnumerateChildren(ExpressionVisitDelegate visitor)
        {
            if (Expressions != null)
            {
                foreach (var outline in Expressions)
                {
                    foreach (var expr in outline)
                    {
                        if (visitor(expr) || expr.EnumerateChildren(visitor))
                        {
                            return true;
                        }                        
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Find the document block in children (valid only for the root expression).
        /// </summary>
        /// <returns></returns>
        public LatexExpression FindDocument()
        {
            if (ExprType != ExpressionType.Root) return null;
            LatexExpression documentExpression = null;
            foreach (var child in Expressions[0])
            {
                if (child.ExprType == ExpressionType.Block && child.Name == "document")
                {
                    documentExpression = child;
                    break;
                }
            }
            return documentExpression;
        }

        /// <summary>
        /// Gets the hash code of this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() + ParentChildNumber * IndexInParentChild;
        }

        /// <summary>
        /// Converts the value of this instance to a System.String.
        /// </summary>
        /// <returns>The System.String instance.</returns>
        public override string ToString()
        {
            return String.Format("({0}) [{1}] {2}", ParentChildNumber, ExprType, Name);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to check.</obj>
        /// <returns>True if the specified System.Object is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {            
            var expr = obj as LatexExpression;
            if (expr == null)
            {
                return false;
            }
            return Name.Equals(expr.Name);
        }

        /// <summary>
        /// Computes the hash code of the corresponding converter to XHTML + MathML.
        /// </summary>
        /// <returns>The hash code.</returns>
        public int GetCommandConverterHashCode()
        {            
            /*if (Expressions != null)
            {
                if (CommandConverter.CommandsWithImportantChildren.Contains(Name))
                {
                    return Name.GetHashCode() ^ ExprType.GetHashCode() ^ Expressions[0][0].GetHashCode();
                }
            }*/
            return Name.GetHashCode() ^ ExprType.GetHashCode() ^ "".GetHashCode();
        }

        /// <summary>
        /// The table of convertors to XHML + MathML arranged by ExpressionType value.
        /// </summary>
        private static readonly Dictionary<ExpressionType, BaseConverter> Converters = new Dictionary<ExpressionType, BaseConverter>
        {
            {ExpressionType.PlainText, new PlainTextConverter()},
            {ExpressionType.Comment, new CommentConverter()},
            {ExpressionType.Verbatim, new VerbatimConverter()},
            {ExpressionType.Root, new RootConverter()},
            {ExpressionType.InlineMath, new MathConverter()},
            {ExpressionType.BlockMath, new MathConverter()},
            {ExpressionType.Block, new BlockConverter()},
            {ExpressionType.Command, new CommandConverter()}
        };

        /// <summary>
        /// Converts the expression and all of its children to XHTML + MathML.
        /// </summary>
        /// <returns>The converted XML string.</returns>
        public string Convert()
        {
            return Converters[ExprType].Convert(this);
        }
    }

    /// <summary>
    /// Indicates the type of the expression.
    /// </summary>
    public enum ExpressionType
    {
        /// <summary>
        /// Indicates the root of a document object tree.
        /// </summary>
        Root,
        /// <summary>
        /// Indicates a plain text block.
        /// </summary>
        PlainText,
        /// <summary>
        /// Indicates a command.
        /// </summary>
        Command,
        /// <summary>
        /// Indicates an inline math.
        /// </summary>
        InlineMath,
        /// <summary>
        /// Indicates a block math.
        /// </summary>
        BlockMath,
        /// <summary>
        /// Indicates a block.
        /// </summary>
        Block,
        /// <summary>
        /// Indicates a comment.
        /// </summary>
        Comment,
        /// <summary>
        /// Indicates a verbatim block.
        /// </summary>
        Verbatim
    }
}
