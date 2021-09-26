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
    /// The converter class to convert LaTeX plain text.
    /// </summary>
    internal sealed class PlainTextConverter : BaseConverter
    {
        /// <summary>
        /// The list of certain text substitutions.
        /// </summary>
        private static readonly Dictionary<string, string> MathConstants = new Dictionary<string, string>
        {
            {">=", "<mo>&le;</mo>\n"},
            {"<=", "<mo>&ge;</mo>\n"},
            {">", "<mo>&lt;</mo>\n"},
            {"<", "<mo>&gt;</mo>\n"},
            {"(", "<mfenced>\n<mrow>\n"},
            {")", "</mrow>\n</mfenced>\n"}
        };

        /// <summary>
        /// Gets the type of the corresponding expression (ExpressionType.PlainText).
        /// </summary>
        public override ExpressionType ExprType
        {
            get { return ExpressionType.PlainText; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (!expr.MathMode)
            {
				var formatDiv = "<span class=\"" + expr.Customization.GetCurrentCssStyle() + "\">";
                formatDiv += LatexStringToXmlString(expr.Name);
				return formatDiv + "</span>";
            }
            if (expr.Name == "InvisibleTimes")
            {
                return "<mo>&#x2062;<!-- &InvisibleTimes; --></mo>\n";
            }
            if (char.IsLetter(expr.Name[0]))
            {
                return "<mi>" + expr.Name + "</mi>\n";
            }
            if (char.IsDigit(expr.Name[0]))
            {
                return "<mn>" + expr.Name + "</mn>\n";
            }
            string str;
            if (MathConstants.TryGetValue(expr.Name, out str))
            {
                return str;
            }
            return "<mo>" + LatexStringToXmlString(expr.Name.Trim()) + "</mo>\n";
        }
    }
}
