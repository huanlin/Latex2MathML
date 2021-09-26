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

namespace Latex2MathML
{
    /// <summary>
    /// The parser of the LaTeX document object tree.
    /// </summary>
    internal sealed class LatexParser
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The source string to parse.
        /// </summary>
        private string _source;

        /// <summary>
        /// The LatexToMathMLConverter class instance to customize the conversion result.
        /// </summary>
        private readonly LatexToMathMLConverter _customization;

        /// <summary>
        /// The root of the document object tree.
        /// </summary>
        private LatexExpression _root;

        /// <summary>
        /// The hash list of custom commands.
        /// </summary>
        private Dictionary<string, LatexExpression> _customCommands;		

        private static readonly List<string> TableLikeBlockNames = new List<string>
        {
            "array", "eqnarray", "tabular"
        };

        public static readonly List<string> InfoNames = new List<string>
        { 
            "author", "title", "date" 
        };

        /// <summary>
        /// Gets the root of the object tree.
        /// </summary>
        public LatexExpression Root 
        {
            get
            {
                if (_root == null)
                {
                    Parse(_customization);
                }
                return _root;
            }
        }		

        /// <summary>
        /// The event to report the progress of the parse process.
        /// </summary>
        public EventHandler<ProgressEventArgs> ProgressEvent;

        private void OnProgressEvent(byte index, byte count)
        {
            if (ProgressEvent != null)
            {
                ProgressEvent(this, new ProgressEventArgs(index, count));
            }
        }

        /// <summary>
        /// Initializes a new instance of the LatexParser class.
        /// </summary>
        /// <param name="source">The source string to build the tree from.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        public LatexParser(string source, LatexToMathMLConverter customization)
        {
            if (String.IsNullOrEmpty(source))
            {
                throw new ArgumentException("The parameter can not be null or empty.", "source");
            }
            _source = source;
            _customization = customization;
        }

        /// <summary>
        /// Loggs a message if not in DEBUG.
        /// </summary>
        /// <param name="message">The message to debug.</param>
        private static void LogInfo(object message)
        {
#if !DEBUF
            Log.Info(message);
#endif
        }

