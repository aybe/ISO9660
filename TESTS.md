The unit tests in the solution are data-based and parse .cue/.iso images.

Images that have been used aren't provided for copyright issues.

Instead, you generate tests using your own images:

- browse to the [templates](ISO9660.Tests/Templates) folder in the test project
- there are text files, each describe parameters for a test
- write adjacent JSON files containing arrays of parameters
- rebuild the solution; the tests will be generated accordingly

**Example:**

`TestDataIsoReadFile.json` for `TestDataIsoReadFile.txt`:

```
[
  {
    "Source": "C:\\Temp\\WipeEout.cue",
    "Target": "SCES_000.10",
    "Sha256": "1cfc2fcc1a87e71d1a506a7dc55975538ebfef44e1850f577da780398d93ae50",
    "Cooked": true
  },
  {
    "Source": "C:\\Temp\\WipEout.cue",
    "Target": "WOPAL.AV",
    "Sha256": "5d35880fb50856309cb87fed18c4911c87814581331aa3b7e938c21342fd889e",
    "Cooked": false
  }
]
```