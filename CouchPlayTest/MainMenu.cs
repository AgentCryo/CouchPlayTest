using CouchPlayTest.Drawing;
using CouchPlayTest.Games;
using CouchPlayTest.Utilities;
using CouchPlayTest.Players;
using CouchPlayTest.Utilities.UI;
using Raylib_cs;
using static CouchPlayTest.Utilities.UI.UiInteractable;

namespace CouchPlayTest;

public class MainMenu
{
    const int VotePoolWidth = 20;
    const int VotePoolHeight = 60;
    const int VotePoolYPos = 112;
    readonly int _numberOfPools = GameTypes.Length;

    int _peopleConnected;
    public static readonly List<Player> Players = [];

    static readonly Type[] GameTypes = [ typeof(Squares), typeof(Test)];
    static Game? _selectedGame = null;
    static double _gameCountDown = 0;
    const int CountDownSeconds = 2;
    static int _playersVoted = 0;
    
    public static class GlobalUi
    {
        public static Dictionary<string, UiInteractable> Ui = new();
    }

    public void Update(double delta)
    {
        if (_selectedGame != null) {
            if (SideMenu.Update(delta, true) || SideMenu.IsOpen) return;
            _selectedGame.Update(delta);
            return;
        }
        if (SideMenu.Update(delta, false) || SideMenu.IsOpen) return;
        
        if (Raylib.IsKeyPressed(KeyboardKey.W) && Players.All(p => p is not Wasd)) Players.Add(new Wasd());
        if(Raylib.IsKeyPressed(KeyboardKey.Up) && Players.All(p => p is not Arrows)) Players.Add(new Arrows());
        if((Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceUp) || (Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY) < -0.5f )) && Players.All(p => p is not Controller)) Players.Add(new Controller());
        _peopleConnected = Players.Count;

        _playersVoted = 0; 
        foreach (var player in Players) {
            PlayerMenuController(player, delta);
        }
        _playersVoted = Players.Count(p => p.Voted != -1);

        var allVoted = Players.Count > 0 && _playersVoted == Players.Count;
        _gameCountDown = allVoted ? _gameCountDown + delta : 0;

        if (!(_gameCountDown >= CountDownSeconds)) return;
        
        var votes = new int[GameTypes.Length];
        foreach (var player in Players) {
            votes[player.Voted]++;
            player.Voted = -1;
        }
        var mostVotedIndex = Array.IndexOf(votes, votes.Max());
        
        _selectedGame = Activator.CreateInstance(GameTypes[mostVotedIndex], [Players.ToArray()]) as Game;
        _gameCountDown = 0;
        _playersVoted = 0;
        SideMenu.IsOpen = false;
    }
    public void Render()
    {
        if (_selectedGame != null) {
            _selectedGame.Render();
            SideMenu.RenderSideMenu(_selectedGame.GameName, true);
        }
        else {
            RenderMainMenu();
            SideMenu.RenderSideMenu("Main Menu", false);
        }
    }

    void PlayerMenuController(Player player, double deltaTime)
    {
        player.Ui.TryAdd("VotingToggle", new UiInteractable());
        player.Ui.TryAdd("VotingSelection", new UiInteractable());
        
        var votingToggle = player.Ui["VotingToggle"].UpdateInputToggle(player, deltaTime);

        player.Voted = votingToggle switch
        {
            { Triggered: true, Value: 1 } => player.MenuVotePoolIndex,
            { Triggered: true, Value: 0 } => -1,
            _ => player.Voted
        };

        var votingSelection = player.Ui["VotingSelection"].UpdateInputSelection(player, deltaTime, UiAxis.X, (0, _numberOfPools-1), player.MenuVotePoolIndex, true);

        if (votingSelection.Triggered) {
            player.MenuVotePoolIndex = (int)votingSelection.Value;
        }
    }
    
    void RenderMainMenu()
    {
        Program.LowRough.DrawStringCentered(70, "Input UP to connect.", Color.White);
        Program.LowRough.DrawStringCentered(80, "Players Connected: " + _peopleConnected, Color.White);
        Program.TitleRough.DrawStringCentered(90, Program.Title, new Color(160, 255, 255, 255));

        Program.LowRough.DrawString(10, 200, $"Players Voted: {_playersVoted}", Color.White);

        Program.LowRough.DrawString(10, 10, "Controllers Connected:", Color.White);
        if (Players.Any(p => p is Wasd)) 
            Program.LowRough.DrawString(10, 18, "WASD", Color.White);
        if (Players.Any(p => p is Arrows)) 
            Program.LowRough.DrawString(10, 26, "Arrows", Color.White);
        if (Players.Any(p => p is Controller)) 
            Program.LowRough.DrawString(10, 34, "Controller", Color.White);

        for (var g = 0; g < GameTypes.Length; g++)
            DrawVotePool(g, GameTypes[g].Name, Color.White);

        for (var p = 0; p < Players.Count; p++)
            DrawPoolVote(Players[p].MenuVotePoolIndex, p, Players[p].Color);

        if (_playersVoted != Players.Count || _playersVoted == 0)
            return;

        DrawingUtility.DrawRectangle(0, Program.ScreenSize / 2 - 40, Program.ScreenSize, 40, new Color(150,150,150,255));
        Program.LowRough.DrawStringCentered(Program.ScreenSize / 2 - 33, "All players have voted.", Color.White);
        Program.LowRough.DrawStringCentered(Program.ScreenSize / 2 - 18, "Game Starting in " + (CountDownSeconds - _gameCountDown).ToString("0.0") + " seconds.", Color.White);
    }

    public static void ReturnToMainMenu()
    {
        SideMenu.IsOpen = false;
        _selectedGame = null;
        
        foreach (var ui in Players.SelectMany(player => player.Ui.Values)) {
            ui.Reset();
        }
    }
    
    void DrawVotePool(int poolIndex, string gameName, Color color)
    {
        int x = (int)Math.Round(Program.ScreenSize / (float)(_numberOfPools + 1) * (poolIndex + 1) - VotePoolWidth / 2f);
        DrawingUtility.DrawRectangle(x, VotePoolYPos, VotePoolWidth, VotePoolHeight + 2, color);
        DrawingUtility.DrawRectangle(x + 1, VotePoolYPos + 1, VotePoolWidth - 2, VotePoolHeight, Color.Black);
        Program.LowRough.DrawString(x + (VotePoolWidth - ((Program.LowRough.FontData.dimensions[0] * gameName.Length) + (gameName.Length - 1))) / 2, VotePoolYPos + VotePoolHeight + 4, gameName, Color.White);
    }

    void DrawPoolVote(int poolIndex, int voteIndex, Color color)
    {
        if (_peopleConnected == 0) return;
        int x = (int)Math.Round(Program.ScreenSize / (float)(_numberOfPools + 1) * (poolIndex + 1) - VotePoolWidth / 2f);
        DrawingUtility.DrawRectangle(x + 1, VotePoolYPos + 1 + VotePoolHeight/_peopleConnected * voteIndex, VotePoolWidth - 2, (VotePoolHeight/_peopleConnected), color);
    }
}