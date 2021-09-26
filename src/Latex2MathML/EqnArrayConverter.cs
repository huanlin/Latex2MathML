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

using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The converter for eqnarray blocks.
    /// </summary>
    internal sealed class EqnArrayConverter : BlockConverter
    {
        /// <summary>
        /// Performs the conversion procedure (comments the expression).
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var bld = new StringBuilder();
            MathConverter.AppendMathProlog(bld, "eqnarray", false, expr.Customization);
            var rows = expr.Expressions[0];
            bld.Append("<mtable>\n");
            for (int i = 0; i < rows.Count; i++)
            {
                bld.Append("<mtr>\n");
                for (int j = 0; j < rows[i].Expressions[0].Count; j++)
                {
                    bld.Append("<mtd columnalign=\"left\">\n<mrow>\n");
					bld.Append(SequenceConverter.ConvertOutline(rows[i].Expressions[0][j].Expressions[0], expr.Customization));
                    bld.Append("</mrow>\n</mtd>\n");
                }
                bld.Append("</mtr>\n");
            }
            bld.Append("</mtable>\n");
            MathConverter.AppendMathEpilog(bld);
            return bld.ToString();
        }

        /// <summary>
        /// Returns true if the block defines a math environment; otherwise, false.
        /// </summary>
        /// <returns></returns>
        public override bool ImpliesMathMode()
        {
            return true;
        }
    }
}

