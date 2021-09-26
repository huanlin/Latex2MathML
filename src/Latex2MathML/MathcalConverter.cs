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
using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for roots.
    /// </summary>
    internal sealed class MathcalCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "mathcal";
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 1; }
        }

        private static readonly Dictionary<char, string> ConversionTable = new Dictionary<char, string>
        {
            #region Initialization
            {'A', "&#x1D4D0;"},  
            {'B', "&#x1D4D1;"},     
            {'C', "&#x1D4D2;"}, 
            {'D', "&#x1D4D3;"}, 
            {'E', "&#x1D4D4;"}, 
            {'F', "&#x1D4D5;"}, 
            {'G', "&#x1D4D6;"}, 
            {'H', "&#x1D4D7;"}, 
            {'I', "&#x1D4D8;"}, 
            {'J', "&#x1D4D9;"}, 
            {'K', "&#x1D4DA;"}, 
            {'L', "&#x1D4DB;"}, 
            {'M', "&#x1D4DC;"}, 
            {'N', "&#x1D4DD;"}, 
            {'O', "&#x1D4DE;"}, 
            {'P', "&#x1D4DF;"}, 
            {'Q', "&#x1D4E0;"}, 
            {'R', "&#x1D4E1;"},                  
            {'S', "&#x1D4E2;"}, 
            {'T', "&#x1D4E3;"}, 
            {'U', "&#x1D4E4;"}, 
            {'V', "&#x1D4E5;"}, 
            {'W', "&#x1D4E6;"}, 
            {'X', "&#x1D4E7;"}, 
            {'Y', "&#x1D4E8;"}, 
            {'Z', "&#x1D4E9;"}, 

            {'a', "&#x1D4EA;"},  
            {'b', "&#x1D4EB;"},     
            {'c', "&#x1D4EC;"}, 
            {'d', "&#x1D4ED;"}, 
            {'e', "&#x1D4EE;"}, 
            {'f', "&#x1D4EF;"}, 
            {'g', "&#x1D4F0;"}, 
            {'h', "&#x1D4F1;"}, 
            {'i', "&#x1D4F2;"}, 
            {'j', "&#x1D4F3;"}, 
            {'k', "&#x1D4F4;"}, 
            {'l', "&#x1D4F5;"}, 
            {'m', "&#x1D4F6;"}, 
            {'n', "&#x1D4F7;"}, 
            {'o', "&#x1D4F8;"}, 
            {'p', "&#x1D4F9;"}, 
            {'q', "&#x1D4FA;"}, 
            {'r', "&#x1D4FB;"},                  
            {'s', "&#x1D4FC;"}, 
            {'t', "&#x1D4FD;"}, 
            {'u', "&#x1D4FE;"}, 
            {'v', "&#x1D4FF;"}, 
            {'w', "&#x1D500;"}, 
            {'x', "&#x1D501;"}, 
            {'y', "&#x1D502;"}, 
            {'z', "&#x1D503;"},
            #endregion
        };

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            var letter = expr.Expressions[0][0].Name[0];
            string converted;
            if (!ConversionTable.TryGetValue(letter, out converted))
            {
                converted = "" + letter;
            }
            return converted;           
        }
    }
}
