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
using System.Collections.Generic;
using System.Text;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for figures.
    /// </summary>
    internal sealed class WrapperConverter : BlockConverter
    {
        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            var bld = new StringBuilder();
            bld.Append("<table class=\"");           
            bld.Append(expr.Name);
            bld.Append("\">\n<tr><td>");
            bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
            bld.Append("</td></tr>");
            string caption = null;         
            expr.EnumerateChildren(child =>
            {
                if (child.ExprType == ExpressionType.Command && child.Name == "caption" && child.Expressions != null)
                {
                    caption = SequenceConverter.ConvertOutline(child.Expressions[0], expr.Customization);
                    return true;
                }
                return false;
            });

            #region Add the caption
            bld.Append("\n<tr><td class=\"caption\">");            
            if (expr.Name == "figure")
            {
                bld.Append(expr.Customization.Localization == "ru" ? "Рисунок" : "Figure");                
            }  
            if (expr.Name == "table")
            {
                bld.Append(expr.Customization.Localization == "ru" ? "Таблица" : "Table");
            }
            if (expr.Name == "algorithm")
            {
                bld.Append(expr.Customization.Localization == "ru" ? "Алгоритм" : "Algorithm");
            } 
            bld.Append(" ");
            bld.Append((int)expr.Tag);
            bld.Append(".");
            if (caption != null)
            {
                bld.Append(" ");
                bld.Append(caption);
            }
            #endregion
            bld.Append("</td></tr>\n");
            bld.Append("</table>\n");
            return bld.ToString();
        }
    }
}

