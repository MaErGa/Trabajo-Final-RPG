using UnityEngine;

public class ControlBatalla : MonoBehaviour
{
    public LogBatalla log;
    public PersonajeBatalla visualEnemigo;
    public DatosEnemigo enemigoActual;

    private bool puedeActuar = true;

    void Start()
    {
        // Si tienes el metodo ConfigurarImagen en PersonajeBatalla, úsalo:
        if(visualEnemigo != null && enemigoActual != null)
            visualEnemigo.ConfigurarImagen(enemigoActual.imagenEnemigo);
            
        log.EscribirMensaje("¡Un enemigo aparece!");
    }

    void Update()
    {
        if (puedeActuar && Input.GetKeyDown(KeyCode.Z))
        {
            Atacar();
        }
    }

    public void Atacar()
    {
        puedeActuar = false;
        log.EscribirMensaje("¡Atacas con la tecla Z!");
        visualEnemigo.RecibirGolpe();
        
        Invoke("TurnoEnemigo", 2f);
    }

    void TurnoEnemigo()
    {
        log.EscribirMensaje("El enemigo contraataca...");
        Invoke("HabilitarTeclado", 2f);
    }

    void HabilitarTeclado()
    {
        puedeActuar = true;
        log.EscribirMensaje("¿Que haras? (Z: Atacar)");
    }
}