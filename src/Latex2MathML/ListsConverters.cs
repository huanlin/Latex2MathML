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
    /// The base converter class for lists.
    /// </summary>
    internal class ListConverter : BlockConverter
    {
        /// <summary>
        /// The attribute to set the list properties.
        /// </summary>
        internal class ListTypeAttribute : System.Attribute
        {
            /// <summary>
            /// Gets the XHTML tag name corresponding to the assosiated list type.
            /// </summary>
            public string TagName { get; private set; }

            /// <summary>
            /// Gets the name of the list type.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Initializes a new instance of the ListTypeAttribute class.
            /// </summary>
            /// <param name="name">The name of the list type. <see cref="System.String"/></param>
            /// <param name="tagName">The name of the corresponding XHTML tag. <see cref="System.String"/></param>
            public ListTypeAttribute(string name, string tagName)
            {
                Name = name;
                TagName = tagName;
            }
        }

        private string _name;
        private string _tagName;

        private void GetProperties()
        {
            if (_tagName == null)
            {
                var attr = (ListTypeAttribute) this.GetType().GetCustomAttributes(typeof (ListTypeAttribute), true)[0];
                _name = attr.Name;
                _tagName = attr.TagName;
            }
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                GetProperties();
                return _name;
            }
        }

        /// <summary>
        /// Performs the conversion procedure.
        /// </summary>
        /// <param name="expr">The expression to convert.</param>
        /// <returns>The conversion result.</returns>
        public override string Convert(LatexExpression expr)
        {
            if (expr.Expressions == null) return "";
            GetProperties();
            var list = "<" + _tagName + ">\n";
            list += SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization);
            list += "</" + _tagName + ">\n";
            return list;
        }

        /// <summary>
        /// The supported Latex list types.
        /// </summary>
        public static readonly List<string> ListNames = new List<string>
        {
            "itemize", "enumerate", "description"
        };
    }

    /// <summary>
    /// The converter class for itemize blocks.
    /// </summary>
    [ListConverter.ListTypeAttribute("itemize", "ul")]
    internal sealed class ItemizeConverter : ListConverter {}

    /// <summary>
    /// The converter class for enumerate blocks.
    /// </summary>
    [ListConverter.ListTypeAttribute("enumerate", "ol")]
    internal sealed class EnumerateConverter : BlockConverter {}

    /// <summary>
    /// The converter class for description blocks.
    /// </summary>
    [ListConverter.ListTypeAttribute("description", "dl")]
    internal sealed class DescriptionConverter : BlockConverter {}

    /// <summary>
    /// The converter class for \item.
    /// </summary>
    internal sealed class ItemCommandConverter : CommandConverter
    {
        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "item";
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
            var bld = new StringBuilder();
            if (expr.Parent.Name == "enumerate" || expr.Parent.Name == "itemize")
            {
                bld.Append("<li>");
                if (expr.Expressions != null)
                {
                    bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
                }
                bld.Append("</li>");
            }
            if (expr.Parent.Name == "description")
            {
                if (expr.Options != null)
                {
                    bld.Append("<dt>");
                    bld.Append(SequenceConverter.ConvertOutline(expr.Options.AsExpressions, expr.Customization));
                    bld.Append("</dt>\n");
                }
                if (expr.Expressions != null)
                {
                    bld.Append("<dd>");
                    bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
                    bld.Append("</dd>\n");
                }                
            }
            return bld.ToString();
        }
    }
}

