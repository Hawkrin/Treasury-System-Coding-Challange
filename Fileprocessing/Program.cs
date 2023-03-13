using System;

class Program {



    /*
    1) tar in ett filnamn, sökvägen till en utdata mapp samt ett datum (på ISO format)
    2) Programmet ska läsa angiven fil och sedan skapa en fil per bank i utdata mappen.
    3) Ibland kommer det med gamla eller framtida transaktion i filen därför ska varje transaktion kategoriseras efter datum så att nästa program i kedjan vet hur transaktionerna ska behandlas.
    */


    public static void FileProcess(string inputFile, string outputFolder, string inputDate) {

        if (string.IsNullOrEmpty(inputFile)) {
            throw new ArgumentException("Input file path cannot be null or empty.");
        }
        
        if (string.IsNullOrEmpty(outputFolder)) {
            throw new ArgumentException("Output folder path cannot be null or empty.");
        }
        
        if (string.IsNullOrEmpty(inputDate)) {
            throw new ArgumentException("Input date cannot be null or empty.");
        }
        
        if (!File.Exists(inputFile)) {
            throw new FileNotFoundException($"Input file not found at path {inputFile}.");
        }

        try {
            
            using (StreamReader reader = new StreamReader(inputFile)) {
                string header = reader.ReadLine(); // Skip the header line
                string line, type = "";
                DateTime InputDate = DateTime.Parse(inputDate);

                // the bank files
                Dictionary<string, string> bankFiles = new Dictionary<string, string>();

                while ((line = reader.ReadLine()) != null) {
                    string[] substrings = line.Split(';');
                    string account = substrings[0];
                    string amount = substrings[1];
                    DateTime date = DateTime.Parse(substrings[2]);
                    string bankName = substrings[3];

                    // creates files for the lines in in.txt, except for Bank.
                    if (bankName != "Bank") {
                        string newFilePath = Path.Combine(outputFolder, Path.ChangeExtension(bankName, ".txt"));
                        bankName = Path.GetFileNameWithoutExtension(newFilePath);

                        // if the file doesnt exist then the header is written
                        if (!bankFiles.ContainsKey(bankName)) {
                            File.WriteAllText(newFilePath, "Konto;Belopp;Datum;Typ\n");
                            bankFiles[bankName] = newFilePath;
                        }

                        switch (DateTime.Compare(InputDate, date)) {
                            case 0:
                                type = "ACTIVE";
                                break;
                            case 1:
                                type = "OLD";
                                break;
                            default:
                                type = "FUTURE";
                                break;
                        }

                        // append to existing file or write to new file
                        if (bankFiles.TryGetValue(bankName, out string filePath)) {
                            File.AppendAllText(filePath, $"{account};{amount};{date.ToString("yyyy-MM-dd")};{type}\n");
                        } else {
                            File.AppendAllText(newFilePath, $"{account};{amount};{date.ToString("yyyy-MM-dd")};{type}\n");
                            bankFiles[bankName] = newFilePath;
                        }
                    }
                }
            } 
        } catch (Exception err) {
            Console.WriteLine($"An error occurred while processing the file: {err.Message}");
        } 
    }


    public static void Main(string[] args) {
        if (args.Length != 3) {
            Console.WriteLine("Usage: FileProcess.exe inputFile outputFolder inputDate");
            return;
        }

        string inputFile = args[0];
        string outputFolder = args[1];
        string inputDate = args[2];

        FileProcess(inputFile, outputFolder, inputDate);
    }
}