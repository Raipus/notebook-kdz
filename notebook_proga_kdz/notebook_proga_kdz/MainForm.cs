using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace notebook_proga_kdz
{
    public partial class MainForm : Form
    {
        private List<Note> notes;

        public MainForm()
        {
            InitializeComponent();
            notes = new List<Note>();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadNotesFromFile();
            RefreshNotesList();
            UpdateChartData();
        }

        private void AddNoteButton_Click(object sender, EventArgs e)
        {
            string title = titleTextBox.Text;
            string text = contentTextBox.Text;
            DateTime timestamp = DateTime.Now;

            Note newNote = new Note(title, text, timestamp);
            notes.Add(newNote);

            ClearFields();
            RefreshNotesList();
            UpdateChartData();
            SaveNotesToFile();
        }

        private void EditNoteButton_Click(object sender, EventArgs e)
        {
            if (notesListBox.SelectedItem != null)
            {
                Note selectedNote = (Note)notesListBox.SelectedItem;

                string newTitle = titleTextBox.Text;
                string newText = contentTextBox.Text;

                selectedNote.Title = newTitle;
                selectedNote.Text = newText;

                ClearFields();
                RefreshNotesList();
                UpdateChartData();
                SaveNotesToFile();
            }
        }

        private void DeleteNoteButton_Click(object sender, EventArgs e)
        {
            if (notesListBox.SelectedItem != null)
            {
                Note selectedNote = (Note)notesListBox.SelectedItem;
                notes.Remove(selectedNote);

                ClearFields();
                RefreshNotesList();
                UpdateChartData();
                SaveNotesToFile();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string keyword = searchTextBox.Text.ToLower();

            IEnumerable<Note> searchResults = notes.Where(note =>
                note.Title.ToLower().Contains(keyword) ||
                note.Text.ToLower().Contains(keyword) ||
                note.Timestamp.ToString().Contains(keyword));

            RefreshNotesList(searchResults);
        }

        private void RefreshNotesList(IEnumerable<Note> noteList = null)
        {
            notesListBox.Items.Clear();

            IEnumerable<Note> displayNotes = noteList ?? notes;

            foreach (Note note in displayNotes)
            {
                notesListBox.Items.Add(note);
            }
        }

        private void ClearFields()
        {
            titleTextBox.Text = "";
            contentTextBox.Text = "";
        }

        private void SaveToFileButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON Files (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string json = JsonConvert.SerializeObject(notes);
                    File.WriteAllText(saveFileDialog.FileName, json);
                    MessageBox.Show("Записи сохранены в файл.");
                }
            }
        }

        private void LoadFromFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    notes = JsonConvert.DeserializeObject<List<Note>>(json);
                    RefreshNotesList();
                    UpdateChartData();
                    MessageBox.Show("Записи загружены из файла.");
                }
            }
        }

        private void LoadNotesFromFile()
        {
            string projectPath = Path.GetDirectoryName(Application.ExecutablePath);
            string filePath = Path.Combine(projectPath, "notes.json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                notes = JsonConvert.DeserializeObject<List<Note>>(json);
            }
        }
        private void SaveNotesToFile()
        {
            string projectPath = Path.GetDirectoryName(Application.ExecutablePath);
            string filePath = Path.Combine(projectPath, "notes.json");

            if (File.Exists(filePath))
            {
                string json = JsonConvert.SerializeObject(notes);
                File.WriteAllText(filePath, json);
            }
            else
            {
                var stream = File.Create(filePath);
                stream.Close();
                string json = JsonConvert.SerializeObject(notes);
                File.WriteAllText(filePath, json);
            }
        }

        private void UpdateChartData()
        {
            var data = notes.GroupBy(note => note.Timestamp.Date).Select(group => new { Date = group.Key, Count = group.Count() }).OrderBy(item => item.Date);

            var xData = data.Select(item => item.Count).ToArray();
            var yData = data.Select(item => item.Date).ToArray();

            chart1.Series[0].Points.DataBindXY(yData, xData);
        }
    }
}