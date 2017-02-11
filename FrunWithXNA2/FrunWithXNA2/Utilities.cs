using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Net;
using System.IO;

namespace FrunWithXNA2
{
    public static class Utilities
    {
        private static ContentManager content;
        public static ContentManager Content { get { return content; } set { content = value; } }

        public static class HighScoreUtilities
        {
            private static WebRequest webRequest;
            private static bool submissionComplete = false;
            private static bool quit = false;

            public static bool SubmitHighScore(string user, double score)
            {
                submissionComplete = false;
                quit = false;

                // Asynchronously submit score
                DoSubmitHighScore(user, score);

                // Wait for a response
                while (!submissionComplete && !quit)
                {
                    System.Threading.Thread.Sleep(100);
                }

                if (quit)
                    return false;

                return true;
            }

            private static void DoSubmitHighScore(string user, double score)
            {
                string URL = "http://rehlander.com/fRun/highscore.php?action=submit&name=" + user + "&score=" + score.ToString() +
                    "&access_code=1234";

                webRequest = HttpWebRequest.Create(URL);
                webRequest.Method = "GET";

                webRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            }

            private static void FinishWebRequest(IAsyncResult result)
            {
                HttpWebResponse objResponse = (HttpWebResponse)webRequest.EndGetResponse(result);
                StreamReader sr = new StreamReader(objResponse.GetResponseStream());
                string strReturn = sr.ReadToEnd();

                if (strReturn == "1")
                    submissionComplete = true;
                else
                    quit = true;
            }
        }
    }
}
