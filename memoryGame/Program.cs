using System;
using System.IO;

namespace memoryGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            Menu menu= new Menu();
            while (menu.keepPlaying) {
                menu.Run();
                menu.checkRestart();
            }
        }

    }
    public class Menu {
        private Boolean selectedDiff = false;
        public Boolean keepPlaying { get; private set; } = true;
        //public String difficulty = "";

        public Boolean getSelectedDiff() { return selectedDiff; }
        public void setSelectedDiff(Boolean selectedDiff) { this.selectedDiff = selectedDiff; }

        public void Run() 
        {
            var game = new MemoryGame();
            Console.WriteLine("Hello User! You are about to play a memory game\n");
            while (!selectedDiff)
            {
                selectDifficulty(game);
            }
            game.Run();
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
            }
            else if (selection == "no" || selection == "n")
            {
                keepPlaying = false;
                Console.WriteLine("Ending application");
                Environment.Exit(0);
            }
            else
            {
                checkRestart();
            }
        }
        public void selectDifficulty(MemoryGame game) 
        {
            Console.WriteLine("Select the difficulty:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Hard");
            
            String selection = Console.ReadLine();
            if (selection == "1")
            {
                game.setChances(10);
                game.setSize(4);
                game.setDifficulty("easy");


                setSelectedDiff(true);
                Console.Clear();
            }
            else if (selection == "2")
            {
                game.setChances(15);
                game.setSize(8);
                game.setDifficulty("hard");

                setSelectedDiff(true);
                Console.Clear();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please choose between the available options! (1 or 2)\n");
            }
        }
    }

    public class MemoryGame 
    {
        string[][] hiddenWords;
        private int chances;
        public int moves { get; set; }
        private double completionTime;
        private int size;
        private String difficulty;
        private String userName;
        private string[] words = File.ReadAllLines(@"..\..\..\resources\Words.txt");
        public string[] scoreboard = File.ReadAllLines(@"..\..\..\resources\Scoreboard.txt");
        public string[] gameWords;
        private Boolean running = true;

        public int getChances() { return chances; }
        public void setChances(int chances) { this.chances = chances; }

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
            //TODO use collections + SET
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
                Console.WriteLine("    You solved the game after " + moves + " moves. It took you " + getCompletionTime() + " seconds");
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
                sw.WriteLine(getUserName() + "|" + $"{dateValue}" + "|" + getCompletionTime() + "|" + moves);
                
            }
        }
        public void showScoreboard() {
            String path = @"..\..\..\resources\Scoreboard.txt";
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
        public void selectCoord(out int row, out int column) {
            string firstPick = "";
            int secondPick = 0;
            Console.WriteLine("Please enter the coordinates of the word you want to uncover (example: A1)");

            while (true)
            {
                Console.Write("Row: ");
                firstPick = Console.ReadLine();
                if (firstPick == "A" || firstPick == "B" || firstPick == "C" || firstPick == "D") 
                {
                    row = firstPick[0] - 'A';
                    if (row >= 0 && row < hiddenWords.Length)
                    {
                        break;
                    }

                //TODO better solution?
                }
                
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
            }

            while (true)
            {
                Console.Write("Column: ");
                while (!int.TryParse(Console.ReadLine(), out secondPick))
                {
                    Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
                }
                if (secondPick > 0 && secondPick < 5)
                {
                    column = --secondPick;
                    break;
                }
                Console.WriteLine("\nThe selected value does not exists or is not allowed! Please select the correct coordinate\n");
            }
        }
        public void drawBoard()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
            Console.WriteLine("\t Level: " + getDifficulty());
            Console.WriteLine("\t Guess chances: " + getChances());
            Console.WriteLine();

            Console.WriteLine("\t   1  2  3  4");
            char rowName = 'A';
            foreach (string[] row in hiddenWords)
            {
                Console.Write("\t" + rowName++ + " ");
                foreach (var word in row)
                {
                    Console.Write(word + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine("—-----------------------------------");
        }
        public void Run() {
            if (difficulty == "easy")
            {
                hiddenWords = new string[2][] { new string[4], new string[4] };
            }
            else {
                hiddenWords = new string[4][] { new string[4], new string[4], new string[4], new string[4] };
            }
            //TODO better solution for matrix size

            DateTime t0 = DateTime.Now;
            gameWords = selectRandomWords();

            int currentColumn = 0;
            int currentRow = 0;

            int previousColumn = 0;
            int previousRow = 0;

            Boolean flag = false;
            Boolean checking = false;

            for (int i = 0; i < hiddenWords.Length; i++)
            {
                for (int j = 0; j < hiddenWords[i].Length; j++)
                {
                    hiddenWords[i][j] = " X";
                }

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

                    selectCoord(out currentRow, out currentColumn);
                    if (hiddenWords[currentRow][currentColumn] == " X")
                    {
                        hiddenWords[currentRow][currentColumn] = gameWords[currentRow * hiddenWords[0].Length + currentColumn];
                        checking = true;
                        while (flag == true)
                        {
                            Console.Clear();
                            drawBoard();
                            flag = false;
                            checking = false;
                            if (hiddenWords[currentRow][currentColumn] == hiddenWords[previousRow][previousColumn])
                            {
                                Console.WriteLine("It's a match! Keep going");
                                Console.ReadLine();
                                setSize(getSize() - 1);
                            }
                            else
                            {
                                Console.WriteLine("Your guess was wrong, try again");
                                hiddenWords[currentRow][currentColumn] = "X";
                                hiddenWords[previousRow][previousColumn] = "X";
                                Console.ReadLine();
                                setChances(getChances() - 1);
                            }
                            moves += 1;
                        }
                        Console.Clear();
                        previousRow = currentRow;
                        previousColumn = currentColumn;

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
    }
}
