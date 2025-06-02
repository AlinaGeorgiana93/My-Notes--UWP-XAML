using System;

namespace My_Notes
{
    public class Note
    {
        public string Emoji { get; set; } = "📝";
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public string DateFormatted => Date.ToString("g");
    }
}
