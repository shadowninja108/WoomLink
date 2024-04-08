namespace WoomLink.xlink2.User.Instance
{
    public class UserInstanceParam
    {
        public struct RandomEvent
        {
            public int OldIndex;
            public int NewIndex;
        }

        public class EmitterContainer
        {
            public int Field0;
            /* Emitter */
            public byte Field10;
        }

        public ModelAssetConnection[] Connections;
        public RandomEvent[] RandomHistory;
        public bool Setup;

        public EmitterContainer[] Emitters;
    }
}
