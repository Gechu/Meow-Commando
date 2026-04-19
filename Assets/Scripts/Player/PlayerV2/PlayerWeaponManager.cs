using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Mount")]
    [SerializeField] private Transform weaponMount;

    [Header("Weapons (prefabs)")]
    [SerializeField] private WeaponBase weaponSlot1;
    [SerializeField] private WeaponBase weaponSlot2;

    private WeaponBase currentWeapon;
    [SerializeField] private PlayerMovementV2 movement;

    private void Start()
    {
        if (weaponSlot1) Equip(weaponSlot1);
    }

    private void Update()
    {
        // TEST: zmiana broni 1/2
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) Equip(weaponSlot1);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) Equip(weaponSlot2);
        }

        // strzelanie (LPM)
        if (currentWeapon != null && Mouse.current != null && Mouse.current.leftButton.isPressed)
            if (!movement.IsDashing)
            {
                currentWeapon.TryShoot();
            }
    }

    public void Equip(WeaponBase weaponPrefab)
    {
        if (!weaponMount || !weaponPrefab) return;

        if (currentWeapon)
            Destroy(currentWeapon.gameObject);

        currentWeapon = Instantiate(weaponPrefab, weaponMount);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.transform.localScale = Vector3.one;

        currentWeapon.OnEquipped();
    }
}