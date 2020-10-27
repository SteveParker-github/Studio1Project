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
using System.Runtime.InteropServices;
using System.Threading;

namespace HauntedHouse
{
    class Program
    {
        //this is to make the program auto full size from:
        //https://www.c-sharpcorner.com/code/448/code-to-auto-maximize-console-application-according-to-screen-width-in-c-sharp.aspx
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        //Constants
        private const string FILELOCATION = @"save.txt";
        private const int SCREENSAVECOUNT = 23;

        //Fields
        private static List<Tuple<string, bool, string, string, string, string>> objects; //keeps a track of all the work the player has done in a certain room. 
        private static List<Tuple<string, int, string>> inventory; //Player's inventory
        private static List<Tuple<string, bool, string, string>> roomDirection; //direction which is allowed in each room
        private static List<string> screenSave;  //saves what is currently on the screen
        private static List<bool> roomDescription; //checks whether to describe the room or not
        private static string text;              //use this to show a message for the player
        private static int score;                //keeps a track of the player's score 
        private static bool gameStart;           //checks to see if the game has started or not
        private static bool menu;                //checks to see if the game is in the menu
        private static string playerLocation;            //the location the of the player. 

        //Main method
        static void Main(string[] args)
        {
            //Set the app to fullScreen
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);

            //Removes the scroll bar
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

