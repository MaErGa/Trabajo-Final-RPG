# Guía de montaje – Menú FF7 adaptado a tus scripts
=====================================================

## Scripts que NO debes tocar (ya funcionan)
- DatosJugador.cs
- EquipoBase.cs
- ItemConsumible.cs
- ConjuroBase.cs
- GlobalEnums.cs

## Scripts nuevos / reemplazados
- **MenuController.cs**  → nuevo, reemplaza al que te generé antes
- **InventoryManager.cs** → reemplaza tu versión anterior (compatible)

---

## 1. Jerarquía UI en Unity

```
Canvas  (Screen Space – Overlay, 1920×1080)
 └─ MenuRoot  (880×580, fondo #0A1850)
     │
     ├─ SidebarPanel  (150×520, lado derecho)
     │   ├─ Button "btnEstado"  → texto "Estado"
     │   ├─ Button "btnItem"    → texto "Item"
     │   ├─ Button "btnEquipo"  → texto "Equipo"
     │   ├─ Separator
     │   └─ TMP "Guardar / Salir" (placeholder)
     │
     ├─ HUD_Bottom  (880×40, abajo del todo)
     │   ├─ TMP "Tiempo:"  + TMP [tmpTiempo]
     │   └─ TMP "Oro:"     + TMP [tmpOro]   ← también está en statsPanel
     │
     ├─ ── PANEL STATS (statsPanel) ────────────────────────────────────
     │   StatsPanel  (700×520)
     │   │
     │   ├─ TMP [tmpNombreJugador]   (20pt, blanco)
     │   │
     │   ├─ Row "Nivel / EXP"
     │   │   ├─ TMP "Nivel"  + TMP [tmpNivel]
     │   │   └─ TMP "EXP"   + TMP [tmpExp] + TMP "/" + TMP [tmpExpSiguiente]
     │   │
     │   ├─ Row "HP"
     │   │   ├─ TMP "HP"  (cyan)
     │   │   ├─ TMP [tmpHpActual]  + TMP "/"  + TMP [tmpHpMax]
     │   │   └─ Slider [sliderHP]  (Min=0, Max=1, no interactivo, relleno verde)
     │   │
     │   ├─ Row "MP"
     │   │   ├─ TMP "MP"  (cyan)
     │   │   ├─ TMP [tmpMpActual]  + TMP "/"  + TMP [tmpMpMax]
     │   │   └─ Slider [sliderMP]  (Min=0, Max=1, no interactivo, relleno azul)
     │   │
     │   ├─ Separator
     │   │
     │   ├─ Row "Stats de combate"
     │   │   ├─ TMP "ATQ" + TMP [tmpAtaque]
     │   │   ├─ TMP "DEF" + TMP [tmpDefensa]
     │   │   ├─ TMP "AGI" + TMP [tmpAgilidad]
     │   │   ├─ TMP "MAG" + TMP [tmpFuerzaMagica]
     │   │   └─ TMP "TER" + TMP [tmpTerapeucidad]
     │   │
     │   ├─ Separator
     │   │
     │   └─ Row "Equipo puesto"
     │       ├─ TMP "Arma"      + TMP [tmpArma]
     │       ├─ TMP "Armadura"  + TMP [tmpArmadura]
     │       ├─ TMP "Escudo"    + TMP [tmpEscudo]
     │       ├─ TMP "Casco"     + TMP [tmpCasco]
     │       └─ TMP "Accesorio" + TMP [tmpAccesorio]
     │
     ├─ ── PANEL INVENTARIO (inventarioPanel, inactivo al inicio) ───────
     │   InventarioPanel  (700×520)
     │   ├─ TMP "INVENTARIO" (título cyan)
     │   ├─ ScrollView  (420×280)
     │   │   └─ Content  → arrastra a [contenedorPadre] del InventoryManager
     │   ├─ TMP [textoNombre]       (nombre del item seleccionado)
     │   ├─ TMP [textoDescripcion]  (descripción larga)
     │   └─ Button [botonUsar]      (inicialmente INACTIVO)
     │       └─ TMP [textoBotonUsar]  texto dinámico "Usar" / "Equipar"
     │
     └─ ── PANEL EQUIPO (equipoPanel, inactivo al inicio) ────────────────
         EquipoPanel  (700×520)
         ├─ TMP "EQUIPACIÓN ACTUAL" (título cyan)
         ├─ Grid 2 columnas:
         │   ├─ TMP "Arma:"      + TMP [equipTmpArma]
         │   ├─ TMP "Armadura:"  + TMP [equipTmpArmadura]
         │   ├─ TMP "Escudo:"    + TMP [equipTmpEscudo]
         │   ├─ TMP "Casco:"     + TMP [equipTmpCasco]
         │   └─ TMP "Accesorio:" + TMP [equipTmpAccesorio]
         └─ TMP [equipTmpBonos]  (resumen ATQ/DEF/AGI totales)
```

