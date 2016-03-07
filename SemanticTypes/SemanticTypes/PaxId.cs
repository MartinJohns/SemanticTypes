using System;

namespace SemanticTypes
{
    [Serializable]
    public class PaxId
    {
        public static readonly PaxId None = default(PaxId);

        public int Value => _value ?? -1;

        public PaxId(int paxId)
        {
            if (paxId < 0)
                throw new ArgumentException("Id must be larger or equal 0.", nameof(paxId));

            _value = paxId;
        }

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => Value.ToString();
        public bool Equals(PaxId other) => _value == other?._value;
        public static bool operator==(PaxId first, PaxId second) => first.Equals(second);
        public static bool operator!=(PaxId first, PaxId second) => !(first == second);

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(PaxId))
                return false;

            var otherPaxId = (PaxId)obj;
            return Equals(otherPaxId);
        }

        private readonly int? _value;
    }
}
