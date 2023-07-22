namespace WoomLink.xlink2
{
    public class SystemSLink : System
    {
        public static SystemSLink Instance { get; set; }
        public static ILockProxy Lock { get; set; }

        public void Initialize(
            /* aal::System* */
            /* heap for initialization */
            /* heap for continual use */
            uint eventPoolNum,
            ILockProxy loc = null
        )
        {
            InitSystem_(eventPoolNum);
            Lock = loc ?? new LockProxyForMutex();
            
            /* Initialize event pool and container/asset executor heaps. */
        }

        public UserInstanceSLink CreateUserInstance(UserInstanceSLink.CreateArgSLink arg /* heap */, uint unk)
        {

            UserInstanceSLink instance = null;
            Lock.Lock();
            var user = SearchUserOrCreate(arg, unk);
            if (user != null)
            {
                instance = new UserInstanceSLink(arg, this, user);
                user.Instances.Add(instance);
            }
            Lock.Unlock();
            return instance;
        }

        public override UserResource CreateUserResource(User user)
        {
            return new UserResourceELink(user);
        }

        public override uint GetUserParamNum() => 8;
        public override string GetModuleName() => "SLink2";

        public override ILockProxy GetModuleLockObj() => Lock;

        public override int GetResourceVersion()
        {
#if XLINK_VER_BLITZ
            return 28;
#elif XLINK_VER_THUNDER
            return 31;
#endif
        }

        public static SystemSLink GetInstance()
        {
            return Instance ??= new SystemSLink();
        }
    }
}
