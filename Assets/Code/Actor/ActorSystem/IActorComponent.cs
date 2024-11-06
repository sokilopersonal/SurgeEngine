namespace SurgeEngine.Code.ActorSystem
{
    public interface IActorComponent
    {
        Actor actor { get; set; }

        void SetOwner(Actor actor)
        {
            this.actor = actor;
            OnInit();
        }

        void OnInit();
    }
}