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
    /// The converter class for single super- and subscripts.
    /// </summary>
    internal sealed class SingleScriptConverter : BlockConverter
    {
        /// <summary>
        /// The script tag name. Can be either sub or sup.
        /// </summary>
        private string _tag;

        /// <summary>
        /// Initializes a new instance of the SingleScriptConverter class.
        /// </summary>
        /// <param name="type">The tag name of the script.</param>
        public SingleScriptConverter(string type)
        {
            _tag = type;
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            var bld = new StringBuilder("<");
            var tag = _tag;
            if (CommandConverter.MathFunctionsScriptCommandConstants.ContainsKey(expr.Expressions[0][0].Name))
            {
                tag = (_tag == "msup") ? "mover" : "munder";
            }
            bld.Append(tag);
            bld.Append(">\n<mrow>\n");
            var first = expr.Expressions[0][0].Convert();
            if (first != "</mrow>\n</mfenced>\n")
            {
                bld.Append(first);
            }
            bld.Append("</mrow>\n<mrow>\n");
            bld.Append(expr.Expressions[0][1].Convert());
            bld.Append("</mrow>\n</");
            bld.Append(tag);
            bld.Append(">\n");
            if (first != "</mrow>\n</mfenced>\n")
            {
                return bld.ToString();
            }
            return first + bld;
        }
    }
}
