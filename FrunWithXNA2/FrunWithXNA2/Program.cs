using System;
using System.IO;

namespace FrunWithXNA2
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (StickPlatformer game = new StickPlatformer())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(@"c:\temp\frunlog.txt", true))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " :: EXCEPTION :: " + e.ToString());
                }
            }

        }
    }
#endif
}

