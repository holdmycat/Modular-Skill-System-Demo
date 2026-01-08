using System;

namespace Ebonor.DataCtrl
{
    [System.Serializable]
    public class NP_BBValue_BuffBindAnimStackState : NP_BBValueBase<eBuffBindAnimStackState>, IEquatable<NP_BBValue_BuffBindAnimStackState>
    {
        public override Type NP_BBValueType
        {
            get
            {
                return typeof(eBuffBindAnimStackState);
            }
        }

        #region 对比函数

        public bool Equals(NP_BBValue_BuffBindAnimStackState other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Value == other.GetValue();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NP_BBValue_BuffBindAnimStackState)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            return !(lhs == rhs);
        }

        // Enum comparisons usually rely on integer value
        public static bool operator >(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            return lhs.GetValue() > rhs.GetValue();
        }

        public static bool operator <(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            return lhs.GetValue() < rhs.GetValue();
        }

        public static bool operator >=(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            return lhs.GetValue() >= rhs.GetValue();
        }

        public static bool operator <=(NP_BBValue_BuffBindAnimStackState lhs, NP_BBValue_BuffBindAnimStackState rhs)
        {
            return lhs.GetValue() <= rhs.GetValue();
        }

        #endregion

        public override ANP_BBValue Clone()
        {
            var copy = new NP_BBValue_BuffBindAnimStackState();
            copy.SetValueFrom(this);
            return copy;
        }
    }
}
