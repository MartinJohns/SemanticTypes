using System;

namespace SemanticTypes.SemanticTypes
{
    [Serializable]
    public struct SsrCode
    {
        public static readonly SsrCode None = default(SsrCode);

        public string Value => _value ?? string.Empty;

        public SsrCode(string ssrCode)
        {
            _value = ssrCode;
        }

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => Value.ToString();
        public bool Equals(SsrCode other) => _value == other._value;
        public static bool operator ==(SsrCode first, SsrCode second) => first.Equals(second);
        public static bool operator !=(SsrCode first, SsrCode second) => !(first == second);

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SsrCode))
                return false;

            var otherSsrCode = (SsrCode)obj;
            return Equals(otherSsrCode);
        }

        private readonly string _value;
    }
}