        /// <summary>
        /// Builds the document object tree.
        /// </summary>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        /// <remarks>The parsing procedure consists of stand-alone passes, so that it resembles a compiler pipeline.</remarks>
        public void Parse(LatexToMathMLConverter customization)
        {
            foreach (var rule in PreformatRules)
            {
                _source = _source.Replace(rule[0], rule[1]);
            }
            var rdr = new StringReader(_source);
            const byte PASS_COUNT = 14;
            byte step = 1;

            // Build the tree
            LogInfo("CreateRoot");
            _root = LatexExpression.CreateRoot(rdr, customization);
            OnProgressEvent(step++, PASS_COUNT);

            // Include the \input files
            LogInfo("IncludeImports");
            IncludeImports(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Rebuild tree with custom commands
            _customCommands = new Dictionary<string, LatexExpression>();
            LogInfo("RecursiveParseCustomCommands");
            RecursiveParseCustomCommands(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Ensure that commands like \title are out of the document scope
            LogInfo("MoveCommandsOutOfDocument");
            MoveCommandsOutOfDocument(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Incapsulate fragments between \begin and \end
            LogInfo("IncapsulateCommands");
            IncapsulateCommands(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Post-parse arrays and tabular
            LogInfo("PostParseTables");
            PostParseTables(Root.Expressions[0], customization);
            OnProgressEvent(step++, PASS_COUNT);

            // Build super- and subscripts
            LogInfo("BuildScripts");
            BuildScripts(Root.Expressions[0], customization);
            OnProgressEvent(step++, PASS_COUNT);            

            // Build lists
            LogInfo("BuildLists");
            BuildLists(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Build paragraph blocks
            LogInfo("BuildParagraphs");
            BuildParagraphs(Root.FindDocument().Expressions[0], customization);
            OnProgressEvent(step++, PASS_COUNT);

            // Simplify math blocks that begin with baseless scripts
            LogInfo("SimplifyScripts");
            SimplifyBaselessScripts(Root.Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

            // Numerate blocks (needed for the next step)
            LogInfo("NumerateBlocks");
            NumerateBlocks(Root.FindDocument().Expressions[0]);
            Root.Customization.Counters.Add("document", 0);
            OnProgressEvent(step++, PASS_COUNT);

            // Simplify math blocks that begin with baseless scripts
            LogInfo("PreProcessLabels");
            PreprocessLabels(Root.FindDocument().Expressions[0]);
            Root.Customization.Counters.Clear();
            OnProgressEvent(step++, PASS_COUNT);

            // Deal with algorithmic blocks
            LogInfo("PreprocessAlgorithms");
            PreprocessAlgorithms(Root.FindDocument().Expressions[0]);
            OnProgressEvent(step++, PASS_COUNT);

			// Attach the bibliography
            LogInfo("AttachBibliography");
            AttachBibliography();
            OnProgressEvent(step++, PASS_COUNT);
			
            LogInfo("Finished");
        }

		/// <summary>
		/// Find the bibliography command and parse the corresponding file. 
		/// </summary>
		private void AttachBibliography()
        {
			string fullName = null;
			Root.EnumerateChildren(expr =>
			{
				if (expr.Name == "bibliography" && expr.Expressions != null)
				{
					var fileName = expr.Expressions[0][0].Name;	
					var sourceDir = Path.GetDirectoryName(_customization.SourcePath);
					if (!Path.IsPathRooted(sourceDir))
					{
						sourceDir = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), sourceDir);	
					}
					fileName = Path.Combine(sourceDir, fileName);
					if (Path.GetExtension(fileName) == "")
					{
						fileName += ".bib";	
					}
					fullName = fileName;
					return true;
				}
				return false;
			});
            if (!String.IsNullOrEmpty(fullName))
            {
                var parser = new BibtexParser(new StringReader(File.ReadAllText(fullName)), _customization);
                _customization.Bibliography = parser.Records;
            }
		}
		
        /// <summary>
        /// Recursively wrap algorithmic block expressions into blocks
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void PreprocessAlgorithms(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Expressions != null &&
                    (outline[i].ExprType == ExpressionType.Command || outline[i].ExprType == ExpressionType.Block))
                {
                    foreach (var subTree in outline[i].Expressions)
                    {
                        PreprocessAlgorithms(subTree);
                    }
                }

                if (outline[i].ExprType == ExpressionType.Block &&
                    (outline[i].Name == "algorithmic" || outline[i].Name == "algorithmicx"))
                {
                    int indentation = 0;
                    int counter = 1;
                    foreach (var part in outline[i].Expressions[0])
                    {
                        if (part.ExprType == ExpressionType.Command)
                        {
                            if (part.Name == "Procedure" || part.Name == "Function" || part.Name == "Begin")
                            {
                                counter++;
                                indentation += 2;
                                continue;
                            }
                            if (part.Name == "EndProcedure" || part.Name == "EndFunction" || part.Name == "End")
                            {
                                counter++;
                                indentation -= 2;
                                continue;
                            }
                            if (part.ExprType == ExpressionType.Command && part.Name == "State")
                            {
                                counter++;
                                part.Tag = new[] {counter, indentation};
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recursively numerate blocks by adding a new expression to the end.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void NumerateBlocks(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Expressions != null &&
                        (outline[i].ExprType == ExpressionType.Command || outline[i].ExprType == ExpressionType.Block))
                {
                    foreach (var subTree in outline[i].Expressions)
                    {
                        NumerateBlocks(subTree);
                    }
                }

                if (outline[i].ExprType == ExpressionType.Block && outline[i].Name != "")
                {
                    int index;
                    if (outline[i].Customization.Counters.ContainsKey(outline[i].Name))
                    {
                        index = ++outline[i].Customization.Counters[outline[i].Name];
                    }
                    else
                    {
                        outline[i].Customization.Counters.Add(outline[i].Name, 1);
                        index = 1;
                    }
                    outline[i].Tag = index;
                }                
            }
        }

        /// <summary>
        /// Recursively walk the tree and handle labels.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void PreprocessLabels(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (!(outline[i].ExprType == ExpressionType.Command && outline[i].Name == "label"))
                {
                    if (outline[i].Expressions != null &&
                        (outline[i].ExprType == ExpressionType.Command || outline[i].ExprType == ExpressionType.Block))
                    {
                        foreach (var subTree in outline[i].Expressions)
                        {
                            PreprocessLabels(subTree);
                        }
                    }
                }
                else
                {
                    var parentBlock = outline[i].Parent;
                    while (parentBlock.ExprType != ExpressionType.Block || parentBlock.Name == "" || parentBlock.Name == "paragraph")
                    {
                        parentBlock = parentBlock.Parent;
                    }
                    int index;
                    if (parentBlock.Name == "document")
                    {
                        index = ++parentBlock.Customization.Counters["document"];
                    }
                    else
                    {
                        index = (int)parentBlock.Tag;
                    }
                    var reference = "";

                    // Flatten the reference
                    outline[i].EnumerateChildren(e =>
                                                     {
                                                         if (e.ExprType == ExpressionType.PlainText)
                                                         {
                                                             reference += e.Name;
                                                         }
                                                         return false;
                                                     });
                    outline[i].Tag = reference;
                    if (!parentBlock.Customization.References.ContainsKey(reference))
                    {
                        parentBlock.Customization.References.Add(reference,
                                                                 new LabeledReference
                                                                     {Kind = parentBlock.Name, Number = index});
                    }
                }
            }
        }

        /// <summary>
        /// Recursively split math blocks that begin with baseless scripts.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void SimplifyBaselessScripts(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.InlineMath)
                {
                    var firstExpr = outline[i].Expressions[0][0];
                    if (firstExpr.ExprType == ExpressionType.Block &&
                        (firstExpr.Name == "^" || firstExpr.Name == "_"))
                    {
                        firstExpr.IndexInParentChild = i;
                        firstExpr.ParentChildNumber = outline[i].ParentChildNumber;
                        firstExpr.Parent = outline[i].Parent;
                        firstExpr.EnumerateChildren(e => e.MathMode = false);

                        for (int j = i; j < outline.Count; j++)
                        {
                            outline[j].IndexInParentChild = j + 1;
                        }

                        for (int j = 1; j < outline[i].Expressions[0].Count; j++)
                        {
                            outline[i].Expressions[0][j].IndexInParentChild--;
                        }
                        outline[i].Expressions[0].RemoveAt(0);
                        outline.Insert(i, firstExpr);
                        i++;
                    }
                }
                else
                {
                    if (outline[i].Expressions != null && 
                        (outline[i].ExprType == ExpressionType.Command || outline[i].ExprType == ExpressionType.Block))
                    {
                        foreach (var subTree in outline[i].Expressions)
                        {
                            SimplifyBaselessScripts(subTree);
                        }
                    }   
                }                
            }
        }

        /// <summary>
        /// Ensure that the commands like \title are encountered before \begin{document}.
        /// This is needed for the valid document information retrieval.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void MoveCommandsOutOfDocument(List<LatexExpression> outline)
        {
            int beginDocumentIndex = -1;
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.Command && outline[i].Name == "begin" &&
                    outline[i].Expressions != null && outline[i].Expressions[0][0].Name == "document")
                {
                    beginDocumentIndex = i;
                }
            }
            if (beginDocumentIndex > -1)
            {
                for (int i = 0; i < outline.Count; i++)
                {
                    if (outline[i].ExprType == ExpressionType.Command && InfoNames.Contains(outline[i].Name) &&
                        i > beginDocumentIndex)
                    {
                        var expr = outline[i];
                        expr.IndexInParentChild = beginDocumentIndex;
                        outline.RemoveAt(i);
                        for (int j = beginDocumentIndex; j < i; j++)
                        {
                            outline[j].IndexInParentChild++;
                        }
                        outline.Insert(beginDocumentIndex, expr);
                    }
                }
            }
        }

