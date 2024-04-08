namespace WoomLink.sead
{
    public class Arena
    {
        public byte[]? Data = null;
        public bool Field10 = false;

        public void Initialize(SizeT size)
        {
            Data = new byte[size];
        }

        public void Destroy()
        {
            if (!Field10)
            {
                Data = null;
            }
            Field10 = false;
        }
    }
}
