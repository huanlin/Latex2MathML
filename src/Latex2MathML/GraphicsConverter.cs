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
using System.IO;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace Latex2MathML
{
    /// <summary>
    /// The converter class for \includegraphics.
    /// </summary>
    internal sealed class GraphicsConverter : CommandConverter
    {
        /// <summary>
        /// Gets the name of the command (section).
        /// </summary>
        public override string Name
        {
            get
            {
                return "includegraphics";
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
            var fileName = expr.Expressions[0][0].Name;
            var sourcePath = Path.GetDirectoryName(expr.Customization.SourcePath)?? "";
            var destPath = Path.GetDirectoryName(expr.Customization.OutputPath)?? "";
			var programPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (String.IsNullOrEmpty(sourcePath))
			{
				sourcePath = programPath;	
			}
			if (String.IsNullOrEmpty(destPath))
			{
				destPath = programPath;
			}
			if (String.IsNullOrEmpty(Path.GetExtension(fileName)))
			{
				fileName += ".eps";
			}
			var sourceFullName = Path.IsPathRooted(fileName)? fileName : Path.Combine(sourcePath, fileName);
			var destFullName = Path.Combine(destPath, Path.GetFileName(fileName)); 
			var ext = Path.GetExtension(fileName);
            if ((ext == ".eps" || ext == ".pdf") && File.Exists(sourceFullName))
            {
                destFullName = Path.ChangeExtension(destFullName, "png");
                if (!File.Exists(destFullName))
                {
                    Process.Start(LatexToMathMLConverter.GhostScriptBinaryPath,
                                  "-dSAFER -dBATCH -dNOPAUSE -sDEVICE=pngalpha -dEPSCrop " +
                                  "-dGraphicsAlphaBits=4 -dTextAlphaBits=4 -sOutputFile=" +
                                  destFullName + " " + sourceFullName);
                }
            }
            else
            {
                if (sourceFullName != destFullName)
                {
                    File.Copy(Path.Combine(sourcePath, fileName), Path.Combine(destPath, fileName), true);
                }
            }
			if (destFullName.StartsWith(programPath))
			{
				destFullName = destFullName.Substring(programPath.Length);
				if (destFullName.StartsWith("" + Path.DirectorySeparatorChar))
				{
					destFullName = destFullName.Substring(1);	
				}
			}
            var img = "<img src=\"" + destFullName + "\" alt=\"\\includegraphics {" + fileName + "}\" />";
            return img;
        }
    }
}
