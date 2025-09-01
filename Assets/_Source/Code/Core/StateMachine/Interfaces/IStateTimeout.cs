namespace SurgeEngine._Source.Code.Core.StateMachine.Interfaces
{
    /// <summary>
    /// Interface to add a delay to states. Set Timeout as the delay time when needed.
    /// </summary>
    public interface IStateTimeout
    {
        float Timeout { get; set; }

        void Tick(float dt)
        {
            Timeout -= dt;
            
            if (Timeout <= 0f)
            {
                Timeout = 0f;
            }
        }
    }
}