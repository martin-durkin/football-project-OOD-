using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace football_project
{
    public partial class FavouritesWindow : Window
    {
        private List<FavouritePlayer> allSaved = new List<FavouritePlayer>();

        public FavouritesWindow()
        {
            InitializeComponent();
            LoadFromDatabase("All");
        }

        // LOAD FROM DATABASE
        private void LoadFromDatabase(string filter)
        {
            try
            {
                using (FootballDbContext db = new FootballDbContext())
                {
                    allSaved = db.FavouritePlayers
                        .OrderByDescending(p => p.DateAdded)
                        .ThenBy(p => p.Name)
                        .ToList();
                }

                ApplyLinqFilter(filter);
            }
            catch (Exception)
            {
                txtStatus.Text = "Error loading from database.";
            }
        }

        // LINQ FILTER BUTTONS
        private void btnLinqFilter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            ApplyLinqFilter(btn.Tag.ToString());
        }

        private void ApplyLinqFilter(string filter)
        {
            if (allSaved == null || allSaved.Count == 0)
            {
                txtStatus.Text = "No saved players found.";
                lbxSaved.ItemsSource = null;
                return;
            }

            IEnumerable<FavouritePlayer> result;

            switch (filter)
            {
                case "GK":
                case "DEF":
                case "MID":
                case "FWD":
                    result = allSaved.Where(p => p.Position == filter);
                    break;

                case "Week":
                    DateTime oneWeekAgo = DateTime.Now.AddDays(-7);
                    result = allSaved.Where(p => p.DateAdded >= oneWeekAgo);
                    break;

                default:
                    result = allSaved;
                    break;
            }

            List<FavouritePlayer> filtered = result.ToList();
            lbxSaved.ItemsSource = filtered;
            txtStatus.Text = filtered.Count + " player(s) shown  |  " + allSaved.Count + " total saved";
        }



        // DELETE FROM DATABASE
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            FavouritePlayer selected = lbxSaved.SelectedItem as FavouritePlayer;
            if (selected == null) return;

            var dlg = new DialogWindow(this, "Remove " + selected.Name + " from saved favourites?", true);
            dlg.ShowDialog();
            if (!dlg.Result) return;

            try
            {
                using (FootballDbContext db = new FootballDbContext())
                {
                    FavouritePlayer toDelete = db.FavouritePlayers
                        .FirstOrDefault(p => p.FavouritePlayerID == selected.FavouritePlayerID);

                    if (toDelete != null)
                    {
                        db.FavouritePlayers.Remove(toDelete);
                        db.SaveChanges();
                        LoadFromDatabase("All");
                    }
                }
            }
            catch (Exception)
            {
                txtStatus.Text = "Something went wrong deleting that player.";
            }
        }
    }
}