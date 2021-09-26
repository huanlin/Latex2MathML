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
using System.Text;
using Latex2MathML;

namespace ltx2mml
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Graphics can be automatically converted from eps/pdf to png. The path to GhostScript must be set to provide that.\n");
            Console.WriteLine("For now, it is hard-coded: d:\\Program Files\\GhostScript\\gs9.00\\bin\\gswin32c.exe");
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ltx2mml doc.tex doc.xhtml");
                return;
            }
#if MONO
		LatexToMathMLConverter.GhostScriptBinaryPath = "gs";	
#else
            LatexToMathMLConverter.GhostScriptBinaryPath = @"d:\Program Files\GhostScript\gs9.00\bin\gswin32c.exe";
#endif
            var lmm = new LatexToMathMLConverter(
                args[0],
                Encoding.UTF8,
                args[1]);
            lmm.ValidateResult = true;
            lmm.Convert();
        }
    }
}
