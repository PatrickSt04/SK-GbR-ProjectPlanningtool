# Feiertagskalender

## Überblick
Der Feiertagskalender ist mandantenfähig umgesetzt. Jeder Eintrag gehört genau einer `Company` und ist nur für Benutzer derselben Company sichtbar.

## Datenmodell
- Tabelle: `HolidayCalendarEntry`
- Pflichtfelder:
  - `HolidayName`
  - `HolidayDate`
  - `HolidayType` (`Feiertag` / `Betriebsurlaub`)
- Audit-Felder:
  - `CreatedById`, `CreatedTimestamp`
  - `LatestModifierId`, `LatestModificationTimestamp`, `LatestModificationText`
  - `DeleteFlag`

## Zugriffssicherheit
Die Razor Pages verwenden `HolidayCalendarPageModel` als gemeinsame Basisklasse.
Dort erfolgt:
- Ermittlung des aktuellen Employees über den eingeloggten User
- Company-Scoping aller Queries (`CompanyScopedHolidays`)
- Laden einzelner Datensätze nur innerhalb der aktuellen Company (`FindHolidayForCurrentCompanyAsync`)

## CRUD-Seiten
- `Index`: Kalenderansicht + Liste aller Einträge
- `Create`: neuen Eintrag anlegen
- `Edit`: Eintrag bearbeiten
- `Details`: Eintrag anzeigen sowie aktivieren/deaktivieren (Soft Delete)

## Navigation
- Einstieg in `CompanySettings` über Button „Feiertagskalender öffnen“
- Zusätzlicher Menüpunkt unter `Einstellungen` in der Hauptnavigation
