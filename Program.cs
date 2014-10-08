// SkyWatcher initialization

using SkyWatcher.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace SkyWatcher
{
    public static class Program
    {
        public static MainForm myForm;
        public static void Main(string[] args)
        {
            CheckArguments(args);
            SkyObjectLibrary.InitializeLibrary();
            Console.SetIn(TextReader.Synchronized(new StreamReader(Console.OpenStandardInput(1024), Console.OutputEncoding, false, 1024, true)));
            Console.WriteLine("SkyWatcher v1.0 with UI Complete (@ 2014-10-08 19:50) Luismark");
            if (args.Length == 2)
                myForm = new MainForm(int.Parse(args[0]), int.Parse(args[1]));
            if (args.Length == 1)
                myForm = new MainForm(args[0]);
            if (args.Length == 0)
                myForm = new MainForm((Math.Abs(DateTime.Now.Month - 7) <= 2) ? "Altair" : "Sirius");
            Application.Run(myForm);
        }
        public static void CheckArguments(string[] args) {
            if (args.Length > 2) {
                Console.WriteLine("Usage: SkyWatcher <[<ra> <dec>] | [<name>]>");
                Environment.Exit(unchecked((int)(0x80070057)));
            }
        }
    }
}
