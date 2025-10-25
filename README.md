# PdfGhostScriptWrapper

Набор из двух проектов .NET 9, предназначенных для работы с GhostScript:

- **PdfGhostScriptWrapper** — библиотека-обертка, отвечающая за поиск исполняемого файла GhostScript, сборку аргументов и запуск процесса.
- **PdfOptimizer.Cli** — консольная утилита, которая использует библиотеку для оптимизации PDF-файлов и пробрасывает нужные опции в GhostScript.

## Возможности

- Кроссплатформенный запуск (Linux, macOS, Windows) за счет автоматического определения имени исполняемого файла GhostScript.
- Управление опциями, влияющими на размер PDF: включение/отключение сабсета шрифтов и их сжатия.
- Поддержка встроенных пресетов GhostScript (`screen`, `ebook`, `printer`, `prepress`, `default`).
- Проброс произвольных аргументов в GhostScript, если требуется тонкая настройка.

## Требования

- .NET SDK 9.0 или новее.
- Установленный GhostScript (путь может быть определен автоматически либо задан переменной `GHOSTSCRIPT_PATH`, либо параметром `--ghostscript`).

## Сборка

```bash
dotnet build
```

## Использование CLI

```bash
dotnet run --project src/PdfOptimizer.Cli -- <input.pdf> <output.pdf> [опции]
```

Доступные опции:

- `--subset-fonts` / `--no-subset-fonts` — включить или отключить сабсет шрифтов (по умолчанию включено).
- `--compress-fonts` / `--no-compress-fonts` — включить или отключить сжатие шрифтов (по умолчанию включено).
- `--preset <name>` — указать встроенный пресет GhostScript.
- `--ghostscript <path>` — явно указать путь к исполняемому файлу GhostScript.
- `--gs-arg <value>` — передать произвольный аргумент в GhostScript; опция может повторяться.
- `--` — все последующие аргументы будут переданы в GhostScript без изменений.

## Пример

```bash
dotnet run --project src/PdfOptimizer.Cli -- input.pdf output.pdf --preset ebook --gs-arg -dCompatibilityLevel=1.4
```
