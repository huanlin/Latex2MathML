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
    /// The base converter class for text size changes.
    /// </summary>
    internal class TextSizeConverter : CommandConverter
    {
        /// <summary>
        /// The attribute to set the text size.
        /// </summary>
        internal class TextSizeAttribute : System.Attribute
        {
            /// <summary>
            /// Gets the text size.
            /// </summary>
            public string Size { get; private set; }

            /// <summary>
            /// Initializes a new instance of the TextSizeAttribute class.
            /// </summary>
            /// <param name="size">The text size. <see cref="System.String"/>
            /// </param>
            public TextSizeAttribute(string size)
            {
                Size = size;
            }
        }

		/// <summary>
		///The dictionary to parse the text size. 
		/// </summary>
		private static readonly Dictionary<string, TextSize> TextSizeDictionary = new Dictionary<string, TextSize>
		{
			{"tiny", TextSize.tiny},
			{"scriptsize", TextSize.scriptsize},
			{"footnotesize", TextSize.footnotesize},
			{"small", TextSize.small},
			{"normalsize", TextSize.normalsize},
			{"large", TextSize.large},
			{"Large", TextSize.Large},
			{"LARGE", TextSize.LARGE},
			{"huge", TextSize.huge},
			{"Huge", TextSize.Huge},
		};	
		
		private string _size;
		
		private void GetSize()
		{
			if (_size == null)
			{
				_size = ((TextSizeAttribute)this.GetType().GetCustomAttributes(typeof(TextSizeAttribute), true)[0]).Size;
			}
		}
		
		/// <summary>
        /// Gets the name of the text size.
        /// </summary>
        public override string Name
        {
            get
            {				
				GetSize();				
                return _size;
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 0; }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {            
            GetSize();
			expr.Customization.CurrentTextSize = TextSizeDictionary[_size];			
            return "";
        }
    }
	
	[TextSize("tiny")]
	internal sealed class TextSize_tiny_Converter : TextSizeConverter {}

	[TextSize("scriptsize")]
	internal sealed class TextSize_scriptsize_Converter : TextSizeConverter {}
	
	[TextSize("footnotesize")]
	internal sealed class TextSize_footnotesize_Converter : TextSizeConverter {}
	
	[TextSize("small")]
	internal sealed class TextSize_small_Converter : TextSizeConverter {}
	
	[TextSize("normalsize")]
	internal sealed class TextSize_normalsize_Converter : TextSizeConverter {}
	
	[TextSize("large")]
	internal sealed class TextSize_large_Converter : TextSizeConverter {}
	
	[TextSize("Large")]
	internal sealed class TextSize_Large_Converter : TextSizeConverter {}
	
	[TextSize("LARGE")]
	internal sealed class TextSize_LARGE_Converter : TextSizeConverter {}
	
	[TextSize("huge")]
	internal sealed class TextSize_huge_Converter : TextSizeConverter {}
	
	[TextSize("Huge")]
	internal sealed class TextSize_Huge_Converter : TextSizeConverter {}
}
