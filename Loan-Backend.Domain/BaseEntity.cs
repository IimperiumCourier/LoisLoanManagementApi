namespace Loan_Backend.Domain
{
    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>
    {
        public TId Id { get; protected set; }

        protected BaseEntity(TId id)
        {
            if (EqualityComparer<TId>.Default.Equals(id, default(TId)))
            {
                throw new ArgumentException("The ID cannot be the type's default value.", nameof(id));
            }
            Id = id;
        }

        protected BaseEntity() { }

        public override bool Equals(object otherObject)
        {
            return Equals(otherObject as BaseEntity<TId>);
        }

        public bool Equals(BaseEntity<TId> other)
        {
            return other != null && EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TId>.Default.GetHashCode(Id);
        }
    }
}