            screenSave = new List<string>();
            inventory = new List<Tuple<string, int, string>>(); //name of item, how many they have on them.
            objects = new List<Tuple<string, bool, string, string, string, string>>(); //name of the location and object, have they used the object, if they activate, if they try to activate again.
            roomDirection = new List<Tuple<string, bool, string, string>>(); //name of direction, can the player go that way, the name of the location, reason why they can't go.
            roomDescription = new List<bool>();  //checks whether to show the room description or not
            gameStart = true;   //checks whether the game has just started
            score = 0;      //the score tally, not currently working
            playerLocation = "MainMenu"; //tells the game where the player is
            menu = true;  //checks if the player is in the menu
            bool exit = false; //bool to get out of the loop
            do
            {
                Console.Clear();
                if (!menu)
                {
                    DisplayUI();
                }
                else
                {
                    MainMenu();
                }
                //the player enters their commands here
                string selection = Console.ReadLine().ToLower();

                if (!selection.Contains("new") && !selection.Contains("save") && !selection.Contains("load") && !selection.Contains("main") && !selection.Contains("resume"))
                {
                    screenSave.Add(" > " + selection);
                }
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
                            if ((!gameStart) && menu)
                            {
                                menu = false;
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

                    case "read": //use an item on an object
                        {
                            Read(playerTexts);
                        }
                        break;

                    case "inventory":
                    case "i":
                        {
                            //checks each item if the player has more than 1 of the items. If they do, it will show what the item is.
                            //If they are carrying nothing, it will inform them they have nothing
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

        static public void DisplayUI()
        {
            int quarterWidth = Console.WindowWidth / 4; // 1/4 of the console width
            int width75 = Console.WindowWidth - quarterWidth; // 75% of the console width
            string temp = "Inventory";
            int middleInv = (width75 + ((quarterWidth / 2) - (temp.Length) / 2)); //the middle where to put the word in the center of the area
            string hr = ""; //to make the horizontal ruler across the screen

            //set the cursor at the top left
            Console.SetCursorPosition(1, 0);
            Console.Write("Location: " + playerLocation); //write the location of the player

            //set the cursor to write out the score
            Console.SetCursorPosition(quarterWidth + quarterWidth / 2, 0);
            Console.Write("|| Score: " + score.ToString());

            //set the curosr to write out the inventory title
            Console.SetCursorPosition(width75, 0);
            Console.Write("||");
            Console.SetCursorPosition(middleInv, 0);
            Console.WriteLine(temp);

            //to create the horizontal line
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                hr = hr + "=";
            }
            Console.WriteLine(hr); //output the line across the screen

            //to create a verticle line.
            for (int i = 2; i < Console.WindowHeight - 1; i++)
            {
                Console.SetCursorPosition(width75, i);
                Console.WriteLine("||");
            }

            //reset the horizontal ruler
            hr = "";
            //create a horizontal ruler that fits for the bottom part of the screen
            for (int i = 0; i < width75; i++)
            {
                hr = hr + "=";
            }
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.WriteLine(hr);

            //Write out the items the player has on the right hand side of the screen
            foreach (var item in inventory)
            {
                int count = 0;
                if (item.Item2 > 0)
                {
                    Console.SetCursorPosition(middleInv, 3 + count);
                    Console.Write(item.Item1);
                    count++;
                }
                if (count == 0)
                {
                    Console.SetCursorPosition(middleInv - 2, 3 + count);
                    Console.Write("You have nothing");
                }
            }

            //resets the screen position
            Console.SetCursorPosition(0, 0);
            Console.SetCursorPosition(0, 2);

            //Cull the list to fit onto the screen
            if (screenSave.Count > SCREENSAVECOUNT)
            {
                screenSave.RemoveRange(0, screenSave.Count - SCREENSAVECOUNT);
            }
            foreach (string line in screenSave) //output any saved text to reload onto the screen
            {
                Console.WriteLine(line);
            }
            //these 3 lines to call the method of the location of the player
            Type type = typeof(Program);
            MethodBase method = type.GetMethod(playerLocation);
            method.Invoke(method, null);
            //set the cursor where the player types
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write(" > ");
        }
        //Saves the game
        static public void SaveGame()
        {
            StreamWriter sw = new StreamWriter(FILELOCATION);

            sw.WriteLine(playerLocation); //saves player's location

            sw.WriteLine(inventory.Count); //saves how many items to load back in
            //saves all the items information to be able to load it back
            foreach (var item in inventory)
            {
                sw.WriteLine(item.Item1);
                sw.WriteLine(item.Item2.ToString());
                sw.WriteLine(item.Item3);
            }

            //same as above but for the objects
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

            //same as above but for roomDirections
            sw.WriteLine(roomDirection.Count);
            foreach (var item in roomDirection)
            {
                sw.WriteLine(item.Item1);
                sw.WriteLine(item.Item2.ToString());
                sw.WriteLine(item.Item3);
                sw.WriteLine(item.Item4);
            }

            //same as above but for RoomDescription
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
            //saves all the screen text
            sw.WriteLine(screenSave.Count);
            foreach (string item in screenSave)
            {
                sw.WriteLine(item);
            }

            sw.Close();

            //Need to get this cleaner.
            Console.Clear();
            Console.WriteLine("Game is saved! Press any key to continue...");
            Console.ReadLine();
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

                //loads the items back to the inventory list
                int count = Convert.ToInt16(sr.ReadLine()); //load how many lines for the list
                for (int i = 0; i < count; i++)
                {
                    inventory.Add(Tuple.Create(sr.ReadLine(),
                                               Convert.ToInt32(sr.ReadLine()),
                                               sr.ReadLine()));
                }

                //same as above but for objects
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

                //same as above but for roomDirection
                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    roomDirection.Add(Tuple.Create(sr.ReadLine(),
                                                   Convert.ToBoolean(sr.ReadLine()),
                                                   sr.ReadLine(),
                                                   sr.ReadLine()));
                }

                //Same as above but for RoomDescription
                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    roomDescription.Add(Convert.ToBoolean(sr.ReadLine()));
                }

                //same as above but for screensave
                count = Convert.ToInt16(sr.ReadLine());
                for (int i = 0; i < count; i++)
                {
                    screenSave.Add(sr.ReadLine());
                }
            }
            sr.Close();

