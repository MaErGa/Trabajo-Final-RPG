using UnityEngine;

public class PosicionGuardada : MonoBehaviour
{
    public static PosicionGuardada Instance;
    public Vector3 ultimaPosicionPueblo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}