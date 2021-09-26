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
    /// The converter class for urls.
    /// </summary>
    internal sealed class UrlCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "url";
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
            if (expr.Expressions == null) return "";
            var url = expr.Expressions[0][0].Name;
            return "<a href=\"" + url + "\">" + url + "</a>";
        }
    }    

    /// <summary>
    /// The converter class for hrefs.
    /// </summary>
    internal sealed class HrefCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "href";
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
            if (expr.Expressions == null) return "";
            var url = expr.Expressions[0][0].Name;
            var text = SequenceConverter.ConvertOutline(expr.Expressions[1], expr.Customization);
            return "<a href=\"" + url + "\">" + text + "</a>";
        }
    }
}
