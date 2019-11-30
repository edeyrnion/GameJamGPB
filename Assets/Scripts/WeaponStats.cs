using UnityEngine;

[CreateAssetMenu(menuName = "PluggableWeapons/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Weapon Stats")]
    public float reloadSpeed = 1f;
    public int bulletsPerClip = 9;
    public int curBulletsInClip = 9;
    public int damage = 35;
    public float fireRate = 1f;
    public int maxAmmo = 20;
    public int currAmmo = 20;

    [Header("Weapon Mesh")]
    public GameObject Weapon;
    public GameObject bullet;
    public Vector3 bulletSpawnPoint;

    [Header("Firing Modes")]
    public bool singleFireMode = true;
    public bool fullAutoFireMode = false;
}