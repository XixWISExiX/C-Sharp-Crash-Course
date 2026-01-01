# ContactManager

## Commands to get app working

Build Project
```bash
dotnet clean
dotnet build
```

Run Tests and Project
`Reminder: Testing folder needs to reference project folder`
```bash
dotnet test
dotnet run --project contact_manager_proj/contact_manager_proj.csproj
```

Release Project
```bash
dotnet clean
dotnet build -c Release
dotnet publish contact_manager_proj/contact_manager_proj.csproj -c Release -r linux-x64 --self-contained false
```

Run Released Version
```bash
dotnet contact_manager_proj/bin/Release/net8.0/linux-x64/publish/contact_manager_proj.dll
```
