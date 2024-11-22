using Il2CppBrokenArrow.Client.Ecs.Controllers;
using Il2CppBrokenArrow.Client.Ecs.Utils;
using UnityEngine;

namespace XDClient.Utilities;

public static class CheatUtilities
{
    public static GameController? GetController()
    {
        var gmObject = GameObject.Find("CONTROLLER(Clone)");

        if (gmObject != null)
            return gmObject.GetComponent<GameController>();
        
        Debug.LogError("ControllerObject not found");
        
        return null;
    }

    public static PlayerInfo? GetLocalPlayer()
    {
        var gmController = GetController();

        if (gmController != null)
            return gmController.GameSession.CurrentPlayer;
        
        Debug.LogError("Controller not found");
        
        return null;
    }
}