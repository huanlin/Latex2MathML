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
using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The universal converter for math blocks.
    /// </summary>
    internal sealed class MathConverter : BaseConverter
    {
        /// <summary>
        /// Performs the conversion procedure of math blocks.
        /// </summary>
        /// <param name="outline">The sequence of expressions to convert.</param>
        /// <param name="altText">The alternative text.</param>
        /// <param name="inline">Indicates whether the math block is inline.</param>
        /// <returns>The converted XML string.</returns>
        private static string CommonConvert(IList<LatexExpression> outline, string altText,
		                                    bool inline, LatexToMathMLConverter customization)
        {
            if (outline.Count == 0) return "";
            var bld = new StringBuilder();            
            AppendMathProlog(bld, altText, inline, customization);
            bld.Append("<mrow>\n");
			bld.Append(SequenceConverter.ConvertOutline(outline, customization));
            bld.Append("\n</mrow>\n");
            AppendMathEpilog(bld);
            return bld.ToString();
        }

        /// <summary>
        /// Appends the opening MathML &lt;math&gt; tag to the specified StringBuilder instance.
        /// </summary>
        /// <param name="bld">The StringBuilder instance.</param>
        /// <param name="altText">The alternative text.</param>
        /// <param name="inline">Indicates whether the math block is inline.</param>
        public static void AppendMathProlog(StringBuilder bld, string altText,
		                                    bool inline, LatexToMathMLConverter customization)
        {
            bld.Append("\n<math xmlns=\"http://www.w3.org/1998/Math/MathML\" alttext=\"");
            bld.Append(altText);
            bld.Append("\" ");
            bld.Append(inline ? "display=\"inline\"" : "display=\"block\"");
            bld.Append(" class=\"" + customization.GetCurrentCssStyle() + "\"");
            bld.Append(">\n<mstyle displaystyle=\"true\" />");
        }

        /// <summary>
        /// Appends the closing MathML &lt;math&gt; tag to the specified StringBuilder instance.
        /// </summary>
        /// <param name="bld">The StringBuilder instance.</param>
        public static void AppendMathEpilog(StringBuilder bld)
        {
            bld.Append("</math>\n");
        }

        /// <summary>
        /// Is not used.
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
            return CommonConvert(expr.Expressions[0], 
                LatexStringToXmlString(expr.Expressions[1][0].Name),
                expr.ExprType == ExpressionType.InlineMath, expr.Customization);
        }
    }
}
