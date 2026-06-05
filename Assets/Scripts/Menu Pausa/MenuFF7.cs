using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// ╔══════════════════════════════════════════════════════════╗
/// ║  MenuFF7  —  Menú principal estilo Final Fantasy VII     ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  • Genera toda la UI por código (nunca se descuadra)     ║
/// ║  • Panel Stats con portrait, LV, HP, MP, barra XP       ║
/// ║  • Panel Inventario — usar consumibles y equipar         ║
/// ║  • Panel Equipo — ver y cambiar equipación               ║
/// ║  • HUD: Tiempo y Oro                                     ║
/// ║  • Se abre/cierra con tecla P                            ║
/// ╠══════════════════════════════════════════════════════════╣
/// ║  SETUP en el Inspector:                                  ║
/// ║    datosJugador   → DatosJugador ScriptableObject        ║
/// ║    portrait       → Sprite del personaje                 ║
/// ║    fuentePixel    → fuente TMP pixel (opcional)          ║
/// ╚══════════════════════════════════════════════════════════╝
/// </summary>
public class MenuFF7 : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────────────────────
    // INSPECTOR
    // ─────────────────────────────────────────────────────────────────────────
    [Header("── Datos ──────────────────────────────────────")]
    public DatosJugador datosJugador;

    [Header("Portrait del personaje")]
    public Sprite portrait;

    [Header("── Estética (opcional) ────────────────────────")]
    public TMP_FontAsset fuentePixel;

    // ─────────────────────────────────────────────────────────────────────────
    // PALETA FF7
    // ─────────────────────────────────────────────────────────────────────────
    static readonly Color C_FONDO = new Color(0.05f, 0.08f, 0.35f, 1f);
    static readonly Color C_MEDIO = new Color(0.10f, 0.15f, 0.55f, 1f);
    static readonly Color C_CLARO = new Color(0.20f, 0.30f, 0.75f, 1f);
    static readonly Color C_BORDE = new Color(0.55f, 0.65f, 1.00f, 1f);
    static readonly Color C_ORO = new Color(1.00f, 0.85f, 0.20f, 1f);
    static readonly Color C_CYAN = new Color(0.40f, 0.90f, 1.00f, 1f);
    static readonly Color C_BLANCO = Color.white;
    static readonly Color C_GRIS = new Color(0.70f, 0.70f, 0.70f, 1f);
    static readonly Color C_VERDE = new Color(0.15f, 0.75f, 0.30f, 1f);
    static readonly Color C_AZUL = new Color(0.20f, 0.50f, 1.00f, 1f);
    static readonly Color C_ROJO = new Color(0.85f, 0.20f, 0.20f, 1f);
    static readonly Color C_SEL = new Color(0.30f, 0.50f, 1.00f, 1f);

    // ─────────────────────────────────────────────────────────────────────────
    // REFERENCIAS UI
    // ─────────────────────────────────────────────────────────────────────────
    Canvas _canvas;
    GameObject _raiz;

    // Stats panel
    Image _imgPortrait;
    TextMeshProUGUI _txtNombre, _txtNivel;
    TextMeshProUGUI _txtHP, _txtMP;
    Image _fillHP, _fillMP, _fillXP;
    TextMeshProUGUI _txtAtq, _txtDef, _txtAgi, _txtMag, _txtTer;
    TextMeshProUGUI _txtArma, _txtArmadura, _txtEscudo, _txtCasco, _txtAccesorio;

    // Inventario panel
    GameObject _panelInventario;
    Transform _contenedorItems;
    TextMeshProUGUI _txtItemNombre, _txtItemDesc;
    Button _btnAccion;
    TextMeshProUGUI _txtBtnAccion;

    // Equipo panel
    GameObject _panelEquipo;
    TextMeshProUGUI _eqArma, _eqArmadura, _eqEscudo, _eqCasco, _eqAccesorio;
    TextMeshProUGUI _eqBonos;

    // HUD
    TextMeshProUGUI _txtOro, _txtTiempo;

    // Sidebar buttons
    Button _btnEstado, _btnItem, _btnEquipo, _btnSalir, _btnMagia;

    // Panel stats GO (para mostrar/ocultar)
    GameObject _panelStats;

    // ─────────────────────────────────────────────────────────────────────────
    // ESTADO
    // ─────────────────────────────────────────────────────────────────────────
    bool _abierto = false;
    float _segundos = 0f;

    ItemConsumible _itemSel;
    EquipoBase _equipoSel;

    // ═════════════════════════════════════════════════════════════════════════
    // UNITY
    // ═════════════════════════════════════════════════════════════════════════
    void Awake()
    {
        ConstruirUI();
        _raiz.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _abierto = !_abierto;
            _raiz.SetActive(_abierto);
            if (_abierto) { AbrirStats(); ActualizarBotonMagia(); }
        }

        if (_abierto)
        {
            _segundos += Time.deltaTime;
            ActualizarTiempo();
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    // NAVEGACIÓN
    // ═════════════════════════════════════════════════════════════════════════
    void AbrirStats()
    {
        _panelStats.SetActive(true);
        _panelInventario.SetActive(false);
        _panelEquipo.SetActive(false);
        ResaltarBtn(_btnEstado);
        ActualizarBotonMagia();
        RefrescarStats();
    }

    void ActualizarBotonMagia()
    {
        if (_btnMagia == null || datosJugador == null) return;
        bool tieneMagia = datosJugador.conjurosAprendidos != null &&
                          datosJugador.conjurosAprendidos.Count > 0;
        _btnMagia.interactable = tieneMagia;
        var img = _btnMagia.GetComponent<Image>();
        if (img) img.color = tieneMagia ? C_MEDIO : new Color(0.15f, 0.15f, 0.25f, 1f);
        var txt = _btnMagia.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.color = tieneMagia ? C_BLANCO : C_GRIS;
    }

    void AbrirInventario()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(true);
        _panelEquipo.SetActive(false);
        ResaltarBtn(_btnItem);
        RefrescarInventario();
    }

    void AbrirEquipo()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(false);
        _panelEquipo.SetActive(true);
        ResaltarBtn(_btnEquipo);
        RefrescarEquipo();
    }

    // ═════════════════════════════════════════════════════════════════════════
    // REFRESCAR DATOS
    // ═════════════════════════════════════════════════════════════════════════
    void RefrescarStats()
    {
        if (datosJugador == null) return;

        if (_imgPortrait && portrait) _imgPortrait.sprite = portrait;
        if (_txtNombre) _txtNombre.text = datosJugador.nombre;
        if (_txtNivel) _txtNivel.text = "LV  " + datosJugador.nivel;

        int hp = datosJugador.hpActual, hpMax = datosJugador.hpMax;
        int mp = datosJugador.mpActual, mpMax = datosJugador.mpMax;

        if (_txtHP) _txtHP.text = $"HP  <color=#FFFFFF>{hp}</color><color=#888888>/</color><color=#FFFFFF>{hpMax}</color>";
        if (_txtMP) _txtMP.text = $"MP  <color=#FFFFFF>{mp}</color><color=#888888>/</color><color=#FFFFFF>{mpMax}</color>";

        if (_fillHP) _fillHP.fillAmount = hpMax > 0 ? (float)hp / hpMax : 0f;
        if (_fillMP) _fillMP.fillAmount = mpMax > 0 ? (float)mp / mpMax : 0f;

        // Barra XP
        if (_fillXP && datosJugador.expSiguienteNivel > 0)
            _fillXP.fillAmount = Mathf.Clamp01((float)datosJugador.experiencia / datosJugador.expSiguienteNivel);

        if (_txtAtq) _txtAtq.text = datosJugador.AtaqueTotal.ToString();
        if (_txtDef) _txtDef.text = datosJugador.DefensaTotal.ToString();
        if (_txtAgi) _txtAgi.text = datosJugador.AgilidadTotal.ToString();
        if (_txtMag) _txtMag.text = datosJugador.fuerzaMagica.ToString();
        if (_txtTer) _txtTer.text = datosJugador.terapeucidad.ToString();

        if (_txtArma) _txtArma.text = datosJugador.armaEquipadaAsset != null ? datosJugador.armaEquipadaAsset.nombre : "——";
        if (_txtArmadura) _txtArmadura.text = datosJugador.armaduraEquipadaAsset != null ? datosJugador.armaduraEquipadaAsset.nombre : "——";
        if (_txtEscudo) _txtEscudo.text = datosJugador.escudoEquipadoAsset != null ? datosJugador.escudoEquipadoAsset.nombre : "——";
        if (_txtCasco) _txtCasco.text = datosJugador.cascoEquipadoAsset != null ? datosJugador.cascoEquipadoAsset.nombre : "——";
        if (_txtAccesorio) _txtAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "——";

        if (_txtOro) _txtOro.text = "Gil  " + datosJugador.oro;
    }

    void RefrescarInventario()
    {
        if (datosJugador == null) return;
        LimpiarContenedor(_contenedorItems);
        _itemSel = null; _equipoSel = null;
        OcultarBtnAccion();
        if (_txtItemNombre) _txtItemNombre.text = "";
        if (_txtItemDesc) _txtItemDesc.text = "";

        // Consumibles
        foreach (var item in datosJugador.mochilaItems)
        {
            if (item == null) continue;
            var cap = item;
            CrearFilaInventario(item.nombre, $"+{item.potencia} {EfectoTexto(item.queCura)}",
                () => SeleccionarConsumible(cap));
        }

        // Plantas medicinales (sistema antiguo)
        if (datosJugador.plantasMedicinales > 0)
            CrearFilaInventario($"Planta Medicinal  x{datosJugador.plantasMedicinales}", "+30 HP",
                () =>
                {
                    _itemSel = null; _equipoSel = null;
                    if (_txtItemNombre) _txtItemNombre.text = "Planta Medicinal";
                    if (_txtItemDesc) _txtItemDesc.text = "Restaura 30 HP.";
                    MostrarBtnAccion("Usar", UsarPlanta);
                });

        // Equipo en armario
        foreach (var eq in datosJugador.armarioEquipo)
        {
            if (eq == null) continue;
            var cap = eq;
            CrearFilaInventario(eq.nombre, $"[{eq.tipoSlot}]",
                () => SeleccionarEquipo(cap));
        }
    }

    void RefrescarEquipo()
    {
        if (datosJugador == null) return;
        if (_eqArma) _eqArma.text = datosJugador.armaEquipadaAsset != null ? datosJugador.armaEquipadaAsset.nombre : "——";
        if (_eqArmadura) _eqArmadura.text = datosJugador.armaduraEquipadaAsset != null ? datosJugador.armaduraEquipadaAsset.nombre : "——";
        if (_eqEscudo) _eqEscudo.text = datosJugador.escudoEquipadoAsset != null ? datosJugador.escudoEquipadoAsset.nombre : "——";
        if (_eqCasco) _eqCasco.text = datosJugador.cascoEquipadoAsset != null ? datosJugador.cascoEquipadoAsset.nombre : "——";
        if (_eqAccesorio) _eqAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "——";

        if (_eqBonos)
            _eqBonos.text = $"ATQ  {datosJugador.AtaqueTotal}     DEF  {datosJugador.DefensaTotal}     AGI  {datosJugador.AgilidadTotal}";
    }

    void ActualizarTiempo()
    {
        if (_txtTiempo == null) return;
        int h = (int)(_segundos / 3600);
        int m = (int)((_segundos % 3600) / 60);
        int s = (int)(_segundos % 60);
        _txtTiempo.text = $"{h}:{m:D2}:{s:D2}";
    }

    // ═════════════════════════════════════════════════════════════════════════
    // INVENTARIO — ACCIONES
    // ═════════════════════════════════════════════════════════════════════════
    void SeleccionarConsumible(ItemConsumible item)
    {
        _itemSel = item; _equipoSel = null;
        if (_txtItemNombre) _txtItemNombre.text = item.nombre;
        if (_txtItemDesc) _txtItemDesc.text = item.descripcion;
        MostrarBtnAccion("Usar", UsarItemSeleccionado);
    }

    void SeleccionarEquipo(EquipoBase eq)
    {
        _equipoSel = eq; _itemSel = null;
        if (_txtItemNombre) _txtItemNombre.text = eq.nombre;
        if (_txtItemDesc) _txtItemDesc.text = eq.descripcion +
            $"\nATQ+{eq.bonoAtaque}  DEF+{eq.bonoDefensa}  AGI+{eq.bonoAgilidad}";
        MostrarBtnAccion("Equipar", EquiparSeleccionado);
    }

    void UsarItemSeleccionado()
    {
        if (_itemSel == null || datosJugador == null) return;
        switch (_itemSel.queCura)
        {
            case TipoEfecto.Vida:
                datosJugador.hpActual = Mathf.Min(datosJugador.hpMax, datosJugador.hpActual + _itemSel.potencia);
                break;
            case TipoEfecto.Mana:
                datosJugador.mpActual = Mathf.Min(datosJugador.mpMax, datosJugador.mpActual + _itemSel.potencia);
                break;
        }
        datosJugador.mochilaItems.Remove(_itemSel);
        _itemSel = null;
        RefrescarInventario();
        RefrescarStats();
    }

    void UsarPlanta()
    {
        if (datosJugador.plantasMedicinales <= 0) return;
        datosJugador.hpActual = Mathf.Min(datosJugador.hpMax, datosJugador.hpActual + 30);
        datosJugador.plantasMedicinales--;
        RefrescarInventario();
        RefrescarStats();
    }

    void EquiparSeleccionado()
    {
        if (_equipoSel == null || datosJugador == null) return;
        datosJugador.EquiparObjeto(_equipoSel);
        datosJugador.armarioEquipo.Remove(_equipoSel);
        _equipoSel = null;
        RefrescarInventario();
        RefrescarStats();
    }

    void MostrarBtnAccion(string texto, UnityEngine.Events.UnityAction accion)
    {
        if (_btnAccion == null) return;
        _btnAccion.gameObject.SetActive(true);
        _btnAccion.onClick.RemoveAllListeners();
        _btnAccion.onClick.AddListener(accion);
        if (_txtBtnAccion) _txtBtnAccion.text = texto;
    }

    void OcultarBtnAccion() { if (_btnAccion) _btnAccion.gameObject.SetActive(false); }

    // ═════════════════════════════════════════════════════════════════════════
    // HELPERS
    // ═════════════════════════════════════════════════════════════════════════
    void LimpiarContenedor(Transform t)
    {
        if (t == null) return;
        foreach (Transform h in t) Destroy(h.gameObject);
    }

    void ResaltarBtn(Button activo)
    {
        foreach (var b in new[] { _btnEstado, _btnItem, _btnEquipo })
        {
            if (b == null) continue;
            var img = b.GetComponent<Image>();
            if (img) img.color = (b == activo) ? C_CLARO : C_MEDIO;
        }
    }

    string EfectoTexto(TipoEfecto e)
    {
        return e switch { TipoEfecto.Vida => "HP", TipoEfecto.Mana => "MP", TipoEfecto.Antidoto => "Antídoto", _ => "" };
    }

    void CrearFilaInventario(string nombre, string detalle, UnityEngine.Events.UnityAction accion)
    {
        var fila = Nodo("Fila_" + nombre, _contenedorItems);
        var rt = fila.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 32);
        var img = fila.AddComponent<Image>(); img.color = C_FONDO;
        var btn = fila.AddComponent<Button>();
        var cb = btn.colors;
        cb.normalColor = C_FONDO; cb.highlightedColor = C_MEDIO; cb.pressedColor = C_SEL;
        btn.colors = cb;
        btn.onClick.AddListener(accion);

        // Columna nombre
        var goN = Nodo("Nombre", fila.transform);
        var rtN = goN.GetComponent<RectTransform>();
        rtN.anchorMin = new Vector2(0, 0); rtN.anchorMax = new Vector2(0.65f, 1);
        rtN.offsetMin = new Vector2(8, 2); rtN.offsetMax = new Vector2(-4, -2);
        var tN = goN.AddComponent<TextMeshProUGUI>();
        tN.text = nombre; tN.fontSize = 12; tN.color = C_BLANCO;
        tN.alignment = TextAlignmentOptions.Left;
        tN.overflowMode = TextOverflowModes.Ellipsis;
        if (fuentePixel) tN.font = fuentePixel;

        // Columna detalle
        var goD = Nodo("Detalle", fila.transform);
        var rtD = goD.GetComponent<RectTransform>();
        rtD.anchorMin = new Vector2(0.65f, 0); rtD.anchorMax = new Vector2(1, 1);
        rtD.offsetMin = new Vector2(4, 2); rtD.offsetMax = new Vector2(-8, -2);
        var tD = goD.AddComponent<TextMeshProUGUI>();
        tD.text = detalle; tD.fontSize = 11; tD.color = C_ORO;
        tD.alignment = TextAlignmentOptions.Right;
        if (fuentePixel) tD.font = fuentePixel;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // CONSTRUCCIÓN UI
    // Resolución de referencia: 1366 × 768
    //
    // Layout:
    //  ┌────────────────────────────┬──────────┐
    //  │  STATS / INVENTARIO / EQUIP│ SIDEBAR  │
    //  │                            │ Estado   │
    //  │                            │ Item     │
    //  │                            │ Equipo   │
    //  │                            │ ──────── │
    //  │                            │ Salir    │
    //  ├────────────────────────────┴──────────┤
    //  │  Time  0:00:00          Gil  0        │
    //  └────────────────────────────────────────┘
    // ═════════════════════════════════════════════════════════════════════════
    void ConstruirUI()
    {
        // ── Canvas ────────────────────────────────────────────────────────────
        var cgo = new GameObject("MenuFF7_Canvas");
        cgo.transform.SetParent(transform);
        _canvas = cgo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 50;
        var cs = cgo.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1366, 768);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        cgo.AddComponent<GraphicRaycaster>();

        // ── Raíz pantalla completa ────────────────────────────────────────────
        _raiz = Nodo("Raiz", cgo.transform);
        Stretch(_raiz.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);

        // ── HUD inferior (tiempo + oro) ───────────────────────────────────────
        var hudGO = PanelFF("HUD", _raiz.transform, new Vector2(0, 718), new Vector2(1366, 50));
        var hudRelleno = hudGO.transform.Find("Relleno");
        Transform hudR = hudRelleno ?? hudGO.transform;

        _txtTiempo = TMP_Anclado(hudR, "TxtTiempo", "0:00:00", 13, C_BLANCO,
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(20, 0), new Vector2(200, 0));
        var lblTiempo = TMP_Anclado(hudR, "LblTiempo", "Time", 13, C_CYAN,
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(-60, 0), new Vector2(120, 0));
        // reordenar para que "Time" quede antes
        lblTiempo.rectTransform.anchoredPosition = new Vector2(20, 0);
        _txtTiempo.rectTransform.anchoredPosition = new Vector2(90, 0);

        // Gil valor (esquina derecha del HUD) — solo un TMP en amarillo
        _txtOro = TMP_Anclado(hudR, "TxtOro", "Gil  0", 13, C_ORO,
            new Vector2(1, 0), new Vector2(1, 1), new Vector2(-220, 0), new Vector2(210, 0));
        _txtOro.alignment = TextAlignmentOptions.Right;

        // ── Sidebar (derecha) ─────────────────────────────────────────────────
        var sidebar = PanelFF("Sidebar", _raiz.transform, new Vector2(1156, 20), new Vector2(190, 690));
        var sbRelleno = sidebar.transform.Find("Relleno");
        Transform sbR = sbRelleno ?? sidebar.transform;

        _btnEstado = BotonSidebar("Estado", sbR, 0, () => AbrirStats());
        _btnItem = BotonSidebar("Item", sbR, 52, () => AbrirInventario());
        _btnEquipo = BotonSidebar("Equipo", sbR, 104, () => AbrirEquipo());
        // Magia: se activa solo si el jugador tiene conjuros aprendidos
        _btnMagia = BotonSidebar("Magia", sbR, 156, null);
        ColorGris(_btnMagia);
        // Config placeholder
        var btnConfig = BotonSidebar("Config", sbR, 208, () => UnityEngine.SceneManagement.SceneManager.LoadScene("Opciones"));
        // Salir siempre al final
        _btnSalir = BotonSidebar("Salir", sbR, 640, () => _raiz.SetActive(false));

        // ── PANEL STATS ───────────────────────────────────────────────────────
        _panelStats = PanelFF("PanelStats", _raiz.transform, new Vector2(20, 20), new Vector2(1120, 690));
        var stRelleno = _panelStats.transform.Find("Relleno");
        Transform stR = stRelleno ?? _panelStats.transform;

        // Portrait
        var portraitGO = Nodo("Portrait", stR);
        var rtP = portraitGO.GetComponent<RectTransform>();
        rtP.anchorMin = new Vector2(0, 1); rtP.anchorMax = new Vector2(0, 1);
        rtP.pivot = new Vector2(0, 1);
        rtP.anchoredPosition = new Vector2(10, -10);
        rtP.sizeDelta = new Vector2(100, 100);
        _imgPortrait = portraitGO.AddComponent<Image>();
        _imgPortrait.color = Color.white;

        // Nombre
        _txtNombre = TMP_Anclado(stR, "Nombre", "Nombre", 18, C_BLANCO,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -10), new Vector2(400, 28));
        _txtNombre.fontStyle = FontStyles.Bold;

        // Nivel
        _txtNivel = TMP_Anclado(stR, "Nivel", "LV  1", 14, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -42), new Vector2(200, 22));

        // HP con barra
        _txtHP = TMP_Anclado(stR, "TxtHP", "HP  0/0", 13, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -68), new Vector2(250, 20));
        _fillHP = CrearBarra(stR, "BarraHP", new Vector2(120, -90), new Vector2(380, 10), C_VERDE);

        // MP con barra
        _txtMP = TMP_Anclado(stR, "TxtMP", "MP  0/0", 13, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(120, -104), new Vector2(250, 20));
        _fillMP = CrearBarra(stR, "BarraMP", new Vector2(120, -126), new Vector2(380, 10), C_AZUL);

        // Barra XP (next level)
        var lblXP = TMP_Anclado(stR, "LblXP", "next level", 10, C_GRIS,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(520, -68), new Vector2(200, 16));
        _fillXP = CrearBarra(stR, "BarraXP", new Vector2(520, -86), new Vector2(300, 10), C_ORO);

        // Separador
        var sep1 = Nodo("Sep1", stR);
        var rtSep1 = sep1.GetComponent<RectTransform>();
        rtSep1.anchorMin = new Vector2(0, 1); rtSep1.anchorMax = new Vector2(1, 1);
        rtSep1.pivot = new Vector2(0, 1);
        rtSep1.anchoredPosition = new Vector2(10, -145); rtSep1.sizeDelta = new Vector2(-20, 2);
        sep1.AddComponent<Image>().color = C_BORDE;

        // Stats de combate
        float sy = -160f;
        _txtAtq = StatFila(stR, "Ataque", "ATQ", ref sy);
        _txtDef = StatFila(stR, "Defensa", "DEF", ref sy);
        _txtAgi = StatFila(stR, "Agilidad", "AGI", ref sy);
        _txtMag = StatFila(stR, "F.Mágica", "MAG", ref sy);
        _txtTer = StatFila(stR, "Terapeucidad", "TER", ref sy);

        // Separador
        var sep2 = Nodo("Sep2", stR);
        var rtSep2 = sep2.GetComponent<RectTransform>();
        rtSep2.anchorMin = new Vector2(0, 1); rtSep2.anchorMax = new Vector2(1, 1);
        rtSep2.pivot = new Vector2(0, 1);
        rtSep2.anchoredPosition = new Vector2(10, sy - 6); rtSep2.sizeDelta = new Vector2(-20, 2);
        sep2.AddComponent<Image>().color = C_BORDE;
        sy -= 18f;

        // Equipo equipado
        _txtArma = EquipoFila(stR, "Arma", "Arma", ref sy);
        _txtArmadura = EquipoFila(stR, "Armadura", "Armadura", ref sy);
        _txtEscudo = EquipoFila(stR, "Escudo", "Escudo", ref sy);
        _txtCasco = EquipoFila(stR, "Casco", "Casco", ref sy);
        _txtAccesorio = EquipoFila(stR, "Accesorio", "Accesorio", ref sy);

        // ── PANEL INVENTARIO ──────────────────────────────────────────────────
        _panelInventario = PanelFF("PanelInventario", _raiz.transform, new Vector2(20, 20), new Vector2(1120, 690));
        _panelInventario.SetActive(false);
        var invRelleno = _panelInventario.transform.Find("Relleno");
        Transform invR = invRelleno ?? _panelInventario.transform;

        // Título
        TMP_Anclado(invR, "TituloInv", "ITEM", 16, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 28));

        // ScrollView lista items
        var scrollInv = CrearScroll(invR, new Vector2(10, -46), new Vector2(-10, -200));
        _contenedorItems = scrollInv.GetComponentInChildren<VerticalLayoutGroup>().transform;

        // Panel info item seleccionado
        var infoGO = Nodo("InfoItem", invR);
        var rtInfo = infoGO.GetComponent<RectTransform>();
        rtInfo.anchorMin = new Vector2(0, 0); rtInfo.anchorMax = new Vector2(1, 0);
        rtInfo.pivot = new Vector2(0, 0);
        rtInfo.anchoredPosition = new Vector2(10, 60); rtInfo.sizeDelta = new Vector2(-20, 120);
        infoGO.AddComponent<Image>().color = C_MEDIO;

        _txtItemNombre = TMP_Anclado(infoGO.transform, "ItemNombre", "", 14, C_ORO,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(8, -8), new Vector2(-8, 26));
        _txtItemDesc = TMP_Anclado(infoGO.transform, "ItemDesc", "", 12, C_BLANCO,
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(8, -40), new Vector2(-8, 8));
        _txtItemDesc.enableWordWrapping = true;

        // Botón acción (Usar / Equipar)
        var btnAccGO = Nodo("BtnAccion", invR);
        var rtBA = btnAccGO.GetComponent<RectTransform>();
        rtBA.anchorMin = new Vector2(0, 0); rtBA.anchorMax = new Vector2(0, 0);
        rtBA.pivot = new Vector2(0, 0);
        rtBA.anchoredPosition = new Vector2(10, 10); rtBA.sizeDelta = new Vector2(160, 44);
        btnAccGO.AddComponent<Image>().color = C_CLARO;
        _btnAccion = btnAccGO.AddComponent<Button>();
        var cbA = _btnAccion.colors;
        cbA.normalColor = C_CLARO; cbA.highlightedColor = C_SEL; cbA.pressedColor = C_BORDE;
        _btnAccion.colors = cbA;
        var txtAccGO = Nodo("Txt", btnAccGO.transform);
        Stretch(txtAccGO.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        _txtBtnAccion = txtAccGO.AddComponent<TextMeshProUGUI>();
        _txtBtnAccion.text = "Usar"; _txtBtnAccion.fontSize = 15;
        _txtBtnAccion.color = C_BLANCO; _txtBtnAccion.alignment = TextAlignmentOptions.Center;
        if (fuentePixel) _txtBtnAccion.font = fuentePixel;
        btnAccGO.SetActive(false);

        // ── PANEL EQUIPO ──────────────────────────────────────────────────────
        _panelEquipo = PanelFF("PanelEquipo", _raiz.transform, new Vector2(20, 20), new Vector2(1120, 690));
        _panelEquipo.SetActive(false);
        var eqRelleno = _panelEquipo.transform.Find("Relleno");
        Transform eqR = eqRelleno ?? _panelEquipo.transform;

        TMP_Anclado(eqR, "TituloEq", "EQUIPO", 16, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 28));

        float ey = -50f;
        _eqArma = EquipoFila(eqR, "EqArma", "Arma", ref ey);
        _eqArmadura = EquipoFila(eqR, "EqArmadura", "Armadura", ref ey);
        _eqEscudo = EquipoFila(eqR, "EqEscudo", "Escudo", ref ey);
        _eqCasco = EquipoFila(eqR, "EqCasco", "Casco", ref ey);
        _eqAccesorio = EquipoFila(eqR, "EqAccesorio", "Accesorio", ref ey);

        var sep3 = Nodo("Sep3", eqR);
        var rtSep3 = sep3.GetComponent<RectTransform>();
        rtSep3.anchorMin = new Vector2(0, 1); rtSep3.anchorMax = new Vector2(1, 1);
        rtSep3.pivot = new Vector2(0, 1);
        rtSep3.anchoredPosition = new Vector2(10, ey - 6); rtSep3.sizeDelta = new Vector2(-20, 2);
        sep3.AddComponent<Image>().color = C_BORDE;
        ey -= 18f;

        _eqBonos = TMP_Anclado(eqR, "EqBonos", "", 13, C_ORO,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(10, ey), new Vector2(-10, 22));
    }

    // ═════════════════════════════════════════════════════════════════════════
    // FÁBRICA DE WIDGETS
    // ═════════════════════════════════════════════════════════════════════════

    GameObject PanelFF(string nombre, Transform padre, Vector2 posTopLeft, Vector2 tamano)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(posTopLeft.x, -posTopLeft.y);
        rt.sizeDelta = tamano;
        go.AddComponent<Image>().color = C_FONDO;
        var b = Nodo("Borde", go.transform); Stretch(b.GetComponent<RectTransform>(), new Vector2(2, 2), new Vector2(-2, -2));
        b.AddComponent<Image>().color = C_BORDE; b.GetComponent<Image>().raycastTarget = false;
        var r = Nodo("Relleno", go.transform); Stretch(r.GetComponent<RectTransform>(), new Vector2(4, 4), new Vector2(-4, -4));
        r.AddComponent<Image>().color = C_FONDO; r.GetComponent<Image>().raycastTarget = false;
        return go;
    }

    Button BotonSidebar(string etiqueta, Transform padre, float yFromTop,
        UnityEngine.Events.UnityAction accion)
    {
        var go = Nodo("Btn_" + etiqueta, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(0, -yFromTop);
        rt.sizeDelta = new Vector2(0, 44);
        var img = go.AddComponent<Image>(); img.color = C_MEDIO;
        var btn = go.AddComponent<Button>();
        var cb = btn.colors;
        cb.normalColor = C_MEDIO; cb.highlightedColor = C_CLARO; cb.pressedColor = C_SEL;
        btn.colors = cb;
        if (accion != null) btn.onClick.AddListener(accion);
        else btn.interactable = false;
        var tGO = Nodo("Txt", go.transform);
        Stretch(tGO.GetComponent<RectTransform>(), new Vector2(12, 0), new Vector2(-12, 0));
        var t = tGO.AddComponent<TextMeshProUGUI>();
        t.text = etiqueta; t.fontSize = 15; t.color = C_BLANCO;
        t.alignment = TextAlignmentOptions.Left;
        if (fuentePixel) t.font = fuentePixel;
        return btn;
    }

    void ColorGris(Button btn)
    {
        if (btn == null) return;
        var img = btn.GetComponent<Image>();
        if (img) img.color = new Color(0.15f, 0.15f, 0.25f, 1f);
        var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.color = C_GRIS;
    }

    Image CrearBarra(Transform padre, string nombre, Vector2 pos, Vector2 size, Color color)
    {
        // Fondo de la barra
        var bg = Nodo(nombre + "_BG", padre);
        var rtBG = bg.GetComponent<RectTransform>();
        rtBG.anchorMin = new Vector2(0, 1); rtBG.anchorMax = new Vector2(0, 1);
        rtBG.pivot = new Vector2(0, 1);
        rtBG.anchoredPosition = pos; rtBG.sizeDelta = size;
        bg.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);

        // Relleno (Image con fillMethod)
        var fill = Nodo(nombre + "_Fill", bg.transform);
        Stretch(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var imgFill = fill.AddComponent<Image>();
        imgFill.color = color;
        imgFill.type = Image.Type.Filled;
        imgFill.fillMethod = Image.FillMethod.Horizontal;
        imgFill.fillAmount = 1f;
        return imgFill;
    }

    // Crea una fila "LABEL  valor" para stats de combate
    TextMeshProUGUI StatFila(Transform padre, string id, string label, ref float y)
    {
        var lbl = TMP_Anclado(padre, "Lbl_" + id, label, 12, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, y), new Vector2(120, 20));
        var val = TMP_Anclado(padre, "Val_" + id, "0", 12, C_BLANCO,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(160, y), new Vector2(120, 20));
        y -= 24f;
        return val;
    }

    // Crea una fila "SLOT  nombre_equipo"
    TextMeshProUGUI EquipoFila(Transform padre, string id, string label, ref float y)
    {
        var lbl = TMP_Anclado(padre, "Lbl_" + id, label, 12, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(20, y), new Vector2(140, 20));
        var val = TMP_Anclado(padre, "Val_" + id, "——", 12, C_BLANCO,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(170, y), new Vector2(500, 20));
        y -= 26f;
        return val;
    }

    TextMeshProUGUI TMP_Anclado(Transform padre, string nombre, string texto,
        float fs, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = fs; t.color = color;
        t.alignment = TextAlignmentOptions.Left;
        t.overflowMode = TextOverflowModes.Ellipsis;
        if (fuentePixel) t.font = fuentePixel;
        return t;
    }

    GameObject CrearScroll(Transform padre, Vector2 offsetMin, Vector2 offsetMax)
    {
        var go = Nodo("Scroll", padre);
        Stretch(go.GetComponent<RectTransform>(), offsetMin, offsetMax);
        go.AddComponent<Image>().color = Color.clear;
        var sr = go.AddComponent<ScrollRect>();
        sr.horizontal = false; sr.scrollSensitivity = 30;
        var vp = Nodo("Viewport", go.transform);
        Stretch(vp.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        vp.AddComponent<RectMask2D>();
        sr.viewport = vp.GetComponent<RectTransform>();
        var ct = Nodo("Content", vp.transform);
        var rtC = ct.GetComponent<RectTransform>();
        rtC.anchorMin = new Vector2(0, 1); rtC.anchorMax = new Vector2(1, 1);
        rtC.pivot = new Vector2(0.5f, 1);
        rtC.sizeDelta = Vector2.zero; rtC.anchoredPosition = Vector2.zero;
        var vlg = ct.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 2; vlg.childControlWidth = true; vlg.childForceExpandWidth = true;
        vlg.childControlHeight = false; vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(4, 4, 4, 4);
        ct.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sr.content = rtC;
        return go;
    }

    GameObject Nodo(string nombre, Transform padre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        return go;
    }

    void Stretch(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}