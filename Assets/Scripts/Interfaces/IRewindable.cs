public interface IRewindable
{
    void Rewind();
    void StopRewind();
    void RewindTimePoints();
    void RecordTimePoints();
    void RemoveFrame();
}
