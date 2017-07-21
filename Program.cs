using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine("Press M/m to add 'meta-cognition' to your thoughts/feelings");
            Console.WriteLine("(thoughts/feelings about your thoughts/feelings) in a situation");
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
            else if (input.ToUpper() == "I")
            {
                ChoosingSitToAddInfo(elements);
            }
            else if (input.ToUpper() == "M")
            {
                AddingMetaCognition(elements);
            }
            else if (input.ToUpper() == "V")
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
            if (input.ToUpper() == "M")
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

        static string forMoreMetaCognition(string input)
        {
            if (input.ToUpper() != "Y" && input.ToUpper() != "N")
            {
                Console.WriteLine();
                Console.WriteLine("Irrelevant input. Either Y/y or N/n.");
                Console.WriteLine();
                input = Console.ReadLine();
                forMoreMetaCognition(input);
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
            Console.WriteLine();
            Console.WriteLine("Press M/m to return to the main meun (at any point)");
            Console.WriteLine();
            Console.WriteLine($"Situation name: {situationText[1]}");
            Console.WriteLine();
            Console.WriteLine($"Enter a {info} that arises in the situation, or C/c to skip writing this type of information: ");
            Console.WriteLine();
            string information = Console.ReadLine();
            ReturnToMain(information);
            if (information.ToUpper() != "C")
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
                if (intialNumLines > 0)
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
                    for (int z = 0; z < intialNumLines; z++)
                    {
                        newFile[z] = textFile[z];
                    }
                }

                File.WriteAllLines($"{situationText[1]}{info}.txt", newFile);

                SituationElements elements = new SituationElements();
                XElement theSituation = elements.allSituations.ElementAt(i - 1);
                elements.thoughts = new XElement(theSituation.Element("thoughts"));
                elements.feelings = new XElement(theSituation.Element("feelings"));
                elements.behaviours = new XElement(theSituation.Element("behaviours"));

                if ($"{info}s" == "thoughts")
                {
                    if (!elements.thoughts.HasElements)
                    {
                        XElement thoughts = new XElement(XName.Get("Path"), "");
                        thoughts.SetValue($"{situationText[1]}{info}.txt");
                        elements.thoughts.Add(thoughts);

                        XElement metaCountThoughts = new XElement(XName.Get("MetaCount"), "");
                        metaCountThoughts.SetValue("0");
                        elements.thoughts.Add(metaCountThoughts);
                    }
                }
                else if ($"{info}s" == "feelings")
                {
                    if (!elements.feelings.HasElements)
                    {
                        XElement feelings = new XElement(XName.Get("Path"), "");
                        feelings.SetValue($"{situationText[1]}{info}.txt");
                        elements.feelings.Add(feelings);

                        XElement metaCountFeelings = new XElement(XName.Get("MetaCount"), "");
                        metaCountFeelings.SetValue("0");
                        elements.feelings.Add(metaCountFeelings);
                    }
                }
                else if ($"{info}s" == "behaviours")
                {
                    if (!elements.behaviours.HasElements)
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
            Console.WriteLine("Press M/m to return to the main meun (at any point)");
            Console.WriteLine();
            Console.WriteLine("Press the number of the situation you wish to add information to:");
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

        #region Meta-cognition

        private static void AddingMetaCognition(Elements elements)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Press M/m to return to the main meun (at any point)");
            Console.WriteLine();
            Console.WriteLine("Press the number of the situation you wish to add meta-cogntition to:");
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
                    ChoosingMetaCognition(situationText, i);
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
            AddingMetaCognition(elements);
        }

        private static void ChoosingMetaCognition(string[] situationText, int i)
        {
            string ForFilling = "";
            string[] allInfo = File.ReadAllLines($"situation{i}.txt");
            IEnumerable<string> numOfInfo = new List<string>();
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Press M/m to the return the main meun (at any point)");
            Console.WriteLine();
            Console.WriteLine($"Situation name: {situationText[1]}");
            Console.WriteLine();
            Console.WriteLine("Do you want to enter thoughts/s about your thought/s, or feeling/s about your feeling/s?");
            Console.WriteLine("(Press T/t to enter thought/s about thought/s, or F/f to enter feeling/s about feeling/s)");
            Console.WriteLine();
            string metaChoice = Console.ReadLine();
            ReturnToMain(metaChoice);
            if (metaChoice.ToUpper() != "F" && metaChoice.ToUpper() != "T")
            {
                ChoosingMetaCognition(situationText, i);
            }
            else if (metaChoice.ToUpper() == "F")
            {
                allInfo = File.ReadAllLines($"{situationText[1]}feeling.txt");
                numOfInfo = allInfo.Where(line => line == "A feeling that arises in the situation: ");
                ForFilling = "feeling";
            }
            else if (metaChoice.ToUpper() == "T")
            {
                allInfo = File.ReadAllLines($"{situationText[1]}thought.txt");
                numOfInfo = allInfo.Where(line => line == "A thought that arises in the situation: ");
                ForFilling = "thought";
            }

            int infoAmount = numOfInfo.Count();

            WritingMetaCognition(situationText, allInfo, infoAmount, ForFilling, i);
        }

        private static void WritingMetaCognition(string[] situationText, string[] allInfo, int infoAmount, string forFilling, int situationNum)
        {
            SituationElements elements = new SituationElements();
            XElement CurrectSituation = new XElement(elements.allSituations.ElementAt(situationNum-1));
            elements.thoughts = new XElement(CurrectSituation.Element("thoughts"));
            elements.feelings = new XElement(CurrectSituation.Element("feelings"));
            XElement relevantMetaCount = new XElement(XName.Get("fgf"));
            XElement path = new XElement(XName.Get("fgd"));
            if(forFilling == "thought")
            {
                relevantMetaCount = new XElement(elements.thoughts.Element("MetaCount"));
                path = new XElement(elements.thoughts.Element("Path"));
            }
            else if(forFilling == "feeling")
            {
                relevantMetaCount = new XElement(elements.feelings.Element("MetaCount"));
                path = new XElement(elements.feelings.Element("Path"));
            }

            int countingLines = 0;
            for (int i = 1; i <= infoAmount; i++)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Press M/m to return to the main meun (at any point)");
                Console.WriteLine();
                Console.WriteLine($"Situation name: {situationText[1]}");
                Console.WriteLine();
                for (int y = 0; y < 7; y++)
                {
                    if (countingLines <= allInfo.Count() - 1)
                    {
                        if (y == 6 && string.IsNullOrEmpty(allInfo[countingLines]))
                        {
                            Console.WriteLine("{no note}");
                        }
                        else
                        {
                            Console.WriteLine(allInfo[countingLines]);
                        }
                        countingLines++;
                    }
                }
                Console.WriteLine();
                countingLines++;

                string[] textPreEdit;

                Console.WriteLine($"Press C/c to skip entering {forFilling}s about the above {forFilling},");
                Console.WriteLine($"Or enter your {forFilling}/s about the above {forFilling}:");
                Console.WriteLine();
                string input = Console.ReadLine();
                ReturnToMain(input);
                if (input.ToUpper() != "C")
                {
                    Console.WriteLine();
                    Console.WriteLine("Additional notes (you can leave this empty): ");
                    Console.WriteLine();
                    string notes = Console.ReadLine();
                    Console.WriteLine();
                    string[] inputForMetaCognition = new string[i * 8];
                    if (File.Exists($"{situationText[1]}{forFilling}{relevantMetaCount.Value + 1}meta.txt"))
                    {
                        textPreEdit = File.ReadAllLines($"{situationText[1]}{forFilling}meta.txt");
                        if (inputForMetaCognition.Count() < textPreEdit.Count())
                        {
                            inputForMetaCognition = new string[textPreEdit.Count()];

                        }
                        for (int z = 0; z < textPreEdit.Count(); z++)
                        {
                            inputForMetaCognition[z] = textPreEdit[z];
                        }

                        File.Delete($"{situationText[1]}{forFilling}{relevantMetaCount.Value + 1}meta.txt");
                    }

                    inputForMetaCognition[0 + ((i - 1) * 8)] = $"a meta-cognition ({forFilling}) about the above {forFilling}:";
                    inputForMetaCognition[1 + ((i - 1) * 8)] = "";
                    inputForMetaCognition[2 + ((i - 1) * 8)] = $"{input}";
                    inputForMetaCognition[3 + ((i - 1) * 8)] = "";
                    inputForMetaCognition[4 + ((i - 1) * 8)] = "Notes about this meta-cognition:";
                    inputForMetaCognition[5 + ((i - 1) * 8)] = "";

                    if (string.IsNullOrEmpty(notes))
                    {
                        inputForMetaCognition[6 + ((i - 1) * 8)] = "{none}";
                    }
                    else
                    {
                        inputForMetaCognition[6 + ((i - 1) * 8)] = $"{notes}";
                    }
                    inputForMetaCognition[7 + ((i - 1) * 8)] = "";

                    int theCount = int.Parse(relevantMetaCount.Value);
                    theCount++;
                    relevantMetaCount.SetValue(theCount.ToString());

                    File.WriteAllLines($"{situationText[1]}{forFilling}{relevantMetaCount.Value}meta.txt", inputForMetaCognition);

                    if (forFilling == "thought")
                    {
                        elements.thoughts.RemoveNodes();
                        elements.thoughts.Add(relevantMetaCount);
                        elements.thoughts.Add(path);
                    }
                    else if (forFilling == "feeling")
                    {
                        elements.feelings.RemoveNodes();
                        elements.feelings.Add(relevantMetaCount);
                        elements.feelings.Add(path);
                    }
                    elements.behaviours = new XElement(CurrectSituation.Element("behaviours"));
                    CurrectSituation.RemoveNodes();
                    CurrectSituation.Add(elements.thoughts);
                    CurrectSituation.Add(elements.feelings);
                    CurrectSituation.Add(elements.behaviours);
                    elements.allSituations.RemoveAt(situationNum - 1);
                    elements.allSituations.Add(CurrectSituation);
                    elements.situations.RemoveNodes();
                    elements.situations.Add(elements.allSituations);
                    elements.situations.Add(elements.countElement);
                    elements.doc.RemoveNodes();
                    elements.doc.Add(elements.situations);

                    elements.doc.Save(@"database.xml");

                    Console.WriteLine();
                    Console.WriteLine($"Do you want to add another {forFilling} about this {forFilling}? (Press Y/y or N/n)");
                    Console.WriteLine();

                    string moreMeta = Console.ReadLine();
                    moreMeta = forMoreMetaCognition(moreMeta);
                    if (moreMeta.ToUpper() == "Y")
                    {
                        AddingEvenMoreMetaCognition(situationText, countingLines, allInfo, relevantMetaCount, i, inputForMetaCognition, forFilling, elements, CurrectSituation, situationNum, path);
                    }
                }
            }
        }

        private static void AddingEvenMoreMetaCognition(string[] situationText, int countingLines, string[] allInfo, XElement relevantMetaCount, int i, string[] inputForMetaCognition, string forFilling, SituationElements elements, XElement CurrectSituation, int situationNum, XElement path)
        {

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Press M/m to return to the main meun (at any point)");
            Console.WriteLine();
            Console.WriteLine($"Situation name: {situationText[1]}");
            Console.WriteLine();
            int newLines = countingLines - 8;
            for (int y = 0; y < 7; y++)
            {
                if (newLines <= allInfo.Count() - 1)
                {
                    if (y == 6 && string.IsNullOrEmpty(allInfo[newLines]))
                    {
                        Console.WriteLine("{no note}");
                    }
                    else
                    {
                        Console.WriteLine(allInfo[newLines]);
                    }
                    newLines++;
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Press C/c to skip entering {forFilling}s about the above {forFilling},");
            Console.WriteLine($"Or enter your {forFilling}/s about the above {forFilling}:");
            Console.WriteLine();
            string input2 = Console.ReadLine();
            ReturnToMain(input2);
            if (input2.ToUpper() != "C")
            {
                Console.WriteLine();
                Console.WriteLine("Additional notes (you can leave this empty): ");
                Console.WriteLine();
                string notes2 = Console.ReadLine();
                Console.WriteLine();

                inputForMetaCognition[0 + ((i - 1) * 8)] = $"a meta-cognition ({forFilling}) about the above {forFilling}:";
                inputForMetaCognition[1 + ((i - 1) * 8)] = "";
                inputForMetaCognition[2 + ((i - 1) * 8)] = $"{input2}";
                inputForMetaCognition[3 + ((i - 1) * 8)] = "";
                inputForMetaCognition[4 + ((i - 1) * 8)] = "Notes about this meta-cognition:";
                inputForMetaCognition[5 + ((i - 1) * 8)] = "";

                if (string.IsNullOrEmpty(notes2))
                {
                    inputForMetaCognition[6 + ((i - 1) * 8)] = "{none}";
                }
                else
                {
                    inputForMetaCognition[6 + ((i - 1) * 8)] = $"{notes2}";
                }
                inputForMetaCognition[7 + ((i - 1) * 8)] = "";

                if (File.Exists($"{situationText[1]}{forFilling}{relevantMetaCount.Value + 1}meta.txt"))
                {
                    File.Delete($"{situationText[1]}{forFilling}{relevantMetaCount.Value + 1}meta.txt");
                }

                int theCount = int.Parse(relevantMetaCount.Value);
                theCount++;
                relevantMetaCount.SetValue(theCount.ToString());

                File.WriteAllLines($"{situationText[1]}{forFilling}{relevantMetaCount.Value}meta.txt", inputForMetaCognition);

                if (forFilling == "thought")
                {
                    elements.thoughts.RemoveNodes();
                    elements.thoughts.Add(relevantMetaCount);
                    elements.thoughts.Add(path);
                }
                else if (forFilling == "feeling")
                {
                    elements.feelings.RemoveNodes();
                    elements.feelings.Add(relevantMetaCount);
                    elements.feelings.Add(path);
                }
                elements.behaviours = new XElement(CurrectSituation.Element("behaviours"));
                CurrectSituation.RemoveNodes();
                CurrectSituation.Add(elements.thoughts);
                CurrectSituation.Add(elements.feelings);
                CurrectSituation.Add(elements.behaviours);
                elements.allSituations.RemoveAt(situationNum - 1);
                elements.allSituations.Add(CurrectSituation);
                elements.situations.RemoveNodes();
                elements.situations.Add(elements.allSituations);
                elements.situations.Add(elements.countElement);
                elements.doc.RemoveNodes();
                elements.doc.Add(elements.situations);

                elements.doc.Save(@"database.xml");

                Console.WriteLine();
                Console.WriteLine($"Do you want to add another {forFilling} about this {forFilling}? (Press Y/y or N/n)");
                Console.WriteLine();

                string finalCheck = Console.ReadLine();
                finalCheck = forMoreMetaCognition(finalCheck);
                if (finalCheck.ToUpper() == "Y")
                {
                    AddingEvenMoreMetaCognition(situationText, countingLines, allInfo, relevantMetaCount, i, inputForMetaCognition, forFilling, elements, CurrectSituation, situationNum, path);
                }
            }
        }
        #endregion
    }
}