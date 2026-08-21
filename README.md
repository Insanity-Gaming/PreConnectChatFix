# PreConnectChatFix for ModSharp

Blocks client chat until ModSharp has called `OnClientPutInServer`.

## Build

Requires .NET 10.

```bash
dotnet restore
dotnet build -c Release
```

Output:

`bin/Release/net10.0/PreConnectChatFix.dll`

## Install

Place the compiled module in your normal ModSharp module location under:

`game/sharp/modules/`

## Behaviour

- Clears stale slot state on `OnClientConnected`.
- Marks a client ready on `OnClientPutInServer`.
- Clears readiness on disconnect.
- Uses ModSharp's `OnClientSayCommand` listener.
- Returns `ECommandAction.Stopped` for pre-connect chat.
- Leaves normal chat untouched with `ECommandAction.Skipped`.
- Uses listener priority 100 so the guard runs early.
- Handles hot reloads without deliberately blocking already in-game clients.

The project references `ModSharp.Sharp.Shared` with `PrivateAssets="all"` as
recommended by the current ModSharp documentation.
