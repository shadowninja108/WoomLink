using NotImplementedException = System.NotImplementedException;

namespace WoomLink.xlink2
{
    public class UserResourceELink : UserResource
    {
        public ResourceAccessorELink Accessor;
        public UserResourceELink(User user) : base(user)
        {
            Accessor = new ResourceAccessorELink(this, SystemELink.Instance);
        }

        public override ResourceAccessor GetAccessor() => Accessor;

        public override ResourceAccessor GetAccessorPtr() => Accessor;
        public override System GetSystem() => SystemELink.Instance;
        public override UserResourceParam AllocResourceParam()
        {
            throw new NotImplementedException();
        }
    }
}
