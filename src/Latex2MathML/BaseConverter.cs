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
    /// The base class for all converters. Provides generic functions and tools.
    /// </summary>
    internal abstract class BaseConverter
    {        
        /// <summary>
        /// Gets the type of the corresponding expression.
        /// </summary>
        public abstract ExpressionType ExprType { get; }        

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public abstract string Convert(LatexExpression expr);        

        /// <summary>
        /// Filters special characters that can not be used in XML directly.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The converted string.</returns>
        protected static string LatexStringToXmlString(string str)
        {
            var conversionTable = new Dictionary<char, string>
            {
                {'<',  "&lt;"},
                {'>',  "&gt;"},
                {'&',  "&amp;"},
                {'"',  "&quot;"},
                {'\'', "&apos;"},
                {'~',  "&#xA0;<!-- &nbsp; -->"}
            };
            foreach (var pair in conversionTable)
            {
                str = str.Replace("" + pair.Key, pair.Value);
            }
            return str;
        }

        protected static string NoLatexString(string str)
        {
            return str;
        }
    }
}
