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

using System;
using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for section commands.
    /// </summary>
    internal sealed class SectionConverter : CommandConverter
    {
        /// <summary>
        /// Gets the name of the command (section).
        /// </summary>
        public override string Name
        {
            get
            {
                return "section*";
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
            expr.Customization.CurrentSectionType = SectionType.Unnumbered;
            var bld = new StringBuilder("<h2 class=\"section\">\n<a id=\"nns");
            var title = SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization);
            int index = expr.Customization.SectionContents[SectionType.Unnumbered].Count;
            expr.Customization.SectionContents[SectionType.Unnumbered].Add(
                new SectionContentsValue(title));
            bld.Append(index);
            bld.Append("\">");
            bld.Append(title);
            bld.Append("</a></h2>\n");
            return bld.ToString();
        }
    }
}
