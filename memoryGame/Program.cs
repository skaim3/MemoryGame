using System;
using System.IO;

namespace memoryGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            Menu menu= new Menu();
            menu.Run();
        }

    }
    public class Menu {
        private Boolean selectedDiff = false;
        //private String difficulty = "";

        public Boolean getSelectedDiff() { return selectedDiff; }
        public void setSelectedDiff(Boolean selectedDiff) { this.selectedDiff = selectedDiff; }

        public void Run() 
        {
            Console.WriteLine("Hello User! You are about to play a memory game\n");
            while (!selectedDiff)
            {
                selectDifficulty();
            }
            checkRestart();
        }
        public void checkRestart() 
        {
            Console.WriteLine();
            Console.WriteLine("Restart? (Y/N)");
            string selection = Console.ReadLine();
            selection = selection.ToLower();
            if (selection == "yes" || selection == "y")
            {
                Console.Clear();
                setSelectedDiff(false);
                Run();
            }
            else if (selection == "no" || selection == "n")
            {
                Console.WriteLine("Ending application");
                Environment.Exit(0);
            }
            else
            {
                checkRestart();
            }
        }
        public void selectDifficulty() 
        {
            Console.WriteLine("Select the difficulty:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Hard");

            String selection = Console.ReadLine();
            if (selection == "1")
            {
                setSelectedDiff(true);
                Console.Clear();
                MemoryGameEasy memoryGameEasy = new MemoryGameEasy();
                memoryGameEasy.Run();
            }
            else if (selection == "2")
            {
                setSelectedDiff(true);
                Console.Clear();
                MemoryGameHard memoryGameHard = new MemoryGameHard();
                memoryGameHard.Run();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please choose between the available options! (1 or 2)\n");
            }
        }
    }

    public class MemoryGame {
        private int chances;
        private int moves;
        private double completionTime;
        private int size;
        private String difficulty;
        private String userName;
        private string[] words = File.ReadAllLines(@"..\..\..\resources\Words.txt");
        public string[] scoreboard;
        public string[] gameWords;
        private Boolean running = true;

        public int getChances() { return chances; }
        public void setChances(int chances) { this.chances = chances; }

        public int getMoves() { return moves; }
        public void setMoves(int moves) { this.moves = moves; }

        public double getCompletionTime() { return completionTime; }
        public void setCompletionTime(double completionTime) { this.completionTime = completionTime; }

        public int getSize() { return size; }
        public void setSize(int size) { this.size = size; }

        public String getDifficulty() { return difficulty; }
        public void setDifficulty(String difficulty) { this.difficulty = difficulty; }

        public String getUserName() { return userName; }
        public void setUserName(String userName) { this.userName = userName; }

        public string[] getWords() { return words; }
        public void setWords(string[] words) { this.words = words; }

        public Boolean getRunning() { return running; }
        public void setRunning(Boolean running) { this.running = running; }


        public string[] selectRandomWords()
        {
            int rand = 0;
            int doubledSize = size * 2;
            string[] randWords = { "" };
            string[] matchingWords = new string[doubledSize];
            Array.Resize(ref randWords, size);
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                rand = random.Next(0, words.Length);
                randWords[i] = words[rand];
            }
            Array.Copy(randWords, 0, matchingWords, 0, size);
            Array.Copy(randWords, 0, matchingWords, doubledSize - size, size);
            shuffleWords(matchingWords);
            return matchingWords;
        }
        public string[] shuffleWords(string[] startingWords)
        {
            int n = startingWords.Length;
            Random random = new Random();
            while (n > 1)
            {
                int k = random.Next(n--);
                string temp = startingWords[n];
                startingWords[n] = startingWords[k];
                startingWords[k] = temp;
            }
            return startingWords;
        }
        public void checkWin()
        {
            if (size == 0)
            {
                Console.Clear();
                Console.WriteLine("\t=========   YOU WIN   =========\t");
                Console.WriteLine("    You matched all the pairs!");
                Console.WriteLine("    You solved the game after " + getMoves() + " moves. It took you " + getCompletionTime() + " seconds");
                setChances(-1);
                showScoreboard();
                saveScore();
                setRunning(false);
            }
        }
        public void checkLose() 
        {
            if (chances == 0) 
            {
                Console.Clear();
                Console.WriteLine("\t=========   GAME OVER   =========\t");
                Console.WriteLine("    You have run out of chances!");
                showScoreboard();
                setRunning(false);
            }
        }
        public void saveScore() 
        {
            var dateTime = DateTime.Now;
            var dateValue = dateTime.ToString("dd/MM/yyyy");
            Console.WriteLine(); 
            Console.WriteLine();
            Console.WriteLine("To save your score, enter your name: ");
            string selectedString = Console.ReadLine();
            setUserName(selectedString);
            String path = @"..\..\..\resources\Scoreboard.txt";
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(getUserName() + "|" + $"{dateValue}" + "|" + getMoves() + "|" + getCompletionTime());
                
            }
        }
        public void showScoreboard() {
            String path = @"..\..\..\resources\Scoreboard.txt";
            scoreboard = File.ReadAllLines(path);
            Console.WriteLine();
            Console.WriteLine("\t=========   BEST SCORES   =========");

            using (StreamReader sr = File.OpenText(path))
            {
                int counter = 0;
                string s = "";
                while ((s = sr.ReadLine()) != null && counter < 10)
                {
                    Console.WriteLine(s);
                    counter++;
                }
            }
            Console.WriteLine();
        }
        public void checkForMatch() 
        { 

        }
        public virtual int selectCoord() {
            return 1;
        }
        public virtual void Run() { }
        public virtual void drawBoard() { }
    }
    public class MemoryGameEasy : MemoryGame
    {
        string[] hiddenWords = { "", "", "", "", "", "", "", "" };
        public override int selectCoord()
        {
            string firstPick = "";
            int secondPick = 0;
            Console.WriteLine("Please enter the coordinates of the word you want to uncover (example: A1)");
            Console.WriteLine("Row: ");
            firstPick = Console.ReadLine();
            Console.WriteLine("Column: ");

            String selectedString = Console.ReadLine();
            Console.WriteLine(selectedString);

            selectedString = selectedString.Replace("\n", "").Replace("\r", "");
            if (selectedString != "")
            {
                secondPick = int.Parse(selectedString);
            }
            else
            {
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
                secondPick = selectCoord();
                return secondPick;
            }
            if (firstPick == "A")
            {
                secondPick -= 1;
            }
            else if (firstPick == "B")
            {
                secondPick += 3;
            }
            else
            {
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
                secondPick = selectCoord();
                return secondPick;
            }

            return secondPick;
        }
        public override void Run()
        {
            DateTime t0 = DateTime.Now;
            setChances(10);
            setSize(4);
            setDifficulty("easy");
            gameWords = selectRandomWords();
            char hideSign = 'X';

            int currentPick = 0;
            int previousPick = 0;

            Boolean flag = false;
            Boolean checking = false;

            for (int i = 0; i < hiddenWords.Length; i++)
            {
                hiddenWords[i] = hideSign.ToString();
            }

            /*
            foreach (string word in gameWords)
            {
                Console.WriteLine(word);
            }
            */
            //Console.WriteLine();

            while (getRunning())
            {
                while (getChances() > 0)
                {
                    drawBoard();

                    currentPick = selectCoord();
                    if (hiddenWords[currentPick] == "X")
                    {
                        hiddenWords[currentPick] = gameWords[currentPick];
                        checking = true;
                        while (flag == true)
                        {
                            if (hiddenWords[currentPick] == hiddenWords[previousPick])
                            {
                                Console.Clear();
                                drawBoard();
                                Console.WriteLine("It's a match! Keep going");
                                flag = false;
                                checking = false;
                                Console.ReadLine();
                                setSize(getSize() - 1);
                                setMoves(getMoves() + 1);
                            }
                            else
                            {
                                Console.Clear();
                                drawBoard();
                                Console.WriteLine("Your guess was wrong, try again");
                                flag = false;
                                checking = false;
                                hiddenWords[currentPick] = hideSign.ToString();
                                hiddenWords[previousPick] = hideSign.ToString();
                                Console.ReadLine();
                                setChances(getChances() - 1);
                                setMoves(getMoves() + 1);
                            }
                        }
                        Console.Clear();
                        previousPick = currentPick;

                        if (checking == true)
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This word was already selected! Press again to continue");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    DateTime t1 = DateTime.Now;
                    TimeSpan ts = (t1 - t0);
                    setCompletionTime(ts.TotalSeconds);
                    checkWin();
                }
                checkLose();

            }
        }
        public override void drawBoard()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
            Console.WriteLine("\t Level: " + getDifficulty());
            Console.WriteLine("\t Guess chances: " + getChances());
            Console.WriteLine();

            Console.WriteLine("\t   1  2  3  4");
            Console.WriteLine("\tA  {0}  {1}  {2}  {3}", hiddenWords[0], hiddenWords[1], hiddenWords[2], hiddenWords[3]);
            Console.WriteLine("\tB  {0}  {1}  {2}  {3}", hiddenWords[4], hiddenWords[5], hiddenWords[6], hiddenWords[7]);

            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
        }


    }
    public class MemoryGameHard : MemoryGame {
        string[] hiddenWords = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public override int selectCoord()
        {
            string firstPick = "";
            int secondPick = 0;
            Console.WriteLine("Please enter the coordinates of the word you want to uncover (example: A1)");
            Console.WriteLine("Row: ");
            firstPick = Console.ReadLine();
            Console.WriteLine("Column: ");

            String selectedString = Console.ReadLine();
            Console.WriteLine(selectedString);

            selectedString = selectedString.Replace("\n", "").Replace("\r", "");
            if (selectedString != "")
            {
                secondPick = int.Parse(selectedString);
            }
            else
            {
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
                secondPick = selectCoord();
                return secondPick;
            }
            if (firstPick == "A")
            {
                secondPick -= 1;
            }
            else if (firstPick == "B")
            {
                secondPick += 3;
            }
            else if (firstPick == "C")
            {
                secondPick += 7;
            }
            else if (firstPick == "D")
            {
                secondPick += 11;
            }
            else 
            {
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
                secondPick = selectCoord();
                return secondPick;
            }
            return secondPick;
        }
        public override void Run() 
        {
            DateTime t0 = DateTime.Now;
            setChances(15);
            setSize(8);
            setDifficulty("hard");
            gameWords = selectRandomWords();
            char hideSign = 'X';

            int currentPick = 0;
            int previousPick = 0;

            Boolean flag = false;
            Boolean checking = false;

            for (int i = 0; i < hiddenWords.Length; i++)
            {
                hiddenWords[i] = hideSign.ToString();
            }

            /*
            foreach (string word in gameWords) { 
                Console.WriteLine(word);
            }
            */
            //Console.WriteLine();

            while (getRunning()) {
                while (getChances() > 0)
                {
                    drawBoard();

                    currentPick = selectCoord();
                    if(hiddenWords[currentPick] == "X"){
                        hiddenWords[currentPick] = gameWords[currentPick];
                        checking = true;
                        while (flag == true)
                        {
                            if (hiddenWords[currentPick] == hiddenWords[previousPick])
                            {
                                Console.Clear();
                                drawBoard();
                                Console.WriteLine("It's a match! Keep going");
                                flag = false;
                                checking = false;
                                Console.ReadLine();
                                setSize(getSize() - 1);
                                setMoves(getMoves() + 1);
                            }
                            else
                            {
                                Console.Clear();
                                drawBoard();
                                Console.WriteLine("Your guess was wrong, try again");
                                flag = false;
                                checking = false;
                                hiddenWords[currentPick] = hideSign.ToString();
                                hiddenWords[previousPick] = hideSign.ToString();
                                Console.ReadLine();
                                setChances(getChances() - 1);
                                setMoves(getMoves() + 1);
                            }
                        }
                        Console.Clear();
                        previousPick = currentPick;

                        if (checking == true)
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("This word was already selected! Press again to continue");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    DateTime t1 = DateTime.Now;
                    TimeSpan ts = (t1 - t0);
                    setCompletionTime(ts.TotalSeconds);
                    checkWin();
                    
                }
                checkLose();

            }
        }
        public override void drawBoard() {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
            Console.WriteLine("\t Level: " + getDifficulty());
            Console.WriteLine("\t Guess chances: " + getChances());
            Console.WriteLine();

            Console.WriteLine("\t   1  2  3  4");
            Console.WriteLine("\tA  {0}  {1}  {2}  {3}", hiddenWords[0], hiddenWords[1], hiddenWords[2], hiddenWords[3]);
            Console.WriteLine("\tB  {0}  {1}  {2}  {3}", hiddenWords[4], hiddenWords[5], hiddenWords[6], hiddenWords[7]);
            Console.WriteLine("\tC  {0}  {1}  {2}  {3}", hiddenWords[8], hiddenWords[9], hiddenWords[10], hiddenWords[11]);
            Console.WriteLine("\tD  {0}  {1}  {2}  {3}", hiddenWords[12], hiddenWords[13], hiddenWords[14], hiddenWords[15]);

            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
        }
    }
    
}
