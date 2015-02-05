using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace greet
{
    class Program
    {
        static int verbosity;

        public static void Main(string[] args)
        {
            bool show_help = false;
            bool images_only = false;
            bool galleries_only = false;
            List<string> names = new List<string>();
            List<string> siteCodes = new List<string>();
            List<string> destCodes = new List<string>();
            List<string> startDates = new List<string>();
            List<string> endDates = new List<string>();

            int repeat = 1;

            var p = new OptionSet()
            {
                { "c|sitecode=", " (required) the source Site Code.",  v=> siteCodes.Add(v) },
                { "d|destcode=", " (required) the Destination Pub Code.",  v=> destCodes.Add(v) },
                { "s|start=", " (required) the start Date.",  v=> startDates.Add(v) },
                { "e|end=", " (required) the End Date.",  v=> endDates.Add(v) },
                { "i|images", " (Optional) migrate Images Only",  v=> images_only = v != null },
                { "g|galleries", " (Optional) migrate Images Only",  v=> galleries_only = v != null },
    			{ "h|help",  " Show this message and exit", v => show_help = v != null },
            };


        //    var p = new OptionSet() {
        //    { "n|name=", "the {NAME} of someone to greet.",
        //      v => names.Add (v) },
        //    { "r|repeat=", 
        //        "the number of {TIMES} to repeat the greeting.\n" + 
        //            "this must be an integer.",
        //      (int v) => repeat = v },
        //    { "v", "increase debug message verbosity",
        //      v => { if (v != null) ++verbosity; } },
        //    { "h|help",  "show this message and exit", 
        //      v => show_help = v != null },
        //};

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                Console.ReadLine();

                return;
            }

            string message;
            if (extra.Count > 0)
            {
                message = string.Join(" ", extra.ToArray());
                Debug("Using new message: {0}", message);
            }
            else
            {
                message = "Hello {0}!";
                Debug("Using default message: {0}", message);
            }

            foreach (string name in names)
            {
                for (int i = 0; i < repeat; ++i)
                    Console.WriteLine(message, name);
            }
            Console.ReadLine();
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: migration [OPTIONS]+ ");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        } // Show Help

        static void Debug(string format, params object[] args)
        {
            if (verbosity > 0)
            {
                Console.Write("# ");
                Console.WriteLine(format, args);
            }
        }
    }// End Program
} // End NameSpace greet
