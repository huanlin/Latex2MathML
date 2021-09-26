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
    /// The base converter class for accents.
    /// </summary>
    internal class AccentConverter : CommandConverter
    {
        /// <summary>
        /// The attribute to set the accent parameters.
        /// </summary>
        internal class AccentAttribute : System.Attribute
        {
            /// <summary>
            /// Gets the Latex name of the accent.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets the corresponding Unicode entity.
            /// </summary>
            public string Value { get; private set; }

            /// <summary>
            /// Gets or sets the value indicating whether the accent can be applied to several expressions as well.
            /// </summary>
            public bool Stretchy { get; private set; }

            /// <summary>
            /// Gets or sets the value indicating whether the accent is applied in math mode.
            /// </summary>
            public bool MathMode { get; private set; }

            /// <summary>
            /// Initializes a new instance of the TextSizeAttribute class.
            /// </summary>
            /// <param name="name">The Latex name of the accent. <see cref="System.String"/> </param>
            /// <param name="value">The corresponding Unicode entity. <see cref="System.String"/> </param>
            /// <param name="stretchy">The value indicating whether the accent can be applied to several expressions as well.</param>
            /// <param name="mathMode">The value indicating whether the accent is applied in math mode.</param>
            public AccentAttribute(string name, string value, bool stretchy, bool mathMode)
            {
                Name = name;
                Value = value;
                Stretchy = stretchy;
                MathMode = mathMode;
            }
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public override string Name
        {
            get
            {
                GetParameters();
                return _name;
            }
        }

        /// <summary>
        /// Gets the expected count of child subtrees.
        /// </summary>
        public override int ExpectedBranchesCount
        {
            get { return 1; }
        }

        private string _name;
        private string _value;
        private bool _stretchy;
        private bool _mathMode;

        private void GetParameters()
        {
            if (_name == null)
            {
                var attr = (AccentAttribute) this.GetType().GetCustomAttributes(typeof (AccentAttribute), true)[0];
                _name = attr.Name;
                _value = attr.Value;
                _stretchy = attr.Stretchy;
                _mathMode = attr.MathMode;
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
            GetParameters();
            var bld = new StringBuilder();
            if (_mathMode)
            {
                bld.Append("<mover accent=\"true\">\n");
                if (!_stretchy)
                {
                    bld.Append(expr.Expressions[0][0].Convert());
                }
                else
                {
                    bld.Append(SequenceConverter.ConvertOutline(expr.Expressions[0], expr.Customization));
                }
                bld.Append("<mo stretchy=\"true\">" + _value + "</mo>\n</mover>\n");
            }
            else
            {
                MathConverter.AppendMathProlog(bld, "accent", true, expr.Customization);
                bld.Append("<mrow>");
                bld.Append("<mover accent=\"true\">\n");
                bld.Append(expr.Expressions[0][0].Convert());
                bld.Append("<mo>" + _value + "</mo>\n</mover>\n");
                bld.Append("</mrow>\n");
                MathConverter.AppendMathEpilog(bld);
            }
            return bld.ToString();
        }
    }

    /// <summary>
    /// The converter class for \overline.
    /// </summary>
    [Accent("overline", "&#x00af;<!-- &OverBar; -->", true, true)]
    internal sealed class OverlineAccentConverter : AccentConverter {}

    /// <summary>
    /// The converter class for \hat.
    /// </summary>
    [Accent("hat", "&#x005E;<!-- &Hat; -->", false, true)]
    internal sealed class HatAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \widehat.
    /// </summary>
    [Accent("widehat", "&#x005E;<!-- &Hat; -->", true, true)]
    internal sealed class WidehatAccentConverter : AccentConverter {}

    /// <summary>
    /// The converter class for \check.
    /// </summary>
    [Accent("check", "&#x2228;<!-- &or; -->", false, true)]
    internal sealed class CheckAccentConverter : AccentConverter {}

    /// <summary>
    /// The converter class for \tilde.
    /// </summary>
    [Accent("tilde", "~", false, true)]
    internal sealed class TildeAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \widetilde.
    /// </summary>
    [Accent("widetilde", "~", true, true)]
    internal sealed class WidetildeAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \acute.
    /// </summary>
    [Accent("acute", "&#x00B4;<!-- &acute; -->", false, true)]
    internal sealed class AcuteAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \grave.
    /// </summary>
    [Accent("grave", "&#x0060;<!-- \\grave -->", false, true)]
    internal sealed class GraveAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \dot.
    /// </summary>
    [Accent("dot", "&#x00B7;<!-- &middot; -->", false, true)]
    internal sealed class DotAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \ddot.
    /// </summary>
    [Accent("ddot", "&#x0308;<!-- \\ddot -->", false, true)]
    internal sealed class DdotAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \breve.
    /// </summary>
    [Accent("breve", "&#x0306;<!-- \\breve -->", false, true)]
    internal sealed class BreveAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \bar.
    /// </summary>
    [Accent("bar", "&#x0304;<!-- \\bar -->", false, true)]
    internal sealed class BarAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \vec.
    /// </summary>
    [Accent("vec", "&#x20D7;<!-- \\vec -->", false, true)]
    internal sealed class VecAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \^.
    /// </summary>
    [Accent("^", "&#x005E;<!-- &Hat; -->", false, false)]
    internal sealed class HatTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \v.
    /// </summary>
    [Accent("v", "&#x2228;<!-- &or; -->", false, false)]
    internal sealed class CheckTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \~.
    /// </summary>
    [Accent("~", "~", false, false)]
    internal sealed class TildeTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \'.
    /// </summary>
    [Accent("'", "&#x00B4;<!-- &acute; -->", false, false)]
    internal sealed class AcuteTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \`.
    /// </summary>
    [Accent("`", "&#x0060;<!-- \\grave -->", false, false)]
    internal sealed class GraveTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \..
    /// </summary>
    [Accent(".", "&#x00B7;<!-- &middot; -->", false, false)]
    internal sealed class DotTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \".
    /// </summary>
    [Accent("\"", "&#x0308;<!-- \\ddot -->", false, false)]
    internal sealed class DdotTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \u.
    /// </summary>
    [Accent("u", "&#x0306;<!-- \\breve -->", false, false)]
    internal sealed class BreveTextAccentConverter : AccentConverter { }

    /// <summary>
    /// The converter class for \=.
    /// </summary>
    [Accent("=", "&#x0304;<!-- \\bar -->", false, false)]
    internal sealed class BarTextAccentConverter : AccentConverter { }
}
