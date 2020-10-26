using System;
using System.IO;
using System.Threading;

namespace _
{
    class Program
    {
        //This is the the method that prints the animation out to the screen. The first argument specifies the time between frames. The second specifies the number of frames used, and the rest point to the relevant .txt files to be used
        public static void Animation(int delay, int framenumber, string file1, string file2, string file3, string file4, string file5)
        {
            string aline, file;

            for (int i = 0; i < framenumber; i++)
            {
                //assigns the file to be read based on the current frame
                switch (i)
                {
                    default:
                        file = file1;
                        break;
                    case 1:
                        file = file2;
                        break;
                    case 2:
                        file = file3;
                        break;
                    case 3:
                        file = file4;
                        break;
                    case 4:
                        file = file5;
                        break;
                }
                Console.Clear();
                StreamReader frame = new StreamReader(@file);
                string everyline = frame.ReadToEnd();
                Console.WriteLine(everyline);
                
                frame.Close();
                Thread.Sleep(delay);
            }
        }
        static void Main(string[] args)
        {
            Animation(1500, 4, "test1.txt", "test2.txt", "test3.txt", "test4.txt", "x");
            Console.Read();
        }
    }
}
