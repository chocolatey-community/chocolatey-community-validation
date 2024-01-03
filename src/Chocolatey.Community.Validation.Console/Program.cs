namespace Chocolatey.Community.Validation.Console
{
    using System;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            throw new ApplicationException("You must launch the copied choco.exe instead of this application!");
        }
    }
}
