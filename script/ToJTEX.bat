@echo off
setlocal
:: 调用 LINQPad 的脚本执行器
"C:\Program Files (x86)\LINQPad5\lprun.exe" "ToJTEX.linq" "%~1"
pause