            menu = false;
        }
        //To give the player any help with commands
        static public void Help(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after the first one and isnt blank
            {
                switch (playerTexts[1])
                {
                    case "look":
                        {
                            text = "Use to look at certain objects. For exmaple 'look chest'.";
                            ShowMessage();
                        }
                        break;

                    case "use":
                        {
                            text = "Use an item in your inventory with an object around the room. For example 'use key on door'.";
                            ShowMessage();
                        }
                        break;

                    case "go":
                        {
                            text = "Use to go in a direction. For example 'go west'";
                            ShowMessage();
                        }
                        break;

                    case "open":
                        {
                            text = "Use to open something. For example 'open chest'";
                            ShowMessage();
                        }
                        break;

                    case "take":
                        {
                            text = "Use to take an item. for exmaple 'take key'";
                            ShowMessage();
                        }
                        break;

                    default:
                        {
                            text = "No information about '" + playerTexts[1] + "'.";
                            ShowMessage();
                        }
                        break;

                }
            }
            else
            {
                text = "Current commands you can use: look, go, use, take, open, help, main, save, load, exit.";
                ShowMessage();
                text = "Type 'help [command]' for more information";
                ShowMessage();
            }
        }

        //Let the player look at something
        static public void Look(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after look and isn't blank
            {
                //if any object contains the word the player typed, and in the same location as the player
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))  
                {
                    //gets the list of what the object is
                    var objectResult = objects.Find(x => x.Item1 == playerLocation + playerTexts[1]);
                    //outputs the description string onto the screen
                    text = objectResult.Item6;
                    ShowMessage();
                }
                else //if can't find the object in question
                {
                    //let the player know the object didn't exist
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else //if nothing after look, output the room description
            {
                roomDescription[Convert.ToInt16(playerLocation.Replace("Room", "")) - 1] = true;
            }
        }

        //to move the player to where they want to go (only via compass directions, not holiday resorts)
        static public void Go(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after go and isn't blank
            {
                //if any direction contains the word the player typed, and in the same location as the player
                if (roomDirection.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    //gets the list of what the direction is
                    var direction = roomDirection.Find(x => x.Item1 == playerLocation + playerTexts[1]);
                    //if the description isn't blank
                    if (direction.Item3 != "")
                    {
                        //if they are allowed to go through
                        if (direction.Item2)
                        {
                            //change the player lcoation to the new location
                            playerLocation = direction.Item3;
                        }
                        else //if they are not allowed, show the message on why they can't
                        {
                            text = direction.Item4;
                            ShowMessage();
                        }
                    }
                    else //if blank, use the default message
                    {
                        text = "You can not go that direction.";
                        ShowMessage();
                    }
                }
            }
            else //if theres nothing after 'go', tell the player the message
            {
                text = "Go where?";
                ShowMessage();
            }
        }

