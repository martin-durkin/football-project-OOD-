using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace football_project
{
    public partial class MainWindow : Window
    {
        private APIService api = new APIService();
        private Random rand = new Random();
        private List<Player> currentSquad = new List<Player>();

        // ObservableCollection - UI updates automatically when items added/removed
        private ObservableCollection<Player> favourites = new ObservableCollection<Player>();

        private string currentFilter = "All";

        public MainWindow()
        {
            InitializeComponent();

            // Bind favourites ObservableCollection to listbox
            lbxSelectedPlayers.ItemsSource = favourites;

            string[] premierLeagueTeams =
            {
                "Arsenal",
                "Aston Villa",
                "Bournemouth",
                "Brentford",
                "Brighton & Hove Albion",
                "Chelsea",
                "Crystal Palace",
                "Everton",
                "Fulham",
                "Ipswich Town",
                "Leeds United",
                "Liverpool",
                "Manchester City",
                "Manchester United",
                "Newcastle United",
                "Nottingham Forest",
                "Southampton",
                "Tottenham Hotspur",
                "West Ham United",
                "Wolverhampton Wanderers"
            };

            lbxTeams.ItemsSource = premierLeagueTeams;
        }

        // TEAM SELECTION
        private async void lbxTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedTeamName = lbxTeams.SelectedItem as string;

            if (selectedTeamName == null) return;

            // Reset UI
            lbxPlayers.ItemsSource = null;
            currentSquad.Clear();
            btnAddRandomPlayer.IsEnabled = false;
            txtSearch.Text = "";
            currentFilter = "All";
            pnlManager.Visibility = Visibility.Collapsed;

            try
            {
                List<Team> foundTeams = await api.SearchTeamsAsync(selectedTeamName);

                if (foundTeams == null || foundTeams.Count == 0)
                {
                    ShowDialog("Could not find that team in the API.");
                    return;
                }

                Team matchedTeam = null;

                for (int i = 0; i < foundTeams.Count; i++)
                {
                    if (foundTeams[i].Name.ToLower() == selectedTeamName.ToLower())
                    {
                        if (foundTeams[i].Gender == "M")
                        {
                            matchedTeam = foundTeams[i];
                        }
                    }
                }

                if (matchedTeam == null)
                {
                    ShowDialog("Could not find the men's team.");
                    return;
                }

                // GetTeamSquadAsync returns both players and manager as a tuple
                var result = await api.GetTeamSquadAsync(matchedTeam.ID, matchedTeam.Name);

                currentSquad = result.Players;

                // Display manager card and team badge if data was returned
                if (result.Manager != null)
                {
                    txtManagerName.Text = result.Manager.Name;
                    txtManagerNationality.Text = result.Manager.Nationality;
                    pnlManager.Visibility = Visibility.Visible;
                }

                

                if (currentSquad == null || currentSquad.Count == 0)
                {
                    ShowDialog("No squad returned for this team.");
                    return;
                }

                // LINQ - sort by position order then name
                currentSquad = currentSquad
                    .OrderBy(p => PositionOrder(p.Position))
                    .ThenBy(p => p.Name)
                    .ToList();

                ApplyFilter();
                btnAddRandomPlayer.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowDialog("Error loading team: " + ex.Message);
            }
        }

        // FILTERING AND SEARCHING
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            currentFilter = btn.Tag.ToString();
            ApplyFilter();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (currentSquad == null || currentSquad.Count == 0)
            {
                lbxPlayers.ItemsSource = null;
                return;
            }

            string searchText = txtSearch.Text?.ToLower() ?? "";

            // LINQ - filter by position and search text
            IEnumerable<Player> filtered = currentSquad;

            if (currentFilter != "All")
            {
                filtered = filtered.Where(p => p.Position == currentFilter);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(p => p.Name.ToLower().Contains(searchText));
            }

            lbxPlayers.ItemsSource = filtered.ToList();
        }

        private int PositionOrder(string position)
        {
            switch (position)
            {
                case "GK":
                    return 1;
                case "DEF":
                    return 2;
                case "MID":
                    return 3;
                case "FWD":
                    return 4;
                default:
                    return 5;
            }
        }


        //Favourites Management
        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Player p = lbxPlayers.SelectedItem as Player;

                if (p == null)
                {
                    ShowDialog("Please select a player first.");
                    return;
                }

                if (PlayerAlreadySelected(p))
                {
                    ShowDialog("That player is already in your favourites.");
                    return;
                }

                favourites.Add(p);
            }
            catch (Exception ex)
            {
                ShowDialog("Error adding player: " + ex.Message);
            }
        }

        //selects a random player from the currently selected team
        //adds them to the selected players listbox
        private void btnAddRandomPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentSquad == null || currentSquad.Count == 0)
                {
                    ShowDialog("Please select a team first to load the squad.");
                    return;
                }

                int index = rand.Next(currentSquad.Count);
                Player randomPlayer = currentSquad[index];

                if (PlayerAlreadySelected(randomPlayer))
                {
                    ShowDialog("That player is already in your favourites.");
                    return;
                }

                favourites.Add(randomPlayer);
            }
            catch (Exception ex)
            {
                ShowDialog("Error adding random player: " + ex.Message);
            }
        }


        //removes the selected player from the selected players listbox

        private void btnRemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Player p = lbxSelectedPlayers.SelectedItem as Player;

                if (p == null)
                {
                    ShowDialog("Select a favourite player to remove.");
                    return;
                }

                favourites.Remove(p);
                ShowDialog(p.Name + " removed from favourites.");
            }
            catch (Exception ex)
            {
                ShowDialog("Error removing player: " + ex.Message);
            }
        }

        // DATABASE - STUBBED UNTIL EF CORE IS ADDED

        private void btnSaveFavourites_Click(object sender, RoutedEventArgs e)
        {
            if (favourites.Count == 0)
            {
                ShowDialog("Add at least 1 player to favourites before saving.");
                return;
            }

            try
            {
                using (FootballDbContext db = new FootballDbContext())
                {
                    int saved = 0;
                    int skipped = 0;

                    foreach (Player p in favourites)
                    {
                        // LINQ - check if already saved
                        bool exists = db.FavouritePlayers
                            .Any(x => x.PlayerID == p.ID && x.TeamID == p.TeamID);

                        if (!exists)
                        {
                            db.FavouritePlayers.Add(new FavouritePlayer
                            {
                                PlayerID = p.ID,
                                Name = p.Name,
                                Position = p.Position,
                                TeamName = p.TeamName,
                                TeamID = p.TeamID,
                                DateAdded = DateTime.Now
                            });
                            saved++;
                        }
                        else
                        {
                            skipped++;
                        }
                    }

                    db.SaveChanges();

                    string msg = saved + " player(s) saved.";
                    if (skipped > 0) msg += "\n" + skipped + " already existed and were skipped.";
                    ShowDialog(msg);
                }
            }
            catch (Exception ex)
            {
                ShowDialog("Error saving: " + ex.Message);
            }
        }
        

        // NAVIGATION
        private void btnViewSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FavouritesWindow win = new FavouritesWindow();
                win.Show();
            }
            catch (Exception ex)
            {
                ShowDialog("Error opening favourites window: " + ex.Message);
            }
        }

        // HELPERS
        private bool PlayerAlreadySelected(Player p)
        {
            // LINQ - check for duplicate by ID and TeamID
            return favourites.Any(existing => existing.ID == p.ID && existing.TeamID == p.TeamID);
        }

        //using dialog window instaed of message box
        private void ShowDialog(string message)
        {
            new DialogWindow(this, message).ShowDialog();
        }

        private bool ShowConfirm(string message)
        {
            var dlg = new DialogWindow(this, message, true);
            dlg.ShowDialog();
            return dlg.Result;
        }
    }
}