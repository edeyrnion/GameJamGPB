using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Range(1f, 10f)] private float _walkingSpeed = 8f;
    [SerializeField] [Range(0f, 1f)] private float _walkingReactionTime = 0.3f;
    [SerializeField] private Transform _handRight;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private List<WeaponStats> _weaponStats;
    [SerializeField] private List<GameObject> _weapons;

    private float axisX;
    private float axisY;

    private float movementSpeedX;
    private float movementSpeedY;

    private int _curWeapon = 0;
    private float _fireTimer;
    private float _weaponSwitchTimer;
    private float _weaponReloadTimer;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;

        for (int i = 0; i < _weaponStats.Count; i++)
        {
            _weapons.Add(Instantiate(_weaponStats[i].Weapon, _handRight));
            _weapons[i].SetActive(false);
        }
        _weapons[_curWeapon].SetActive(true);

        LabelManager.CurWeapon = _weaponStats[_curWeapon].name;
        LabelManager.Ammo = _weaponStats[_curWeapon].currAmmo;
        LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;
    }

    private void Update()
    {
        axisX = Mathf.SmoothDamp(axisX, Input.GetAxisRaw("Horizontal"), ref movementSpeedX, _walkingReactionTime * 0.2f);
        axisY = Mathf.SmoothDamp(axisY, Input.GetAxisRaw("Vertical"), ref movementSpeedY, _walkingReactionTime * 0.2f);

        transform.Translate(axisX * _walkingSpeed * Time.deltaTime, 0f, axisY * _walkingSpeed * Time.deltaTime, Space.World);

        if (GetMousePosInWorldSpace(out Vector3 mousePos))
        {
            mousePos.y = transform.position.y;
            transform.LookAt(mousePos);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            FireWeapon();
        }


        if (_weaponSwitchTimer < Time.time - 0.3f)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                PreviousWeapon();
                _weaponSwitchTimer = Time.time;
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                NextWeapon();
                _weaponSwitchTimer = Time.time;
            }
        }


        if (_weaponReloadTimer < Time.time - 0.3f)
        {

            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadWeapon();
                _weaponReloadTimer = Time.time;
            }
        }
    }

    private bool GetMousePosInWorldSpace(out Vector3 mousePos)
    {
        Vector3 mp = Input.mousePosition;
        var ray = _cam.ScreenPointToRay(mp);

        if (Physics.Raycast(ray, out RaycastHit hitinfo, 100f, _groundLayer))
        {
            mousePos = hitinfo.point;
            return true;
        }

        mousePos = new Vector3();
        return false;
    }

    private void FireWeapon()
    {
        if (_fireTimer < Time.time - _weaponStats[_curWeapon].fireRate)
        {
            if (_weaponStats[_curWeapon].curBulletsInClip > 0f)
            {
                Instantiate(_weaponStats[_curWeapon].bullet, _handRight.position + (_handRight.rotation * _weaponStats[_curWeapon].bulletSpawnPoint), _handRight.rotation);

                _weaponStats[_curWeapon].curBulletsInClip--;

                Debug.Log("Fiering Weapon " + _curWeapon);

                LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;
            }

            _fireTimer = Time.time;
        }
    }

    private void NextWeapon()
    {
        _weapons[_curWeapon].SetActive(false);

        if (_curWeapon == _weaponStats.Count - 1)
        {
            _curWeapon = 0;
        }
        else
        {
            _curWeapon++;
        }

        LabelManager.CurWeapon = _weaponStats[_curWeapon].name;
        LabelManager.Ammo = _weaponStats[_curWeapon].currAmmo;
        LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;

        _weapons[_curWeapon].SetActive(true);
    }

    private void PreviousWeapon()
    {
        _weapons[_curWeapon].SetActive(false);

        if (_curWeapon == 0)
        {
            _curWeapon = _weaponStats.Count - 1;
        }
        else
        {
            _curWeapon--;
        }

        LabelManager.CurWeapon = _weaponStats[_curWeapon].name;
        LabelManager.Ammo = _weaponStats[_curWeapon].currAmmo;
        LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;

        _weapons[_curWeapon].SetActive(true);
    }

    private void ReloadWeapon()
    {
        var curWeaponStat = _weaponStats[_curWeapon];

        if (curWeaponStat.curBulletsInClip < curWeaponStat.bulletsPerClip)
        {
            if (curWeaponStat.bulletsPerClip - curWeaponStat.curBulletsInClip <= curWeaponStat.currAmmo)
            {
                curWeaponStat.currAmmo = curWeaponStat.currAmmo - (curWeaponStat.bulletsPerClip - curWeaponStat.curBulletsInClip);
                curWeaponStat.curBulletsInClip = curWeaponStat.bulletsPerClip;
                LabelManager.Ammo = _weaponStats[_curWeapon].currAmmo;
                LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;

                Debug.Log("Reload");
            }
            else
            {
                curWeaponStat.curBulletsInClip += curWeaponStat.currAmmo;
                curWeaponStat.currAmmo = 0;
                LabelManager.Ammo = _weaponStats[_curWeapon].currAmmo;
                LabelManager.Magazin = _weaponStats[_curWeapon].curBulletsInClip;
            }
        }
    }
}