        /// <summary>
        /// Substitute \input-s with the corresponding file contents and parse them, too.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void IncludeImports(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.Command)
                {
                    var cmd = outline[i];
                    var parent = outline[0].Parent;
                    if (cmd.Name == "input")
                    {
                        var fileName = Path.Combine(Path.GetDirectoryName(cmd.Customization.SourcePath), cmd.Expressions[0][0].Name);
                        string input = File.ReadAllText(fileName);
                        LogInfo("CreateRoot (" + fileName + ")");
                        var root = LatexExpression.CreateRoot(new StringReader(input), cmd.Customization);

                        // Now insert the real content
                        outline.RemoveAt(i);
                        int count = root.Expressions[0].Count;

                        // Update indices of further expressions
                        for (int j = i; j < outline.Count; j++)
                        {
                            outline[j].IndexInParentChild = j + count;
                        }

                        outline.InsertRange(i, root.Expressions[0]);

                        // Update indices of inserted expressions
                        for (int j = 0; j < count; j++)
                        {
                            outline[i + j].IndexInParentChild = i + j;
                            outline[i + j].Parent = parent;
                            // No need to set ParentChildNumber - it is 0 already
                        }
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Recursively build paragragh structure.
        /// </summary>
        /// <param name="list">The outline of a LatexExpression instance.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        private static void BuildParagraphs(List<LatexExpression> list, LatexToMathMLConverter customization)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Expressions != null)
                {
                    foreach (var subTree in list[i].Expressions)
                    {
                        BuildParagraphs(subTree, customization);
                    }
                }               
                bool isParagraph = list[i].ExprType == ExpressionType.Command &&
                    list[i].Name.IndexOf("paragraph") > -1 && !list[i].MathMode;                
                bool isLikeParagraph = !list[i].MathMode && (list[i].ExprType == ExpressionType.Command &&
                    list[i].Name.IndexOf("section") > -1 || 
                    list[i].ExprType == ExpressionType.BlockMath ||
                    list[i].ExprType == ExpressionType.Block && list[i].Name != "" ||
                    list[i].ExprType == ExpressionType.Verbatim);
                if (isParagraph || isLikeParagraph)
                {
                    int j;
                    #region Set j to the paragraph breaker expression index
                    bool found = false;
                    for (j = i + 1; j < list.Count; j++)
                    {
                        if (list[j].ExprType == ExpressionType.Command &&
                            (list[j].Name.IndexOf("paragraph") > -1 || list[j].Name.IndexOf("section") > -1) ||
                            list[j].ExprType == ExpressionType.Block && list[j].Name != "" ||
                            list[j].ExprType == ExpressionType.BlockMath ||
                            list[j].ExprType == ExpressionType.Verbatim)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        j = list.Count;
                    }
                    #endregion
                    if (isParagraph)
                    {
                        #region Create and link the block
                        var block = new List<LatexExpression>(j - i - 1);
                        List<LatexExpression> paragraphTitle = null;
                        if (list[i].Expressions != null)
                        {
                            paragraphTitle = list[i].Expressions[0];
                        }
                        list[i] = new LatexExpression(list[i].Name, list[i].Parent, list[i].ParentChildNumber, i, customization);
                        int parentChildNumber = 0;
                        if (paragraphTitle != null)
                        {
                            list[i].Expressions[0] = paragraphTitle;
                            list[i].Expressions.Add(new List<LatexExpression>());
                            parentChildNumber = 1;
                        }
                        for (int k = i + 1; k < j; k++)
                        {
                            list[k].Parent = list[i];
                            list[k].ParentChildNumber = parentChildNumber;
                            list[k].IndexInParentChild = k - i - 1;
                            block.Add(new LatexExpression(list[k]));
                        }
                        list[i].Expressions[parentChildNumber] = block;
                        #endregion
                        if (j - i - 1 > 0)
                        {
                            list.RemoveRange(i + 1, j - i - 1);
                            #region Update indices after the block
                            int delta = j - i - 1;
                            for (int k = i + 1; k < list.Count; k++)
                            {
                                list[k].IndexInParentChild -= delta;
                            }
                            #endregion
                        }                        
                    }
                    if (isLikeParagraph && j - i > 1)
                    {
                        #region Create and link the new block
                        var expr = new LatexExpression("paragraph", list[i + 1].Parent, list[i + 1].ParentChildNumber, i + 1, customization);
                        for (int k = i + 1; k < j; k++)
                        {
                            list[k].Parent = expr;
                            list[k].ParentChildNumber = 0;
                            list[k].IndexInParentChild = k - i - 1;
                            expr.Expressions[0].Add(new LatexExpression(list[k]));
                        }
                        list[i + 1] = expr;
                        #endregion
                        
                        if (j - i - 1 > 1)
                        {
                            list.RemoveRange(i + 2, j - i - 2);
                            #region Update indices after the block
                            int delta = j - i - 2;
                            for (int k = i + 2; k < list.Count; k++)
                            {
                                list[k].IndexInParentChild -= delta;
                            }
                            #endregion
                        }                       
                    }                                        
                }
            }
        }

