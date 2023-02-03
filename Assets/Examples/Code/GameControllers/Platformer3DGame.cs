public class Platformer3DGame : Game
{
    public void Awake()
    {
        engines = new Engine[] {
            new CharacterControllerPlatformer3DEngine(),
            new CameraPlatformer3DEngine(),
            new VelocityEngine(),
        };
    }
}