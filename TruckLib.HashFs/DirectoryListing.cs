using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib.HashFs
{
    /// <summary>
    /// Represents a directory listing file in a HashFS archive.
    /// </summary>
    /// <param name="Subdirectories">The names of subdirectories in this directory.</param>
    /// <param name="Files">The names of files in this directory.</param>
    public record DirectoryListing(List<string> Subdirectories, List<string> Files);
}
