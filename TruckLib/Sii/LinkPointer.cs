namespace TruckLib.Sii
{
    internal record struct LinkPointer(string Value)
    {
        public static implicit operator LinkPointer(string value) => new(value);
        public static implicit operator string(LinkPointer lp) => lp.Value;
    }
}
