using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Chocolatey CCR Extension")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCulture("")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("da76f5f0-7803-4691-b3c9-08901650c88e")]
