namespace System.Collections.Generic
{
    internal class List : List<object>
    {
        public List(int capacity) : base(capacity)
        {
        }

        public static implicit operator List<object>(List v)
        {
            throw new NotImplementedException();
        }
    }
}