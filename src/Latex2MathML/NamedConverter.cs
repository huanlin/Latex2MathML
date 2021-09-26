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

namespace Latex2MathML
{
    /// <summary>
    /// The intermediate converter class which defines the "Name" property.
    /// </summary>
    internal abstract class NamedConverter : BaseConverter
    {
        /// <summary>
        /// Gets the name of the corresponding expression.
        /// </summary>
        public abstract string Name { get; }
    }
}
