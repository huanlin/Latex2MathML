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
    /// The converter class for paragraphs and subparagraphs.
    /// </summary>
    internal sealed class ParagraphConverter : BlockConverter
    {
        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.MathMode) return "";
            var bld = new StringBuilder();
            int parentChildNumber = 0;
            if (expr.Expressions.Count > 1)
            {
                parentChildNumber = 1;
                bld.Append("<h4 class=\"");
                bld.Append(expr.Name);
                bld.Append("\">");
                for (int i = 0; i < expr.Expressions[0].Count; i++)
                {
                    bld.Append(expr.Expressions[0][i].Convert());
                }
                bld.Append("</h4>\n");
            }
            bld.Append("<p>\n");
            for (int i = 0; i < expr.Expressions[parentChildNumber].Count; i++)
            {
                bld.Append(expr.Expressions[parentChildNumber][i].Convert());
            }
            bld.Append("</p>\n");
            return bld.ToString();
        }
    }
}
