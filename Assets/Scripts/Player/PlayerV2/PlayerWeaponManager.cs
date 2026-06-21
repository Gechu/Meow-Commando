using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Mount")]
    [SerializeField] private Transform weaponMount;

    [Header("Weapons (prefabs)")]
    [SerializeField] private WeaponBase weaponSlot1;
    [SerializeField] private WeaponBase weaponSlot2;
    [SerializeField] private WeaponBase weaponSlot3;
    [SerializeField] private WeaponBase weaponSlot4;
    [SerializeField] private WeaponBase weaponSlot5;

    private WeaponBase currentWeapon;
    [SerializeField] private PlayerMovementV2 movement;
    private WeaponBase[] weapons;

    void Awake()
    {
        weapons = new WeaponBase[]
        {
            weaponSlot1,
            weaponSlot2,
            weaponSlot3,
            weaponSlot4,
            weaponSlot5
        };
    }

    private void Start()
    {
        if (weaponSlot1) Equip(weaponSlot1);
    }

    private void Update()
    {
        // TEST: zmiana broni 
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) TryEquip(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) TryEquip(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) TryEquip(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) TryEquip(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) TryEquip(4);
        }

        // strzelanie (LPM)
        if (currentWeapon != null && Mouse.current != null && Mouse.current.leftButton.isPressed && !movement.IsDashing)
        {
            currentWeapon.TryShoot();
        }
    }

    void TryEquip(int index)
    {
        if (!PlayerDataManager.Instance.unlockedWeapons[index])
            return;

        Equip(weapons[index]);
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

    public int GetUnlockedWeaponCount()
    {
        int count = 0;

        foreach (bool unlocked in PlayerDataManager.Instance.unlockedWeapons)
        {
            if (unlocked)
                count++;
        }

        return count;
    }
}