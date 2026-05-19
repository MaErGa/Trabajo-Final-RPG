using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement; // Necesario para detectar las escenas en ejecución

// Clase que define qué se guarda en el JSON
[System.Serializable]
public class DatosGuardado
{
    // Stats básicos
    public string nombre;
    public int nivel;
    public int hpMax;
    public int hpActual;
    public int mpMax;
    public int mpActual;
    public int fuerza;
    public int agilidad;
    public int defensa;
    public int oro;
    public int experiencia;
    public int expSiguienteNivel;

    // Atributos mágicos
    public int fuerzaMagica;
    public int terapeucidad;

    // Equipación (nombres para identificar los assets)
    public string armaEquipada;
    public string armaduraEquipada;
    public string escudoEquipado;
    public string cascoEquipado;
    public string accesorioEquipado;

    // Inventario antiguo
    public int plantasMedicinales;
    public int colaDeConejo;

    // Conjuros aprendidos (guardamos los nombres)
    public string[] conjurosAprendidos;

    // Inventario dinámico (guardamos los nombres)
    public string[] mochilaItems;
    public string[] armarioEquipo;

    // Posición del jugador
    public float posX;
    public float posY;

    // Guarda el nombre exacto de la escena activa en el JSON
    public string nombreEscena;
}

public class SistemaGuardado : MonoBehaviour
{
    public static SistemaGuardado instancia;

    // Referencia al ScriptableObject del jugador
    public DatosJugador datosRyo;

    // Carpeta donde se guarda: Assets de equipo, items y conjuros
    [Header("Assets de Referencia (para cargar por nombre)")]
    public EquipoBase[] todosLosEquipos;
    public ItemConsumible[] todosLosItems;
    public ConjuroBase[] todosLosConjuros;

    private string rutaGuardado => Application.persistentDataPath + "/partida.json";

    // Variable que recordará temporalmente el mapa extraído del archivo JSON
    [HideInInspector] public string escenaCargadaAutomatica;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // ── Guardar ──────────────────────────────────────────────

    public void Guardar()
    {
        DatosGuardado datos = new DatosGuardado();

        datos.nombre = datosRyo.nombre;
        datos.nivel = datosRyo.nivel;
        datos.hpMax = datosRyo.hpMax;
        datos.hpActual = datosRyo.hpActual;
        datos.mpMax = datosRyo.mpMax;
        datos.mpActual = datosRyo.mpActual;
        datos.fuerza = datosRyo.fuerza;
        datos.agilidad = datosRyo.agilidad;
        datos.defensa = datosRyo.defensa;
        datos.oro = datosRyo.oro;
        datos.experiencia = datosRyo.experiencia;
        datos.expSiguienteNivel = datosRyo.expSiguienteNivel;
        datos.fuerzaMagica = datosRyo.fuerzaMagica;
        datos.terapeucidad = datosRyo.terapeucidad;
        datos.plantasMedicinales = datosRyo.plantasMedicinales;
        datos.colaDeConejo = datosRyo.colaDeConejo;

        // Equipación
        datos.armaEquipada = datosRyo.armaEquipadaAsset != null ? datosRyo.armaEquipadaAsset.nombre : "";
        datos.armaduraEquipada = datosRyo.armaduraEquipadaAsset != null ? datosRyo.armaduraEquipadaAsset.nombre : "";
        datos.escudoEquipado = datosRyo.escudoEquipadoAsset != null ? datosRyo.escudoEquipadoAsset.nombre : "";
        datos.cascoEquipado = datosRyo.cascoEquipadoAsset != null ? datosRyo.cascoEquipadoAsset.nombre : "";
        datos.accesorioEquipado = datosRyo.accesorioEquipadoAsset != null ? datosRyo.accesorioEquipadoAsset.nombre : "";

        // Conjuros aprendidos
        var conjuros = new string[datosRyo.conjurosAprendidos.Count];
        for (int i = 0; i < datosRyo.conjurosAprendidos.Count; i++)
            conjuros[i] = datosRyo.conjurosAprendidos[i] != null ? datosRyo.conjurosAprendidos[i].nombreConjuro : "";
        datos.conjurosAprendidos = conjuros;

        // Mochila
        var mochila = new string[datosRyo.mochilaItems.Count];
        for (int i = 0; i < datosRyo.mochilaItems.Count; i++)
            mochila[i] = datosRyo.mochilaItems[i] != null ? datosRyo.mochilaItems[i].nombre : "";
        datos.mochilaItems = mochila;

        // Armario de equipo
        var armario = new string[datosRyo.armarioEquipo.Count];
        for (int i = 0; i < datosRyo.armarioEquipo.Count; i++)
            armario[i] = datosRyo.armarioEquipo[i] != null ? datosRyo.armarioEquipo[i].nombre : "";
        datos.armarioEquipo = armario;

        // Posición del jugador
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            datos.posX = jugador.transform.position.x;
            datos.posY = jugador.transform.position.y;
        }

