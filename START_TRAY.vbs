Set WshShell = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
currentDir = fso.GetParentFolderName(WScript.ScriptFullName)
exePath = currentDir & "\dist\dcbypass.exe"

If fso.FileExists(exePath) Then
    Set objShell = CreateObject("Shell.Application")
    objShell.ShellExecute exePath, "--tray", "", "runas", 0
Else
    WScript.Echo "Error: dist\dcbypass.exe not found!"
End If
