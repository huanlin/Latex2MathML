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
using System.Collections.Generic;

namespace Latex2MathML
{
    /// <summary>
    /// The block converter for simple sequences of expressions.
    /// </summary>
    internal sealed class SequenceConverter : BlockConverter
    {
        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            return ConvertOutline(expr.Expressions[0], expr.Customization);
        }
		
		public static string ConvertOutline(IList<LatexExpression> outline, LatexToMathMLConverter customization)
		{
			var bld = new StringBuilder();
			var backupTextSize = customization.CurrentTextSize;
			var backupTextStyle = customization.CurrentTextStyle;
            foreach (var child in outline)
            {
                bld.Append(child.Convert());
            }
			customization.CurrentTextSize = backupTextSize;
			customization.CurrentTextStyle = backupTextStyle;
            return bld.ToString();
		}
    }
}
