Set WshShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
currentDir = fso.GetParentFolderName(WScript.ScriptFullName)

exePath = currentDir & "\bin\dcbypass.exe"
If Not fso.FileExists(exePath) Then
    exePath = currentDir & "\dcbypass.exe"
End If
If Not fso.FileExists(exePath) Then
    exePath = currentDir & "\dist\dcbypass.exe"
End If

If fso.FileExists(exePath) Then
    Set objShell = CreateObject("Shell.Application")
    objShell.ShellExecute exePath, "--tray", "", "runas", 0
Else
    WScript.Echo "Error: bin\dcbypass.exe not found!"
End If
