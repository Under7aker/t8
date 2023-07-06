using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace t8
{
    public partial class Form1 : Form
    {
        private const string apiUrl = "http://api.tvmaze.com/search/shows";
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                try
                {
                    // Blocăm butonul pentru a preveni căutări multiple simultane
                    button1.Enabled = false;

                    // Apelăm căutarea într-un fir de execuție separat
                    ShowResult showResult = await Task.Run(() => SearchShow(searchText));

                    // Verificăm dacă s-a găsit un rezultat valid
                    if (showResult != null)
                    {
                        // Formatăm informațiile despre show
                        string showInfo = FormatShowInfo(showResult);

                        // Actualizăm textBox1 cu informațiile formatate
                        textBox1.Text = showInfo;
                    }
                    else
                    {
                        textBox1.Text = "Nu s-a găsit niciun rezultat pentru căutarea specificată.";
                    }
                }
                catch (Exception ex)
                {
                    textBox1.Text = "A apărut o eroare în timpul căutării: " + ex.Message;
                }
                finally
                {
                    // Deblocăm butonul
                    button1.Enabled = true;
                }
            }
            else
            {
                textBox1.Text = "Introduceți un text pentru căutare.";
            }
        }

        private async Task<ShowResult> SearchShow(string searchText)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{apiUrl}?q={searchText}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ShowResult[] results = JsonConvert.DeserializeObject<ShowResult[]>(json);
                    return results?.FirstOrDefault();
                }
                else
                {
                    throw new Exception("A apărut o eroare în timpul căutării.");
                }
            }
        }

        private string FormatShowInfo(ShowResult showResult)
        {
            StringBuilder sb = new StringBuilder();

            Show show = showResult.Show;

            //sb.AppendLine("ID: " + showResult.Id);
            //sb.AppendLine("URL: " + showResult.Url);

            if (show != null)
            {
                sb.AppendLine("Nume: " + show.Name);
                sb.AppendLine("Tip: " + TranslateType(show.Type));
                sb.AppendLine("Limba: " + show.Language);

                if (show.Genres != null && show.Genres.Any())
                {
                    sb.AppendLine("Genuri: " + TranslateGenres(show.Genres));
                }

                sb.AppendLine("Status: " + TranslateStatus(show.Status));
                sb.AppendLine("Durată: " + show.Runtime);

                sb.AppendLine("Premieră: " + show.Premiered);
                sb.AppendLine("Încheiat: " + show.Ended);
                sb.AppendLine("Site oficial: " + show.OfficialSite);

                if (show.Schedule != null)
                {
                    sb.AppendLine("Program: ");
                    sb.AppendLine("Ora: " + show.Schedule.Time);
                    sb.AppendLine("Zile: " + TranslateDays(show.Schedule.Days));
                }

                if (show.Rating != null)
                {
                    sb.AppendLine("Rating: " + show.Rating.Average);
                }

                if (show.Network != null)
                {
                    sb.AppendLine("Rețea: " + show.Network.Name);
                }

                if (show.WebChannel != null)
                {
                    sb.AppendLine("Canal web: " + show.WebChannel.Name);
                  //  sb.AppendLine("Țară: " + show.WebChannel.Country.Name);
                    sb.AppendLine("Site oficial: " + show.WebChannel.OfficialSite);
                }


            }

            return sb.ToString();
        }

        private string TranslateType(string type)
        {
            // Mapare între tipurile de seriale în engleză și traducerile în română
            Dictionary<string, string> typeTranslations = new Dictionary<string, string>()
    {
        { "Scripted", "Scenarizat" },
        { "Reality", "Realitate" },
        { "Animation", "Animație" },
        // Adăugați alte tipuri și traduceri după necesitate
    };

            if (typeTranslations.ContainsKey(type))
            {
                return typeTranslations[type];
            }

            return type;
        }

        private string TranslateGenres(List<string> genres)
        {
            // Mapare între genurile de seriale în engleză și traducerile în română
            Dictionary<string, string> genreTranslations = new Dictionary<string, string>()
    {
        { "Comedy", "Comedie" },
        { "Drama", "Dramă" },
        { "Action", "Acțiune" },
        // Adăugați alte genuri și traduceri după necesitate
    };

            List<string> translatedGenres = new List<string>();

            foreach (var genre in genres)
            {
                if (genreTranslations.ContainsKey(genre))
                {
                    translatedGenres.Add(genreTranslations[genre]);
                }
                else
                {
                    translatedGenres.Add(genre);
                }
            }

            return string.Join(", ", translatedGenres);
        }

        private string TranslateStatus(string status)
        {
            // Mapare între statusurile serialelor în engleză și traducerile în română
            Dictionary<string, string> statusTranslations = new Dictionary<string, string>()
    {
        { "Running", "În desfășurare" },
        { "Ended", "Încheiat" },
        { "To Be Determined", "De determinat" },
        // Adăugați alte statusuri și traduceri după necesitate
    };

            if (statusTranslations.ContainsKey(status))
            {
                return statusTranslations[status];
            }

            return status;
        }

        private string TranslateDays(List<string> days)
        {
            // Mapare între zilele săptămânii în engleză și traducerile în română
            Dictionary<string, string> dayTranslations = new Dictionary<string, string>()
    {
        { "Monday", "Luni" },
        { "Tuesday", "Marți" },
        { "Wednesday", "Miercuri" },
        // Adăugați alte zile și traduceri după necesitate
    };

            List<string> translatedDays = new List<string>();

            foreach (var day in days)
            {
                if (dayTranslations.ContainsKey(day))
                {
                    translatedDays.Add(dayTranslations[day]);
                }
                else
                {
                    translatedDays.Add(day);
                }
            }

            return string.Join(", ", translatedDays);
        }

        public class ShowResult
        {
            public int Id { get; set; }
            public string Url { get; set; }
            public Show Show { get; set; }
        }

        public class Show
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Language { get; set; }
            public List<string> Genres { get; set; }
            public string Status { get; set; }
            public int? Runtime { get; set; }

            public string Premiered { get; set; }
            public string Ended { get; set; }
            public string OfficialSite { get; set; }
            public Schedule Schedule { get; set; }
            public Rating Rating { get; set; }
            public Network Network { get; set; }
            public WebChannel WebChannel { get; set; }

        }

        public class Schedule
        {
            public string Time { get; set; }
            public List<string> Days { get; set; }
        }

        public class Rating
        {
            public double? Average { get; set; }
        }

        public class Network
        {
            public string Name { get; set; }
        }

        public class WebChannel
        {
            public string Name { get; set; }
            public Country Country { get; set; }
            public string OfficialSite { get; set; }
        }

        public class Country
        {
            public string Name { get; set; }
        }
    }
}


