using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyNotesApp
{
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        private List<Note> notes = new List<Note>();

        public MainPage()
        {
            this.InitializeComponent();

            // Load theme from localSettings or default to system
            if (localSettings.Values.TryGetValue("AppTheme", out object savedTheme))
            {
                var theme = (string)savedTheme;
                if (theme == "Dark")
                {
                    this.RequestedTheme = ElementTheme.Dark;
                    ThemeToggle.IsChecked = true;
                }
                else
                {
                    this.RequestedTheme = ElementTheme.Light;
                    ThemeToggle.IsChecked = false;
                }
            }
            else
            {
                // Default to system theme
                this.RequestedTheme = ElementTheme.Default;
            }

            LoadNotes();
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            string noteText = NoteInput.Text.Trim();
            if (!string.IsNullOrEmpty(noteText))
            {
                notes.Add(new Note
                {
                    Emoji = "📝",
                    Text = noteText,
                    Date = DateTime.Now
                });

                SaveToSettings();
                LoadNotes();
                NoteInput.Text = "";
            }
        }

        private void LoadNotes()
        {
            var noteBackground = (Windows.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["NoteBackgroundBrush"];
            var noteBorder = (Windows.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["NoteBorderBrush"];
            var deleteButtonBrush = (Windows.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["DeleteButtonBrush"];
            var dateTextBrush = (Windows.UI.Xaml.Media.SolidColorBrush)Application.Current.Resources["DateTextBrush"];

            NotesList.Items.Clear();
            notes = LoadFromSettings();

            for (int i = 0; i < notes.Count; i++)
            {
                Note note = notes[i];

                StackPanel panel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 5),
                    Background = noteBackground,
                    BorderThickness = new Thickness(1),
                    BorderBrush = noteBorder,
                    Padding = new Thickness(10),
                    CornerRadius = new Windows.UI.Xaml.CornerRadius(5),
                };

                TextBlock emojiText = new TextBlock
                {
                    Text = note.Emoji,
                    FontSize = 24,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                StackPanel textPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Width = 300
                };

                TextBlock noteText = new TextBlock
                {
                    Text = note.Text,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 16
                };

                TextBlock dateText = new TextBlock
                {
                    Text = note.DateFormatted,
                    FontSize = 12,
                    Foreground = dateTextBrush
                };

                textPanel.Children.Add(noteText);
                textPanel.Children.Add(dateText);

                Button deleteButton = new Button
                {
                    Content = "🗑️",
                    Tag = i,
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Transparent),
                    BorderThickness = new Thickness(0),
                    FontSize = 20,
                    Width = 40,
                    Height = 40,
                    Foreground = deleteButtonBrush
                };
                deleteButton.Click += DeleteButton_Click;

                panel.Children.Add(emojiText);
                panel.Children.Add(textPanel);
                panel.Children.Add(deleteButton);

                NotesList.Items.Add(panel);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                if (index >= 0 && index < notes.Count)
                {
                    notes.RemoveAt(index);
                    SaveToSettings();
                    LoadNotes();
                }
            }
        }

        private List<Note> LoadFromSettings()
        {
            object saved = localSettings.Values["notes"];
            if (saved is string json)
            {
                try
                {
                    return JsonConvert.DeserializeObject<List<Note>>(json) ?? new List<Note>();
                }
                catch
                {
                    return new List<Note>();
                }
            }
            return new List<Note>();
        }

        private void SaveToSettings()
        {
            string json = JsonConvert.SerializeObject(notes, Formatting.Indented);
            localSettings.Values["notes"] = json;
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
            localSettings.Values["AppTheme"] = "Dark";
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Light;
            localSettings.Values["AppTheme"] = "Light";
        }
    }

    public class Note
    {
        public string Emoji { get; set; } = "📝";
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public string DateFormatted => Date.ToString("g");
    }
}
