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

namespace Latex2MathML
{
    /// <summary>
    /// The converter class to convert LaTeX comments.
    /// </summary>
    internal sealed class CommentConverter : BaseConverter
    {
        /// <summary>
        /// Gets the type of the corresponding expression (ExpressionType.Comment).
        /// </summary>
        public override ExpressionType ExprType
        {
            get { return ExpressionType.Comment; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var str = LatexStringToXmlString(expr.Name);
            while (str.IndexOf("--") > -1)
            {
                str = str.Replace("--", "-");
            }
            return "<!-- " + str + " -->\n";
        }
    }
}
