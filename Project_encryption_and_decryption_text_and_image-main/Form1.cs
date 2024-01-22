using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_encryption_and_decryption_text_and_image_main
{
    public partial class Form1 : Form
    {
        public Bitmap originalImage;
        public Bitmap encryptedImage;
        public Bitmap decryptedImage;
        public Dictionary<char, char> encryptionKey = new Dictionary<char, char>();
        public Dictionary<char, char> decryptionKey = new Dictionary<char, char>();
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        private string key;

        private const string PlayfairKey = "KEYWORD"; // The Playfair encryption key
        public Form1()
        {
            InitializeComponent();
            InitializeKeys();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            // Generate a random key of the same length as the alphabet
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < Alphabet.Length; i++)
            {
                int index = random.Next(Alphabet.Length);
                char c = Alphabet[index];
                sb.Append(c);
            }

            key = sb.ToString();

        }
        public string Encrypt_Caser(string plaintext, int key)
        {
            string result = "";

            foreach (char letter in plaintext)
            {
                if (char.IsLetter(letter))
                {
                    char shifted = (char)(letter + key);

                    if (char.IsUpper(letter))
                    {
                        if (shifted > 'Z')
                            shifted = (char)(shifted - 26);
                    }
                    else
                    {
                        if (shifted > 'z')
                            shifted = (char)(shifted - 26);
                    }

                    result += shifted;
                }
                else
                {
                    result += letter;
                }
            }

            return result;
        }
        private string DecryptCaesar(string cipherText, int shift)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char c in cipherText)
            {
                // Check if the character is a letter
                if (char.IsLetter(c))
                {
                    char shiftedChar = (char)(c - shift);

                    // Handle wrapping around the alphabet
                    if (char.IsUpper(c) && shiftedChar < 'A')
                    {
                        shiftedChar = (char)(shiftedChar + 26);
                    }
                    else if (char.IsLower(c) && shiftedChar < 'a')
                    {
                        shiftedChar = (char)(shiftedChar + 26);
                    }

                    decryptedText.Append(shiftedChar);
                }
                else
                {
                    // Preserve non-letter characters
                    decryptedText.Append(c);
                }
            }

            return decryptedText.ToString();
        }
        public void InitializeKeys()
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string shuffledAlphabet = "XPMGTDHLYONZBWEARKJUFSCIQV";

            // Create encryption key
            for (int i = 0; i < alphabet.Length; i++)
            {
                encryptionKey[alphabet[i]] = shuffledAlphabet[i];
            }

            // Create decryption key (reverse mapping)
            foreach (var pair in encryptionKey)
            {
                decryptionKey[pair.Value] = pair.Key;
            }
        }
        public string EncryptMonoalphabetic(string plainText)
        {
            StringBuilder encryptedText = new StringBuilder();

            foreach (char c in plainText.ToUpper())
            {
                if (encryptionKey.ContainsKey(c))
                {
                    encryptedText.Append(encryptionKey[c]);
                }
                else
                {
                    encryptedText.Append(c); // Preserve non-alphabetic characters
                }
            }

            return encryptedText.ToString();
        }

        // Decrypts ciphertext using the monoalphabetic cipher
        public string DecryptMonoalphabetic(string cipherText)
        {
            StringBuilder decryptedText = new StringBuilder();

            foreach (char c in cipherText.ToUpper())
            {
                if (decryptionKey.ContainsKey(c))
                {
                    decryptedText.Append(decryptionKey[c]);
                }
                else
                {
                    decryptedText.Append(c); // Preserve non-alphabetic characters
                }
            }

            return decryptedText.ToString();
        }




        private string Encrypt_Playfair(string plaintext)
        {
            string key = PrepareKey(PlayfairKey);
            char[,] playfairMatrix = GeneratePlayfairMatrix(key);

            StringBuilder encryptedText = new StringBuilder();

            for (int i = 0; i < plaintext.Length; i += 2)
            {
                char firstChar = plaintext[i];
                char secondChar = (i + 1 < plaintext.Length) ? plaintext[i + 1] : 'X';

                int[] firstCharPosition = FindCharPosition(playfairMatrix, firstChar);
                int[] secondCharPosition = FindCharPosition(playfairMatrix, secondChar);

                int firstRow = firstCharPosition[0];
                int firstCol = firstCharPosition[1];
                int secondRow = secondCharPosition[0];
                int secondCol = secondCharPosition[1];

                if (firstRow == secondRow)
                {
                    firstCol = (firstCol + 1) % 5;
                    secondCol = (secondCol + 1) % 5;
                }
                else if (firstCol == secondCol)
                {
                    firstRow = (firstRow + 1) % 5;
                    secondRow = (secondRow + 1) % 5;
                }
                else
                {
                    int temp = firstCol;
                    firstCol = secondCol;
                    secondCol = temp;
                }

                encryptedText.Append(playfairMatrix[firstRow, firstCol]);
                encryptedText.Append(playfairMatrix[secondRow, secondCol]);
            }

            return encryptedText.ToString();
        }

        private string Decrypt_playfair(string encryptedText)
        {
            string key = PrepareKey(PlayfairKey);
            char[,] playfairMatrix = GeneratePlayfairMatrix(key);

            StringBuilder decryptedText = new StringBuilder();

            for (int i = 0; i < encryptedText.Length; i += 2)
            {
                char firstChar = encryptedText[i];
                char secondChar = encryptedText[i + 1];

                int[] firstCharPosition = FindCharPosition(playfairMatrix, firstChar);
                int[] secondCharPosition = FindCharPosition(playfairMatrix, secondChar);

                int firstRow = firstCharPosition[0];
                int firstCol = firstCharPosition[1];
                int secondRow = secondCharPosition[0];
                int secondCol = secondCharPosition[1];

                if (firstRow == secondRow)
                {
                    firstCol = (firstCol - 1 + 5) % 5;
                    secondCol = (secondCol - 1 + 5) % 5;
                }
                else if (firstCol == secondCol)
                {
                    firstRow = (firstRow - 1 + 5) % 5;
                    secondRow = (secondRow - 1 + 5) % 5;
                }
                else
                {
                    int temp = firstCol;
                    firstCol = secondCol;
                    secondCol = temp;
                }

                decryptedText.Append(playfairMatrix[firstRow, firstCol]);
                decryptedText.Append(playfairMatrix[secondRow, secondCol]);
            }

            return decryptedText.ToString();
        }

        private string PrepareKey(string key)
        {
            // Remove duplicates and replace 'J' with 'I'
            string preparedKey = new string(key.Distinct().ToArray()).Replace("J", "I");

            // Add remaining letters of the alphabet
            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";

            foreach (char letter in alphabet)
            {
                if (!preparedKey.Contains(letter))
                {
                    preparedKey += letter;
                }
            }

            return preparedKey;
        }

        private char[,] GeneratePlayfairMatrix(string key)
        {
            char[,] playfairMatrix = new char[5, 5];
            int row = 0, col = 0;

            foreach (char letter in key)
            {
                playfairMatrix[row, col] = letter;
                col++;

                if (col == 5)
                {
                    col = 0;
                    row++;
                }
            }

            string alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ";

            foreach (char letter in alphabet)
            {
                if (!key.Contains(letter))
                {
                    playfairMatrix[row, col] = letter;
                    col++;

                    if (col == 5)
                    {
                        col = 0;
                        row++;
                    }
                }
            }

            return playfairMatrix;
        }

        private int[] FindCharPosition(char[,] playfairMatrix, char targetChar)
        {
            int[] position = new int[2];

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    if (playfairMatrix[row, col].ToString() == targetChar.ToString())
                    {
                        position[0] = row;
                        position[1] = col;
                        return position;
                    }
                }
            }

            return position;
        }
        private string EncryptPolyalphabetic(string plaintext, string key)
        {
            StringBuilder ciphertext = new StringBuilder();
            int keyIndex = 0;

            foreach (char c in plaintext)
            {
                if (char.IsLetter(c))
                {
                    int plainIndex = char.ToUpper(c) - 'A';
                    int shift = key[keyIndex % key.Length] - 'A';

                    int cipherIndex = (plainIndex + shift) % 26;

                    char encryptedChar = (char)('A' + cipherIndex);
                    ciphertext.Append(char.IsLower(c) ? char.ToLower(encryptedChar) : encryptedChar);

                    keyIndex++;
                }
                else
                {
                    ciphertext.Append(c);
                }
            }

            return ciphertext.ToString();
        }

        private string DecryptPolyalphabetic(string ciphertext, string key)
        {
            StringBuilder plaintext = new StringBuilder();
            int keyIndex = 0;

            foreach (char c in ciphertext)
            {
                if (char.IsLetter(c))
                {
                    int cipherIndex = char.ToUpper(c) - 'A';
                    int shift = key[keyIndex % key.Length] - 'A';

                    int plainIndex = (cipherIndex - shift + 26) % 26;

                    char decryptedChar = (char)('A' + plainIndex);
                    plaintext.Append(char.IsLower(c) ? char.ToLower(decryptedChar) : decryptedChar);

                    keyIndex++;
                }
                else
                {
                    plaintext.Append(c);
                }
            }

            return plaintext.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selected_item = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selected_item))
            {
                if (comboBox1.SelectedItem == "Caesar")
                {
                    textBox3.Text = Encrypt_Caser(textBox2.Text, Convert.ToInt32(textBox1.Text));
                }
                else if (comboBox1.SelectedItem == "Monoalphabetic")
                {
                    textBox3.Text = EncryptMonoalphabetic(textBox2.Text);
                }
                else if (comboBox1.SelectedItem == "Polyalphabetic")
                {
                    string key = textBox1.Text.ToUpper(); // Ensure the key is in uppercase
                    string ciphertext = textBox2.Text;

                    string decryptedText = EncryptPolyalphabetic(ciphertext, key);
                    textBox3.Text = decryptedText;

                }

                else if (comboBox1.SelectedItem == "Playfair")
                {

                    string plaintext = textBox2.Text.ToUpper().Replace("J", "I"); // Replace 'J' with 'I' (Playfair rule)

                    string encryptedText = Encrypt_Playfair(plaintext);
                    textBox3.Text = encryptedText;
                }



            }
            else
            {
                MessageBox.Show("please Select item ");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string selected_item = comboBox1.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selected_item))
            {
                if (comboBox1.SelectedItem == "Caesar")
                {
                    string cipherText = textBox3.Text.Trim();
                    int shift;

                    if (!int.TryParse(textBox1.Text, out shift))
                    {
                        MessageBox.Show("Please enter a valid shift value.");
                        return;
                    }

                    textBox3.Text = DecryptCaesar(textBox2.Text, Convert.ToInt32(textBox1.Text));
                }
                else if (comboBox1.SelectedItem == "Monoalphabetic")
                {
                    string cipherText = textBox2.Text.Trim();
                    string decryptedText = DecryptMonoalphabetic(cipherText);
                    textBox3.Text = decryptedText;

                }
                else if (comboBox1.SelectedItem == "Polyalphabetic")
                {
                    string key = textBox1.Text.ToUpper(); // Ensure the key is in uppercase
                    string ciphertext = textBox2.Text;

                    string decryptedText = DecryptPolyalphabetic(ciphertext, key);
                    textBox3.Text = decryptedText;

                }

                else if (comboBox1.SelectedItem == "Playfair")
                {
                    string encryptedText = textBox2.Text.ToUpper();

                    string decryptedText = Decrypt_playfair(encryptedText);
                    textBox3.Text = decryptedText;
                }
            }
            else
            {
                MessageBox.Show("please Select item ");
            }
        }

        private Bitmap EncryptImage(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            Bitmap encrypted = new Bitmap(width, height);

            byte encryptionKey = 123;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    int red = pixel.R ^ encryptionKey;
                    int green = pixel.G ^ encryptionKey;
                    int blue = pixel.B ^ encryptionKey;

                    encrypted.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return encrypted;
        }
        private Bitmap DecryptImage(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            Bitmap decrypted = new Bitmap(width, height);

            byte encryptionKey = 123;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    int red = pixel.R ^ encryptionKey;
                    int green = pixel.G ^ encryptionKey;
                    int blue = pixel.B ^ encryptionKey;

                    decrypted.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            return decrypted;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp; *.jpg; *.png)|*.bmp;*.jpg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = originalImage;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                encryptedImage = EncryptImage(originalImage);
                pictureBox2.Image = encryptedImage;
            }
            else
            {
                MessageBox.Show("Please load an image first.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (encryptedImage != null)
            {
                decryptedImage = DecryptImage(encryptedImage);
                pictureBox2.Image = decryptedImage;
            }
            else
            {
                MessageBox.Show("Please encrypt an image first.");
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
           
        }
    }
}