        //let a player use an item onto an object
        static public void Use(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after item and isn't blank
            {
                //if any item contains the word the player typed, and in the same location as the player
                if (inventory.Any(c => c.Item1.Contains(playerTexts[1])))
                {
                    //gets the list of what the item is
                    var itemResult = inventory.Find(x => x.Item1 == (playerTexts[1]));
                    if (itemResult.Item2 > 0)
                    {
                        //if the player typed more than more than 2 words
                        if (playerTexts.Length > 2)
                        {
                            //if theres an object the player has typed
                            if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[playerTexts.Length - 1])))
                            {
                                //get the list of the object in question
                                var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[playerTexts.Length - 1]));
                                //if the object matches the item
                                if (objectResult.Item5 == itemResult.Item1)
                                {
                                    //if the object is not activated
                                    if (!objectResult.Item2)
                                    {
                                        //show the message of success and create the list to change the status from false to true;
                                        text = objectResult.Item3;
                                        ShowMessage();
                                        objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                                        objects.Remove(objectResult);
                                    }
                                    else //if the object is already activated, tell the user that it has
                                    {
                                        text = objectResult.Item4;
                                        ShowMessage();
                                    }
                                }
                                else // if the object didn't match the item, then message the player
                                {
                                    text = "It wasn't intended to be used like that.";
                                    ShowMessage();
                                }
                            }
                            else //if didn't match the object, give the message
                            {
                                text = "I didn't understand after " + playerTexts[0] + " " + playerTexts[1];
                                ShowMessage();
                            }
                        }
                        else // if th eplayer didn't type anything after the item, give the message
                        {
                            text = "use " + itemResult.Item1 + " on what?";
                            ShowMessage();
                        }
                    }
                    else //if the player doesn't have the item. give the message
                    {
                        text = "You do not have that item";
                        ShowMessage();
                    }
                }
            }
            else // if couldn't match the item name. message
            {
                text = "Use what?";
                ShowMessage();
            }
        }

        //let the player open something
        static public void Open(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after open and isn't blank
            {
                //if any object contains the word the player typed, and in the same location as the player
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    //gets the list of what the object is
                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[1]));
                    //if the object has the usage of open
                    if (objectResult.Item5 == "open")
                    {
                        //if the object hasn't already been activated
                        if (!objectResult.Item2)
                        {
                            //tell th eplayer the success message and change the status from false to true
                            text = objectResult.Item3;
                            ShowMessage();
                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                            objects.Remove(objectResult);
                        }
                        else //if already activated, message
                        {
                            text = objectResult.Item4;
                            ShowMessage();
                        }
                    }
                    else //if the object can't be operated in that manner. message
                    {
                        text = "You can't open that";
                        ShowMessage();
                    }
                }
                else //if the object didn't exist. message
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else //if th eplayer didn't write anything after 'open'.
            {
                text = "Open what?";
                ShowMessage();
            }
        }

        //let player take an item
        static public void Take(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after take and isn't blank
            {
                //if any object contains the word the player typed, and in the same location as the player
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    //gets the list of what the object is
                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[1]));
                    //if the object has the usage of take
                    if (objectResult.Item5 == "take")
                    {
                        //if the object hasn't been activated
                        if (!objectResult.Item2)
                        {
                            //show success message, change the status from false to true. change the item status by increasing its number by 1.
                            text = objectResult.Item3;
                            ShowMessage();
                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                            objects.Remove(objectResult);
                            var itemResult = inventory.Find(x => x.Item1 == (playerTexts[1]));
                            inventory.Add(Tuple.Create(itemResult.Item1, itemResult.Item2 + 1, itemResult.Item3));
                            inventory.Remove(itemResult);
                        }
                        else //if the object has already been activated. message
                        {
                            text = objectResult.Item4;
                            ShowMessage();
                        }
                    }
                    else //if the object can't be used like that. message
                    {
                        text = "You can't take that";
                        ShowMessage();
                    }
                }
                else //if object didn't exist. message
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else // if nothing written after "take". message
            {
                text = "Take what?";
                ShowMessage();
            }
        }

        //let the player open something
        static public void Read(string[] playerTexts)
        {
            if ((playerTexts.Length > 1) && (playerTexts[1] != "")) //if theres more words after open and isn't blank
            {
                //if any object contains the word the player typed, and in the same location as the player
                if (objects.Any(c => c.Item1.Contains(playerLocation + playerTexts[1])))
                {
                    //gets the list of what the object is
                    var objectResult = objects.Find(x => x.Item1 == (playerLocation + playerTexts[1]));
                    //if the object has the usage of open
                    if (objectResult.Item5 == "read")
                    {
                        //if the object hasn't already been activated
                        if (!objectResult.Item2)
                        {
                            //tell th eplayer the success message and change the status from false to true
                            text = objectResult.Item3;
                            ShowMessage();
                            objects.Add(Tuple.Create(objectResult.Item1, true, objectResult.Item3, objectResult.Item4, objectResult.Item5, objectResult.Item6));
                            objects.Remove(objectResult);
                        }
                        else //if already activated, message
                        {
                            text = objectResult.Item4;
                            ShowMessage();
                        }
                    }
                    else //if the object can't be operated in that manner. message
                    {
                        text = "You can't read that";
                        ShowMessage();
                    }
                }
                else //if the object didn't exist. message
                {
                    text = "I didn't understand after " + playerTexts[0];
                    ShowMessage();
                }
            }
            else //if th eplayer didn't write anything after 'open'.
            {
                text = "Read what?";
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
            inventory.Add(Tuple.Create("amulet", 0, "The amulet is antique gold with large cracked emerald in the center.")); //Room1 key to unlock the door in Room1.
            inventory.Add(Tuple.Create("paper", 0, "Folded up paper under the amulet...what could be on it?")); //Room1 key to unlock the door in Room1.
            playerLocation = "Room1"; //players starting location
            gameStart = false; //tells the game the player is now playing the game.
            menu = false; //lets the game your not in the menu screen
            roomDescription.Add(true);//room1
            roomDescription.Add(true);//room2
            roomDescription.Add(true);//room3
            roomDescription.Add(true);//room4
            Console.Clear();
        }

        //shows the player the message and saves it
        public static void ShowMessage()
        {
            int maxWidth = Console.WindowWidth - (Console.WindowWidth / 4); //the maximum width allowed
            string[] texts = text.Split(" "); //split the string into each word
            text = " "; //reset the text string to a single space.
                for (int i = 0; i < texts.Length; i++)
                {
                    //if the current text plus the next word is less or equal to the maximum width allowed
                    if ((text.Length + texts[i].Length + 1) <= maxWidth) 
                    {
                        //add the next word plus a space
                        text = text + texts[i] + " ";
                    }
                    else //if the maximum width has been exceeded
                    {
                        //output the current text and reset the string with the current word
                        screenSave.Add(text);
                        Console.WriteLine(text);
                        text = " " + texts[i] + " ";
                    }
                }
            //at the end, output what ever is left.
            screenSave.Add(text);
            Console.WriteLine(text);
        }

        //This is the the method that prints the animation out to the screen. The first argument specifies the time between frames. The second specifies the file to read from
        public static void Animation(int delay, string file)
        {
            string aline;
            string filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;
            filePath = Directory.GetParent(Directory.GetParent(filePath).FullName).FullName;

            StreamReader frame = new StreamReader(filePath + "/" + @file);
            while (!frame.EndOfStream)
            {
                aline = frame.ReadLine();
                if (aline == "break") //when adding animations, put all the frames into a single .txt file, and add only the word "break" on it's own line inbetween
                {
                    Thread.Sleep(delay);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine(aline);
                }
            }
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
                text = "You awaken in a dark room you do not recognize. " +
                        "You are cold and lying on the wooden floorboards in the center of the room. " +
                        "Through the window moonlight illuminates the few objects in the room. " +
                        "An old chest lies in the corner under a thick layer of dust. " +
                        "It is as if the house has not been lived in in many years. " +
                        "There is a door to the west.";
                ShowMessage();
                roomDescription[0] = false;
            }
        }

        //First room of the game
        static public void Room2()
        {
            //if door doesn't exist, create it
            if (!objects.Any(c => c.Item1.Contains("Room2door")))
            {
                objects.Add(Tuple.Create("Room2door",
                                         false,
                                         "you open the door",
                                         "Why would lock yourself in? You only just unlocked it!",
                                         "key",
                                         "describe the door"));
            }
            //if bed doesn't exist, create it
            if (!objects.Any(c => c.Item1.Contains("Room2bed")))
            {
                objects.Add(Tuple.Create("Room2bed", //Name of object
                                         false,        //State of the object
                                         "You don’t want to lie on this bed, there is mold everywhere", //text when first activate
                                         "no, THERE IS MOLD!", //text when activating the second time.
                                         "lie", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "faded and so moldy"));      //text describing what it is when the "player" looks at it  
            }
            if (!objects.Any(c => c.Item1.Contains("Room2duchess")))
            {
                objects.Add(Tuple.Create("Room2duchess", //Name of object
                                         false,        //State of the object
                                         "Has the jewelry box on top. Go to duchess to access jewelrybox.", //text when first activate
                                         "maybe you should look in the jewelrybox", //text when activating the second time.
                                         "look", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "A broken mirror above an old duchess reflects a full moon."));      //text describing what it is when the "player" looks at it  
            }
            if (!objects.Any(c => c.Item1.Contains("Room2jewelrybox")))
            {
                objects.Add(Tuple.Create("Room2jewelrybox", //Name of object
                                         false,        //State of the object
                                         "The music screeches and falters, in the box, lying on a bed of faded velvet lies an amulet with some paper folded underneath. " +
                                         "The amulet is antique gold with large cracked emerald in the center. ",//text when first activate
                                         "you've already looked in, now what?", //text when activating the second time.
                                         "open", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "antique jewelrybox with creepy music"));      //text describing what it is when the "player" looks at it  
            }

            var objectResult = objects.Find(x => x.Item1 == "Room2jewelrybox");
            //if chest is opened and key doesn't exist, create it
            if (objectResult.Item2 && (!objects.Any(c => c.Item1.Contains("Room2amulet"))))
            {
                objects.Add(Tuple.Create("Room2amulet",
                                         false,
                                         "You take the amulet",
                                         "You already have the amulet",
                                         "take",
                                         "inscription of R.A.B on the back"));
            }
            if (objectResult.Item2 && (!objects.Any(c => c.Item1.Contains("Room2paper"))))
            {
                objects.Add(Tuple.Create("Room2paper",
                                         false,
                                         "Touch at your own risk… When worn this amulet allows the wearer to see those on the ‘other side’…but know this, " +
                                         "if you can see them, they can see you…If you’ve touched it, it’s too late, they know you’re here…",
                                         "You read the paper",
                                         "read",
                                         "Folded up paper under the amulet...what could be on it?"));
            }
        
            //if direction in room1 equals 0, create all the directions
            if (roomDirection.Count(c => c.Item1.Contains("Room2")) == 0)
            {
                roomDirection.Add(Tuple.Create("Room2north", //what room this is and what direction
                                               false,        //is the player able to go this way   
                                               "",           //the name of the method it will go           
                                               ""));         //The reason they cant go this way, leave as blank if u cant go this way at all
                roomDirection.Add(Tuple.Create("Room2south",
                                               false,
                                               "",
                                               ""));
                roomDirection.Add(Tuple.Create("Room2east",
                                               false,
                                               "",
                                               ""));
                roomDirection.Add(Tuple.Create("Room2west",
                                               true,
                                               "Room3",
                                               ""));
            }
            //if door is open create direction west with true, and delete original
            objectResult = objects.Find(x => x.Item1 == "Room2door");
            if (objectResult.Item2)
            {
                var direction = roomDirection.Find(x => x.Item1 == "Room2west"); //finding the tuple
                roomDirection.Add(Tuple.Create(direction.Item1, true, direction.Item3, direction.Item4)); //creating a new tuple that allows to go through the door
                roomDirection.Remove(direction); //removing the old tuple
            }

            //description of the room
            if (roomDescription[1])
            {
                text = "You step through the door and find yourself in an old bedroom. Thick dust is everywhere, like the first room, it has been untouched for years. " +
                    "Mold is eating away at the bed spread. The curtains hang in strips letting in a little moonlight. A broken mirror above an old duchess reflects a full moon. " +
                    "You hear strange music coming from the jewelrybox upon the duchess. The door you entered from is to the east and another door is to the west. ";
                ShowMessage();
                roomDescription[1]=false;
                roomDescription[2] = true;
            }
        }

        //Hallway
        static public void Room3()
        {
            if (!objects.Any(c => c.Item1.Contains("Room3portraits")))
            {
                objects.Add(Tuple.Create("Room3portraits",
                                         false,
                                         "You can not remove them.",
                                         "You can not remove them.",
                                         "use",
                                         "All these people look haunted and miserable; " +
                                         "you feel unnerved and look away. You don’t" +
                                         " like the way their eyes follow you"));
            }
            
            if (!objects.Any(c => c.Item1.Contains("Room3rats")))
            {
                objects.Add(Tuple.Create("Room3rats", //Name of object
                                         false,        //State of the object
                                         "You attempt to grab one, and recieve a nast bite for your efforts", //text when first activate
                                         "That ended badly last time", //text when activating the second time.
                                         "", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "The rats look desiesed and sickly"));      //text describing what it is when the "player" looks at it  
            }
            if (!objects.Any(c => c.Item1.Contains("Room3stairs")))
            {
                objects.Add(Tuple.Create("Room3stairs", //Name of object
                                         false,        //State of the object
                                         "You decend the stairs", //text when first activate
                                         "You navigate the stairs", //text when activating the second time.
                                         "use", //What verb need to use it, (Might be able to have multiple uses, i.e. "open, move")  
                                         "The stairs look rickety but should hold your weight"));      //text describing what it is when the "player" looks at it  
            }
         
            //if direction in room1 equals 0, create all the directions
            if (roomDirection.Count(c => c.Item1.Contains("Room3")) == 0)
            {
                roomDirection.Add(Tuple.Create("Room3north", //what room this is and what direction
                                               false,        //is the player able to go this way   
                                               "",           //the name of the method it will go           
                                               "You peek inside. Nothing but broken furniture and rats, a large dark stain in the " +
                                               "center. Something awful happened in this room. You can feel it."));         //The reason they cant go this way, leave as blank if u cant go this way at all
                roomDirection.Add(Tuple.Create("Room3south",
                                               true,
                                               "Room2",
                                               ""));
                roomDirection.Add(Tuple.Create("Room3east",
                                               false,
                                               "",
                                               ""));
                roomDirection.Add(Tuple.Create("Room3west",
                                               true,
                                               "Room4",
                                               ""));
            }
            //description of the room
            if (roomDescription[2])
            {
                text = "You enter the hallway; you are on the " +
                    "top floor of what looks like a two story " +
                    "house long abandoned by previous tenants." +
                    " Faded portraits from the Victorian era line " +
                    "the walls. You see another room at the end of " +
                    "the hallway, the door is hanging off the hinges. " +
                    "To your left, stairs lead down to the ground floor.";
                ShowMessage();
                roomDescription[2] = false;
                roomDescription[1] = true;
            }

        }

        //Foyer
        static public void Room4()
        {
            if (roomDirection.Count(c => c.Item1.Contains("Room4")) == 0)
            {
                roomDirection.Add(Tuple.Create("Room4north", //what room this is and what direction
                                               false,        //is the player able to go this way   
                                               "",           //the name of the method it will go           
                                               ""));         //The reason they cant go this way, leave as blank if u cant go this way at all
                roomDirection.Add(Tuple.Create("Room4south",
                                               true,
                                               "Room3",
                                               ""));
                roomDirection.Add(Tuple.Create("Room4east",
                                               false,
                                               "",
                                               ""));
                roomDirection.Add(Tuple.Create("Room3west",
                                               false,
                                               "",
                                               ""));
            }
            //description of the room
            if (roomDescription[3])
            {
                text = "You walk down slowly, some of these stairs look like they " +
                    "won’t hold your weight, as you near the bottom, footsteps sound" +
                    " at the top. You turn around, no one is there. Is it your imagination?" +
                    " The stairs feel like they are never ending despite there only being 15" +
                    " or so. You finally reach the bottom. By this time, you have broken out in" +
                    " a cold sweat, every step you took, another followed, only shadows were" +
                    " there when you looked back. You are facing the front door; the kitchen lies to" +
                    " the right, a drawing room to the left. A rack beside the bottom of the stairs holds a moth eaten fur coat";
                ShowMessage();
                roomDescription[3] = false;
                roomDescription[2] = true;
            }

        }
    }
}