        /// <summary>
        /// Recursively build lists structure.
        /// </summary>
        /// <param name="outline">The outline of the root LatexExpression instance.</param>
        private static void BuildLists(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Expressions != null)
                {
                    foreach (var subTree in outline[i].Expressions)
                    {
                        BuildLists(subTree);
                    }
                }
                if (outline[i].ExprType == ExpressionType.Block &&
                    ListConverter.ListNames.Contains(outline[i].Name) &&
                    outline[i].Expressions != null)
                {
                    #region Transform the found list
                    var block = outline[i];
                    var buffers = new List<List<LatexExpression>>();
                    var buffer = new List<LatexExpression>();
                    var itemCommands = new List<LatexExpression>();
                    for (int m = 0; m < block.Expressions[0].Count; m++)
                    {
                        var expr = block.Expressions[0][m];
                        if (!(expr.ExprType == ExpressionType.Command && expr.Name == "item"))
                        {
                            buffer.Add(expr);
                            block.Expressions[0].RemoveAt(m);
                            m--;
                        }
                        else
                        {
                            itemCommands.Add(expr);
                            if (buffer.Count > 0)
                            {
                                buffers.Add(buffer);
                                buffer = new List<LatexExpression>();
                            }
                        }
                    }
                    if (buffer.Count > 0)
                    {
                        buffers.Add(buffer);
                    }
                    for (int j = 0; j < itemCommands.Count; j++)
                    {
                        itemCommands[j].IndexInParentChild = j;                        
                        for (int k = 0; k < buffers[j].Count; k++)
                        {
                            buffers[j][k].IndexInParentChild = k;
                            buffers[j][k].Parent = itemCommands[j];
                        }
                        itemCommands[j].Expressions = new List<List<LatexExpression>> { buffers[j] };
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Recursively build scripts (for msup, munder, etc.)
        /// </summary>
        /// <param name="list">The outline of a LatexExpression instance.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        private static void BuildScripts(List<LatexExpression> list, LatexToMathMLConverter customization)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Expressions != null)
                {
                    foreach (var subTree in list[i].Expressions)
                    {
                        BuildScripts(subTree, customization);
                    }
                }
                if (list[i].ExprType == ExpressionType.Block &&
                    (list[i].Name == "^" || list[i].Name == "_") && i > 0)
                {
                    if (list[i - 1].ExprType == ExpressionType.Command && list[i - 1].Name == "limits")
                    {
                        list[i - 1].EraseFromParent();
                        i--;
                    }
                    #region Place the previous expression to the script block                    
                    if (i < list.Count - 1 && list[i + 1].ExprType == ExpressionType.Block &&
                        (list[i + 1].Name == "^" || list[i + 1].Name == "_"))
                    {                        
                        var block = new LatexExpression("script" + list[i].Name + list[i + 1].Name, list[i].Parent,
                            list[i].ParentChildNumber, i - 1, customization);                        
                        block.MathMode = list[i].MathMode;                                                
                        block.Expressions[0].Add(new LatexExpression(list[i - 1]));
                        block.Expressions[0].Add(new LatexExpression(list[i]));
                        block.Expressions[0].Add(new LatexExpression(list[i + 1]));
                        block.Expressions[0][0].Parent = block;
                        block.Expressions[0][0].ParentChildNumber = 0;
                        block.Expressions[0][0].IndexInParentChild = 0;
                        block.Expressions[0][0].MathMode = block.MathMode;
                        block.Expressions[0][1].Parent = block;
                        block.Expressions[0][1].ParentChildNumber = 0;
                        block.Expressions[0][1].IndexInParentChild = 1;
                        block.Expressions[0][1].MathMode = block.MathMode;
                        block.Expressions[0][2].Parent = block;
                        block.Expressions[0][2].ParentChildNumber = 0;
                        block.Expressions[0][2].IndexInParentChild = 2;
                        block.Expressions[0][2].MathMode = block.MathMode;
                        list[i - 1] = block;
                        list[i].EraseFromParent();
                        list[i].EraseFromParent();
                    }
                    else
                    {
                        var block = new LatexExpression("script" + list[i].Name, list[i].Parent, list[i].ParentChildNumber, i - 1, customization);
                        block.MathMode = list[i].MathMode;
                        block.Expressions[0].Add(new LatexExpression(list[i - 1]));
                        block.Expressions[0].Add(new LatexExpression(list[i]));
                        block.Expressions[0][0].Parent = block;
                        block.Expressions[0][0].ParentChildNumber = 0;
                        block.Expressions[0][0].IndexInParentChild = 0;
                        block.Expressions[0][0].MathMode = block.MathMode;
                        block.Expressions[0][1].Parent = block;
                        block.Expressions[0][1].ParentChildNumber = 0;
                        block.Expressions[0][1].IndexInParentChild = 1;
                        block.Expressions[0][1].MathMode = block.MathMode;                        
                        list[i - 1] = block;
                        list[i].EraseFromParent();
                    }
                    i--;
                    #endregion                                               
                }
            }
        }

