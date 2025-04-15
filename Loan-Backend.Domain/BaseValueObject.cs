using System.Reflection;

namespace Loan_Backend.Domain
{

    public abstract class BaseValueObject<T> : IEquatable<T>
        where T : BaseValueObject<T>
    {
        public override bool Equals(object obj)
        {
            if (obj is T other)
            {
                return Equals(other);
            }
            return false;
        }

        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            var fields = GetFields();
            foreach (var field in fields)
            {
                var value1 = field.GetValue(this);
                var value2 = field.GetValue(other);

                if (!object.Equals(value1, value2))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 17;
            foreach (var field in GetFields())
            {
                var value = field.GetValue(this);
                hashCode = hashCode * 59 + (value != null ? value.GetHashCode() : 0);
            }
            return hashCode;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var type = GetType();
            while (type != typeof(object))
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    yield return field;
                }
                type = type.BaseType;
            }
        }

        public static bool operator ==(BaseValueObject<T> x, BaseValueObject<T> y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(BaseValueObject<T> x, BaseValueObject<T> y)
        {
            return !(x == y);
        }
    }

}
