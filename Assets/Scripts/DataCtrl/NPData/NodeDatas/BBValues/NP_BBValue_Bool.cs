//------------------------------------------------------------
// File: NP_BBValue_Bool.cs
// Created: 2025-12-01
// Purpose: Blackboard value wrapper for bool.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;

namespace Ebonor.DataCtrl
{
   
    public class NP_BBValue_Bool: NP_BBValueBase<bool>, IEquatable<NP_BBValue_Bool>
    {
        public override Type NP_BBValueType
        {
            get
            {
                return typeof (bool);
            }
        }

        #region  

        public bool Equals(NP_BBValue_Bool other)
        {
            // If parameter is null, return false.
            if (System.Object.ReferenceEquals(other, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (System.Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return this.Value == other.GetValue();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((NP_BBValue_Bool) obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator ==(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            // Check for null on left side.
            if (System.Object.ReferenceEquals(lhs, null))
            {
                if (System.Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            return false;
        }

        public static bool operator <(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            return false;
        }

        public static bool operator >=(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            return false;
        }

        public static bool operator <=(NP_BBValue_Bool lhs, NP_BBValue_Bool rhs)
        {
            return false;
        }

        #endregion
       
    }
}
