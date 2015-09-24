using UnityEngine;

public class WeaponContainer : MonoBehaviour
{
    public WeaponModel[] WeaponModels;

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static WeaponContainer _instance;

    public static WeaponContainer Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<WeaponContainer>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
