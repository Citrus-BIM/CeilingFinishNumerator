# CeilingFinishNumerator

## Назначение

Revit-модуль для помещений, квартир, отделки, полов, потолков, стен или связанных ведомостей.

Тип репозитория: активный модуль RibbonCITRUS.

## Технологический стек

- C# / .NET
- TargetFramework: net8.0, net48, net8.0-windows
- WPF/XAML
- Windows Forms
- Autodesk Revit API
- NuGet: System.Resources.Extensions, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio

## Структура репозитория

- `CeilingFinishNumerator/`
- `CeilingFinishNumerator.Tests/`
- `CeilingFinishNumeratorSpectrum/`
- `data/`
- `.gitattributes`
- `.gitignore`
- `CeilingFinishNumerator.dll`
- `CeilingFinishNumerator.dll.config`
- `CeilingFinishNumerator.pdb`
- `CeilingFinishNumerator.sln`

## Сборка и запуск

Основная точка сборки: `CeilingFinishNumerator.sln`.

```powershell
dotnet build .\CeilingFinishNumerator.sln
```

Для Revit-плагинов может потребоваться сборка в конфигурации целевой версии Revit (`R20xx`) через Visual Studio/MSBuild.

## Тесты

Найдены тестовые проекты:
- `CeilingFinishNumerator.Tests/CeilingFinishNumerator.Tests.csproj`

Обычно запуск:

```powershell
dotnet test
```

## Интеграции и зависимости

- Модуль относится к активной линейке RibbonCITRUS и обычно подключается через общий Revit ribbon-host.

## Важные ограничения

- Специальные ограничения не выявлены автоматически; перед изменениями свериться с владельцем проекта и кодом.

## Статус документации

Документация сформирована автоматически 2026-06-24 по локальной структуре репозитория, проектным файлам и существующим Markdown-документам. Если назначение описано предположительно, перед разработкой нужно сверить его с владельцем проекта или исходным кодом.

