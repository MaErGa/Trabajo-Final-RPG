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
    Button _btnEstado, _btnItem, _btnEquipo, _btnSalir, _btnMagia, _btnConfig;

    // Panel Config inline
    GameObject      _panelConfig;
    UnityEngine.UI.Slider _sliderVolConf;
    UnityEngine.UI.Slider _sliderCRTConf;
    TMPro.TextMeshProUGUI _txtVolConf, _txtCRTConf;

    // Panel stats GO (para mostrar/ocultar)
    GameObject _panelStats;

    // Panel magia
    GameObject      _panelMagia;
    Transform       _contenedorMagias;
    ConjuroBase     _conjuroSel;
    TextMeshProUGUI _txtMagNombre, _txtMagDesc;
    Button          _btnMagAccion;
    TextMeshProUGUI _txtBtnMagAccion;

    // Texto XP numérico
    TextMeshProUGUI _txtXP;

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
        if (_panelMagia  != null) _panelMagia.SetActive(false);
        if (_panelConfig != null) _panelConfig.SetActive(false);
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
        _btnMagia.onClick.RemoveAllListeners();
        if (tieneMagia) _btnMagia.onClick.AddListener(AbrirMagia);
        var img = _btnMagia.GetComponent<Image>();
        if (img) img.color = tieneMagia ? C_MEDIO : new Color(0.15f, 0.15f, 0.25f, 1f);
        var txt = _btnMagia.GetComponentInChildren<TextMeshProUGUI>();
        if (txt) txt.color = tieneMagia ? C_BLANCO : C_GRIS;
    }

    void IrAlTitulo()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Titulo");
    }

    void AbrirConfig()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(false);
        _panelEquipo.SetActive(false);
        if (_panelMagia   != null) _panelMagia.SetActive(false);
        if (_panelConfig  != null) _panelConfig.SetActive(true);
        ResaltarBtn(_btnConfig);
        // Sincronizar sliders con valores actuales
        if (_sliderVolConf != null) _sliderVolConf.value = PlayerPrefs.GetFloat("vol_general", 1f);
        if (_sliderCRTConf != null) _sliderCRTConf.value = PlayerPrefs.GetFloat("crt_intensidad", 0.35f);
    }

    void AbrirMagia()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(false);
        _panelEquipo.SetActive(false);
        _panelMagia.SetActive(true);
        ResaltarBtn(_btnMagia);
        if (_txtMagNombre) _txtMagNombre.text = "";
        if (_txtMagDesc)   _txtMagDesc.text   = "";
        if (_btnMagAccion) _btnMagAccion.gameObject.SetActive(false);
        _conjuroSel = null;
        RefrescarMagia();
    }

    void RefrescarMagia()
    {
        if (datosJugador == null || _contenedorMagias == null) return;
        LimpiarContenedor(_contenedorMagias);

        if (datosJugador.conjurosAprendidos == null || datosJugador.conjurosAprendidos.Count == 0)
        {
            CrearFilaInventario_En(_contenedorMagias, "Sin conjuros aprendidos", "", null);
            return;
        }
        foreach (var conjuro in datosJugador.conjurosAprendidos)
        {
            if (conjuro == null) continue;
            var cap = conjuro;
            string coste = cap.costeMP > 0 ? cap.costeMP + " MP" : "--";
            CrearFilaInventario_En(_contenedorMagias, cap.nombreConjuro, coste,
                () => SeleccionarConjuro(cap));
        }
    }

    void SeleccionarConjuro(ConjuroBase conjuro)
    {
        _conjuroSel = conjuro;
        if (_txtMagNombre) _txtMagNombre.text = conjuro.nombreConjuro;
        string desc = conjuro.descripcion;
        if (conjuro.costeMP > 0) desc += $"   Coste: {conjuro.costeMP} MP";
        if (conjuro.valorEfecto > 0) desc += $"   Efecto: +{conjuro.valorEfecto}";
        if (_txtMagDesc) _txtMagDesc.text = desc;

        if ((int)conjuro.tipo == 0)
        {
            if (_btnMagAccion) _btnMagAccion.gameObject.SetActive(true);
            if (_btnMagAccion)
            {
                _btnMagAccion.onClick.RemoveAllListeners();
                _btnMagAccion.onClick.AddListener(UsarConjuro);
            }
            if (_txtBtnMagAccion) _txtBtnMagAccion.text = "Usar";
        }
        else
        {
            if (_btnMagAccion) _btnMagAccion.gameObject.SetActive(false);
            if (_txtMagDesc) _txtMagDesc.text += "\n(Solo usable en combate)";
        }
    }

    void UsarConjuro()
    {
        if (_conjuroSel == null || datosJugador == null) return;
        if (datosJugador.mpActual < _conjuroSel.costeMP)
        {
            if (_txtMagDesc) _txtMagDesc.text = "¡No tienes suficiente MP!";
            if (_btnMagAccion) _btnMagAccion.gameObject.SetActive(false);
            return;
        }
        datosJugador.mpActual -= _conjuroSel.costeMP;
        datosJugador.hpActual  = Mathf.Min(datosJugador.hpMax,
                                            datosJugador.hpActual + _conjuroSel.valorEfecto);
        if (_txtMagDesc)
            _txtMagDesc.text = $"¡{_conjuroSel.nombreConjuro} usado!\n+{_conjuroSel.valorEfecto} HP  |  MP: {datosJugador.mpActual}/{datosJugador.mpMax}";
        if (_btnMagAccion) _btnMagAccion.gameObject.SetActive(false);
        _conjuroSel = null;
        RefrescarStats();
    }

    void AbrirInventario()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(true);
        _panelEquipo.SetActive(false);
        if (_panelMagia  != null) _panelMagia.SetActive(false);
        if (_panelConfig != null) _panelConfig.SetActive(false);
        ResaltarBtn(_btnItem);
        RefrescarInventario();
    }

    void AbrirEquipo()
    {
        _panelStats.SetActive(false);
        _panelInventario.SetActive(false);
        _panelEquipo.SetActive(true);
        if (_panelMagia  != null) _panelMagia.SetActive(false);
        if (_panelConfig != null) _panelConfig.SetActive(false);
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
        if (_txtXP)
        {
            if (datosJugador.nivel >= 99)
                _txtXP.text = "MAX";
            else
            {
                int falta = Mathf.Max(0, datosJugador.expSiguienteNivel - datosJugador.experiencia);
                _txtXP.text = $"{datosJugador.experiencia}/{datosJugador.expSiguienteNivel}  (-{falta})";
            }
        }

        if (_txtAtq) _txtAtq.text = datosJugador.AtaqueTotal.ToString();
        if (_txtDef) _txtDef.text = datosJugador.DefensaTotal.ToString();
        if (_txtAgi) _txtAgi.text = datosJugador.AgilidadTotal.ToString();
        if (_txtMag) _txtMag.text = datosJugador.fuerzaMagica.ToString();
        if (_txtTer) _txtTer.text = datosJugador.terapeucidad.ToString();

        if (_txtArma) _txtArma.text = datosJugador.armaEquipadaAsset != null ? datosJugador.armaEquipadaAsset.nombre : "--";
        if (_txtArmadura) _txtArmadura.text = datosJugador.armaduraEquipadaAsset != null ? datosJugador.armaduraEquipadaAsset.nombre : "--";
        if (_txtEscudo) _txtEscudo.text = datosJugador.escudoEquipadoAsset != null ? datosJugador.escudoEquipadoAsset.nombre : "--";
        if (_txtCasco) _txtCasco.text = datosJugador.cascoEquipadoAsset != null ? datosJugador.cascoEquipadoAsset.nombre : "--";
        if (_txtAccesorio) _txtAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "--";

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

        // Éter (sistema antiguo)
        if (datosJugador.eter > 0)
            CrearFilaInventario($"Éter  x{datosJugador.eter}", "+30 MP",
                () =>
                {
                    _itemSel = null; _equipoSel = null;
                    if (_txtItemNombre) _txtItemNombre.text = "Éter";
                    if (_txtItemDesc) _txtItemDesc.text = "Restaura 30 MP.";
                    MostrarBtnAccion("Usar", UsarEter);
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
        if (_eqArma) _eqArma.text = datosJugador.armaEquipadaAsset != null ? datosJugador.armaEquipadaAsset.nombre : "--";
        if (_eqArmadura) _eqArmadura.text = datosJugador.armaduraEquipadaAsset != null ? datosJugador.armaduraEquipadaAsset.nombre : "--";
        if (_eqEscudo) _eqEscudo.text = datosJugador.escudoEquipadoAsset != null ? datosJugador.escudoEquipadoAsset.nombre : "--";
        if (_eqCasco) _eqCasco.text = datosJugador.cascoEquipadoAsset != null ? datosJugador.cascoEquipadoAsset.nombre : "--";
        if (_eqAccesorio) _eqAccesorio.text = datosJugador.accesorioEquipadoAsset != null ? datosJugador.accesorioEquipadoAsset.nombre : "--";

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

    void UsarEter()
    {
        if (datosJugador.eter <= 0) return;
        datosJugador.mpActual = Mathf.Min(datosJugador.mpMax, datosJugador.mpActual + 30);
        datosJugador.eter--;
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
        foreach (var b in new[] { _btnEstado, _btnItem, _btnEquipo, _btnMagia, _btnConfig })
        {
            if (b == null) continue;
            var img = b.GetComponent<Image>();
            if (img) img.color = (b == activo) ? C_CLARO : C_MEDIO;
        }
    }

    // Slider para el panel config (340×20)
    UnityEngine.UI.Slider CrearSliderOpciones(Transform padre, string nombre, Vector2 pos)
    {
        var go = Nodo(nombre, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(340, 20);

        var bg = Nodo("Background", go.transform);
        Stretch(bg.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        bg.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 1f);

        var fillArea = Nodo("Fill Area", go.transform);
        var rtFA = fillArea.GetComponent<RectTransform>();
        rtFA.anchorMin = Vector2.zero; rtFA.anchorMax = Vector2.one;
        rtFA.offsetMin = new Vector2(5, 2); rtFA.offsetMax = new Vector2(-5, -2);

        var fill = Nodo("Fill", fillArea.transform);
        Stretch(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var imgFill = fill.AddComponent<Image>();
        imgFill.color = C_BORDE;

        var handleArea = Nodo("Handle Slide Area", go.transform);
        Stretch(handleArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        var handle = Nodo("Handle", handleArea.transform);
        handle.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 0);
        var imgHandle = handle.AddComponent<Image>();
        imgHandle.color = Color.white;

        var slider = go.AddComponent<UnityEngine.UI.Slider>();
        slider.fillRect   = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = imgHandle;
        slider.direction  = UnityEngine.UI.Slider.Direction.LeftToRight;
        slider.minValue = 0f; slider.maxValue = 1f;
        return slider;
    }

    void EtiquetaConfig(Transform padre, string id, string texto, float y)
    {
        var go = Nodo(id, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(20, y); rt.sizeDelta = new Vector2(300, 22);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = 13; t.color = C_CYAN;
        t.alignment = TextAlignmentOptions.Left;
        if (fuentePixel) t.font = fuentePixel;
    }

    // Separador horizontal
    void BarraSep(Transform padre, float y)
    {
        var go = Nodo("Sep", padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(10, y); rt.sizeDelta = new Vector2(-20, 2);
        go.AddComponent<Image>().color = C_BORDE;
    }

    string EfectoTexto(TipoEfecto e)
    {
        return e switch { TipoEfecto.Vida => "HP", TipoEfecto.Mana => "MP", TipoEfecto.Antidoto => "Antídoto", _ => "" };
    }

    void CrearFilaInventario(string nombre, string detalle, UnityEngine.Events.UnityAction accion)
        => CrearFilaInventario_En(_contenedorItems, nombre, detalle, accion);

    void CrearFilaInventario_En(Transform contenedor, string nombre, string detalle,
        UnityEngine.Events.UnityAction accion)
    {
        var fila = Nodo("Fila_" + nombre, contenedor);
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
        // Config: abre panel inline
        _btnConfig = BotonSidebar("Config", sbR, 208, () => AbrirConfig());

        // Titulo: vuelve al menú principal
        BotonSidebar("Titulo", sbR, 588, IrAlTitulo);

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
        rtP.sizeDelta = new Vector2(100, 126);
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
        _txtXP = TMP_Anclado(stR, "TxtXP", "0/0", 10, C_ORO,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(520, -100), new Vector2(300, 16));

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
        var scrollInv = CrearScroll(invR, new Vector2(10, -46), new Vector2(-10, -120));
        _contenedorItems = scrollInv.GetComponentInChildren<VerticalLayoutGroup>().transform;

        // Panel info item seleccionado
        var infoGO = Nodo("InfoItem", invR);
        var rtInfo = infoGO.GetComponent<RectTransform>();
        rtInfo.anchorMin = new Vector2(0, 0); rtInfo.anchorMax = new Vector2(1, 0);
        rtInfo.pivot = new Vector2(0, 0);
        rtInfo.anchoredPosition = new Vector2(10, 50); rtInfo.sizeDelta = new Vector2(-20, 62);
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

        // ── PANEL MAGIA ───────────────────────────────────────────────────────
        _panelMagia = PanelFF("PanelMagia", _raiz.transform, new Vector2(20, 20), new Vector2(1120, 690));
        _panelMagia.SetActive(false);
        var magRelleno = _panelMagia.transform.Find("Relleno");
        Transform magR = magRelleno ?? _panelMagia.transform;

        TMP_Anclado(magR, "TituloMag", "MAGIA", 16, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 28));

        // Lista conjuros con scroll (deja espacio abajo para info)
        var scrollMag = CrearScroll(magR, new Vector2(10, -46), new Vector2(-10, -120));
        _contenedorMagias = scrollMag.GetComponentInChildren<VerticalLayoutGroup>().transform;

        // Panel info conjuro seleccionado
        var infoMagGO = Nodo("InfoMagia", magR);
        var rtIM = infoMagGO.GetComponent<RectTransform>();
        rtIM.anchorMin = new Vector2(0, 0); rtIM.anchorMax = new Vector2(1, 0);
        rtIM.pivot = new Vector2(0, 0);
        rtIM.anchoredPosition = new Vector2(10, 50); rtIM.sizeDelta = new Vector2(-20, 62);
        infoMagGO.AddComponent<Image>().color = C_MEDIO;

        _txtMagNombre = TMP_Anclado(infoMagGO.transform, "MagNombre", "", 14, C_ORO,
            new Vector2(0, 1), new Vector2(1, 1), new Vector2(8, -8), new Vector2(-8, 26));
        _txtMagDesc = TMP_Anclado(infoMagGO.transform, "MagDesc", "", 12, C_BLANCO,
            new Vector2(0, 0), new Vector2(1, 1), new Vector2(8, -38), new Vector2(-8, 8));
        _txtMagDesc.enableWordWrapping = true;

        // Botón Usar
        var btnMagGO = Nodo("BtnUsarMagia", magR);
        var rtBM = btnMagGO.GetComponent<RectTransform>();
        rtBM.anchorMin = new Vector2(0, 0); rtBM.anchorMax = new Vector2(0, 0);
        rtBM.pivot = new Vector2(0, 0);
        rtBM.anchoredPosition = new Vector2(10, 8); rtBM.sizeDelta = new Vector2(160, 38);
        btnMagGO.AddComponent<Image>().color = C_CLARO;
        _btnMagAccion = btnMagGO.AddComponent<Button>();
        var cbMag = _btnMagAccion.colors;
        cbMag.normalColor = C_CLARO; cbMag.highlightedColor = C_SEL; cbMag.pressedColor = C_BORDE;
        _btnMagAccion.colors = cbMag;
        var txtMagGO = Nodo("Txt", btnMagGO.transform);
        Stretch(txtMagGO.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
        _txtBtnMagAccion = txtMagGO.AddComponent<TextMeshProUGUI>();
        _txtBtnMagAccion.text = "Usar"; _txtBtnMagAccion.fontSize = 15;
        _txtBtnMagAccion.color = C_BLANCO; _txtBtnMagAccion.alignment = TextAlignmentOptions.Center;
        if (fuentePixel) _txtBtnMagAccion.font = fuentePixel;
        btnMagGO.SetActive(false);

        // ── PANEL CONFIG inline ───────────────────────────────────────────────
        _panelConfig = PanelFF("PanelConfig", _raiz.transform, new Vector2(20, 20), new Vector2(1120, 690));
        _panelConfig.SetActive(false);
        var cfgRelleno = _panelConfig.transform.Find("Relleno");
        Transform cfgR = cfgRelleno ?? _panelConfig.transform;

        TMP_Anclado(cfgR, "TituloConf", "CONFIG", 16, C_CYAN,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 28));

        // Fila Volumen
        float cy = -70f;
        EtiquetaConfig(cfgR, "LblVol", "Volumen General", cy);
        _sliderVolConf = CrearSliderOpciones(cfgR, "SliderVol", new Vector2(330, cy - 10));
        _sliderVolConf.value = PlayerPrefs.GetFloat("vol_general", 1f);
        _sliderVolConf.onValueChanged.AddListener(OnVolumenCambiado);
        _txtVolConf = TextoValorSlider(cfgR, "TxtVol",
            Mathf.RoundToInt(_sliderVolConf.value * 100).ToString(), cy);

        // Separador
        BarraSep(cfgR, -110f);

        // Fila CRT
        cy = -130f;
        EtiquetaConfig(cfgR, "LblCRT", "Filtro CRT", cy);
        _sliderCRTConf = CrearSliderOpciones(cfgR, "SliderCRT", new Vector2(330, cy - 10));
        _sliderCRTConf.value = PlayerPrefs.GetFloat("crt_intensidad", 0.35f);
        _sliderCRTConf.onValueChanged.AddListener(OnCRTCambiado);
        _txtCRTConf = TextoValorSlider(cfgR, "TxtCRT",
            Mathf.RoundToInt(_sliderCRTConf.value * 100).ToString(), cy);

        BarraSep(cfgR, -170f);
    }

    TextMeshProUGUI TextoValorSlider(Transform padre, string id, string texto, float y)
    {
        var go = Nodo(id, padre);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1); rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(682, y - 4);
        rt.sizeDelta = new Vector2(60, 22);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = texto; t.fontSize = 13; t.color = C_BLANCO;
        t.alignment = TextAlignmentOptions.Left;
        if (fuentePixel) t.font = fuentePixel;
        return t;
    }

    void OnVolumenCambiado(float v)
    {
        AudioListener.volume = v;
        PlayerPrefs.SetFloat("vol_general", v);
        if (_txtVolConf) _txtVolConf.text = Mathf.RoundToInt(v * 100).ToString();
    }

    void OnCRTCambiado(float v)
    {
        if (CRTEffect.instancia != null) CRTEffect.instancia.SetIntensidad(v);
        else PlayerPrefs.SetFloat("crt_intensidad", v);
        if (_txtCRTConf) _txtCRTConf.text = Mathf.RoundToInt(v * 100).ToString();
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
        var val = TMP_Anclado(padre, "Val_" + id, "--", 12, C_BLANCO,
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