---

## 2. Prefab "SlotItem" (para las filas del inventario)

```
SlotItem  (LayoutElement, altura preferida 38px)
 ├─ Image fondo (color #0D2070, full-stretch)
 └─ Button (cubre todo el objeto)
     └─ TMP "textoSlot"  (12pt, blanco, alineado izquierda, padding 8px)
```

- Guarda el prefab en `Assets/Prefabs/UI/SlotItem.prefab`
- Arrástralo a `prefabSlot` del InventoryManager en el Inspector

---

## 3. Configurar MenuController en Inspector

1. Crea un **GameObject vacío** llamado `MenuController` dentro de `MenuRoot`
2. Añádele el script `MenuController.cs`
3. Arrastra tu **ScriptableObject DatosJugador** (el de Ryo / tu jugador) al campo `datosJugador`
4. Arrastra cada panel y cada TMP a sus campos correspondientes
5. Arrastra los **Buttons del sidebar** a `btnItem`, `btnEquipo`, `btnEstado`

---

## 4. Configurar InventoryManager en Inspector

El `InventoryManager` va en el **GameObject del InventarioPanel** (o en un hijo).

| Campo              | Qué arrastrar                                    |
|--------------------|--------------------------------------------------|
| datosJugador       | Tu ScriptableObject de jugador                   |
| prefabSlot         | Assets/Prefabs/UI/SlotItem.prefab                |
| contenedorPadre    | El objeto Content dentro del ScrollView          |
| textoNombre        | TMP del nombre del item seleccionado             |
| textoDescripcion   | TMP de la descripción larga                      |
| botonUsar          | El Button "Usar/Equipar" (se activa al pulsar)   |
| textoBotonUsar     | El TMP hijo de botonUsar                         |

---

## 5. Conectar InventoryManager al MenuController

En el Inspector del `MenuController`, arrastra el `InventoryManager`
del InventarioPanel al campo `inventoryManager`.

Esto permite que al volver desde el inventario a stats, los datos se refresquen.

---

## 6. Paleta de colores

| Elemento              | Color hex  |
|-----------------------|------------|
| Fondo panel           | #0A1850    |
| Fondo fila (par)      | #0D2070    |
| Fondo fila hover      | #1A3AB8    |
| Texto principal       | #FFFFFF    |
| Etiquetas HP/MP/stats | #00E5CC    |
| Texto seleccionado    | #FFE060    |
| Borde paneles         | #2244AA    |
| Barra HP              | #44CC66    |
| Barra MP              | #4488FF    |

---

## 7. Flujo de uso

```
Abrir menú
  │
  ├─ [Estado]   → Stats del jugador (nombre, nivel, HP, MP, ATQ, DEF...)
  │               + equipo actualmente puesto
  │
  ├─ [Item]     → Lista de mochilaItems + plantasMedicinales + armarioEquipo
  │               Clic en consumible → botón "Usar" → aplica efecto → se elimina
  │               Clic en equipo     → botón "Equipar" → llama datosJugador.EquiparObjeto()
  │
  └─ [Equipo]   → Vista rápida de los 5 slots equipados + bonos totales
```

---

## 8. Añadir más items consumibles fácilmente

1. Clic derecho → Create → RPG → Item Consumible
2. Rellena `nombre`, `queCura` (TipoEfecto.Vida/Mana/Antidoto), `potencia`
3. En runtime: `datosJugador.mochilaItems.Add(miItem);`
   y llama `inventoryManager.ActualizarInventarioUI();`

El menú lo mostrará automáticamente en la siguiente apertura.
