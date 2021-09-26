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
    /// The converter class for roots.
    /// </summary>
    internal sealed class SqrtCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "sqrt";
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var bld = new StringBuilder();
            if (expr.Options == null)
            {
                bld.Append("<msqrt>\n<mrow>\n");
                bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
                bld.Append("</mrow>\n</msqrt>\n");
                return bld.ToString();
            }
            bld.Append("<mroot>\n<mrow>\n");
            bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
            bld.Append("</mrow>\n<mrow>\n");
			bld.Append(SequenceConverter.ConvertOutline(expr.Options.AsExpressions, expr.Customization));
            bld.Append("</mrow>\n</mroot>\n");
            return bld.ToString();
        }
    }
}
