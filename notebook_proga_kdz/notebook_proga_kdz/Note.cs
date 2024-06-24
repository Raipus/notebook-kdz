using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notebook_proga_kdz
{
    public class Note
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }

        public Note()
        {
        }

        public Note(string title, string text, DateTime timestamp)
        {
            Title = title;
            Text = text;
            Timestamp = timestamp;
        }

        public override string ToString()
        {
            return $"{Title} - {Timestamp} - {Text}";
        }
    }
}
