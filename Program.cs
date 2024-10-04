using System;
using System.Collections.Generic;
using System.IO;

namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name; // protein name
        public string organism;
        public string formula; // formula
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();

        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');
                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                data.Add(protein);
            }
            reader.Close();
        }

        static void Output(StreamWriter writer, string message)
        {
            Console.WriteLine(message);
            writer.WriteLine(message);
        }

        static void ProcessCommands(string filename, string outputFile)
        {
            using StreamReader reader = new StreamReader(filename);
            using StreamWriter writer = new StreamWriter(outputFile);
            
            Output(writer, "Maksim Porva");
            Output(writer, "Genetic Searching");
            Output(writer, "----------------------------------------");

            int counter = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                counter++;
                string[] command = line.Split('\t');

                switch (command[0])
                {
                    case "search":
                        string decodedFormula = Decoding(command[1]);
                        Output(writer, $"{counter.ToString("D3")}   search   {decodedFormula}");
                        Output(writer, "organism" + "\t" + "\t" + "protein");
                        int index = Search(command[1]);
                        if (index != -1)
                        {
                            Output(writer, $"{data[index].organism}    {data[index].name}");
                        }
                        else
                        {
                            Output(writer, "NOT FOUND");
                        }
                        break;

                    case "diff":
                        Output(writer, $"{counter.ToString("D3")}   diff   {command[1]} vs {command[2]}");
                        int diffCount = Diff(command[1], command[2]);

                        if (diffCount == -1)
                        {
                            Output(writer, $"MISSING: {command[1]} {command[2]}");
                        }
                        else
                        {
                            Output(writer, $"amino-acids difference:\n{diffCount}");
                        }
                        break;

                    case "mode":
                        Output(writer, $"{counter.ToString("D3")}   mode   {command[1]}");
                        string modeResult = Mode(command[1]);

                        if (modeResult == null)
                        {
                            Output(writer, $"MISSING: {command[1]}");
                        }
                        else
                        {
                            Output(writer, $"amino-acid occurs:\n{modeResult}");
                        }
                        break;

                    default:
                        Output(writer, $"{counter.ToString("D3")}   unknown command");
                        break;
                }
                
                Output(writer, "----------------------------------------");
            }
        }        
        
        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    for (int j = 0; j < conversion - 1; j++) decoded += letter;
                }
                else decoded += formula[i];
            }
            return decoded;
        }

        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i;
            }
            return -1;
        }

        static int Diff(string seq1, string seq2)
        {
            string decoded1 = GetFormula(seq1);
            string decoded2 = GetFormula(seq2);

            if (decoded1 == null || decoded2 == null)
                return -1;

            int lengthDiff = Math.Abs(decoded1.Length - decoded2.Length);
            int mismatchCount = 0;

            for (int i = 0; i < Math.Min(decoded1.Length, decoded2.Length); i++)
            {
                if (decoded1[i] != decoded2[i])
                    mismatchCount++;
            }

            return mismatchCount + lengthDiff;
        }

        static string Mode(string sequenceName)
        {
            string decodedSequence = GetFormula(sequenceName);
            if (decodedSequence == null)
                return null;

            Dictionary<char, int> frequencyMap = new Dictionary<char, int>();

            foreach (char aminoAcid in decodedSequence)
            {
                if (!frequencyMap.ContainsKey(aminoAcid))
                {
                    frequencyMap[aminoAcid] = 0;
                }
                frequencyMap[aminoAcid]++;
            }

            char mostFrequentAminoAcid = ' ';
            int highestCount = 0;

            foreach (var entry in frequencyMap)
            {
                if (entry.Value > highestCount || (entry.Value == highestCount && entry.Key < mostFrequentAminoAcid))
                {
                    mostFrequentAminoAcid = entry.Key;
                    highestCount = entry.Value;
                }
            }

            return $"{mostFrequentAminoAcid}          {highestCount}";
        }

        static void Main(string[] args)
        {
            ReadGeneticData("sequences.2.txt");
            ProcessCommands("commands.2.txt", "genedata.2.txt");
        }
    }
}
