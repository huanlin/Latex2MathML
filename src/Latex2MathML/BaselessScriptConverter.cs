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
    /// The block converter for baseless scripts. It transforms ^A to &lt;sup&gt;A&lt;/sup&gt;.
    /// </summary>
    internal sealed class BaselessConverter : BlockConverter
    {
        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            if (expr.Parent.ExprType == ExpressionType.Block && expr.Parent.Name.IndexOf("script") > -1)
            {
                return SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization);
            }
            var bld = new StringBuilder();
            string tag;
            tag = expr.Name == "^" ? "sup" : "sub";
            bld.Append("<" + tag + ">\n");			
            bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
            bld.Append("</" + tag + ">");
            return bld.ToString();
        }
    }
}
