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

using System.Collections.Generic;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for block expressions.
    /// </summary>
    internal class BlockConverter : NamedConverter
    {
        /// <summary>
        /// The list of available block converters hashed by a block name.
        /// </summary>
        protected static readonly Dictionary<string, BlockConverter> BlockConverters = new Dictionary<string, BlockConverter>
        {
            {"{}", new SequenceConverter()},
            {"document", new SequenceConverter()},
            {"^", new BaselessConverter()},
            {"_", new BaselessConverter()},
            {"left", new AlignmentConverter("left")},
            {"center", new AlignmentConverter("center")},
            {"right", new AlignmentConverter("right")},
            {"array", new ArrayConverter()},
            {"eqnarray", new EqnArrayConverter()},
            {"eqnarray*", new EqnArrayConverter()},
            {"equation", new EquationConverter()},
            {"script^", new SingleScriptConverter("msup")},
            {"script_", new SingleScriptConverter("msub")},
            {"script^_", new DoubleScriptConverter("msupsub")},
            {"script_^", new DoubleScriptConverter("msubsup")},
            {"paragraph", new ParagraphConverter()},
            {"subparagraph", new ParagraphConverter()},
            {"figure", new WrapperConverter()},
            {"table", new WrapperConverter()},
            {"algorithm", new WrapperConverter()},
            //{"algorithmic", new AlgorithmicConverter()},  
            {"definition", new DefinitionConverter()},            
            {"tabular", new TabularConverter()},
			{"abstract", new AbstractConverter()},
            {"itemize", new ItemizeConverter()},
            {"enumerate", new EnumerateConverter()},
            {"description", new DescriptionConverter()},
            {"quote", new QuoteConverter()},
            {"quotation", new QuotationConverter()},
            {"PlainText", new SequenceConverter()}
        };

        public static bool ImpliesMathMode(string name)
        {
            BlockConverter converter;
            if (BlockConverters.TryGetValue(name, out converter))
            {
                return converter.ImpliesMathMode();
            }
            return false;
        }

        /// <summary>
        /// Returns true if the block defines a math environment; otherwise, false.
        /// </summary>
        /// <returns></returns>
        public virtual bool ImpliesMathMode()
        {
            return false;
        }        

        /// <summary>
        /// Gets or sets the label assosiated with this block.
        /// </summary>
        public LabeledReference? Label { get; set; }

        /// <summary>
        /// Gets an empty string. This property must be overriden in all the inheritors.
        /// </summary>
        public override string Name
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the type of the corresponding expression (ExpressionType.Block).
        /// </summary>
        public override ExpressionType ExprType
        {
            get { return ExpressionType.Block; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            BlockConverter converter;
            if (BlockConverters.TryGetValue(expr.Name, out converter))
            {
                return converter.Convert(expr);
            }
            return (new UnknownBlockConverter()).Convert(expr);
        }
    }
}
