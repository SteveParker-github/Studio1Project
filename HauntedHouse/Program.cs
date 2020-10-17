using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HauntedHouse
{
    class Program
    {
        //Constants
        private const string FILELOCATION = @"save.txt";
        //Fields
        private static List<bool> conditions;    //keeps a track of all the work the player has done. //maybe have a list of list of bool       
        private static List<string> screenSave;  //saves what is currently on the screen

        //devMode constant
        private const bool DEVMODE = false;

        //Main method
        static void Main(string[] args)
        {
            MainMenu(MethodBase.GetCurrentMethod());
        }

        //Main menu
        static public void MainMenu(MethodBase method)
        {
            string selection;
            bool exitLoop = false;
            List<string> title = new List<string>();
            string fileLocation = @"save.txt";
            if ((method.ToString() == "Void Main(System.String[])")||(method.ToString() == "Void End()"))
            {
                title.Add("New Game");
            }
            else
            {
                title.Add("Continue");
                title.Add("Save Game");
            }
            if (File.Exists(fileLocation))
            {
                title.Add("Load Game");
            }
            title.Add("Exit");

            do
            {
                //Main menu text
                Console.Clear();
           
                if (DEVMODE) //tells the location of the previous method
                {
                    Console.WriteLine(method.ToString());
                }

                Console.SetCursorPosition(0, 8);
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + ("Welcome to Haunted House".Length / 2)) + "}", "Welcome to Haunted House"));
                Console.SetCursorPosition(0, 10);
                for (int i = 0; i < title.Count; i++)
                {
                    Console.WriteLine("{0," + ((Console.WindowWidth / 2) + (title[i].Length / 2)) + "}", title[i]);
                }
                // selection to call out task or exit the program
                Console.SetCursorPosition(40, 20);
                Console.WriteLine("#");
                Console.SetCursorPosition(41, 20);
                selection = Console.ReadLine().ToLower();
                switch (selection)
                {
                    case "new game":
                        {
                            if (title.Contains("New Game"))
                            {
                                NewGame();
                            }
                        }
                        break;

                    case "save game":
                        {
                            if (title.Contains("Save Game"))
                            {
                                SaveGame(method.ToString());
                                Console.WriteLine("Press any key to continue");
                                Console.ReadLine();
                            }
                        }
                        break;

                    case "load game":
                        {
                            if (title.Contains("Load Game"))
                            {
                                LoadGame();
                            }
                        }
                        break;

                    case "exit":
                        {
                            exitLoop = true;
                        }
                        break;

                    case "continue":
                        {
                            if (title.Contains("Continue"))
                            {
                                exitLoop = true;
                            }
                        }
                        break;

                    default:
                        break;
                }
            } while (!exitLoop);
            if (selection == "continue")
            { 
            Console.Clear();
            method.Invoke(method, null);
            }
            else
            {
                Environment.Exit(0);
            }
        }

        //Save game method
        static public void SaveGame(string method)
        {
            //What needs to be saved?
            //method location, list of conditions, items, screensave text.
            StreamWriter sw = new StreamWriter(FILELOCATION);
            string methodName = method.Replace("Void", "");
            methodName = methodName.Replace("()", "");
            methodName = methodName.Replace(" ", "");
            sw.WriteLine(methodName);

            sw.WriteLine(conditions.Count);
            foreach (bool item in conditions)
            {
                sw.WriteLine(item);
            }
            //until screensave is working, i've disabled this for now
            /*
            sw.WriteLine(screenSave.Count);
            foreach (string item in screenSave)
            {
                sw.WriteLine(item);
            }*/
            sw.Close();
            Console.WriteLine("Game is saved!");
        }

        //Load Game method
        static public void LoadGame()
        {
            string methodName = "";
            StreamReader sr = new StreamReader(FILELOCATION);

            if (conditions != null)
            {
                conditions.Clear();
            }
            else
            {
                conditions = new List<bool>();
            }
            if (screenSave != null)
            {
                screenSave.Clear();
            }
            else
            {
                screenSave = new List<string>();
            }
            while (!sr.EndOfStream)
            {
                methodName = sr.ReadLine();
                int count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    conditions.Add(Convert.ToBoolean(sr.ReadLine()));
                }
                //Again until screen save is working
                /*count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    screenSave.Add(sr.ReadLine());
                }*/
            }
            sr.Close();
            Console.Clear();
            Type type = typeof(Program);
            MethodBase method = type.GetMethod(methodName);
            method.Invoke(method, null);
        }

        //New game method
        static public void NewGame()
        {
            Console.Clear();
            LoadConditions();
            Room1();
        }

        //Load all the conditions for the game
        public static void LoadConditions()
        {
            conditions = new List<bool>();
            conditions.Add(false); //chestopen
            conditions.Add(false); //picked up key
            conditions.Add(true); //door is locked
        }

        //First room of the game
        static public void Room1()
        {
            bool leave = false;

            Console.WriteLine("It's a room with a chest. theres a door to the left");

            do
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "open chest":
                        {
                            if (conditions[0])
                            {
                                Console.WriteLine("The chest is already open.");
                            }
                            else
                            {
                                Console.WriteLine("You open the chest. There is a key inside");
                                Conditions[0] = true;
                            }
                        }
                        break;

                    case "take key":
                        {
                            if ((!conditions[1])&&(conditions[0]))
                            {
                                Console.WriteLine("you take the key");
                                conditions[1] = true;
                            }
                            else
                            {
                                if (conditions[1])
                                {
                                    Console.WriteLine("You have already picked up the key.");
                                }
                            }
                        }
                        break;

                    case "use key on door":
                        {
                            if ((conditions[1]) && (conditions[2]))
                            {
                                Console.WriteLine("You unlock the door");
                                conditions[2] = false;
                            }
                            else
                            {
                                if (conditions[2])
                                {
                                    Console.WriteLine("what key? you don't have a key");
                                }
                                else
                                {
                                    Console.WriteLine("The door is already unlocked. did you want to lock it?");
                                    if (Console.ReadLine().ToLower() == "yes")
                                    {
                                        Console.WriteLine("You locked the door... for some reason.");
                                        conditions[2] = true;
                                    }
                                }
                            }
                        }
                        break;

                    case "go left":
                        {
                            if (!conditions[2])
                            {
                                leave = true;
                            }
                            else
                            {
                                Console.WriteLine("The door is locked. Maybe there is a key somewhere.");
                            }
                        }
                        break;

                    case "main menu":
                        {
                            MainMenu(MethodBase.GetCurrentMethod());
                        }
                        break;
                }

            } while (!leave);
            Room2();
        }


        //Second room of the game
        static public void Room2()
        {
            bool leave = false;
            bool left = false;

            Console.WriteLine("You are in another room. Another door is to the left, the door you came through was to the right");
            do
            {
                switch (Console.ReadLine().ToLower())
                {
                    case "go left":
                        {
                            leave = true;
                            left = true;
                        }
                        break;

                    case "go right":
                        {
                            leave = true;
                        }
                        break;

                    case "main menu":
                        {
                            MainMenu(MethodBase.GetCurrentMethod());
                        }
                        break;
                }

            } while (!leave);

            if (left)
            {
                End();
            }
            else
            {
                Room1();
            }
        }

        //the end sequence
        public static void End()
        {
            Console.Clear();
            Console.WriteLine("That is the end for now.");
            Console.WriteLine("Press any key to go back to main menu...");
            Console.ReadLine();
            MainMenu(MethodBase.GetCurrentMethod());
        }
        //Properties
        public static List<bool> Conditions { get => conditions; set => conditions = value; }
        public static List<string> ScreenSave { get => screenSave; set => screenSave = value; }
    }
}
