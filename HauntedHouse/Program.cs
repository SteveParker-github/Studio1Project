/* Program name:            Haunted House (Work in title)
 * Project file name:       HauntedHouse
 * Author:                  Steve Parker, 
 * Date:                    17/10/2020
 * Language:                C#
 * Platform:                Microsoft Visual Studio 2019
 * Purpose:                 To work in a team environment by making a text based adventure game.
 * Description:             Explore a haunted house using text commands ...
 *
 * known bugs:              
 * Additional features:     
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HauntedHouse
{
    class Program
    {
        //devMode constant
        private const bool DEVMODE = false;

        //Constants
        private const string FILELOCATION = @"save.txt";
        private const int SCREENSAVECOUNT = 20;

        //Fields
        private static List<List<bool>> roomConditions; //keeps a track of all the work the player has done in a certain room. 
        private static List<int> inventory; //Player's inventory
        private static List<string> inventoryName; //The names of each item
        private static List<string> screenSave;  //saves what is currently on the screen
        private static string text;              //use this to show a message for the player

        //Main method
        static void Main(string[] args)
        {
            roomConditions = new List<List<bool>>();
            inventoryName = new List<string>();
            //Room1
            roomConditions.Add(new List<bool>());
            //Room2
            roomConditions.Add(new List<bool>());
            inventoryName.Add("Rusty door key");
            //declare all the inventory here.
            inventory = new List<int>(); //would need to create a list of all the items we will have in the game.
            inventory.Add(0); //Room1 key to go to Room2.

            screenSave = new List<string>();
            MainMenu(MethodBase.GetCurrentMethod());
        }

        //Main menu
        static public void MainMenu(MethodBase method)
        {
            string selection;
            bool exitLoop = false;
            List<string> title = new List<string>();

            do
            {
                //Main menu text
                Console.Clear();

                //main menu reload
                title.Clear();
                if ((method.ToString() == "Void Main(System.String[])") || (method.ToString() == "Void End()"))
                {
                    title.Add("New Game");
                }
                else
                {
                    title.Add("Resume");
                    title.Add("Save Game");
                }
                if (File.Exists(FILELOCATION))
                {
                    title.Add("Load Game");
                }
                title.Add("Exit");

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

                    case "resume":
                        {
                            if (title.Contains("Resume"))
                            {
                                exitLoop = true;
                            }
                        }
                        break;

                    default:
                        break;
                }
            } while (!exitLoop);
            if (selection == "resume")
            {
                Console.Clear();
                foreach (string line in screenSave)
                {
                    Console.WriteLine(line);
                }
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

            sw.WriteLine(roomConditions.Count);
            foreach (List<bool> room in roomConditions)
            {
                sw.WriteLine(room.Count);
                foreach (bool item in room)
                {
                sw.WriteLine(item);
                }
            }

            sw.WriteLine(inventory.Count);
            foreach (int item in inventory)
            {
                sw.WriteLine(item);
            }

            //removes strings if above certain count and saves each line.
            if (screenSave.Count > SCREENSAVECOUNT)
            {
                screenSave.RemoveRange(0, screenSave.Count - SCREENSAVECOUNT);
            }
            sw.WriteLine(screenSave.Count);
            foreach (string item in screenSave)
            {
                sw.WriteLine(item);
            }
            sw.Close();
            Console.WriteLine("Game is saved!");
        }

        //Load Game method
        static public void LoadGame()
        {
            string methodName = "";
            screenSave.Clear();

            StreamReader sr = new StreamReader(FILELOCATION);

            // clears what ever is in the conditions list or creates it 
                roomConditions.Clear();

            while (!sr.EndOfStream)
            {
                methodName = sr.ReadLine();

                int count = Convert.ToInt16(sr.ReadLine()); // counts how many rooms are in the game.
                for (int i = 0; i < count; i++)
                {
                    roomConditions.Add(new List<bool>());
                    int room = Convert.ToInt16(sr.ReadLine()); // counts how many conditions are in the room.
                    for (int j = 0; j < room; j++)
                    {
                        if (room > 0)
                        {
                            roomConditions[i].Add(Convert.ToBoolean(sr.ReadLine()));
                        }
                    }
                }

                inventory.Clear();

                int items = Convert.ToInt16(sr.ReadLine()); // count how many items are in the game.
                for (int i = 0; i < items; i++)
                {
                    inventory.Add(Convert.ToInt16(sr.ReadLine()));
                }

                count = Convert.ToInt16(sr.ReadLine()); // count how many lines to load
                for (int i = 0; i < count; i++)
                {
                    screenSave.Add(sr.ReadLine());
                }
            }
            sr.Close();
            Console.Clear();
            Type type = typeof(Program);
            MethodBase method = type.GetMethod(methodName);
            foreach (string line in screenSave)
            {
                Console.WriteLine(line);
            }
            method.Invoke(method, null);
        }

        //New game method
        static public void NewGame()
        {
            Console.Clear();
            LoadRooms();
            Room1();
        }

        //Load all the conditions for the game
        public static void LoadRooms()
        {
            //roomConditions = new List<List>();
            //roomConditions.Add(conditions = new List<bool>());
        }

        //shows the player the message and saves it
        public static void ShowMessage()
        {
            Console.WriteLine(text);
            screenSave.Add(text);
        }

        //tells the player what they have on them
        public static void CheckInventory()
        {
            int count = 0;
            text = "You are carrying:";
            ShowMessage();
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] > 0)
                {
                    text = inventoryName[i];
                    ShowMessage();
                    count++;
                }
            }
            if (count == 0)
            {
                text = "Nothing";
                ShowMessage();
            }
        }
        //First room of the game
        static public void Room1()
        {
            var room = roomConditions[0]; //makes it easier to write than roomConditions[0]...
            if (room.Count == 0)
            {
                room.Add(false);
                room.Add(false);
                room.Add(true);
            }
            //Thought it was nicer to show what it does rather than numbers
            var chestOpen = room[0];
            var pickedKey = room[1];
            var doorLocked = room[2];

            bool leave = false;
           
            text = "It's a room with a chest. There's a door to the left";
            if (screenSave.Count > 0)
            {
                if (text != screenSave[screenSave.Count - 1])
                {
                    ShowMessage();
                }
            }
            else
            {
                ShowMessage();
            }

            do
            {
                text = Console.ReadLine().ToLower();
                screenSave.Add(text);
                switch (text)
                {
                    case "open chest":
                        {
                            if (chestOpen)
                            {
                                text = "The chest is already open.";
                                ShowMessage();
                            }
                            else
                            {
                                text = "You open the chest. There is a key inside";
                                ShowMessage();
                                chestOpen = true;
                            }
                        }
                        break;

                    case "take key":
                        {
                            if ((!pickedKey)&&(chestOpen))
                            {
                                text = "you take the key";
                                ShowMessage();
                                pickedKey = true;
                                inventory[0] = 1;
                            }
                            else
                            {
                                if (pickedKey)
                                {
                                    text = "You have already picked up the key.";
                                    ShowMessage();
                                }
                            }
                        }
                        break;

                    case "use key on door":
                        {
                            if ((inventory[0] == 1) && (doorLocked)) //changed pickedkey to key in inventory
                            {
                                text = "You unlock the door";
                                ShowMessage();
                                doorLocked = false;
                            }
                            else
                            {
                                if (inventory[0] == 0)
                                {
                                    text = "what key? you don't have a key";
                                    ShowMessage();
                                    Console.WriteLine();
                                }
                                else
                                {
                                    text = "The door is already unlocked. did you want to lock it?";
                                    ShowMessage();
                                    text = Console.ReadLine().ToLower();
                                    screenSave.Add(text);
                                    if (text == "yes")
                                    {
                                        text = "You locked the door... for some reason.";
                                        ShowMessage();
                                        doorLocked = true;
                                    }
                                }
                            }
                        }
                        break;

                    case "go left":
                        {
                            if (!doorLocked)
                            {
                                leave = true;
                            }
                            else
                            {
                                text = "The door is locked. Maybe there is a key somewhere.";
                                ShowMessage();
                            }
                        }
                        break;

                    case "main menu":
                        {
                            screenSave.Remove(text);
                            MainMenu(MethodBase.GetCurrentMethod());
                        }
                        break;

                    case "inventory":
                        {
                            CheckInventory();
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

            text = "You are in another room. Another door is to the left, the door you came through is to the right";
            if (screenSave.Count > 0)
            {
                if (text != screenSave[screenSave.Count - 1])
                {
                    ShowMessage();
                }
            }
            else
            {
                ShowMessage();
            }

            do
            {
                text = Console.ReadLine().ToLower();
                screenSave.Add(text);
                switch (text)
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
                            screenSave.Remove(text);
                            MainMenu(MethodBase.GetCurrentMethod());
                        }
                        break;

                    case "inventory":
                        {
                            CheckInventory();
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
    }
}
