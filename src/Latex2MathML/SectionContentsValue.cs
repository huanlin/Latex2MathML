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
    /// The structure which represents a section content.
    /// </summary>
    internal struct SectionContentsValue
    {
        /// <summary>
        /// The list of subsections ordered by type.
        /// </summary>
        private Dictionary<SectionType, List<string>> _subsections;

        /// <summary>
        /// Gets the list of subsections ordered by type.
        /// </summary>
        public Dictionary<SectionType, List<string>> Subsections
        {
            get
            {
                return _subsections;
            }
        }

        /// <summary>
        /// The section title.
        /// </summary>
        private string _title;

        /// <summary>
        /// Gets the section title.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }
        }

        /// <summary>
        /// Initializes a new instance of the SectionContentsValue struct.
        /// </summary>
        /// <param name="title">The section title.</param>
        public SectionContentsValue(string title)
        {
            _title = title;
            _subsections = new Dictionary<SectionType, List<string>>();
            _subsections.Add(SectionType.Numbered, new List<string>());
            _subsections.Add(SectionType.Unnumbered, new List<string>());
        }
    }
}
