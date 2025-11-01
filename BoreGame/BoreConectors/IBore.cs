namespace BoreGame.BoreConectors;

public interface IBore
{
    public string BorePatch { get; set; }
    public string OpenPort(string openedPort, string server);

    public void StopOpened();
}