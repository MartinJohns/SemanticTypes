using System;

namespace SemanticTypes.SemanticTypes
{

    [Serializable]
    public struct SsrGroupCode
    {
        public static readonly SsrGroupCode None = default(SsrGroupCode);

        public string Value => _value ?? string.Empty;

        public SsrGroupCode(string ssrGroupCode)
        {
            _value = ssrGroupCode;
        }

        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => Value.ToString();
        public bool Equals(SsrGroupCode other) => _value == other._value;
        public static bool operator ==(SsrGroupCode first, SsrGroupCode second) => first.Equals(second);
        public static bool operator !=(SsrGroupCode first, SsrGroupCode second) => !(first == second);

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(SsrGroupCode))
                return false;

            var otherSsrGroupCode = (SsrGroupCode)obj;
            return Equals(otherSsrGroupCode);
        }

        private readonly string _value;
    }
}
