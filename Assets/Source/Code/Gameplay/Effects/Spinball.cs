namespace SurgeEngine.Source.Code.Gameplay.Effects
{
    public class Spinball : Effect
    {
        public override void Toggle(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}