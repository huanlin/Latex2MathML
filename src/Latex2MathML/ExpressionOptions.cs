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
    /// The class to handle LaTeX command options.
    /// </summary>
    internal sealed class ExpressionOptions
    {
        /// <summary>
        /// Get or sets the options as Dictionary&lt;string, string&gt;.
        /// </summary>
        public Dictionary<string, string> AsKeyValue { get; set; }

        /// <summary>
        /// Gets or sets the options as List&lt;LatexExpression&gt;.
        /// </summary>
        public List<LatexExpression> AsExpressions { get; set; }

        /// <summary>
        /// Initializes a new instance of the ExpressionOptions class.
        /// </summary>
        public ExpressionOptions() { }

        /// <summary>
        /// Initializes a new instance of the ExpressionOptions class.
        /// </summary>
        /// <param name="options">The options to copy from.</param>
        public ExpressionOptions(ExpressionOptions options)
        {
            if (options != null)
            {
                if (options.AsKeyValue != null)
                {
                    AsKeyValue = new Dictionary<string, string>(options.AsKeyValue);
                }
                if (options.AsExpressions != null)
                {
                    AsExpressions = new List<LatexExpression>(options.AsExpressions);
                }
            }
        }
    }
}
