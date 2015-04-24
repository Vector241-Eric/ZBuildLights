namespace ZWaveControl
{
    public interface IZWaveNodeList
    {
        void AddNode(Node node);
        Node[] AllNodes { get; }
    }
}