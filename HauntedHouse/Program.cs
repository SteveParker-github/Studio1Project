using System;
using System.Collections.Generic;

namespace HauntedHouse
{
    class Program
    {
        private static List<bool> conditions;



        //Main Menu
        static void Main(string[] args)
        {
            string selection;
            string[] title = { "New Game", "Save Game", "Load Game", "Exit" };

            do
            {
                //Main menu text
                Console.Clear();
                Console.SetCursorPosition(0, 8);
                Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + ("Welcome to Haunted House".Length / 2)) + "}", "Welcome to Haunted House"));
                Console.SetCursorPosition(0, 10);
                for (int i = 0; i < title.Length; i++)
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
                        NewGame();
                        break;

                    case "save game":
                        SaveGame();
                        break;

                    case "load game":
                        LoadGame();
                        break;

                    case "exit":

                    default:
                        break;
                }
            } while (selection != "exit");
        }

        //Save game method
        static public void SaveGame()
        {

        }

        //Load Game method
        static public void LoadGame()
        {

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
                }

            } while (!leave);

            if (left)
            {
                Console.Clear();
                Console.WriteLine("That is the end for now.");
                Console.WriteLine("Press any key to go back to main menu...");
                Console.ReadLine();
            }
            else
            {
                Room1();
            }
        }

        //Properties
        public static List<bool> Conditions { get => conditions; set => conditions = value; }
    }
}
