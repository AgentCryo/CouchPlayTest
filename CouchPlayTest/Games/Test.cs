using Color = Raylib_cs.Color;
using CouchPlayTest.Utilities;

namespace CouchPlayTest.Games;

public class Test(Player[] players) : Game(players, "Test")
{    
    public override void Update(double delta)
    {
        foreach (var player in Players) {
            player.Update(delta);
        }
    }

    public override void Render()
    {
        foreach (var player in Players) {
            player.Render();
        }
        Program.LowRough.DrawStringCentered(10, "Welcome to Test!", new Color(0, 255, 255));
    }
}