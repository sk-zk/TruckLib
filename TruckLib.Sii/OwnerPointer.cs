namespace TruckLib.Sii
{
    public record struct OwnerPointer(string Value)
    {
        public static implicit operator OwnerPointer(string value) => new(value);
        public static implicit operator string(OwnerPointer op) => op.Value;
    }
}
