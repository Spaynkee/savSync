using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net;
using FluentFTP;

namespace savSync
{
    struct game
    {

        public string gameName;
        public string gamePath;
    }

    public partial class Form1 : Form
    {
        List<game> gameList = new List<game>();
        string listPath = "C://list.txt";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string line;
            string listGameName;
            string listGamePath;


            //if file exists, then parse it. if it doesn't create it and move on.
            if (File.Exists(listPath) == true)
            {

                //Read through file, creating a gameList
                using (StreamReader sw = new StreamReader(listPath))
                {
                    while ((line = sw.ReadLine()) != null)
                    {
                        //skip any misshappen blank line
                        if (line == "")
                        {
                            continue;
                        }

                        int indexOfComma = line.IndexOf(",");
                        int lineLength = line.Length;

                        listGameName = line.Substring(0, indexOfComma);
                        listGamePath = line.Substring(indexOfComma+1, lineLength - indexOfComma-1);

                        game tempGame = new game();
                        tempGame.gameName = listGameName;
                        tempGame.gamePath = listGamePath;
                        gameList.Add(tempGame);
                    }
                }

                //Refresh the listbox with the fancy new gameList
                refreshListBox();
            }
            else
            {
                File.CreateText(listPath);
            }

            //Parse through each line of list file, 
        }

        public void refreshListBox()
        {
            //Clear the listbox
            lstGames.Items.Clear();

            for (int x = 0; x < gameList.Count(); x++)
            {
                lstGames.Items.Add(gameList[x].gameName);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Open a file browser,
            if (openGame.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openGame.FileName;
                int indexOfSlash = selectedFile.LastIndexOf("\\");

                string newGameName = selectedFile.Substring(indexOfSlash + 1, selectedFile.Length - indexOfSlash - 1);
                string newGamePath = selectedFile;

                //Create a new gameObject to store the name/path in so we can add to list
                game newGame = new game();

                newGame.gameName = newGameName;
                newGame.gamePath = newGamePath;
                gameList.Add(newGame);

                //Update the listFile
                using (StreamWriter sw = File.AppendText(listPath))
                {
                    sw.WriteLine(newGame.gameName + "," + newGame.gamePath);

                }
            }

            //refresh Listbox
            refreshListBox();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            //remove the selected item from the gameList
            gameList.RemoveAt(lstGames.SelectedIndex);

            File.Delete(listPath);

            //Write a new list file
            using (StreamWriter sw = File.AppendText(listPath))
            {
                for (int x = 0; x < gameList.Count; x++)
                {
                    sw.WriteLine(gameList[x].gameName + "," + gameList[x].gamePath);
                }

            }
            refreshListBox();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {

            //Here we get from the FTP server the filename
        }

        private void btnGet_Click_1(object sender, EventArgs e)
        {
            string downloadSavePath = gameList[lstGames.SelectedIndex].gamePath;
            string downloadSaveName = gameList[lstGames.SelectedIndex].gameName;

            FtpClient client = new FtpClient("192.168.1.3");

            client.Credentials = new NetworkCredential("kitchen", "lalala22");

            client.Connect();
            //if the file exists on the ftp server, download it to the path recorded in the list
            if (client.FileExists("//Desktop//FTP//"+downloadSaveName))
            {
                bool success = client.DownloadFile(downloadSavePath, "//Desktop//FTP//"+downloadSaveName);

                if (success == true)
                {
                    MessageBox.Show("It success");
                }
                else
                {
                    MessageBox.Show("It failure");
                }
            }

            client.Disconnect();

        }

        private void btnPut_Click(object sender, EventArgs e)
        {

            //gets the file we're gonna ftp
            string uploadSavePath = gameList[lstGames.SelectedIndex].gamePath;
            string uploadSaveName = gameList[lstGames.SelectedIndex].gameName;

            FtpClient client = new FtpClient("192.168.1.3");

            client.Credentials = new NetworkCredential("kitchen", "lalala22");

            client.Connect();

            bool success = client.UploadFile(uploadSavePath, "//Desktop//FTP//"+uploadSaveName, FtpExists.Overwrite);
            if (success == true)
            {
                MessageBox.Show("It success");
            }
            else
            {
                MessageBox.Show("It failure");
            }
            client.Disconnect();
        }
    }
    }
