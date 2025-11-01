using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using BoreGame.BoreConectors;

namespace BoreGame;

public partial class BoreMainWindow : Window
{
    private static readonly string profilesPath = "Profiles";
    
    private ProfileManager ProfileManager = new ProfileManager(profilesPath);
    private BoreClassic BoreClassic = new BoreClassic("Bores/bore");
    public BoreMainWindow()
    {
        InitializeComponent();

        SetProfiles();
    }

    private void SetProfiles()
    {
        foreach (var profile in ProfileManager.GetProfileNames())
        {
            ProfilesListBox.Items.Add(profile);
        }
        
    }

    private void ProfilesListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var profileName = ProfilesListBox.SelectedIndex;
        
        Console.WriteLine(profileName.ToString());

        var profilePatch = ProfileManager.GetProfilePath(ProfilesListBox.SelectedItem.ToString());
        
        
         var base64String =  File.ReadAllText(profilePatch);
         var profile =  JsonSerializer.Deserialize<BoreGameObjects.ProfileObject>(base64String);

        var bitmap = ProfileManager.FromBase64(profile.BackgroundImageBase64);
        
        
        BackgroundImage.Source = bitmap;

        PortTextBox.Text = profile.Port;


    }

    private void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        //throw new System.NotImplementedException();
    }

    private async void CopyButton_Click(object? sender, RoutedEventArgs e)
    {
        var clipboard = GetTopLevel(this)?.Clipboard;
        if (clipboard!= null)
        {
            await clipboard.SetTextAsync(IpTextBox.Text ?? "");
            
            // Визуальная обратная связь
            var originalContent = CopyButton.Content;
            CopyButton.Content = "✓ Скопировано!";
                
            await System.Threading.Tasks.Task.Delay(2000);
            CopyButton.Content = originalContent;
        }
    }

    private void ServerComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {

        
        string json = JsonSerializer.Serialize(new BoreGameObjects.ProfileObject("bore.pub", "25565", BoreGameObjects.EBoreGameType.BoreClassic,""));
        Console.WriteLine(json);
       //File.WriteAllText("Minecraft.json",json);
        //JsonSerializer.Deserialize<BoreGameObjects.ProfileObject>(json);
   
    }

    void changeButtonOpendeContent(bool status)
    {
        if (status==false)
        {
            ButtonOpenPort.Content = "🔓 Открыть порт";
            IpTextBox.Text = "-";
        }
        else
        {
            ButtonOpenPort.Content = "🔓 Порт открыт";
        }
        
    }
    private bool _isPortOpen = false;
    private async void ButtonOpenPort_Click(object? sender, RoutedEventArgs e)
    {
        BoreClassic.StopOpened();

        if (_isPortOpen==false)
        {
            _isPortOpen = true;
            changeButtonOpendeContent(true);
            var tunnelUrl = BoreClassic.OpenPort(PortTextBox.Text, BoreClassic.ServerDefault);
            
            IpTextBox.Text = tunnelUrl;
            
            while (BoreClassic.IsRuned())
            {
                await Task.Delay(2000);
            }

            changeButtonOpendeContent(false);
            _isPortOpen = false;
            
        }
        else
        {
            _isPortOpen = false;
            changeButtonOpendeContent(false);
          
        }

    
      
        
       

       

        

    }
}