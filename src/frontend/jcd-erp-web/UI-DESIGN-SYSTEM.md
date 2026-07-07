# JCD ERP — Sistema de Diseño UI

> **Documento oficial de interfaz.** Consultar antes de crear pantallas o componentes.  
> **Implementación:** `src/styles/` · **Última actualización:** 2026-07-07

---

## 1. Identidad visual

| Atributo | Definición |
|----------|------------|
| **Estilo** | SaaS empresarial moderno, limpio, confiable |
| **Referentes** | Stripe Dashboard, Linear, SAP Fiori (modernizado) |
| **Tono** | Profesional, sobrio, sin decoración excesiva |
| **Idioma UI** | Español (i18n EN pendiente) |

### Principios

1. **Claridad** — jerarquía visual obvia, sin ruido.
2. **Consistencia** — mismos tokens en toda la app.
3. **Densidad equilibrada** — espacio para respirar, pero eficiente para ERP.
4. **Sidebar oscura fija** — identidad enterprise en light y dark mode.
5. **Solo tokens CSS** — nunca colores hardcodeados en componentes (salvo auth panel).

---

## 2. Tipografía

| Uso | Token / valor |
|-----|----------------|
| Familia | `Plus Jakarta Sans` (`--font-sans`) |
| Código | `JetBrains Mono` (`--font-mono`) |
| Título página | `--text-3xl` + `--font-bold` |
| Título sección | `--text-2xl` + `--font-bold` |
| Cuerpo | `--text-base` |
| Labels | `--text-sm` + `--font-semibold` |
| Captions | `--text-xs` |

---

## 3. Paleta de color

### Marca

| Token | Light | Uso |
|-------|-------|-----|
| `--brand-primary` | `#1d5bff` | Botones, links, acentos |
| `--brand-primary-hover` | `#1648cc` | Hover botón primario |
| `--brand-primary-soft` | `#e8efff` | Fondos suaves, iconos KPI |
| `--brand-accent` | `#0ea5e9` | Gradientes, detalles |

### Superficies (contenido)

| Token | Uso |
|-------|-----|
| `--color-bg` | Fondo general del área de contenido |
| `--color-surface` | Cards, header, tablas |
| `--color-surface-muted` | Inputs, headers de tabla |
| `--color-border` | Bordes estándar |

### Sidebar (siempre oscura)

| Token | Valor |
|-------|-------|
| `--sidebar-bg` | `#0b1220` |
| `--sidebar-text` | `#94a3b8` |
| `--sidebar-text-active` | `#ffffff` |
| `--sidebar-accent` | `#4d8dff` |

### Semánticos

| Token | Uso |
|-------|-----|
| `--color-success` | Activo, confirmado |
| `--color-warning` | Advertencias |
| `--color-error` | Errores, validación |
| `--color-info` | Información, badges |

---

## 4. Espaciado y forma

- **Grid base:** 4px (`--space-1` = 0.25rem)
- **Padding contenido:** `--space-6` (1.5rem)
- **Radius cards:** `--radius-lg` (1rem)
- **Radius inputs/botones:** `--radius-md` (0.75rem)
- **Sombras:** `--shadow-xs` a `--shadow-lg` (progresivas)

---

## 5. Estructura de layouts

### App autenticada (`main-layout`)

```
┌──────────────┬────────────────────────────────────┐
│   Sidebar    │  Header (usuario + acciones)       │
│   (oscuro)   ├────────────────────────────────────┤
│              │  Contenido (router-outlet)         │
│   Nav items  │                                    │
└──────────────┴────────────────────────────────────┘
```

- Sidebar ancho: `--sidebar-width` (17rem)
- Header alto: `--header-height` (4rem)
- Contenido max: `--content-max-width` (80rem)

### Auth (`auth-layout`)

- Panel izquierdo: gradiente marca + grid pattern + features
- Panel derecho: card centrada (`.auth-card`)

---

## 6. Componentes globales (clases CSS)

> Definidos en `src/styles/_components.scss`. **Reutilizar siempre.**

| Clase | Uso |
|-------|-----|
| `.page` / `.page__header` | Encabezado de pantalla |
| `.card` | Contenedor genérico |
| `.kpi-card` / `.kpi-grid` | Métricas dashboard |
| `.form-field` | Label + input + error |
| `.form-row` | Dos columnas en formularios |
| `.btn` `.btn--primary` `.btn--secondary` `.btn--ghost` | Botones |
| `.alert` `.alert--error` | Mensajes |
| `.auth-card` | Login / registro |
| `.table-wrapper` + `.data-table` | Listados |
| `.badge` | Estados (success, info, muted) |
| `.avatar` | Iniciales de usuario |

### Reglas de botones

| Acción principal | `.btn.btn--primary` |
| Acción secundaria | `.btn.btn--secondary` |
| Acción terciaria / toolbar | `.btn.btn--ghost` |
| Ancho completo en forms | + `.btn--block` |

---

## 7. Patrones por tipo de pantalla

### Listado (Users, Roles, Settings)

```html
<section class="page">
  <header class="page__header">
    <h1>Título</h1>
    <p>Descripción breve.</p>
  </header>
  <div class="table-wrapper">
    <table class="data-table">...</table>
  </div>
</section>
```

### Formulario modal / página

- Usar `.form-field` por campo
- Validación: `.form-field__error`
- Ayuda: `.form-field__hint`

### Dashboard / KPIs

- Usar `.kpi-grid` + `.kpi-card`
- Icono en `.kpi-card__icon` (SVG 20px)
- Valor en `.kpi-card__value`
- Contexto en `.kpi-card__hint`

---

## 8. Dark / Light mode

- Atributo en `<html>`: `data-theme="light"` | `data-theme="dark"`
- Servicio: `ThemeService` (`core/theme/theme.service.ts`)
- **Sidebar no cambia** — siempre tema oscuro enterprise
- El contenido principal sí alterna según tema

---

## 9. Iconografía

- **Estilo:** SVG outline, stroke 2px, sin relleno
- **Tamaño nav:** 20px (1.25rem)
- **Tamaño KPI:** 20px dentro de contenedor 40px
- **No usar emojis** en navegación ni acciones principales

---

## 10. Reglas para desarrolladores

| # | Regla |
|---|-------|
| UI-01 | Usar variables CSS (`var(--token)`), no hex en componentes |
| UI-02 | Estilos globales en `src/styles/`, no duplicar en features |
| UI-03 | Component SCSS solo para layout específico del componente |
| UI-04 | Textos de UI en español hasta implementar i18n |
| UI-05 | Toda pantalla nueva usa `.page` + `.page__header` |
| UI-06 | Tablas siempre dentro de `.table-wrapper` |
| UI-07 | Un solo botón primario por vista |
| UI-08 | Actualizar este doc al agregar tokens o componentes |

---

## 11. Archivos del design system

```
src/styles/
├── _tokens.scss      # Variables (source of truth)
├── _base.scss        # Reset, body, tipografía base
└── _components.scss  # Componentes reutilizables

src/styles.scss       # Entry point (@use)
```

---

## 12. Roadmap UI

- [ ] Componentes shared Angular (DataTable, Pagination)
- [x] Modal de formulario (`app-form-modal`) y confirmación (`app-confirm-dialog`)
- [ ] i18n ES/EN con ngx-translate
- [ ] Icon set unificado (Lucide o similar)
- [ ] Skeleton loaders
- [ ] Toast / notificaciones
- [ ] Sidebar colapsable en tablet
