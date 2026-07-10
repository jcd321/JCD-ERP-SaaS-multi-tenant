# JCD ERP — Frontend (Angular 21)

SPA del proyecto **JCD ERP**. Documentación general en la raíz del repositorio.

| Recurso | Ubicación |
|---------|-----------|
| README principal | [../../README.md](../../README.md) |
| Progreso del proyecto | [../../docs/PROJECT-PROGRESS.md](../../docs/PROJECT-PROGRESS.md) |
| Sistema de diseño UI | [UI-DESIGN-SYSTEM.md](./UI-DESIGN-SYSTEM.md) |

## Desarrollo

```bash
npm install
npm start          # http://localhost:4200
npm run build      # producción
```

Requiere la API en `http://localhost:5000` (ver README principal).

## Arquitectura frontend

- **Angular 21** — standalone components
- **NgRx 21** — Actions → Effects → Service → Reducer → Facade
- **Features** — `src/app/features/<module>/`
- **Store** — `src/app/store/<module>/`
- **i18n** — `public/i18n/es.json`, `public/i18n/en.json`

### Módulos de inventario (Fase 3)

`stock` · `movements` · `kardex` · `transfers` · `adjustments`

Tras crear movimiento, transferencia o ajuste, `store/inventory/inventory-sync.effects.ts` recarga stock, movimientos y kardex.

## Convenciones

- Consultar `UI-DESIGN-SYSTEM.md` antes de crear pantallas nuevas
- Rutas protegidas con `permissionGuard` en `app.routes.ts`
- Errores de API mapeados en `core/platform/platform-error-messages.ts`
