using XDClient.Utilities;

namespace XDClient.Feature;

public class PointsEditor
{
    public void AddPoints(int count)
    {
        var player = CheatUtilities.GetLocalPlayer();

        if (player == null)
            return;

        var currentPoints = player.Points;

        currentPoints.Value += count;
        
        player.Points = currentPoints;
    }
    
    public void RemovePoints(int count)
    {
        var player = CheatUtilities.GetLocalPlayer();

        if (player == null)
            return;

        var currentPoints = player.Points;

        currentPoints.Value -= count;
        
        player.Points = currentPoints;
    }
}