        // Registramos el nombre exacto de la escena donde se guardó
        datos.nombreEscena = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaGuardado, json);
        Debug.Log("Partida guardada en: " + rutaGuardado);
    }

    // ── Cargar ───────────────────────────────────────────────

    public bool ExistePartida()
    {
        return File.Exists(rutaGuardado);
    }

    public void Cargar()
    {
        if (!ExistePartida())
        {
            Debug.LogWarning("No existe archivo de partida.");
            return;
        }

        string json = File.ReadAllText(rutaGuardado);
        DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

        datosRyo.nombre = datos.nombre;
        datosRyo.nivel = datos.nivel;
        datosRyo.hpMax = datos.hpMax;
        datosRyo.hpActual = datos.hpActual;
        datosRyo.mpMax = datos.mpMax;
        datosRyo.mpActual = datos.mpActual;
        datosRyo.fuerza = datos.fuerza;
        datosRyo.agilidad = datos.agilidad;
        datosRyo.defensa = datos.defensa;
        datosRyo.oro = datos.oro;
        datosRyo.experiencia = datos.experiencia;
        datosRyo.expSiguienteNivel = datos.expSiguienteNivel;
        datosRyo.fuerzaMagica = datos.fuerzaMagica;
        datosRyo.terapeucidad = datos.terapeucidad;
        datosRyo.plantasMedicinales = datos.plantasMedicinales;
        datosRyo.colaDeConejo = datos.colaDeConejo;

        // Equipación
        datosRyo.armaEquipadaAsset = BuscarEquipo(datos.armaEquipada);
        datosRyo.armaduraEquipadaAsset = BuscarEquipo(datos.armaduraEquipada);
        datosRyo.escudoEquipadoAsset = BuscarEquipo(datos.escudoEquipado);
        datosRyo.cascoEquipadoAsset = BuscarEquipo(datos.cascoEquipado);
        datosRyo.accesorioEquipadoAsset = BuscarEquipo(datos.accesorioEquipado);

        // Conjuros aprendidos
        datosRyo.conjurosAprendidos.Clear();
        foreach (var nombre in datos.conjurosAprendidos)
        {
            var conjuro = BuscarConjuro(nombre);
            if (conjuro != null) datosRyo.conjurosAprendidos.Add(conjuro);
        }

        // Mochila
        datosRyo.mochilaItems.Clear();
        foreach (var nombre in datos.mochilaItems)
        {
            var item = BuscarItem(nombre);
            if (item != null) datosRyo.mochilaItems.Add(item);
        }

        // Armario
        datosRyo.armarioEquipo.Clear();
        foreach (var nombre in datos.armarioEquipo)
        {
            var equipo = BuscarEquipo(nombre);
            if (equipo != null) datosRyo.armarioEquipo.Add(equipo);
        }

        // Guardar posición para aplicarla al cargar la escena
        posicionGuardada = new Vector3(datos.posX, datos.posY, 0);
        hayPosicionGuardada = true;

        // Recuperamos la escena leída del archivo .json
        escenaCargadaAutomatica = datos.nombreEscena;

        Debug.Log("Partida cargada correctamente.");
    }

    // ── Borrar ───────────────────────────────────────────────

    public void BorrarPartida()
    {
        if (File.Exists(rutaGuardado))
        {
            File.Delete(rutaGuardado);
            Debug.Log("Partida borrada.");
        }
        datosRyo.ReiniciarPersonaje();
    }

    // ── Posición guardada ────────────────────────────────────

    [HideInInspector] public Vector3 posicionGuardada;
    [HideInInspector] public bool hayPosicionGuardada = false;

    public void AplicarPosicionJugador()
    {
        if (!hayPosicionGuardada) return;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            jugador.transform.position = posicionGuardada;
            hayPosicionGuardada = false;
        }
    }

    // ── Búsqueda de assets por nombre ────────────────────────

    EquipoBase BuscarEquipo(string nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return null;
        foreach (var e in todosLosEquipos)
            if (e != null && e.nombre == nombre) return e;
        return null;
    }

    ItemConsumible BuscarItem(string nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return null;
        foreach (var i in todosLosItems)
            if (i != null && i.nombre == nombre) return i;
        return null;
    }

    ConjuroBase BuscarConjuro(string nombre)
    {
        if (string.IsNullOrEmpty(nombre)) return null;
        foreach (var c in todosLosConjuros)
            if (c != null && c.nombreConjuro == nombre) return c;
        return null;
    }
}