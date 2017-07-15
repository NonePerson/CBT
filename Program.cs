using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CBT
{
    class Program
    {
        #region the main function that manages everything
        static void Main()
        {
            Console.Title = "CBT";
            Console.SetWindowSize(100, 40);
            Console.WriteLine();
            Console.WriteLine("Situations database for cognitive behavioral therapy");
            Console.WriteLine();
            Console.WriteLine("Press C/c to create a new situation");
            Console.WriteLine();
            Console.WriteLine("Press I/i to add information about your thoughts/feelings/behaviours in a situation");
            Console.WriteLine();
            Console.WriteLine("Press M/m to add 'meta-cognition' to your thoughts (thoughts about your thoughts) in a situation");
            Console.WriteLine();
            Console.WriteLine("Press V/v to view everything about a situation");
            Console.WriteLine();
            string input = Console.ReadLine();
            Elements elements = new Elements();
            if (input.ToUpper() == "C")
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Press M/m to the return the main meun (at any point)");
                Console.WriteLine();
                Console.WriteLine("Enter a name for the situtation: ");
                Console.WriteLine();
                string name = Console.ReadLine();
                ReturnToMain(name);
                Console.WriteLine();
                Console.WriteLine("Add additional notes about the situation (you can leave this empty)");
                Console.WriteLine();
                string notes = Console.ReadLine();
                ReturnToMain(notes);
                int theCount = int.Parse(elements.actualCount.Value);
                theCount++;
                elements.actualCount.SetValue(theCount.ToString());
                string[] textForSituation = new string[6];
                textForSituation[0] = "Situtation name:";
                textForSituation[1] = $"{name}";
                textForSituation[2] = "";
                textForSituation[3] = $"Notes:";
                textForSituation[4] = $"{notes}";
                textForSituation[5] = "";
                File.WriteAllLines($@"situation{theCount}.txt", textForSituation);

                XElement situation = new XElement(XName.Get("Situation"), "");
                XElement thoughts = new XElement(XName.Get("thoughts"), "");
                XElement feelings = new XElement(XName.Get("feelings"), "");
                XElement behaviours = new XElement(XName.Get("behaviours"), "");
                elements.situations.Element("Count").ReplaceNodes(elements.actualCount);
                situation.Add(thoughts);
                situation.Add(feelings);
                situation.Add(behaviours);
                elements.situations.Add(situation);
                elements.doc.ReplaceNodes(elements.situations);
                elements.doc.Save(@"database.xml");

                Console.WriteLine();
                Console.WriteLine("The situation has been created!");
                Console.WriteLine("Press any key to return to the main meun");
                Console.WriteLine();
                Console.ReadLine();
                Console.Clear();
                Main();
            }
            else if(input.ToUpper() == "I")
            {
                ChoosingSitToAddInfo(elements);
            }
            else if(input.ToUpper() == "M")
            {

            }
            else if(input.ToUpper() == "V")
            {

            }
            else
            {
                Console.Clear();
                Main();
            }
        }

        #endregion

        #region Just checking input

        static void ReturnToMain(string input)
        {
            if(input.ToUpper() == "M")
            {
                Console.Clear();
                Main();
            }
        }
        static string CheckValidNumInput(string input)
        {
            int testInput;
            if (!int.TryParse(input, out testInput) && input.ToUpper() != "M")
            {
                Console.WriteLine();
                Console.WriteLine("Irrelevant input. Either a number or M/m. Input again:");
                Console.WriteLine();
                input = Console.ReadLine();
                CheckValidNumInput(input);
                Console.WriteLine();
            }
            return input;
        }

        #endregion

        #region Entering all the info

        static void EnteringInformation(string[] situationText, int i, string info)
        {
            string[] textFile;
            Console.Clear();
            Console.WriteLine($"{situationText[1]}");
            Console.WriteLine();
            Console.WriteLine($"Enter a {info} that arises in the situation, or C/c to skip writing this type of information: ");
            Console.WriteLine();
            string information = Console.ReadLine();
            ReturnToMain(information);
            if(information.ToUpper() != "C")
            {
                Console.WriteLine();
                Console.WriteLine("Additional notes (you can leave this empty): ");
                Console.WriteLine();
                string notes = Console.ReadLine();
                ReturnToMain(notes);
                int intialNumLines = 0;
                bool wasThereAFile = false;
                if (File.Exists($"{situationText[1]}{info}.txt"))
                {
                    textFile = File.ReadAllLines($"{situationText[1]}{info}.txt");
                    intialNumLines = textFile.Count();
                    wasThereAFile = true;
                }
                textFile = new string[intialNumLines];
                if(intialNumLines > 0)
                {
                    textFile = File.ReadAllLines($"{situationText[1]}{info}.txt");
                    File.Delete($"{situationText[1]}{info}.txt");
                }

                string[] newFile = new string[intialNumLines + 8];
                newFile[intialNumLines] = $"A {info} that arises in the situation: ";
                newFile[intialNumLines + 1] = "";
                newFile[intialNumLines + 2] = $"{information}";
                newFile[intialNumLines + 3] = "";
                newFile[intialNumLines + 4] = $"Notes about the {info}: ";
                newFile[intialNumLines + 5] = "";
                newFile[intialNumLines + 6] = $"{notes}";
                newFile[intialNumLines + 7] = "";

                if (wasThereAFile)
                {
                    for(int z = 0; z < intialNumLines; z++)
                    {
                        newFile[z] = textFile[z];
                    }
                }

                File.WriteAllLines($"{situationText[1]}{info}.txt", newFile);

                SituationElements elements = new SituationElements();
                XElement theSituation = elements.allSituations.ElementAt(i-1);
                elements.thoughts = new XElement(theSituation.Element("thoughts"));
                elements.feelings = new XElement(theSituation.Element("feelings"));
                elements.behaviours = new XElement(theSituation.Element("behaviours"));

                if ($"{info}s" == "thoughts")
                {
                    if(!elements.thoughts.HasElements)
                    {
                        XElement thoughts = new XElement(XName.Get("Path"), "");
                        thoughts.SetValue($"{situationText[1]}{info}.txt");
                        elements.thoughts.Add(thoughts);
                    }
                }
                else if ($"{info}s" == "feelings")
                {
                    if(!elements.feelings.HasElements)
                    {
                        XElement feelings = new XElement(XName.Get("Path"), "");
                        feelings.SetValue($"{situationText[1]}{info}.txt");
                        elements.feelings.Add(feelings);
                    }
                }
                else if ($"{info}s" == "behaviours")
                {
                    if(!elements.behaviours.HasElements)
                    {
                        XElement behaviours = new XElement(XName.Get("Path"), "");
                        behaviours.SetValue($"{situationText[1]}{info}.txt");
                        elements.behaviours.Add(behaviours);
                    }
                }

                elements.allSituations.ElementAt(i - 1).RemoveNodes();
                elements.allSituations.ElementAt(i - 1).Add(elements.thoughts);
                elements.allSituations.ElementAt(i - 1).Add(elements.feelings);
                elements.allSituations.ElementAt(i - 1).Add(elements.behaviours);

                elements.situations.RemoveNodes();
                elements.situations.Add(elements.countElement);
                elements.situations.Add(elements.allSituations);

                elements.doc.ReplaceNodes(elements.situations);
                elements.doc.Save(@"database.xml");

                Console.WriteLine();
                Console.WriteLine($"The {info} has been written.");
                Console.WriteLine();
                Console.WriteLine($"Are there more {info}s that arise in this situation? (press Y/y or N/n)");
                Console.WriteLine();
                string areThere = Console.ReadLine();
                ReturnToMain(areThere);
                if (areThere.ToUpper() == "Y")
                {
                    situationText = File.ReadAllLines($@"situation{i}.txt");
                    EnteringInformation(situationText, i, info);
                }
            }
        }

        #endregion

        #region Choosing a situation to add info

        static void ChoosingSitToAddInfo(Elements elements)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Press M/m to the return the main meun (at any point)");
            Console.WriteLine();
            string[] situationText;
            int theCount = int.Parse(elements.actualCount.Value);
            for (int i = 0; i <= theCount; i++)
            {
                if (File.Exists($@"situation{i}.txt"))
                {
                    situationText = File.ReadAllLines($@"situation{i}.txt");
                    Console.WriteLine($"{situationText[1]} (situation number {i})");
                    Console.WriteLine();
                }
            }
            string input2 = Console.ReadLine();
            input2 = CheckValidNumInput(input2);
            ReturnToMain(input2);
            for (int i = 1; i <= theCount; i++)
            {
                if (int.Parse(input2) == i)
                {
                    situationText = File.ReadAllLines($@"situation{i}.txt");
                    EnteringInformation(situationText, i, "thought");
                    situationText = SituationFile(situationText, i);
                    EnteringInformation(situationText, i, "feeling");
                    situationText = SituationFile(situationText, i);
                    EnteringInformation(situationText, i, "behaviour");
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("You're done!");
                    Console.WriteLine("Press any key to return to the main meun");
                    Console.WriteLine();
                    Console.ReadLine();
                    Console.Clear();
                    Main();
                }
            }
            ChoosingSitToAddInfo(elements);
        }

        #endregion

        #region Updating situation file

        static string[] SituationFile(string[] file, int i)
        {
            file = File.ReadAllLines($@"situation{i}.txt");
            return file;
        }

        #endregion
    }
}