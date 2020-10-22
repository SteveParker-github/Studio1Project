/* Program name:            Haunted House (Work in title)
 * Project file name:       HauntedHouse
 * Author:                  Steve Parker, 
 * Date:                    20/10/2020
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
using System.Linq;
using System.Reflection;

namespace HauntedHouse
{
    class Program
    {
        //Constants
        private const string FILELOCATION = @"save.txt";
        private const int SCREENSAVECOUNT = 20;

        //Fields
        private static List<Tuple<string, bool, string, string, string, string>> objects; //keeps a track of all the work the player has done in a certain room. 
        private static List<Tuple<string, int>> inventory; //Player's inventory
        private static List<Tuple<string, bool, string, string>> roomDirection; //direction which is allowed in each room
        private static List<string> screenSave;  //saves what is currently on the screen
        private static string text;              //use this to show a message for the player
        private static bool gameStart;           //checks to see if the game has started or not
        private static string playerLocation;            //the location the of the player. 

        //Main method
        static void Main(string[] args)
        {
            screenSave = new List<string>();
            inventory = new List<Tuple<string, int>>(); //name of item, how many they have on them.
            objects = new List<Tuple<string, bool, string, string, string, string>>(); //name of the location and object, have they used the object, if they activate, if they try to activate again.
            roomDirection = new List<Tuple<string, bool, string, string>>(); //name of direction, can the player go that way, the name of the location, reason why they can't go.
            gameStart = true;
            playerLocation = "MainMenu";

            bool exit = false;
            do
            {
                Type type = typeof(Program);
                MethodBase method = type.GetMethod(playerLocation);
                method.Invoke(method, null);
                string selection = Console.ReadLine().ToLower();
                string[] playertexts = selection.Split(" ");
                switch (playertexts[0])
                {
                    case "new":
                        {
                            Console.Clear();
                            if (gameStart)
                            {
                                NewGame();
                            }
                        }
                        break;

                    case "save":
                        {
                            Console.Clear();
                            if (!gameStart)
                            {
                                //SaveGame();
                            }
                        }
                        break;

                    case "load":
                        {
                            Console.Clear();
                            if (File.Exists(FILELOCATION))
                            {
                                //LoadGame();
                            }
                        }
                        break;

                    case "exit":
                        {
                            exit = true;
                        }
                        break;

                    case "main":
                        {
                            Console.Clear();
                            MainMenu();
                        }
                        break;

                    case "resume":
                        {
                            Console.Clear();
                            if (!gameStart)
                            {
                                foreach (var item in screenSave)
                                {
                                    Console.WriteLine(item);
                                }
                            }
                        }
                        break;

                    case "go":
                        {
                            if ((playertexts.Length > 1) && (playertexts[1] != ""))
                            {
                                if (roomDirection.Any(c => c.Item1.Contains(playerLocation + playertexts[1])))
                                {
                                    var direction = roomDirection.Find(x => x.Item1 == playerLocation + playertexts[1]);
                                    if (direction.Item3 != "")
                                    {
                                        if (direction.Item2)
                                        {
                                            playerLocation = direction.Item3;
                                        }
                                        else
                                        {
                                            Console.WriteLine(direction.Item4);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You can not go that direction.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Go where?");
                            }
                        }
                        break;

                    case "open":
                        {
                            if ((playertexts.Length > 1) && (playertexts[1] != ""))
                            {
                                if (objects.Any(c => c.Item1.Contains(playerLocation + playertexts[1])))
                                {
                                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playertexts[1]));
                                    if (objectResult.Item5 == "open")
                                    {
                                        if (!objectResult.Item2)
                                        {
                                            Console.WriteLine(objectResult.Item3);
                                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                                            objects.Remove(objectResult);
                                        }
                                        else
                                        {
                                            Console.WriteLine(objectResult.Item4);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You can't open that");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("I didn't understand after " + playertexts[0]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Open what?");
                            }
                        }
                        break;

                    case "use":
                        {
                            if ((playertexts.Length > 1) && (playertexts[1] != ""))
                            {
                                if (inventory.Any(c => c.Item1.Contains(playertexts[1])))
                                {
                                    var itemResult = inventory.Find(x => x.Item1 == (playertexts[1]));
                                    if (itemResult.Item2 > 0)
                                    {
                                        if (playertexts.Length > 2)
                                        {
                                            if (objects.Any(c => c.Item1.Contains(playerLocation + playertexts[playertexts.Length - 1])))
                                            {
                                                var objectResult = objects.Find(x => x.Item1 == (playerLocation + playertexts[playertexts.Length - 1]));
                                                if (!objectResult.Item2)
                                                {
                                                    Console.WriteLine(objectResult.Item3);
                                                    objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                                                    objects.Remove(objectResult);
                                                }
                                                else
                                                {
                                                    Console.WriteLine(objectResult.Item4);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("I didn't understand after " + playertexts[0] + " " + playertexts[1]);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("use " + itemResult.Item1 + " on what?");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You do not have that item");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Use what?");
                            }
                        }
                        break;

                    case "look":
                        {

                        }
                        break;

                    case "take":
                        {
                            if ((playertexts.Length > 1) && (playertexts[1] != ""))
                            {
                                if (objects.Any(c => c.Item1.Contains(playerLocation + playertexts[1])))
                                {
                                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playertexts[1]));
                                    if (objectResult.Item5 == "take")
                                    {
                                        if (!objectResult.Item2)
                                        {
                                            Console.WriteLine(objectResult.Item3);
                                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                                            objects.Remove(objectResult);
                                            var itemResult = inventory.Find(x => x.Item1 == (playertexts[1]));
                                            inventory.Add(Tuple.Create(itemResult.Item1, itemResult.Item2 + 1));
                                            inventory.Remove(itemResult);
                                        }
                                        else
                                        {
                                            Console.WriteLine(objectResult.Item4);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You can't take that");
                                    }                                      
                                }
                                else
                                {
                                    Console.WriteLine("I didn't understand after " + playertexts[0]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Take what?");
                            }
                        }
                        break;

                    case "help":
                        {
                            Console.WriteLine("Instructions to play the game");
                        }
                        break;

                    default:
                        {
                            Console.WriteLine("I didn't understand that");
                        }
                        break;
                }
            }
            while (exit == false);
        }

        static public void MainMenu()
        {
            List<string> title = new List<string>();

            //Main menu text
            Console.Clear();

            //main menu reload
            title.Clear();

            if (gameStart)
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
        }

        //load all the items and conditions to the list
        static public void NewGame()
        {

            //declare all the inventory here.
            inventory.Add(new Tuple<string, int>("key", 0)); //Room1 key to go to Room2.
            playerLocation = "Room1";
            gameStart = false;
            Console.Clear();
        }

        //shows the player the message and saves it
        public static void ShowMessage()
        {
            Console.WriteLine(text);
            screenSave.Add(text);
        }

        //First room of the game, Use this as the template
        static public void Room1()
        {
            //if chest doesn't exist, create it
            if (!objects.Any(c => c.Item1.Contains("Room1chest")))
            {
                objects.Add(Tuple.Create("Room1chest", false, "You opened the chest. There is a key inside", "the chest is already open", "open", "describe the chest"));
            }
            //if door doesn't exist, create it
            if (!objects.Any(c => c.Item1.Contains("Room1door")))
            {
                objects.Add(Tuple.Create("Room1door", false, "you open the door", "You locked the door again", "", "describe the door"));
            }
            //if chest is opened and key doesn't exist, create it
            var objectResult = objects.Find(x => x.Item1 == "Room1chest");
            if (objectResult.Item2 && (!objects.Any(c => c.Item1.Contains("Room1key"))))
            {
                objects.Add(Tuple.Create("Room1key", false, "You take the key", "You already have the key", "take", "describe the key"));
            }
            //if direction in room1 equals 0, create all the directions
            if (roomDirection.Count(c => c.Item1.Contains("Room1")) == 0)
            {
                roomDirection.Add(Tuple.Create("Room1north", false, "", ""));
                roomDirection.Add(Tuple.Create("Room1south", false, "", ""));
                roomDirection.Add(Tuple.Create("Room1east", false, "", ""));
                roomDirection.Add(Tuple.Create("Room1west", false, "Room2", "The door is locked, maybe there's a key somewhere in this room *shrugs*"));
            }
            //if door is open create direction west with true, and delete original
            objectResult = objects.Find(x => x.Item1 == "Room1door");
            if (objectResult.Item2)
            {
                var direction = roomDirection.Find(x => x.Item1 == "Room1west");
                roomDirection.Add(Tuple.Create(direction.Item1, true, direction.Item3, direction.Item4));
                roomDirection.Remove(direction);
            }

            text = "It's a room with a chest. West of you is a door";
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
        }

        //First room of the game
        static public void Room2()
        {
            if (!roomDirection.Any(c => c.Item1.Contains("Room2west")))
            {
                roomDirection.Add(Tuple.Create("Room2north", false, "", ""));
                roomDirection.Add(Tuple.Create("Room2south", false, "", ""));
                roomDirection.Add(Tuple.Create("Room2east", true, "Room1", ""));
                roomDirection.Add(Tuple.Create("Room2west", true, "End", ""));
            }

            text = "You walk into a new room. West from you is a new door. East is where you came from.";
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
        }

        //the end sequence
        public static void End()
        {
            Console.Clear();
            Console.WriteLine("That is the end for now.");
            Console.WriteLine("Press any key to go back to main menu...");
            playerLocation = "MainMenu";
        }
    }
}
