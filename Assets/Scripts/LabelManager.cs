using UnityEngine;

public class LabelManager : MonoBehaviour
{
    public static string CurWeapon { get; set; }
    public static int Ammo { get; set; }
    public static int Magazin { get; set; }

    private void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(10, 30, 300, 20), "Current Weapon: " + CurWeapon);
        GUI.Label(new Rect(10, 60, 300, 20), "Ammo: " + Ammo);
        GUI.Label(new Rect(10, 90, 300, 20), "Magazin: " + Magazin);
    }
}
