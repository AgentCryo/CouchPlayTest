using Color = Raylib_cs.Color;
using CouchPlayTest.Utilities;

namespace CouchPlayTest.Games;

public class Squares(Player[] players) : Game(players, "Squares")
{    
    public override void Update(double delta)
    {
        foreach (var player in Players) {
            player.Update(delta);
            player.Transform.Position += player.GetInput();
        }
    }

    public override void Render()
    {
        foreach (var player in Players) {
            player.Render();
            player.DrawPlayer();
        }
        Program.LowRough.DrawStringCentered(10, "Welcome to Squares!", new Color(0, 255, 255));
    }
}