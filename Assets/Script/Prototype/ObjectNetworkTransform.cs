using Unity.Netcode.Components;

namespace Game
{
    public class ObjectNetworkTransform : NetworkTransform
    {
        protected override void OnAuthorityPushTransformState(ref NetworkTransformState networkTransformState)
        {
            base.OnAuthorityPushTransformState(ref networkTransformState);
            print("OnAuthorityPushTransformState");
            //////
        }
    }
}