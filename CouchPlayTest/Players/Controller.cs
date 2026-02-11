using System.Numerics;
using CouchPlayTest.Utilities;
using Raylib_cs;
using Transform = CouchPlayTest.Utilities.Transform;

namespace CouchPlayTest.Players;

public class Controller() : Player(Color.Blue)
{
    public override Vector2 GetInput()
    {
        float x = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftX);
        float y = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY);

        return Utility.ClampMagnitude(new Vector2(x, y), 1);
    }

    public override bool GetSpecialInput()
    {
        return Raylib.IsGamepadButtonDown(0, GamepadButton.RightFaceDown);
    }
}