        /// <summary>
        /// Recursively finds the last command for the RecursiveParseCustomCommands.
        /// </summary>
        /// <param name="list">The list of expressions to search.</param>
        /// <returns>The last command or null.</returns>
        private static LatexExpression RecursiveFindCommandHandler(IList<LatexExpression> list)
        {
            for (int k = list.Count - 1; k > -1; k--)
            {
                if (list[k].ExprType == ExpressionType.Command)
                {
                    return list[k];
                }
                if (list[k].ExprType == ExpressionType.Block)
                {                    
                    for (int i = list[k].Expressions.Count - 1; i > -1; i--)
                    {
                        var res = RecursiveFindCommandHandler(list[k].Expressions[i]);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Recursively rebuilds the expression outline.
        /// </summary>
        /// <param name="outline">The outline of a LatexExpression instance.</param>
        private void RecursiveParseCustomCommands(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.Command)
                {
                    #region If is a command
                    var cmd = outline[i];
                    LatexExpression value;
                    if (_customCommands.TryGetValue(cmd.Name, out value))
                    {
                        var ind = cmd.MathMode ? 2 : 1;                                                    
                        for (int j = 0; j < value.Expressions[ind].Count; j++)
                        {
                            value.Expressions[ind][j] = new LatexExpression(value.Expressions[ind][j])
                            {
                                Parent = cmd.Parent,
                                ParentChildNumber = cmd.ParentChildNumber,
                                IndexInParentChild = i + j,
                                MathMode = cmd.MathMode,                                
                            };
                        }                        
                        outline.RemoveAt(i);
                        for (int j = i; j < outline.Count; j++)
                        {
                            outline[j].IndexInParentChild = j + value.Expressions[ind].Count;
                        }
                        outline.InsertRange(i, value.Expressions[ind]);
                        #region Very hard core logic for macros with unexpected parameters
                        if (value.Options == null)
                        {
                            if (cmd.Expressions != null)
                            {
                                var expr = RecursiveFindCommandHandler(value.Expressions[ind]);
                                if (expr != null)
                                {
                                    var root = expr.Parent.Expressions[expr.ParentChildNumber];
                                    bool scriptFix = (expr.IndexInParentChild < root.Count - 1) &&
                                        root[expr.IndexInParentChild + 1].ExprType == ExpressionType.Block &&
                                        root[expr.IndexInParentChild + 1].Name == "^" ||
                                        root[expr.IndexInParentChild + 1].Name == "_";
                                    if (!scriptFix)
                                    {
                                        foreach (var subTree in cmd.Expressions)
                                        {
                                            foreach (var child in subTree)
                                            {
                                                child.Parent = expr;
                                                child.MathMode = cmd.MathMode;
                                            }
                                        }
                                        expr.Expressions = new List<List<LatexExpression>>(cmd.Expressions);
                                        expr.Options = new ExpressionOptions(cmd.Options);
                                    }
                                    else
                                    {                                        
                                        int offset = 2;
                                        if ((expr.IndexInParentChild < root.Count - 2) &&
                                            root[expr.IndexInParentChild + 2].ExprType == ExpressionType.Block &&
                                            root[expr.IndexInParentChild + 2].Name == "^" ||
                                        root[expr.IndexInParentChild + 2].Name == "_")
                                        {
                                            offset++;
                                        }
                                        var parent = expr.Parent;
                                        int index = 0;
                                        int localIndex = 0;
                                        for (int k = 0; k < cmd.Expressions.Count; k++)
                                        {
                                            for (int m = 0; m < cmd.Expressions[k].Count; m++)
                                            {
                                                cmd.Expressions[k][m].Parent = expr.Parent;
                                                cmd.Expressions[k][m].ParentChildNumber = expr.ParentChildNumber;
                                                cmd.Expressions[k][m].IndexInParentChild = expr.IndexInParentChild + index++;
                                                cmd.Expressions[k][m].MathMode = expr.MathMode;
                                            }
                                            parent.Expressions[expr.ParentChildNumber].InsertRange(
                                                expr.IndexInParentChild + localIndex + offset, cmd.Expressions[k]);
                                            localIndex += cmd.Expressions[k].Count;
                                        }
                                        for (int k = expr.IndexInParentChild + ind; k < expr.Parent.Expressions[expr.ParentChildNumber].Count; k++)
                                        {
                                            parent.Expressions[expr.ParentChildNumber][k].IndexInParentChild += index;
                                        }
                                    }
                                }
                                cmd.Expressions = null;
                                cmd.Options = null;
                            }
                        }
                        #endregion
                        else                        
                        #region Parameters were expected
                        {
                            // TODO: check
                            int optCount = int.Parse(value.Options.AsKeyValue.Keys.GetEnumerator().Current);
                            for (int j = 0; j < optCount; j++)
                            {
                                for (int k = 0; k < value.Expressions[1].Count; k++)
                                {
                                    outline[i + k].RecursiveReplace("#" + (j + 1), cmd.Expressions[j]);
                                }
                            }
                            cmd.Expressions = null;
                        }
                        #endregion
                    }
                    else
                    {
                        if (cmd.Expressions != null)
                        {
                            foreach (var childOutline in cmd.Expressions)
                            {
                                RecursiveParseCustomCommands(childOutline);
                            }
                        }
                    }
                    if (cmd.Name == "newcommand" && cmd.Expressions != null)
                    {
                        _customCommands.Add(cmd.Expressions[0][0].Name, new LatexExpression(cmd));
                    }
                    // TODO: renewcommand
                    #endregion
                }
                if (outline[i].ExprType == ExpressionType.InlineMath ||
                    outline[i].ExprType == ExpressionType.BlockMath ||
                    outline[i].ExprType == ExpressionType.Block)
                {
                    #region If is a block
                    var block = outline[i];
                    if (block.Expressions != null)
                    {
                        RecursiveParseCustomCommands(block.Expressions[0]);
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// Recursively incapsulates tree fragments between \begin and \end.
        /// </summary>
        /// <param name="outline">The outline of a LatexExpression instance.</param>
        private static void IncapsulateCommands(List<LatexExpression> outline)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.Command &&
                    outline[i].Name == "begin")
                {
                    var cmdValue = outline[i].Expressions[0][0].Name;
                    int j;
                    for (j = i;
                        outline[j].Name != "end" || outline[j].Expressions == null || outline[j].Expressions[0][0].Name != cmdValue;
                        j++) { }
                    int length = j - i - 1;
                    var subOutline = new LatexExpression[length];
                    // Cut the right chunk
                    outline.CopyTo(i + 1, subOutline, 0, length);
                    outline.RemoveRange(i + 1, length + 1);
                    // Update outline[i]
                    outline[i].Name = cmdValue;
                    outline[i].ExprType = ExpressionType.Block;
                    outline[i].Expressions.RemoveAt(0);
                    for (int k = 0; k < outline[i].Expressions.Count; k++)
                    {
                        foreach (var expr in outline[i].Expressions[k])
                        {
                            expr.ParentChildNumber--;
                        }
                    }
                    // Update outline
                    for (int k = i + 1; k < outline.Count; k++)
                    {
                        outline[k].IndexInParentChild -= length + 1;
                    }                    
                    // Update subOutline
                    int parentChildNumber = outline[i].Expressions.Count;
                    for (int k = 0; k < subOutline.Length; k++)
                    {
                        subOutline[k].Parent = outline[i];
                        subOutline[k].ParentChildNumber = parentChildNumber;
                        subOutline[k].IndexInParentChild = k;
                    }                                       
                    // Link subOutline
                    outline[i].Expressions.Add(new List<LatexExpression>(subOutline));
                    IncapsulateCommands(outline[i].Expressions[parentChildNumber]);
                }
            }
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Expressions != null)
                {
                    foreach (var subTree in outline[i].Expressions)
                    {
                        IncapsulateCommands(subTree);
                    }
                }
            }
        }

        /// <summary>
        /// Post-parses arrays and tabular.
        /// </summary>
        /// <param name="outline">The outline of a LatexExpression instance.</param>
        /// <param name="customization">The LatexToMathMLConverter class instance to customize the conversion result.</param>
        private static void PostParseTables(IList<LatexExpression> outline, LatexToMathMLConverter customization)
        {
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].ExprType == ExpressionType.Block &&
                    (TableLikeBlockNames.Contains(outline[i].Name)))
                {
                    int parentChildNumber = outline[i].Expressions.Count == 1? 0 : 1;
                    var main = outline[i].Expressions[parentChildNumber];
                    var table = new List<List<List<LatexExpression>>>(3) {new List<List<LatexExpression>>(3)};                    
                    #region Build table
                    int rowIndex = 0;
                    for (int j = 0; ; j++)
                    {
                        var cell = new List<LatexExpression>(2);
                        for (; j < main.Count &&
                            !(main[j].Name == "\\" && main[j].ExprType == ExpressionType.Command) &&
                            !(main[j].Name == "&" && main[j].ExprType == ExpressionType.PlainText)
                            ; j++)
                        {
                            // \hline won't do the same in XHTML
                            if (main[j].ExprType == ExpressionType.Command && main[j].Name == "hline") continue;
                            if (main[j].ExprType == ExpressionType.Comment) continue;
                            cell.Add(main[j]);
                        }
                        if (cell.Count > 0)
                        {
                            table[rowIndex].Add(cell);
                        }
                        if (j == main.Count)
                        {
                            break;
                        }
                        if (main[j].Name == "\\")
                        {
                            rowIndex++;
                            table.Add(new List<List<LatexExpression>>(3));
                        }
                    }
                    if (table[rowIndex].Count == 0)
                    {
                        table.RemoveAt(rowIndex);
                    }
                    #endregion
                    #region Link table
                    main = new List<LatexExpression>(table.Count);
                    for (int j = 0; j < table.Count; j++)
                    {
                        // Add row
                        main.Add(new LatexExpression("", outline[i], parentChildNumber, j, customization));
                        for (int k = 0; k < table[j].Count; k++)
                        {
                            // Add column cell
                            main[j].Expressions[0].Add(new LatexExpression("", main[j], 0, k, customization));
                            for (int m = 0; m < table[j][k].Count; m++)
                            {
                                table[j][k][m].Parent = main[j].Expressions[0][k];
                                table[j][k][m].ParentChildNumber = 0;
                                table[j][k][m].IndexInParentChild = m;
                                // Add cell atom
                                main[j].Expressions[0][k].Expressions[0].Add(table[j][k][m]);
                            }
                        }
                    }
                    outline[i].Expressions[parentChildNumber] = main;
                    #endregion
                }
            }
            for (int i = 0; i < outline.Count; i++)
            {
                if (outline[i].Expressions != null)
                {
                    foreach (var subTree in outline[i].Expressions)
                    {
                        PostParseTables(subTree, customization);
                    }
                }
            }
        }

        /// <summary>
        /// Important substitution rules which guarantee the failsafe work of the parser.
        /// </summary>
        public static readonly List<string[]> PreformatRules = new List<string[]>
        {
            new[] {"]\n", "] \n"},
            new[] {"}\n", "} \n"},
            new[] {"$\n", "$ \n"},
            new[] {@"\\", @"\\ "}
        };
    }

    /// <summary>
    /// The event arguments class to report the progress of the parse process.
    /// </summary>
    internal class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the index of the current pass.
        /// </summary>
        public byte Index { get; private set; }

        /// <summary>
        /// Gets the count of the passes.
        /// </summary>
        public byte Count { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProgressEventArgs class.
        /// </summary>
        /// <param name="index">The index of the current pass.</param>
        /// <param name="count">The count of the passes.</param>
        public ProgressEventArgs(byte index, byte count)
        {
            Index = index;
            Count = count;
        }
    }
}
