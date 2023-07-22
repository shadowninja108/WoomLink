namespace WoomLink.xlink2
{
    class SystemELink : System
    {
        public static SystemELink Instance { get; set; }
        public static ILockProxy Lock { get; set; }

        public void Initialize(
            /* sead::ptcl::PtclSystem* */
            /* heap for initialization */
            /* heap for continual use */
            uint eventPoolNum,
            ILockProxy loc = null)
        {
            InitSystem_(eventPoolNum);

            /* Initialize state from PctlSystem */

            Lock = loc ?? new LockProxyForMutex();

            /* Initialize event pool and container/asset executor heaps. */
        }

        public UserInstanceELink CreateUserInstance(UserInstance.CreateArg arg /* heap */, uint unk)
        {

            UserInstanceELink instance = null;
            Lock.Lock();
            var user = SearchUserOrCreate(arg, unk);
            if (user != null)
            {
                instance = new UserInstanceELink(arg, this, user);
                user.Instances.Add(instance);
            }
            Lock.Unlock();
            return instance;
        }

        public override UserResource CreateUserResource(User user)
        {
            return new UserResourceSLink(user);
        }

        public override uint GetUserParamNum() => 0;
        public override string GetModuleName() => "ELink2";

        public override ILockProxy GetModuleLockObj() => Lock;

        public override int GetResourceVersion()
        {
#if XLINK_VER_BLITZ
            return 30;
#elif XLINK_VER_THUNDER
            return 34;
#endif
        }

        public static SystemELink GetInstance()
        {
            return Instance ??= new SystemELink();
        }
    }
}
