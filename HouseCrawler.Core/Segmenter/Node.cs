namespace JiebaNet.Segmenter
{
    public class Node
    {
        public char Value { get; }
        public Node Parent { get; }

        public Node(char value, Node parent)
        {
            Value = value;
            Parent = parent;
        }
    }
}