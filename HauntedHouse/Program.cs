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
        private const int SCREENSAVECOUNT = 40;

        //Fields
        private static List<Tuple<string, bool, string, string, string, string>> objects; //keeps a track of all the work the player has done in a certain room. 
        private static List<Tuple<string, int, string>> inventory; //Player's inventory
        private static List<Tuple<string, bool, string, string>> roomDirection; //direction which is allowed in each room
        private static List<string> screenSave;  //saves what is currently on the screen
        private static List<bool> roomDescription; //checks whether to describe the room or not
        private static string text;              //use this to show a message for the player
        private static bool gameStart;           //checks to see if the game has started or not
        private static bool menu;                //checks to see if the game is in the menu
        private static string playerLocation;            //the location the of the player. 

        //Main method
        static void Main(string[] args)
        {
            screenSave = new List<string>();
            inventory = new List<Tuple<string, int, string>>(); //name of item, how many they have on them.
            objects = new List<Tuple<string, bool, string, string, string, string>>(); //name of the location and object, have they used the object, if they activate, if they try to activate again.
            roomDirection = new List<Tuple<string, bool, string, string>>(); //name of direction, can the player go that way, the name of the location, reason why they can't go.
            roomDescription = new List<bool>();
            gameStart = true;
            playerLocation = "MainMenu";
            menu = true;
            bool exit = false;
            do
            {
                //these 3 lines to call the method of the location of the player
                Type type = typeof(Program);
                MethodBase method = type.GetMethod(playerLocation);
                method.Invoke(method, null);

                //the player enters their commands here
                string selection = Console.ReadLine().ToLower();
                screenSave.Add(selection);
                //splits the string into each word
                string[] playerTexts = selection.Split(" ");
                //finds what the first word does
                switch (playerTexts[0])
                {
                    case "new": //to start a new game
                        {
                            if (gameStart)
                            {
                                NewGame();
                            }
                        }
                        break;

                    case "save": //to save game
                        {
                                SaveGame();
                        }
                        break;

                    case "load": //to load game
                        {
                            if (File.Exists(FILELOCATION))
                            {
                                LoadGame();
                            }
                        }
                        break;

                    case "exit": //to quit the game
                        {
                            exit = true;
                        }
                        break;

                    case "main": //to return to the main menu
                        {
                            menu = true;
                            MainMenu();
                        }
                        break;

                    case "resume": //to resume game (bug test: if the player des this in the game, what happens?)
                        {
                            if ((!gameStart)&& menu)
                            {
                                Console.Clear();
                                foreach (var item in screenSave)
                                {
                                    Console.WriteLine(item);
                                }
                            }
                            else if (!menu)
                            {
                                text = "Can't use that command here";
                                ShowMessage();
                            }
                        }
                        break;

                    case "go": //go to a direction in the game
                        {
                            Go(playerTexts);
                        }
                        break;

                    case "open": //open an object
                        {
                            Open(playerTexts);
                        }
                        break;

                    case "use": //use an item on an object
                        {
                            Use(playerTexts);
                        }
                        break;

                    case "look": //look at something, either room, object or items
                        {
                            Look(playerTexts);
                        }
                        break;

                    case "take": //take object
                        {
                            Take(playerTexts);
                        }
                        break;
                    case "inventory":
                    case "i":
                        {
                            foreach (var item in inventory)
                            {
                                int counter = 0;
                                if (item.Item2 > 0)
                                {
                                    text = item.Item1 + " " + item.Item3;
                                    ShowMessage();
                                    counter++;
                                }
                                if (counter == 0)
                                {
                                    text = "You are carrying nothing... i mean nothing at all, not even lint or a loose string.";
                                    ShowMessage();
                                }

                            }
                        }
                        break;

                    case "help": //help command
                        {
                            Help(playerTexts);
                        }
                        break;

                    default: //if nothing else
                        {
                            text = "I didn't understand that";
                            ShowMessage();
                        }
                        break;
                }
            }
            while (exit == false); //only quit the loop if they quit
        }
        //Saves the game
        static public void SaveGame()
        {
            StreamWriter sw = new StreamWriter(FILELOCATION);

            sw.WriteLine(playerLocation); //saves player's location

            sw.WriteLine(inventory.Count);
            foreach (var item in inventory)
            {
                sw.WriteLine(item.Item1);
                sw.WriteLine(item.Item2.ToString());
                sw.WriteLine(item.Item3);
            }

            sw.WriteLine(objects.Count);
            foreach (var item in objects)
            {
                sw.WriteLine(item.Item1);
                sw.WriteLine(item.Item2.ToString());
                sw.WriteLine(item.Item3);
                sw.WriteLine(item.Item4);
                sw.WriteLine(item.Item5);
                sw.WriteLine(item.Item6);
            }

            sw.WriteLine(roomDirection.Count);
            foreach (var item in roomDirection)
            {
                sw.WriteLine(item.Item1);
                sw.WriteLine(item.Item2.ToString());
                sw.WriteLine(item.Item3);
                sw.WriteLine(item.Item4);
            }

            sw.WriteLine(roomDescription.Count);
            foreach (var item in roomDescription)
            {
                sw.WriteLine(item.ToString());
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

        //Load the game 
        static public void LoadGame()
        {
            //clear the list before loading
            inventory.Clear();
            objects.Clear();
            roomDirection.Clear();
            roomDescription.Clear();
            screenSave.Clear();

            StreamReader sr = new StreamReader(FILELOCATION);

            while (!sr.EndOfStream)
            {
                playerLocation = sr.ReadLine(); //load players location

                int count = Convert.ToInt16(sr.ReadLine()); //load how many lines for the list
                for (int i = 0; i < count; i++)
                {
                    inventory.Add(Tuple.Create(sr.ReadLine(), 
                                               Convert.ToInt32(sr.ReadLine()),
                                               sr.ReadLine()));
                }

                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    objects.Add(Tuple.Create(sr.ReadLine(),
                                             Convert.ToBoolean(sr.ReadLine()),
                                             sr.ReadLine(),
                                             sr.ReadLine(),
                                             sr.ReadLine(),
                                             sr.ReadLine()));
                }

                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    roomDirection.Add(Tuple.Create(sr.ReadLine(),
                                                   Convert.ToBoolean(sr.ReadLine()),
                                                   sr.ReadLine(),
                                                   sr.ReadLine()));
                }

                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    roomDescription.Add(Convert.ToBoolean(sr.ReadLine()));
                }

                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    screenSave.Add(sr.ReadLine());
                }
            }
            sr.Close();
            Console.Clear();
            foreach (string line in screenSave)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("Game Loaded");
        }
        //To give the player any help with commands
        static public void Help(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                switch (playerTexts[1])
                {
                    case "look":
                        {
                            Console.WriteLine("Use to look at certain objects. For exmaple 'look chest'.");
                        }
                        break;

                    case "use":
                        {
                            Console.WriteLine("Use an item in your inventory with an object around the room. For example 'use key on door'.");
                        }
                        break;

                    case "go":
                        {
                            Console.WriteLine("Use to go in a direction. For example 'go west'");
                        }
                        break;

                    case "open":
                        {
                            Console.WriteLine("Use to open something. For example 'open chest'");
                        }
                        break;

                    case "take":
                        {
                            Console.WriteLine("Use to take an item. for exmaple 'take key'");
                        }
                        break;

                    default:
                        {
                            Console.WriteLine("No information about '" + playerTexts[1] + "'.");
                        }
                        break;

                }
            }
            else
            {
                Console.WriteLine("Current commands you can use: look, go, use, take, open, help, main, save, load, exit.");
                Console.WriteLine("Type 'help [command]' for more information");
            }
        }

        //Let the player look at something
        static public void Look(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    var objectResult = objects.Find(x => x.Item1 == playerLocation + playerTexts[1]);
                    text = objectResult.Item6;
                    ShowMessage();
                }
                else
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else
            {
                roomDescription[Convert.ToInt16(playerLocation.Replace("Room", "")) - 1] = true;
            }
        }

        //to move the player to where they want to go (only via compass directions, not holiday resorts)
        static public void Go(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                if (roomDirection.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    var direction = roomDirection.Find(x => x.Item1 == playerLocation + playerTexts[1]);
                    if (direction.Item3 != "")
                    {
                        if (direction.Item2)
                        {
                            playerLocation = direction.Item3;
                        }
                        else
                        {
                            text = direction.Item4;
                            ShowMessage();
                        }
                    }
                    else
                    {
                        text = "You can not go that direction.";
                        ShowMessage();
                    }
                }
            }
            else
            {
                text = "Go where?";
                ShowMessage();
            }
        }

        //let a player use an item onto an object
        static public void Use(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                if (inventory.Any(c => c.Item1.Contains(playerTexts[1])))
                {
                    var itemResult = inventory.Find(x => x.Item1 == (playerTexts[1]));
                    if (itemResult.Item2 > 0)
                    {
                        if (playerTexts.Length > 2)
                        {
                            if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[playerTexts.Length - 1])))
                            {
                                var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[playerTexts.Length - 1]));
                                if (objectResult.Item5 == itemResult.Item1)
                                {
                                    if (!objectResult.Item2)
                                    {
                                        text = objectResult.Item3;
                                        ShowMessage();
                                        objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                                        objects.Remove(objectResult);
                                    }
                                    else
                                    {
                                        text = objectResult.Item4;
                                        ShowMessage();
                                    }
                                }
                                else
                                {
                                    text = "It wasn't intended to be used like that.";
                                    ShowMessage();
                                }
                            }
                            else
                            {
                                text = "I didn't understand after " + playerTexts[0] + " " + playerTexts[1];
                                ShowMessage();
                            }
                        }
                        else
                        {
                            text = "use " + itemResult.Item1 + " on what?";
                            ShowMessage();
                        }
                    }
                    else
                    {
                        text = "You do not have that item";
                        ShowMessage();
                    }
                }
            }
            else
            {
                text = "Use what?";
                ShowMessage();
            }
        }

        //let the player open something
        static public void Open(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[1]));
                    if (objectResult.Item5 == "open")
                    {
                        if (!objectResult.Item2)
                        {
                            text = objectResult.Item3;
                            ShowMessage();
                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                            objects.Remove(objectResult);
                        }
                        else
                        {
                            text = objectResult.Item4;
                            ShowMessage();
                        }
                    }
                    else
                    {
                        text = "You can't open that";
                        ShowMessage();
                    }
                }
                else
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else
            {
                text = "Open what?";
                ShowMessage();
            }
        }

        //let player take an item
        static public void Take(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != ""))
            {
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[1]));
                    if (objectResult.Item5 == "take")
                    {
                        if (!objectResult.Item2)
                        {
                            text = objectResult.Item3;
                            ShowMessage();
                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                            objects.Remove(objectResult);
                            var itemResult = inventory.Find(x => x.Item1 == (playerTexts[1]));
                            inventory.Add(Tuple.Create(itemResult.Item1, itemResult.Item2 + 1, itemResult.Item3));
                            inventory.Remove(itemResult);
                        }
                        else
                        {
                            text = objectResult.Item4;
                            ShowMessage();
                        }
                    }
                    else
                    {
                        text = "You can't take that";
                        ShowMessage();
                    }
                }
                else
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else
            {
                text = "Take what?";
                ShowMessage();
            }
        }
        static public void MainMenu()
        {
            List<string> title = new List<string>();

            //clears the text
            Console.Clear();

            //deletes the title list
            title.Clear();

            //if game has just been booted up...
            if (gameStart)
            {
                title.Add("New Game"); //add the words new game
            }
            else //otherwise...
            {
                title.Add("Resume"); //add resume and save game
                title.Add("Save Game");
            }

            //if a save file is found...
            if (File.Exists(FILELOCATION))
            {
                title.Add("Load Game"); //add load game
            }
            title.Add("Exit"); //add exit always

            Console.SetCursorPosition(0, 8); //set the cursor 8 lines down
            //Write the welcome line in the center of the console
            Console.WriteLine(String.Format("{0," + 
                                           ((Console.WindowWidth / 2) + 
                                           ("Welcome to Haunted House".Length / 2)) + 
                                           "}", "Welcome to Haunted House"));
            Console.SetCursorPosition(0, 10); //move the cursor down 1 line
            //loop the amount of titles on the main page
            for (int i = 0; i < title.Count; i++) 
            {
                Console.WriteLine("{0," + 
                                 ((Console.WindowWidth / 2) + 
                                 (title[i].Length / 2)) + 
                                 "}", title[i]);
            }
            //set the cursor 40 characters in, and 20 lines from the top
            Console.SetCursorPosition(40, 20);
            Console.WriteLine("#"); //something to show where the player will be entering something
            Console.SetCursorPosition(41, 20); //set the cursor 1 character from the right of the above message.
        }

        //load all the items and conditions to the list
        static public void NewGame()
        {

            //declare all the inventory here.
            inventory.Add(Tuple.Create("key", 0, "It's a shiny key.... only joking, its rusted beyond believe but still works.")); //Room1 key to unlock the door in Room1.
            playerLocation = "Room1"; //players starting location
            gameStart = false; //tells the game the player is now playing the game.
            menu = false; //lets the game your not in the menu screen
            roomDescription.Add(true);
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
                objects.Add(Tuple.Create("Room1chest", //Name of object
                                         false,        //State of the object
                                         "You opened the chest. There is a key inside", //text when first activate
                                         "the chest is already open", //text when activating the second time.
                                         "open", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "describe the chest"));      //text describing what it is when the "player" looks at it  
            }
            //if door doesn't exist, create it
            if (!objects.Any(c => c.Item1.Contains("Room1door")))
            {
                objects.Add(Tuple.Create("Room1door", 
                                         false, 
                                         "you open the door", 
                                         "Why would lock yourself in? You only just unlocked it!", 
                                         "key", 
                                         "describe the door"));
            }

            var objectResult = objects.Find(x => x.Item1 == "Room1chest");
            //if chest is opened and key doesn't exist, create it
            if (objectResult.Item2 && (!objects.Any(c => c.Item1.Contains("Room1key"))))
            {
                objects.Add(Tuple.Create("Room1key", 
                                         false, 
                                         "You take the key", 
                                         "You already have the key", 
                                         "take", 
                                         "describe the key"));
            }
            //if direction in room1 equals 0, create all the directions
            if (roomDirection.Count(c => c.Item1.Contains("Room1")) == 0)
            {
                roomDirection.Add(Tuple.Create("Room1north", //what room this is and what direction
                                               false,        //is the player able to go this way   
                                               "",           //the name of the method it will go           
                                               ""));         //The reason they cant go this way, leave as blank if u cant go this way at all
                roomDirection.Add(Tuple.Create("Room1south", 
                                               false, 
                                               "", 
                                               ""));
                roomDirection.Add(Tuple.Create("Room1east", 
                                               false, 
                                               "", 
                                               ""));
                roomDirection.Add(Tuple.Create("Room1west", 
                                               false, 
                                               "Room2", 
                                               "The door is locked, maybe there's a key somewhere in this room *shrugs*"));
            }
            //if door is open create direction west with true, and delete original
            objectResult = objects.Find(x => x.Item1 == "Room1door");
            if (objectResult.Item2)
            {
                var direction = roomDirection.Find(x => x.Item1 == "Room1west"); //finding the tuple
                roomDirection.Add(Tuple.Create(direction.Item1, true, direction.Item3, direction.Item4)); //creating a new tuple that allows to go through the door
                roomDirection.Remove(direction); //removing the old tuple
            }

            //description of the room
            if (roomDescription[0])
            {
                text = "It's a room with a chest. West of you is a door";
                ShowMessage();
                roomDescription[0] = false;
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
