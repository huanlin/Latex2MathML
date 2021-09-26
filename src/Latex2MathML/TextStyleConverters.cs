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
	internal class TextStyleConverter : CommandConverter
	{
        /// <summary>
        /// The attribute to set the text style.
        /// </summary>
        internal class TextStyleAttribute : System.Attribute
        {
            /// <summary>
            /// Gets the text style.
            /// </summary>
            public TextStyle Style { get; private set; }

            /// <summary>
            /// Gets the corresponding Latex command name.
            /// </summary>
            public string StyleCommandName { get; private set; }

            /// <summary>
            /// Initializes a new instance of the TextStyleAttribute class.
            /// </summary>
            /// <param name="style">The text style. <see cref="TextStyle"/></param>
            /// <param name="styleCommandName">The corresponding Latex command name.</param>
            public TextStyleAttribute(TextStyle style, string styleCommandName)
            {
                Style = style;
                StyleCommandName = styleCommandName;
            }
        }

		private TextStyle _style;
		private string _styleCommandName;
		
		private void GetStyle()
		{
			if (_styleCommandName == null)
			{
				var attr = (TextStyleAttribute)this.GetType().GetCustomAttributes(typeof(TextStyleAttribute), true)[0];
				_style = attr.Style;
				_styleCommandName = attr.StyleCommandName;
			}
		}
		
		/// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                GetStyle();
                return _styleCommandName;
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
			GetStyle();
            var bld = new StringBuilder();
            var backupTextStyle = expr.Customization.CurrentTextStyle;
			expr.Customization.CurrentTextStyle |= _style;
			bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
			expr.Customization.CurrentTextStyle = backupTextStyle;
            return bld.ToString();
        }
	}
	
	/// <summary>
    /// The converter class for \textit.
    /// </summary>
    [TextStyle(TextStyle.Italic, "textit")]
    internal sealed class TextitStyleConverter : TextStyleConverter {}   
	
	/// <summary>
    /// The converter class for \emph.
    /// </summary>
    [TextStyle(TextStyle.Italic, "emph")]
    internal sealed class EmphStyleConverter : TextStyleConverter {}
    
	/// <summary>
    /// The converter class for \textbf.
    /// </summary>
    [TextStyle(TextStyle.Bold, "textbf")]
    internal sealed class TextbfStyleConverter : TextStyleConverter {}
	
	/// <summary>
    /// The converter class for \underline.
    /// </summary>
    [TextStyle(TextStyle.Underlined, "underline")]
    internal sealed class UnderlineStyleConverter : TextStyleConverter
	{
	    /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
	    {
            if (expr.MathMode == false)
            {
                return base.Convert(expr);
            }
            if (expr.Expressions == null) return "";
            var bld = new StringBuilder();
            bld.Append("<munder accent=\"true\">\n");
            bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
            bld.Append("<mo stretchy=\"true\">&#x00af;<!-- &OverBar; --></mo>\n</munder>\n");                       
            return bld.ToString();
	    }
	}
}