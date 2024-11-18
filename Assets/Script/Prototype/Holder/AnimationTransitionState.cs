using System;
using Unity.Netcode;

public class AnimationTransitionState : INetworkSerializable, IEquatable<AnimationTransitionState>
{
    public string stateName;
    public int stateIndex;

    public bool Equals(AnimationTransitionState other)
    {
        return stateName.Equals(other.stateName) && stateIndex == other.stateIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(stateName);
            writer.WriteValueSafe(stateIndex);
        } 
        else
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out stateName);
            reader.ReadValueSafe(out stateIndex);
        }
    }
}